using System;
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

        public void Run()
        {
            new string[] { "EUR/USD", "USD/JPY", "GBP/USD", "USD/CHF", "AUD/USD", "USD/CAD" }.ToList().ForEach(instrument => {
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
