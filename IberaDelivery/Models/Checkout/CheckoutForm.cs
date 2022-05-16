using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace IberaDelivery.Models
{
    public partial class CheckoutForm
    {
        public CheckoutForm()
        {

        }
        public int ShipmentId { get; set; }
        public int CardId { get; set; }

    }
}