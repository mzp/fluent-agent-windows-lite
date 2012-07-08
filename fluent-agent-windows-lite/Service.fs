module Service
open System.Diagnostics
open System.ServiceProcess
open System.ComponentModel
open System.Collections

type FluentdService() =
    inherit ServiceBase()

    let mutable agent : Agent.t option =
        None

    override this.OnStart(_ : string[]) : unit =
        do  Util.logger().Info("start service")
            Config.defaultPath ()
            |> Util.tee (fun s -> Util.logger().Info("load from " + s))
            |> Config.load
            |> fun a -> agent <- Some(a)
        async{ Option.iter Agent.start agent }
        |> Async.Start

    override this.OnStop() : unit =
        Util.logger().Info("stop service")
        Option.iter Agent.stop agent
        agent <- None

    static member Run() =
        new FluentdService()
        |> ServiceBase.Run

[<RunInstaller(true)>]
type FluentdInstaller() as this =
    inherit System.Configuration.Install.Installer()
    do let serviceInstaller = new ServiceInstaller()
       let processInstaller = new ServiceProcessInstaller()
       processInstaller.Account            <- ServiceAccount.LocalSystem
       serviceInstaller.StartType          <- ServiceStartMode.Automatic
       serviceInstaller.ServicesDependedOn <- [| "EventLog" |]
       serviceInstaller.ServiceName        <- "Fluentd Tiny Agent"
       serviceInstaller.Description        <- "A log transfer agent, for Fluentd's 'forward' input."
       ignore <| this.Installers.Add(serviceInstaller)
       ignore <| this.Installers.Add(processInstaller)
