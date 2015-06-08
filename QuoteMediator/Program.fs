
module QuoteMediator

open RabbitMq
open QuoteEngine.Interfaces
open System

[<EntryPoint>]
let main argv = 
    printfn "%A" argv

    let subscriber = new RabbitMq.Subscriber()

    let inter = subscriber:> QuoteEngine.Interfaces.ISubscriber

    let subscribeThread = async{
                     
                                inter.Subscribe "quote" (fun inBytes -> 
                                                                printfn "%A" inBytes
                                                                inBytes
                                                        )
                                    |> ignore
                            }

    Async.Start subscribeThread

    Console.ReadLine()
        |> ignore

    inter.Stop()

    0 // return an integer exit code
