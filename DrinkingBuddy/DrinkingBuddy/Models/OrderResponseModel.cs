using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrinkingBuddy.Entities;

namespace DrinkingBuddy.Models
{

    public class CouponResponseModel
    {
        private string _BarName;
      
        public int HotelMarketingCouponId { get; set; }
        public int HotelID { get; set; }
        public string CouponName { get; set; }
        public string CouponCode { get; set; }
        public string BarName
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Hotel = _context.Hotels.Where(m => m.HotelID == this.HotelID).FirstOrDefault();
                    if (Hotel != null)
                    {
                        if (Hotel.HotelName != null)
                        { _BarName = Hotel.HotelName; }
                        else { _BarName = ""; }
                    }
                    else
                    {
                        _BarName = "";
                    }
                }
                return _BarName;
            }



        }
        public Nullable<System.DateTime> CouponEndDate { get; set; }
        public Nullable<System.DateTime> CouponEndTime { get; set; }

    }

    public class SpecialReponseModel
    {
        private DateTime _EndDate;
        private string _HotelName;
        public int HotelSpecialID { get; set; }
        public int HotelId { get; set; }
        public string DescriptionOfSpecial { get; set; }
        public DateTime EndDate
        {

            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Special = _context.HotelSpecialsMetas.Where(m => m.HotelSpecialID == this.HotelSpecialID).FirstOrDefault();
                    Special.EndDate = this._EndDate;
                }
                return _EndDate;


            }

        }
        public string HotelName
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Special = _context.Hotels.Where(m => m.HotelID == this.HotelId).FirstOrDefault();
                    Special.HotelName = this._HotelName;
                }
                return _HotelName;


            }
        }



    }



    public class CurrentOrderResponse
    {
        public int PatronsOrdersID { get; set; }
        public string HotelName { get; set; }
        public DateTime? DateTimeOfOrder { get; set; }
        public Nullable<decimal> FinalAmount { get; set; }
        public string Status { get; set; }
        public int? EstMinutes { get; set; }
        public int? DrinkCount { get; set; }
        public Nullable<int> LinQ { get; set; }
        public List<CurrentDrink> DrinkList { get; set; }
    }


    public class CurrentDrink
    {
        public string DrinkName { get; set; }
        public int? QTYOrdered { get; set; }
        public string Size { get; set; }
        public Nullable<decimal> Price { get; set; }
        public int? EstMinutes { get; set; }

    }

    public class OrderHistoryResponse
    {
        public int PatronsOrdersID { get; set; }
        public string HotelName { get; set; }
        public DateTime? DateTimeOfOrder { get; set; }
        public Nullable<decimal> FinalAmount { get; set; }
        public int? DrinkCount { get; set; }
        public List<DrinkHistory> DrinkList { get; set; }
    }

    public class DrinkHistory
    {
        public string DrinkName { get; set; }
        public string Size { get; set; }
        public Nullable<decimal> Price { get; set; }
        public int? QuantityOrdered { get; set; }
    }

    public class TrackingResponse
    {
        public string Status { get; set; }
        public int? EstMinutes { get; set; }

    }

    public class CheckInitialResponse
    {
        public bool IsInitiated { get; set; }
        public int? PatronID { get; set; }

    }


    public class CheckIntialIneral {

        public string OrderTime { get; set; }
        public int? PatronID { get; set; }


    }




}




