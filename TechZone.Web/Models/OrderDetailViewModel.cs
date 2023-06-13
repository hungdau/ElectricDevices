using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechZone.Model.Models;

namespace TechZone.Web.Models
{
    public class OrderDetailViewModel
    {
        public int ID { set; get; }

        public int OrderID { set; get; }

        public int ProductID { set; get; }

        public int Quantity { set; get; }

        public decimal Price { set; get; }

        public DateTime? OrderDate { set; get; }

        public DateTime? DeliveryDate { set; get; }

        public bool IsOrder { set; get; }

        public bool IsDelivery { set; get; }

        public ProductViewModel Product { set; get; }
    }
}