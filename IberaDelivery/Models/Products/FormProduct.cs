using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IberaDelivery.Models
{
    public partial class FormProduct
    {
        public FormProduct()
        {

        }

         public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int ProviderId { get; set; }
        public int Stock { get; set; }
        [RegularExpression(@"^\d+(.\d{1,2})?$", ErrorMessage ="Format is not valid")]
        public string Price { get; set; }
        [RegularExpression(@"^\d+(.\d{1,2})?$", ErrorMessage ="Format is not valid")]
        public string Iva { get; set; }

        public IFormFile[] Image { get; set; } = null!;

        //public Image[] Image2 { get; set; } = null!;

       

    }
}
