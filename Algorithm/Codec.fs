namespace Algorithm

module Codec =

    open System
    open FSharp.Data
    open Nata.Core
    open Nata.Fun.JsonPath

    let create (startAt, path) : Codec<string,DateTime option> =
        String.tryStartAt startAt
        >> Option.get
        >> JsonValue.Parse
        >> JsonValue.tryFind path
        >> Option.map (JsonValue.toType >> DateTime.toLocal), Codec.NotImplemented
        