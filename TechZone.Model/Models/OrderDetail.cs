using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechZone.Model.Models
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { set; get; }

        public int OrderID { set; get; }

        public int ProductID { set; get; }

        public int Quantity { set; get; }

        public decimal Price { set; get; }

        public DateTime? OrderDate { set; get; }

        public DateTime? DeliveryDate { set; get; }

        public bool IsOrder { set; get; }

        public bool IsDelivery { set; get; }

        [ForeignKey("OrderID")]
        public virtual Order Order { set; get; }

        [ForeignKey("ProductID")]
        public virtual Product Product { set; get; }
    }
}