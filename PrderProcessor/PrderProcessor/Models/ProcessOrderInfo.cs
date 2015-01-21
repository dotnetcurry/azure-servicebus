using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Models
{
    /// <summary>
    /// The Model clas for Managing the Order Processing 
    /// </summary>
    public class ProcessOrderInfo
    {
        public int ProcessOrderId { get; set; }
        public int OrderId { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string OrderStatus { get; set; }
    }
}
