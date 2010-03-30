﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spoffice.Website.Models.Output
{
    public class StatusOutput
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public FavouriteOutput Favourite { get; set; }
    }
}
