using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;

namespace QuoteEngineApp
{
    class Program
    {
        static byte[] Processor(byte[] inBytes)
        {
            return new byte[1];
        }

        static void Main(string[] args)
        {

            QuoteEngine.Interfaces.ISubscriber server = new RabbitMq.Subscriber();
            server.Subscribe("quote",  FSharpFunc<byte[],byte[]>.FromConverter(Processor) );
            
        }
    }
}
