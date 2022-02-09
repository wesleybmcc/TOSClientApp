using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using ThinkOrSwim;

namespace ThinkOrSwim
{
    internal class TOSWorker : IDisposable
    {
        public bool KeepAlive { get; set; } = true;
        private Client client = new Client();
        private bool includeForex = false;
        private bool includeFutures = true;

        public void Run()
        {
            var futures = new string[] { "/ES:XCME", "/NQ:XCME", "/RTY:XCME", "/YM:XCBT", "/CL:XNYM", "/GC:XCEC" };
            var forex = new string[] { "EUR/USD", "USD/JPY", "GBP/USD", "USD/CHF", "AUD/USD", "USD/CAD" };

            var activeList = new List<string>();

            if(includeFutures)
            {
                activeList.AddRange(futures.ToList());
            }

            if(includeForex)
            {
                activeList.AddRange(forex.ToList());
            }

            activeList.ForEach(instrument => {
                client.Add(instrument, QuoteType.Bid);
                client.Add(instrument, QuoteType.Ask);
            });
            
            while (KeepAlive)
            {
                foreach (var quote in client.Quotes())
                {
                    string message = string.Format("{0} {1} {2}", quote.Symbol,
                        quote.Type.ToString(), quote.Value);
                    Console.WriteLine(message);

                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "hello",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "hello",
                                             basicProperties: null,
                                             body: body);
                    }

                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
