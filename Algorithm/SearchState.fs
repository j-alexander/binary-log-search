namespace Algorithm

open System

type SearchState = {
    Target : DateTime
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
    | Found of DateTime*int64

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SearchState =

    let empty = { Target=DateTime.Now
                  Status=Idle
                  Position=None }
