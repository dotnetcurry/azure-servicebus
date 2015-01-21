using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderPlacer.Models;

namespace OrderPlacer.Controllers
{
    public class OrderManagerController : Controller
    {


        PlaceOrder obj;

        public OrderManagerController()
        {
            obj = new PlaceOrder();
        }
        // GET: OrderManager
        public ActionResult Index()
        {
            var info = new CustomerOrderItemMaster() { 
             Order =new OrderMaster() 
            };  
            //Customers Info
            ViewBag.CustomerId = new SelectList(obj.GetCustomers(), "CustomerId", "CustomerName");
            //Items Info
            ViewBag.ItemId = new SelectList(obj.GetItems(), "ItemId", "ItemName");
            return View(info);
        }

        [HttpPost]
        public ActionResult Index(CustomerOrderItemMaster c)
        {
            obj.CreateOrder(c);
            ViewBag.CustomerId = new SelectList(obj.GetCustomers(), "CustomerId", "CustomerName");
            ViewBag.ItemId = new SelectList(obj.GetItems(), "ItemId", "ItemName");
            return View(c);
        }


    }
}