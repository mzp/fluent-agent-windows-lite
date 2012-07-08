module TailWatcher
open System.Timers
open System.IO

type t = {
    stream   : FileStream;
    previous : string;
    lines    : string list;
    tag      : string;
    updatePosition : int64 -> unit
}

let seekToSavedPos (stream : #Stream) pos =
    Maybe.maybe {
        let! pos   = pos
        let reader = new BinaryReader(pos)
        let! n     = Util.sure reader.ReadInt64 ()
        let  _     = stream.Seek(n, SeekOrigin.Begin)
        return ()
    } |> ignore

let savePos (pos : #Stream) n =
    pos.Seek(0L, SeekOrigin.Begin) |> ignore
    let writer =
        new BinaryWriter(pos)
    writer.Write(n : int64)

let make tag path (posFile : string option) : t =
    let stream =
        new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
    let updatePosition =
        posFile
        |> Option.map (fun path -> new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        |> Util.tee (seekToSavedPos stream)
        |> Option.map savePos
        |> Maybe.getOrElse ignore
    {
        stream   = stream;
        previous = "";
        lines    = [];
        tag      = tag;
        updatePosition = updatePosition
    }

let lines { tag = tag; lines = lines } =
    (tag, lines)

(*
   This is very frigile.
   Do not call read function from multi thread
*)
let buffer =
    Array.create 1024 (byte 0)

let splitLines (str : string) =
    str.Split([| "\r\n"; "\n"; "\r" |], System.StringSplitOptions.None)

let read ({ previous = previous;  stream = stream } as me) =
    async {
        let! size =
            stream.AsyncRead(buffer)
        me.updatePosition(stream.Position)
        let str =
            new System.String(Array.map char buffer, 0, size)
        let me' =
            (previous + str)
            |> splitLines
            |> List.ofArray
            |> Util.butLast
            |> Option.map (fun (lines, previous) ->
                           { me with previous = previous; lines = lines })
            |> Maybe.getOrElse me
        return me'
     }

let rec Sequence = function
    | [] ->
        async { return [] }
    | x :: xs ->
        async {
            let! y  = x
            let! ys = Sequence xs
            return (y :: ys)
        }

let watch (xs : t list) : t list =
    xs
    |> List.map read
    |> Sequence
    |> Async.RunSynchronously
