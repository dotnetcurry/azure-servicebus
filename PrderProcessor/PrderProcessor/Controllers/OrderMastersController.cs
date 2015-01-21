using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using OrderProcessor.Models;

namespace OrderProcessor.Controllers
{
    public class OrderMastersController : Controller
    {
        private CompanyEntities1 db = new CompanyEntities1();

        // GET: OrderMasters
        public ActionResult Index()
        {
            List<OrderMaster> Orders = new List<OrderMaster>(); 

            //Get Message from Queue
            var msgs = new QueueMessageReceiver().GetOrdersFromQueue();

            if (msgs!=null)
            {
                var orderMasters = db.OrderMasters.Include(o => o.CustomerMaster).Include(o => o.ItemMaster);

                foreach (var msg in msgs)
                {
                    foreach (var ord in orderMasters)
                    {
                        if (msg.OrderId == ord.OrderId)
                        {
                            Orders.Add(ord);
                            break;
                        }

                    }
                }

                return View(Orders);
            }
            else
            {
                return View("Index");
            }
        }

        /// <summary>
        /// Process Order View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProcessOrder(int id)
        {
            var Ord = db.OrderMasters.Find(id);

            ProcessOrderInfo ordProcess = new ProcessOrderInfo();
            ordProcess.OrderId = Ord.OrderId;
            ordProcess.CustomerId = Ord.CustomerId;
            ordProcess.CustomerName = db.CustomerMasters.Find(Ord.CustomerId).CustomerName;
            ordProcess.ItemId = Ord.ItemId;
            ordProcess.ItemName = db.ItemMasters.Find(Ord.ItemId).ItemName;
            ordProcess.Quantity = Ord.Quantity;
            ordProcess.AvailableQuantity = db.ItemMasters.Find(Ord.ItemId).AvailableQuantity;
            ordProcess.OrderStatus = "Processing";
            ViewBag.OrderStatus = new SelectList(new List<string>() {"Approved","Rejected"});
            return View("ProcessOrder",ordProcess);
        }

        [HttpPost]
        public ActionResult ProcessOrder(ProcessOrderInfo p)
        {
            ProcessOrder OrdToProcess = new ProcessOrder();

            OrdToProcess.OrderId = p.OrderId;
            OrdToProcess.CustomerId = p.CustomerId;
            OrdToProcess.ItemId = p.ItemId;
            OrdToProcess.OrderStatus = p.OrderStatus;

            //Save th processor Order Data
            db.ProcessOrders.Add(OrdToProcess);
            db.SaveChanges();

            if(OrdToProcess.OrderStatus=="Approved")
            { 
                //Update Item Quantity
                var itemUpdated = db.ItemMasters.Find(OrdToProcess.ItemId);
                itemUpdated.AvailableQuantity -= p.Quantity;
                db.SaveChanges();
            }

            //Delete the Order if it is Approved
            var order = db.OrderMasters.Find(OrdToProcess.OrderId);
            db.OrderMasters.Remove(order);
            db.SaveChanges();
            return View("ProcessedOrders", db.ProcessOrders.ToList());

        }
       
    }
}
