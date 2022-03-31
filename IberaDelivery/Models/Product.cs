﻿using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class Product
    {
        public Product()
        {
            Comments = new HashSet<Comment>();
            LnOrders = new HashSet<LnOrder>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long CategoryId { get; set; }
        public long ProviderId { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal Iva { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual User Provider { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<LnOrder> LnOrders { get; set; }
    }
}
