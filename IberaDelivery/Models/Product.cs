using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IberaDelivery.Models
{
    public partial class Product
    {
        public Product()
        {
            Comments = new HashSet<Comment>();
            Images = new HashSet<Image>();
            LnOrders = new HashSet<LnOrder>();
        }

        public int Id { get; set; }

        [StringLength(20, MinimumLength = 1)]
        [Required]
        public string Name { get; set; } = null!;

        [StringLength(500, MinimumLength = 1)]
        [Required]
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int ProviderId { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal Iva { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual User Provider { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<LnOrder> LnOrders { get; set; }

        
    }
}
