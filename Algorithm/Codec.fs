namespace Algorithm

module Codec =

    open System
    open FSharp.Data
    open Nata.Core
    open Nata.Fun.JsonPath

    let create (toTarget) (path) : Codec<string,Target option> =
        JsonValue.Parse
        >> JsonValue.tryFind path
        >> Option.map (JsonValue.toType >> toTarget), Codec.NotImplemented
        
    let createTimestamp =
        create (DateTime.toLocal >> Target.Timestamp)

    let createText =
        create (string >> Target.Text)

    let createNumber =
        create (decimal >> Target.Number)

