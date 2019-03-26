using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RequisitionPortal.Models
{
    public class VendorModel
    {
        public int Id { get; set; }

        [Required, Display(Name = "Vendor ID")]
        public string VendorUID { get; set; }

        [Required(ErrorMessage ="Select a Name")]
        public string Name { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        [Display(Name = "Zip Code")]
        public string CitySTZip { get; set; }

        public string Contact { get; set; }

        [Display(Name = "Phone Number")]
        public string Telephone1 { get; set; }

        [Display(Name = "Mobile Number")]
        public string Telephone2 { get; set; }

        [Display(Name = "Tax ID Number")]
        public string TaxIDNo { get; set; }

        [Display(Name = "Fax Number")]
        public string FaxNo { get; set; }

        public string Terms { get; set; }

        [Display(Name = "Vendor Since")]
        public DateTime? VendSince { get; set; }

        public string Email { get; set; }
    }
}