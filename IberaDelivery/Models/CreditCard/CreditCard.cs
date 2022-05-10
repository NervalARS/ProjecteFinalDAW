using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class CreditCard
    {
        public int Id { get; set; }
        public string CardNumber { get; set; } = null!;
        public int UserId { get; set; }
        public string Cardholder { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
