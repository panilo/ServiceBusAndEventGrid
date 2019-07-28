using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using FirstServiceBusQueueConsole.Entities;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace FirstServiceBusQueueConsole.Business
{
    public class SBConsumer : ServiceBusHandler
    {
        public DateTime LastMessageReceivedOn { get; private set; }

        public SBConsumer(ServiceBusConfiguration sbConfig)
            :base(sbConfig.ConnectionString, sbConfig.QueueName)
        {
            RegisterMessageHandler();
        }

        private void RegisterMessageHandler()
        {
            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false                
            };

            // Register the function that will process messages
            queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);
        }

        private async Task ProcessMessageAsync(Message msg, CancellationToken token)
        {
            this.LastMessageReceivedOn = DateTime.Now;

            //Write received message 
            Console.WriteLine($"Received message: {CastBytesToObject(msg.Body).ToString()}");

            //Cancell message from the queue (no one else will process the same message)
            await queueClient.CompleteAsync(msg.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exArgs.Exception}.");
            var context = exArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;   
        }
    }
}
