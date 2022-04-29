using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class Adress
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public int PostalCode { get; set; }
        public int TargetNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Cvv { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
