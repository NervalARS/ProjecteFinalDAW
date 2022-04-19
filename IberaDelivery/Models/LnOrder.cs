using System;
using System.Collections.Generic;

namespace IberaDelivery.Models
{
    public partial class LnOrder
    {
        public int NumOrder { get; set; }
        public int NumLine { get; set; }
        public int Quantity { get; set; }
        public decimal TotalImport { get; set; }
        public int RefProduct { get; set; }

        public virtual Order NumOrderNavigation { get; set; } = null!;
        public virtual Product RefProductNavigation { get; set; } = null!;
    }
}
