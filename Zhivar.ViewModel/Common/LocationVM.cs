using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Zhivar.ViewModel.Common
{
    public class LocationVM
    {
        [Key]
        public int LocId { get; set; }
        [Display(Name = "Description"), MaxLength(8000, ErrorMessage = "Title cannot be more than 8000 characters long."), Required(ErrorMessage = "You must enter a description in {0} field")]

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Park/Location Name"), MaxLength(120, ErrorMessage = "Park or Location name is too long"), Required]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:N10}", ApplyFormatInEditMode = true), Required]
        public decimal Longitude { get; set; }

        [DisplayFormat(DataFormatString = "{0:N10}", ApplyFormatInEditMode = true), Required]
        public decimal Latitude { get; set; }
        
    }

}
