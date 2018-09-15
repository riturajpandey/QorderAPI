using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrinkingBuddy.Entities;

namespace DrinkingBuddy.Models
{

    public class CouponResponseModel
    {
        private string _FreeItemMenu;
        public int HotelMarketingCouponId { get; set; }
        public string CouponName { get; set; }
        public string CouponCode { get; set; }
        public DateTime CouponEndTime { get; set; }
        public int DiscountPercent { get; set; }
        public int FreeItemMenuId { get; set; }
        public string FreeItemMenu
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Menu = _context.HotelMenus.Where(m => m.HotelMenuID == this.FreeItemMenuId).FirstOrDefault();
                    Menu.DrinkName = this._FreeItemMenu;
                }
                return _FreeItemMenu;
            }
        }
        public int FreeItemQty { get; set; }
    }   

    public class SpecialReponseModel {
        private DateTime _EndDate;
        private string _HotelName;
        public int HotelSpecialID { get; set; }
        public int HotelId { get; set; }
        public string DescriptionOfSpecial { get; set; }
        public DateTime EndDate {

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
        public string HotelName {
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
}