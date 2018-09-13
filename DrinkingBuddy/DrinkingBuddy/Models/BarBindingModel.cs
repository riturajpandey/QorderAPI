using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrinkingBuddy.Models
{
    public class BarBindingModel
    {


    }

    public class BarMapModel
    {
        public string SessionToken { get; set; }
        public int? PatronID { get; set; }
        public decimal? CurrentLat { get; set; }
        public decimal? CurrentLong { get; set; }
    }

}