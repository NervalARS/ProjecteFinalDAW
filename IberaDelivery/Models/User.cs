using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            Orders = new HashSet<Order>();
            Products = new HashSet<Product>();
        }

        public long Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Rol { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
