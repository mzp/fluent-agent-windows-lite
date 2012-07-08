module Agent

type t = {
    mutable tails   : TailWatcher.t list;
    mutable fluentd : Fluentd.t;
    mutable enable  : bool
}

let make tails fluentd = {
    tails = tails;
    fluentd = fluentd;
    enable = false
}

let loop state =
    while state.enable do
        state.tails <- 
            TailWatcher.watch state.tails
        let lines =
            state.tails
            |> List.map TailWatcher.lines
            |> List.filter (fun (t,xs) -> xs <> [])
        state.fluentd <- 
            List.fold (fun s (tag, lines) -> Fluentd.emit tag lines s) state.fluentd lines
        System.Threading.Thread.Sleep(1000)
    done

let stop (state : t) =
    state.enable <- false

let start (state : t) : unit =
    state.enable <- true;
    loop state
