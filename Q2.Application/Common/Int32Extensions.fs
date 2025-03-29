module System.Int32

let tryParse (s: string) =
    match Int32.TryParse s with
    | true, i -> Some i
    | _ -> None
