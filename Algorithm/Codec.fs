namespace Algorithm

module Codec =

    open System
    open FSharp.Data
    open Nata.Core
    open Nata.Fun.JsonPath

    let create (path) : Codec<string,DateTime option> =
        JsonValue.Parse
        >> JsonValue.tryFind path
        >> Option.map (JsonValue.toType >> DateTime.toLocal), Codec.NotImplemented
        