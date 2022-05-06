using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class Shipment
    {
        public int Id { get; set; }
        public string Address { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public int UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
