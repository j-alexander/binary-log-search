﻿namespace Algorithm

open System
open Nata.IO
open Nata.IO.Capability
open Nata.Core

type Connection =
    { name : string
      indexOf : Indexer<int64>
      readFrom : ReaderFrom<DateTime option,int64> }

type LogStore =
    { name : string
      channel : string
      connect : Func<string,string,Codec<string,DateTime option>,Connection[]> }

module EventStore =

    open Nata.IO.EventStore

    let connect (host:string) (stream:string) (codec:Codec<string,DateTime option>) : Connection[] =
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

        [| { Connection.name = stream
             Connection.indexOf = indexOf
             Connection.readFrom = readFrom } |]

    let store = { LogStore.name="EventStore"
                  LogStore.channel="Stream"
                  LogStore.connect=new Func<_,_,_,_>(connect) }

module Kafka =

    open Nata.IO.Kafka

    let connect (host:string) (topic:string) (codec:Codec<string,DateTime option>) : Connection[] =
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
                    { Connection.name = name
                      Connection.indexOf = indexOf
                      Connection.readFrom = readFrom }
        |]

    let store = { LogStore.name="Kafka"
                  LogStore.channel="Topic"
                  LogStore.connect=new Func<_,_,_,_>(connect) }

module Connections =

    let kafka = Kafka.store
    
    let eventstore = EventStore.store

    let all = [| Kafka.store; EventStore.store |]