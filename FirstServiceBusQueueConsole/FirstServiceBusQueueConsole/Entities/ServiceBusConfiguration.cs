using System;
using System.Collections.Generic;
using System.Text;

namespace FirstServiceBusQueueConsole.Entities
{
    public class ServiceBusConfiguration
    {
        public string ConnectionString { get; private set; }
        public string QueueName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cs">ConfigurationString</param>
        /// <param name="qn">QueueName</param>
        public ServiceBusConfiguration(string cs, string qn)
        {
            this.ConnectionString = cs;
            this.QueueName = qn;
        }

        public override string ToString()
        {
            return string.Format("CS: {0} - PK: {1}", this.ConnectionString, this.QueueName);
        }
    }
}
