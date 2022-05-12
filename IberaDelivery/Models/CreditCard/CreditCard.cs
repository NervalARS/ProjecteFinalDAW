using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class CreditCard
    {
        public CreditCard()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string CardNumber { get; set; } = null!;
        public int UserId { get; set; }
        public string Cardholder { get; set; } = null!;

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
    }
}
