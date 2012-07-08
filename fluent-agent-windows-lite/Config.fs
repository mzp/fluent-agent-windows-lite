module Config
open System.IO
open DynamicJson

let readAll path =
    use reader =
        new StreamReader(new FileStream(path, FileMode.Open))
    reader.ReadToEnd()

let str (value : JsonValue) =
    (value :?> JsonString).Value

let num (value : JsonValue) =
    (value :?> JsonNumber).Value |> int
        
let load (path:string) =
    let json =
        readAll path
        |> DynamicJson.JsonObject.Parse
    let host =
        str json.["host"] 
    let port =
        Util.sure (fun () -> num json.["port"]) ()
        |> Maybe.getOrElse 24224
    let fluentd =
        Fluentd.make host port
    let tails = 
        (json.["files"] :?> JsonArray).Values
        |> Array.map begin fun obj ->
            let obj = 
                obj :?> JsonObject
            let tag =
                str obj.["tag"]
            let path =
                str obj.["path"]
            let pos = 
                Util.sure (fun () -> str obj.["pos"]) ()
            TailWatcher.make tag path pos
        end
    Agent.make (List.ofArray tails) fluentd

let appPath () =
    let asm = 
        System.Reflection.Assembly.GetEntryAssembly()
    Maybe.nullable asm
    |> Option.map (fun x -> x.Location)
    |> Option.map System.IO.Path.GetDirectoryName

let defaultPath () =
    appPath ()
    |> Option.map (fun dir -> System.IO.Path.Combine(dir, "fluent-agent-windows.json"))
    |> Option.get