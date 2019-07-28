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
    public class SBPublisher : ServiceBusHandler
    {        
        public SBPublisher(ServiceBusConfiguration sbConfig)
            :base(sbConfig.ConnectionString, sbConfig.QueueName)
        { }

        public async Task SendMessage(object payload)
        {
            try
            {
                //First get a queue message
                Message msg = new Message(CastObjectToBytes(payload));
                //Then send it to the queue! 
                await queueClient.SendAsync(msg);
            }catch(Exception e)
            {
                Console.Error.WriteLine($"An exception has been thrown by SendMessage on {DateTime.Now}. Details: {e.Message}");
                Console.Error.Flush();
            }
        }        
    }
}
