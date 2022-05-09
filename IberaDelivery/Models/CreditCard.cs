using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class CreditCard
    {
        public int Id { get; set; }
        public string TargetNumber { get; set; } = null!;
        public int UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
