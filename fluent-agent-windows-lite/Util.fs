module Util

let rec butLast = function
    | [] ->
        None
    | [ x ] ->
        Some ([], x)
    | x :: xs ->
        Maybe.maybe {
            let! (ys, y) = butLast xs
            return (x :: ys, y)
        }

let tee f x =
    (f x); x

let sure f x =
    try
        Some (f x)
    with e ->
        None

let logger =
    let info =
        new System.IO.FileInfo(System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)
    log4net.Config.XmlConfigurator.Configure(log4net.LogManager.GetRepository(), info) |> ignore
    fun () ->
        log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
