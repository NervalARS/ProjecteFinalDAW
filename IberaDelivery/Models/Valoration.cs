using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class Valoration
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
