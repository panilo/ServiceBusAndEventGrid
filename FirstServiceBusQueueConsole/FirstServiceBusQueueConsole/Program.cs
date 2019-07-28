using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FirstServiceBusQueueConsole.Business;
using FirstServiceBusQueueConsole.Entities;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;

namespace FirstServiceBusQueueConsole
{
    class Program
    {
        private static IConfiguration _appConfig;

        static void Main(string[] args)
        {                        
            Console.WriteLine("Hello World!");
            Program p = new Program(args);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
            Console.WriteLine("======================================================");

            p.MainAsync().GetAwaiter().GetResult();

            Console.WriteLine("END");
            Console.ReadKey(true);
        }

        public string GetPlainConfig()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var v in _appConfig.AsEnumerable())
            {
                sb.AppendFormat("K: {0} - V: {1}{2}", v.Key, v.Value, Environment.NewLine);
            }

            return sb.ToString();
        }

        public Program(string[] args)
        {
            BuildConfig(args);                  
        }

        async Task MainAsync()
        {
            var messageNumber = 10;

            var serviceBusConfiguration = new ServiceBusConfiguration(_appConfig["ServiceBus:ConnectionString"], _appConfig["ServiceBus:QueueName"]);
            Console.WriteLine(serviceBusConfiguration.ToString());

            var myConsumer = new SBConsumer(serviceBusConfiguration);
            var myPublisher = new SBPublisher(serviceBusConfiguration);

            for (var i = 0; i < messageNumber; i++)
            {
                var payload = $"I'm the message number {i + 1} sent on date {DateTime.Now}";
                await myPublisher.SendMessage(payload);
                Console.WriteLine($"Sent message {i + 1}");
            }

            while (true)
            {
                Thread.Sleep(10000);
                Console.WriteLine("Waiting messagges...");
                if((DateTime.Now - myConsumer.LastMessageReceivedOn).TotalSeconds > 10)
                {
                    break;
                }
            }
        }

        private void BuildConfig(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            _appConfig = config;
        }
    }
}
