using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class Image
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public byte[] Image1 { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
