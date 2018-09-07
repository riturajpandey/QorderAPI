using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrinkingBuddy.Entities;

namespace DrinkingBuddy.Models
{
    public class HotelResponseModel
    {
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress1 { get; set; }
        public string HotelAddress2 { get; set; }
        public string HotelSuburb { get; set; }
        public string HotelPostcode { get; set; }
        public Nullable<int> HotelStateID { get; set; }
        public byte[] HotelLogo { get; set; }
        public bool UseStartOrder { get; set; }
        public bool UseSpecialCodeForPickup { get; set; }
        public string FacebookLink { get; set; }
        public string TwitterAccount { get; set; }
        public Nullable<decimal> HotelLat { get; set; }
        public Nullable<decimal> HotelLong { get; set; }
        public string OptionBarSalesFirstOrderOrFastestWay { get; set; }
        public bool UseLocationsForOrders { get; set; }
    }


    public class HotelMenuCatagoriesResponseModel
    {
        private int _Drinkcount;

        public int HotelMenuCategoryID { get; set; }
        public int HotelID { get; set; }
        public string CategoryName { get; set; }
        public int Drinkcount
        {
            get{

                using(DrinkingBuddyEntities _context=new DrinkingBuddyEntities())
                {
                    _Drinkcount = _context.HotelMenus.Where(m => m.HotelCategoryID == this.HotelMenuCategoryID).Count();
               }

                return _Drinkcount;
            }



        }
        public bool IsActive { get; set; }
        public byte[] CategoryImage { get; set; }
        public Nullable<int> ViewOrder { get; set; }
        public Nullable<int> ReportOrder { get; set; }

    }

    public class HotelsMenuResponseModel
    {
        public int HotelMenuID { get; set; }
        public int HotelID { get; set; }
        public int HotelCategoryID { get; set; }
        public Nullable<int> HotelSubCategoryID { get; set; }
        public string DrinkName { get; set; }
        public Nullable<decimal> DrinkSize { get; set; }
        public string DrinkUnitMlLitreUnit { get; set; }
        public Nullable<decimal> DrinkPrice { get; set; }
        public byte[] DrinkImage { get; set; }
        public Nullable<int> MinutesToServeItem { get; set; }
        public bool IsActive { get; set; }
        public Nullable<decimal> CostOfItem { get; set; }
        public bool OnBarSalesQuickList { get; set; }
        public Nullable<int> HotelTaxGroupID { get; set; }
        public string IngredientsForPatronsApp { get; set; }
        public string WhereFromForPatronsApp { get; set; }
        public string PercentAlcoholForPatronsApp { get; set; }

    }

    public class IngredientResponseModel
    {
        public string DrinkIngredient { get; set; }
        public string AlcoholPrecent { get; set; }

    }


}