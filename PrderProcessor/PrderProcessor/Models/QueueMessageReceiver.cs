using System.Collections.Generic;

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;

namespace OrderProcessor.Models
{
    /// <summary>
    /// The Class Constructor Receive Messages from Service Bus Queue
    /// </summary>
    public class QueueMessageReceiver
    {
        string connStr = "";
        NamespaceManager nameSpaceManager;

        public QueueMessageReceiver()
        {
            connStr = ConfigurationSettings.AppSettings["Microsoft.ServiceBus.ConnectionString"];

            nameSpaceManager = NamespaceManager.CreateFromConnectionString(connStr);


        

        }

        /// <summary>
        /// Read All messages from Queue and Put it in the List of OrderMaster
        /// </summary>
        /// <returns></returns>
        public List<OrderMaster> GetOrdersFromQueue()
        {
            QueueClient qClient = QueueClient.CreateFromConnectionString(connStr, "orderqueue", ReceiveMode.PeekLock);
            //BrokeredMessage message = qClient.Receive();

           

            var messages = qClient.ReceiveBatch(1);

            List<OrderMaster> Orders = new List<OrderMaster>();

            
            foreach (var item in messages)
            {
                Orders.Add(item.GetBody<OrderMaster>());
                item.Complete(); //Remove Message from queue
            }

            return Orders;
        }
    }
}
