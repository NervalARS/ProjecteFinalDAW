using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class LnOrder
    {
        public long NumOrder { get; set; }
        public long NumLine { get; set; }
        public int Quantity { get; set; }
        public decimal TotalImport { get; set; }
        public long RefProduct { get; set; }

        public virtual Order NumOrderNavigation { get; set; } = null!;
        public virtual Product RefProductNavigation { get; set; } = null!;
    }
}
