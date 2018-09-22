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


    public class HotelSpecial
    {
        public int HotelSpecialID { get; set; }
        public Nullable<int> HotelSpecialsMetaID { get; set; }
        public string DiscountType { get; set; }
        public Nullable<decimal> DiscountAmountG { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> SubCategoryID { get; set; }
        public string DrinkName { get; set; }
        public Nullable<decimal> DrinkSize { get; set; }
        public string DescriptionOfSpecial { get; set; }
        public Nullable<System.DateTime> StartDateG { get; set; }
        public Nullable<System.DateTime> EndDateG { get; set; }
        public Nullable<System.DateTime> StartTimeG { get; set; }
        public Nullable<System.DateTime> EndTimeG { get; set; }
        public string RepeatInterval { get; set; }


    }

    

}






