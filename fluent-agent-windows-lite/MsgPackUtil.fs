module MsgPackUtil
open System
open MsgPack

let toIList xs =
    (Array.ofList xs) :> System.Collections.Generic.IList<MessagePackObject>  

let array xs =
    new MessagePackObject(xs : System.Collections.Generic.IList<MessagePackObject>)

let str s =
    new MessagePackObject(s : string)

let unixEpoch = 
    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

let date (n : DateTime) =
    (n - unixEpoch).TotalSeconds |> int

let int (n : int) =
    new MessagePackObject(n)

let obj xs =
    new MessagePackObject(new MessagePackObjectDictionary(xs : System.Collections.Generic.IDictionary<MessagePackObject, MessagePackObject>))
