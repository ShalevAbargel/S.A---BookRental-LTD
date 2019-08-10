using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace S.A___BookRental_LTD.Models
{
    public class ThumbnailModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
        public string GenereName { get; set; }
        public Boolean MostRecomended = false;
    }
}