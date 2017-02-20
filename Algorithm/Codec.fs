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
        
    let createTimestamp (path) : Codec<string,Target option> =
        create (DateTime.toLocal >> Target.Timestamp) path

    let createText (path) : Codec<string,Target option> =
        create (string >> Target.Text) path

    let createNumber (path) : Codec<string,Target option> =
        create (decimal >> Target.Number) path

