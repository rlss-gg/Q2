[<AutoOpen>]
module Q2.Presentation.Patterns

let (|Equals|_|) a b =
    if a = b then Some () else None
