using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace S.A___BookRental_LTD.Models
{
    public class Genre
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [DisplayName("Genre Name")]
        public string Name { get; set; }

    }
}