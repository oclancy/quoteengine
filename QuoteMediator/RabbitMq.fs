module RabbitMq

open System
open QuoteEngine.Interfaces
open RabbitMQ.Client
open System.Collections.Generic

    type MySimpleRpcServer(subs:MessagePatterns.Subscription, callback:(byte[]->byte[])) =
        inherit MessagePatterns.SimpleRpcServer(subs) with
            override this.HandleSimpleCall (isRedelivered:bool, requestProperties:IBasicProperties, body:byte[], replyProperties:byref<IBasicProperties>)  =
                let res = callback( body )
                res

            override this.HandleCall(isRedelivered:bool, requestProperties:IBasicProperties, body:byte[], replyProperties:byref<IBasicProperties>)  =
                let res = callback( body )
                res

            override this.HandleCast(isRedelivered:bool, requestProperties:IBasicProperties, body:byte[])  =
                callback( body )
                 |> ignore
       
    type Subscriber() =
        let cf = new ConnectionFactory()
        let connection = cf.CreateConnection() 
        let mutable model = connection.CreateModel()
        let queue = model.QueueDeclare()
         
        interface ISubscriber with
            member this.Subscribe (topic:string) (callback: byte[] -> byte[]) = 
                
                cf.Uri <- "amqp://guest:guest@localhost:5672"
                cf.UserName <- "guest"
                cf.Password <- "guest"

                model.QueueBind( queue.QueueName, "amq.direct", topic)
                use subscriber = new MessagePatterns.Subscription( model, queue.QueueName )
                use rpcServer = new MySimpleRpcServer(subscriber, callback)
                rpcServer.MainLoop()
                true
            member this.Stop ()=
                model.Close()
