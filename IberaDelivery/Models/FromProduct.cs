﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;  

namespace IberaDelivery.Models
{
    public partial class FromProduct
    {
        public FromProduct()
        {
            
        }

      
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int ProviderId { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal Iva { get; set; }

       public IFormFile[] Image { get; set; }

       
    }
}