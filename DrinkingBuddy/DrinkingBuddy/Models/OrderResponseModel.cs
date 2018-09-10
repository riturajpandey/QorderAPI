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

        public int HotelSpecialID { get; set; }
        public int HotelId { get; set; }
        public string DescriptionOfSpecial { get; set; }
        public DateTime EndDate { get; set; }
        public string HotelName { get; set; }



    }
}