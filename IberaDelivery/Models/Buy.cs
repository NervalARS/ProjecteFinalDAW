using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace IberaDelivery.Models
{
    public partial class Buy
    {
        public Buy()
        {

        }
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public int PostalCode { get; set; }
        [Required]
        public int TargetNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-yyyy}", ApplyFormatInEditMode = false)]
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Range(100,999,ErrorMessage="Rating must between 100 to 999")] 
        [Required]
        public int Cvv { get; set; }

    

       

    }
}
