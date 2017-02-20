namespace Algorithm

open System

type SearchState = {
    Target : Target
    Status : Status
    Position : Position option
}

and Position = {
    Minimum : int64
    Maximum : int64
    LowerBound : int64
    UpperBound : int64
    QueryAt : int64
    Current : int64
}

and Status =
    | Idle
    | Scan
    | Seek
    | Cancelled
    | NotFound 
    | Found of Target*int64

and Target =
    | Timestamp of DateTime
    | Text of string
    | Number of decimal

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SearchState =

    let create target = { Target=target
                          Status=Idle
                          Position=None }
