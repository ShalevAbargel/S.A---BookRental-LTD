using S.A___BookRental_LTD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace S.A___BookRental_LTD.ViewModel
{
    public class ThumbnailBoxViewModel
    {
        public IEnumerable<ThumbnailModel> Thumbnails { get; set; }
    }
}