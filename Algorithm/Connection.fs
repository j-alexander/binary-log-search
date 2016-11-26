namespace Algorithm

open System
open Nata.IO
open Nata.IO.Capability
open Nata.Core

type Connection =
    { name : string
      indexOf : Indexer<int64>
      readFrom : ReaderFrom<DateTime option,int64> }

module Kafka =

    open Nata.IO.Kafka

    let connect (host:string, topic:string, codec:Codec<string,DateTime option>) : Connection[] =
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