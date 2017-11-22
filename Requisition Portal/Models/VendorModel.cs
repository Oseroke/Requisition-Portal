using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Requisition_Portal.Models
{
    public class VendorModel
    {
        public int Id { get; set; }

        [Required, Display(Name = "Vendor ID")]
        public string VendorUID { get; set; }

        [Required(ErrorMessage ="Select a Name")]
        public string Name { get; set; }
        public string Address { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        public string Email { get; set; }
    }
}