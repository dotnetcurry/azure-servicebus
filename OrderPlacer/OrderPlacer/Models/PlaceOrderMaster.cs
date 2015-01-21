using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace OrderPlacer.Models
{

    /// <summary>
    /// Class for Combining Customer, Order and Item Information with
    /// CustomerId and ItemId
    /// </summary>
    /// 
    public class CustomerOrderItemMaster
    {
        public int CustomerId { get; set; }
        public OrderMaster Order { get; set; }
        public int ItemId { get; set; }

    }

    /// <summary>
    /// Class cotaining method for placind order
    /// </summary>
    public class PlaceOrder
    {
        CompanyEntities ctx;
        public PlaceOrder()
        {
            ctx = new CompanyEntities(); 
        }

        /// <summary>
        /// Method to Read all Items
        /// </summary>
        /// <returns></returns>
        public List<ItemMaster> GetItems()
        {
            return ctx.ItemMasters.ToList();
        }
        /// <summary>
        /// The Customers
        /// </summary>
        /// <returns></returns>
        public List<CustomerMaster> GetCustomers()
        { 
            return ctx.CustomerMasters.ToList();
        }

        /// <summary>
        /// Method to Create Order and Storing it in Queue
        /// </summary>
        /// <param name="coi"></param>
        public void CreateOrder(CustomerOrderItemMaster coi)
        {
            OrderMaster ord = new OrderMaster()
            {
                 OrderId = coi.Order.OrderId,
                 CustomerId= coi.CustomerId,
                 Quantity = coi.Order.Quantity,
                 ItemId = coi.ItemId,
                  OrderDate = DateTime.Now
            };

            ctx.OrderMasters.Add(ord);
            ctx.SaveChanges();  

            //Code for Adding Message in Queue
            
            //S1: The NamespaceManager object used to manage queues, topics etc.
            string connString = ConfigurationSettings.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(connString);

            //S2: The Queue Description using QueueName
            QueueDescription queue = new QueueDescription("orderqueue");
            queue.MaxSizeInMegabytes = 5120; //5 MB
            queue.DefaultMessageTimeToLive = new TimeSpan(0, 30, 0); //For 30 Mins
            
            
            //S3: Send the Message
            QueueClient client = QueueClient.CreateFromConnectionString(connString, "orderqueue");
            BrokeredMessage message = new BrokeredMessage(ord);
            client.Send(message);
            //Ends Here

            
        }

    }

}
