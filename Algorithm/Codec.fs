namespace Algorithm

module Codec =

    open System
    open FSharp.Data
    open FSharp.Data.JsonPath
    open Nata.Core

    let create (toTarget) (path) : Codec<string,Target option> =
        JsonValue.Parse
        >> JsonPath.tryFind path
        >> Option.map (JsonValue.toType >> toTarget), Codec.NotImplemented
        
    let createTimestamp =
        create (DateTime.toLocal >> Target.Timestamp)

    let createText =
        create (string >> Target.Text)

    let createNumber =
        create (decimal >> Target.Number)

