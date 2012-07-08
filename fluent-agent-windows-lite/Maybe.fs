module Maybe

type MaybeBuilder() =
    member this.Bind(x, f) =
        Option.bind f x

    member this.Return(x) =
        Some x

let getOrElse value = function
    | None -> value
    | Some x -> x

let maybe =
    new MaybeBuilder()

let nullable x =
    if x = null then
        None
    else 
        Some x