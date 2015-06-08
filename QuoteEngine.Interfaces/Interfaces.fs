// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

namespace QuoteEngine

    module Interfaces =

        type ISubscriber =
            abstract member Subscribe : string -> (byte[] -> byte[]) -> bool
            abstract member Stop : unit -> unit

        type IPublisher =
            abstract member Subscribe : string -> (byte[] -> byte[]) -> bool

