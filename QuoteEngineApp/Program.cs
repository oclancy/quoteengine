using Microsoft.FSharp.Core;
using NLog;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace QuoteEngineApp
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static IRedisClientsManager RedisManager;
        static readonly KeyValuePair<string,string> MsgType = new KeyValuePair<string, string>("MsgType", "48" );
        static readonly KeyValuePair<string,string> OnBehalfOfCompID = new KeyValuePair<string, string>("OnBehalfOfCompID", "115" );
        
        static readonly KeyValuePair<string,string> QuoteReqID = new KeyValuePair<string, string>("QuoteReqID", "131" );
        static readonly KeyValuePair<string,string> SecurityID = new KeyValuePair<string, string>("SecurityID", "48" );
        static readonly KeyValuePair<string,string> Symbol = new KeyValuePair<string, string>("Symbol", "55" );
        static readonly KeyValuePair<string,string> SecurityExchange = new KeyValuePair<string, string>("SecurityExchange", "207" );
        static readonly KeyValuePair<string,string> OrderQty = new KeyValuePair<string, string>("OrderQty", "38" );


        static readonly ReadOnlyDictionary<string,string> Fields = new ReadOnlyDictionary<string, string>( new Dictionary<string,string>()
                                                                 {
                                                                     {MsgType.Key, MsgType.Value},
                                                                     {OnBehalfOfCompID.Key, OnBehalfOfCompID.Value},
                                                                     {QuoteReqID.Key, QuoteReqID.Value},
                                                                     {SecurityID.Key, SecurityID.Value},
                                                                     {Symbol.Key, Symbol.Value},
                                                                     {SecurityExchange.Key, SecurityExchange.Value},
                                                                     {OrderQty.Key, OrderQty.Value}
                                                                 });
        static byte[] Processor(byte[] inBytes)
        {
            var msg = Encoding.ASCII
                              .GetString(inBytes)
                              .Split(';')
                              .Select(str => str.Split('='))
                              .ToDictionary(strArr => strArr[0], strArr => strArr[1]);

            if (msg[MsgType.Key].Equals("R", StringComparison.InvariantCulture))
                ProcessQuoteRequest(msg);
            if (msg[MsgType.Key].Equals("D", StringComparison.InvariantCulture))
                ProcessOrder(msg);


            return new byte[1];
        }

        private static void ProcessOrder(Dictionary<string, string> msg)
        {
        }

        private static void ProcessQuoteRequest(Dictionary<string, string> msg)
        {
            if (msg.ContainsKey(SecurityID.Key) &&
                msg.ContainsKey(Symbol.Key) &&
                msg.ContainsKey(OrderQty.Key))
            {
                try
                {
                    var client = RedisManager.GetClient();
                    client.ExecLuaShaAsInt(""


                }
                catch (RedisException ex)
                {
                    Logger.ErrorException("Could not allocate qty", ex);
                }
            }
        }

        static void Main(string[] args)
        {
            ConfigureRedis();
            QuoteEngine.Interfaces.ISubscriber server = new RabbitMq.Subscriber();
            server.Subscribe("quote",  FSharpFunc<byte[],byte[]>.FromConverter(Processor) );
            
        }

        private static void ConfigureRedis()
        {
            try
            {
                var sentinel = new RedisSentinel(ConfigurationManager.AppSettings["RedisServer"], "MyMaster");

                RedisManager = sentinel.Start();
    
                Logger.Info( "RedisSentinel created" );
            }
            catch (Exception ex)
            {
                Logger.ErrorException(string.Format("Could not create RedisSentinel"), ex);
                throw;
            }
        }
    }
}
