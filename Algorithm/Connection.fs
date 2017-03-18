namespace Algorithm

open System
open Nata.IO
open Nata.IO.Capability
open Nata.Core

type LogConnection =
    { name : string
      indexOf : Indexer<int64>
      readFrom : ReaderFrom<Target option,int64> }

type LogStore =
    { name : string
      channel : string
      connect : string->string->Codec<string,Target option>->LogConnection[] }

module EventStore =

    open Nata.IO.EventStore

    let [<Literal>] Name = "EventStore"
    let [<Literal>] Channel = "Stream"

    let connect (host:string) (stream:string) (codec:Codec<string,Target option>) : LogConnection[] =
        let settings =
            { Settings.Server = { Server.Host = host; Port = 1113 }
              Settings.User = { User.Name = "admin"; Password = "changeit" }
              Settings.Options = { Options.BatchSize = 1 } }
        
        let streamFor =
            let dataCodec =
                Codec.BytesToString
                |> Codec.concatenate codec
            settings
            |> Stream.connect
            |> Source.mapData dataCodec
            |> Source.mapIndex (int64, int)
        
        let indexOf, readFrom =
            let stream =
                streamFor stream
            stream |> indexer,
            stream |> readerFrom

        [| { name = stream
             indexOf = indexOf
             readFrom = readFrom } |]

    let store = { LogStore.name=Name
                  LogStore.channel=Channel
                  LogStore.connect=connect }

module KafkaNet =

    open Nata.IO.KafkaNet

    let [<Literal>] Name = "KafkaNet"
    let [<Literal>] Channel = "Topic"

    let connect (host:string) (topic:string) (codec:Codec<string,Target option>) : LogConnection[] =
        let cluster = [ host ]
        let partitions =
            let indexOf = 
                Topic.connect cluster topic
                |> indexer
            indexOf Position.Start
            |> Offsets.partitions
                
        [|
            for partition in partitions do
                let partitionFor =
                    let dataCodec =
                        Codec.BytesToString
                        |> Codec.concatenate codec
                    let indexCodec =
                        Offset.Codec.OffsetToInt64 partition
                    TopicPartition.connect cluster
                    |> Source.mapData dataCodec
                    |> Source.mapIndex indexCodec
                    
                let name = sprintf "%s/%d" topic partition
                let indexOf, readFrom =
                    let partition =
                        partitionFor (topic,partition)
                    partition |> indexer,
                    partition |> readerFrom

                yield
                    { name = name
                      indexOf = indexOf
                      readFrom = readFrom }
        |]

    let store = { LogStore.name=Name
                  LogStore.channel=Channel
                  LogStore.connect=connect }

module Kafunk =

    open Nata.IO.Kafunk

    let [<Literal>] Name = "Kafunk"
    let [<Literal>] Channel = "Topic"

    let connect (host:string) (topic:string) (codec:Codec<string,Target option>) : LogConnection[] =
        let cluster = Kafunk.Connection.create { Settings.defaultSettings with Hosts=[ host ] }
        let partitions =
            let indexOf = 
                Topic.connect cluster topic
                |> indexer
            indexOf Position.Start
            |> Offsets.partitions
                
        [|
            for partition in partitions do
                let partitionFor =
                    let dataCodec =
                        Codec.BytesToString
                        |> Codec.concatenate codec
                    let indexCodec =
                        Offset.Codec.OffsetToInt64 partition
                    TopicPartition.connect cluster
                    |> Source.mapData dataCodec
                    |> Source.mapIndex indexCodec
                    
                let name = sprintf "%s/%d" topic partition
                let indexOf, readFrom =
                    let partition =
                        partitionFor (topic,partition)
                    partition |> indexer,
                    partition |> readerFrom

                yield
                    { name = name
                      indexOf = indexOf
                      readFrom = readFrom }
        |]

    let store = { LogStore.name=Name
                  LogStore.channel=Channel
                  LogStore.connect=connect }

module Connections =

    let kafkanet = KafkaNet.store
    let kafunk = Kafunk.store
    let eventstore = EventStore.store

    let all = [| kafkanet; kafunk; eventstore |]