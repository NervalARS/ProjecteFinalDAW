using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class Comment
    {
        public long Id { get; set; }
        public string? Contens { get; set; }
        public long UserId { get; set; }
        public long ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
