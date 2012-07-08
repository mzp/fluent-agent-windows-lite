[<EntryPointAttribute>]
let main (args : string array)=
    if Array.length args = 0 then
        Service.FluentdService.Run()
    else if args.[0] = "/run" then
        Config.defaultPath ()
        |> Util.tee (fun s -> Util.logger().Info("load from " + s))
        |> Config.load
        |> Agent.start
    0
