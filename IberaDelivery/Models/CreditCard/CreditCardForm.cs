using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IberaDelivery.Models
{
    public partial class CreditCardForm
    {

        [Required]
        public string TargetNumber { get; set; } = null!;
        [Required]
        public string Cardholder { get; set; } = null!;
    }
}
