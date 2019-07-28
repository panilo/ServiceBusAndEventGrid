using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EventHubs.SendTestClient
{
    class Program
    {
        private static EventHubClient eventHubClient;
        IConfiguration config; 

        static void Main(string[] args)
        {
            Console.WriteLine("Sending and receiving messages from a Event Hub");
            Program p = new Program();

            p.MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            BuildConfig();
        }

        private async Task MainAsync()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(config["EventHubConfig:ConnectionString"])
            {
                EntityPath = config["EventHubConfig:Name"]
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendMessagesToEventHub(100);

            await eventHubClient.CloseAsync();

            await ProcessMessages();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        private async Task ProcessMessages()
        {
            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(
                config["EventHubConfig:Name"], 
                PartitionReceiver.DefaultConsumerGroupName, 
                config["EventHubConfig:ConnectionString"], 
                config["EventHubConfig:StorageConfig:ConnectionString"], 
                config["EventHubConfig:StorageConfig:ContainerName"]);

            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }

        private async Task SendMessagesToEventHub(int numberOfMessages)
        {
            for (var i = 0; i < numberOfMessages; i++)
            {
                try
                {
                    var message = $"Message {i+1}";
                    Console.WriteLine($"Sending message: {message}");
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numberOfMessages} messages sent.");
        }        

        private void BuildConfig()
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", false)
                .Build();
        }
    }
}
