using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FirstServiceBusQueueConsole.Business
{
    public abstract class ServiceBusHandler
    {
        /// <summary>
        /// Connection string to ServiceBus (it's the endpoint)
        /// </summary>
        private readonly string _connectionString;
        /// <summary>
        /// ServiceBus QueueName
        /// </summary>
        private readonly string _queueName;

        protected static IQueueClient queueClient;        

        public ServiceBusHandler(string connectionString, string queueName)
        {
            this._connectionString = connectionString;
            this._queueName = queueName;

            if(queueClient == null)
                queueClient = new QueueClient(this._connectionString, this._queueName);
        }

        public Task CloseAsync()
        {
            return queueClient.CloseAsync();
        }

        protected byte[] CastObjectToBytes(object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        protected object CastBytesToObject(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using(MemoryStream ms = new MemoryStream(bytes))
            {
                return bf.Deserialize(ms);
            }
        }
    }
}
