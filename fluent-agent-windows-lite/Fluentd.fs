module Fluentd
open System
open System.Net.Sockets
open System.IO
open MsgPack
open MsgPackUtil

type t = {
    client : TcpClient;
    time   : DateTime;
    doConnect : unit -> System.Net.Sockets.TcpClient
}

let connect host port () =
    try
        let client = 
            new System.Net.Sockets.TcpClient(host, port)
        client.NoDelay <- true
        client.SendBufferSize <- 0
        client.SendTimeout <- 0
        client
    with e ->
        Util.logger().Error("connection error", e)
        raise e

let reconnect doConnect =
    let client =
        doConnect ()
    {
        doConnect = doConnect;
        client = client;
        time   = DateTime.Now
    }

let make host port =
    reconnect <| connect host port

let autoReconnect ({ time = time } as state) =
    if (time - DateTime.Now).TotalMinutes >= 30. then
        reconnect state.doConnect
    else
        state

let toObject time tag messages =
    let xs =
        messages
        |> List.map (fun m -> array [| int (date time); obj (dict [ str "message", str m ]) |])
        |> Array.ofList
    array [| str tag; array xs |]

let serialize (obj : MessagePackObject) =
    let stream =
        new MemoryStream()
    let packer =
        Packer.Create(stream)
    packer.Pack(obj)
    (packer.Position, stream.GetBuffer())

let emit tag (xs : string list) (state : t) =
    try
        let now = 
            DateTime.Now
        let size, buffer =
            toObject now tag xs |> serialize
        state.client.GetStream().Write(buffer, 0, Core.Operators.int size)
        state.client.GetStream().Flush()
        state
    with e ->
        Util.logger().Error("send to fluentd", e)
        reconnect state.doConnect
