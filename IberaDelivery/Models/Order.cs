﻿using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class Order
    {
        public Order()
        {
            LnOrders = new HashSet<LnOrder>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Import { get; set; }
        public int UserId { get; set; }
        public int? ShipmentId { get; set; }
        public int? CreditCardId { get; set; }

        public virtual CreditCard? CreditCard { get; set; }
        public virtual Shipment? Shipment { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual ICollection<LnOrder> LnOrders { get; set; }
    }
}
