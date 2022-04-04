using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        public string? Contens { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
