using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrinkingBuddy.Models
{
    public class BarBindingModel { }

    public class BarMapModel
    {
        public string Token { get; set; }
        public int PatronID { get; set; }
        public decimal CurrentLat { get; set; }
        public decimal CurrentLong { get; set; }
    }

    public class ConnnectBarModel
    {
        public int PatronId { get; set; }
        public int HotelId { get; set; }
    }

    public class LeaveBarModel
    {
        public int HotelID { get; set; }
        public int PatronID { get; set; }
        public DateTime LogoutDateTime { get; set; }
    }

}






