using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IberaDelivery.Models
{
    public partial class ViewUserLogin
    {
        public ViewUserLogin()
        {
        }
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}