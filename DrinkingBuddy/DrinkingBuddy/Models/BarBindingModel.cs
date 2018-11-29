using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrinkingBuddy.Entities;

namespace DrinkingBuddy.Models
{
    public class BarBindingModel
    {

    }

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

    }

    public class PatronsFavouritesResponseModel
    {
        private string _HotelCatagoryName;
        private string _HotelSubCatagoryName;
        private string _HotelName;
        public int PatronsFavouriteID { get; set; }
        public int HotelCatagoryID { get; set; }
        public int HotelSubCatagoryID { get; set; }
        public int HotelMenuID { get; set; }
        public string HotelCatagoryName
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.HotelMenusCategories.Where(m => m.HotelMenuCategoryID == this.HotelCatagoryID).FirstOrDefault();
                    if (Catagory == null)
                    {
                        _HotelCatagoryName = "";
                    }
                    else
                    {
                        _HotelCatagoryName = Catagory.CategoryName;
                    }
                }

                return _HotelCatagoryName;
            }

        }
        public string HotelSubCatagoryName
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.HotelMenusSubCategories.Where(m => m.HotelMenusSubCategoryID == this.HotelSubCatagoryID).FirstOrDefault();
                    if (Catagory == null)
                    {
                        _HotelSubCatagoryName = "";
                    }
                    else
                    {
                        _HotelSubCatagoryName = Catagory.SubCategoryName;
                    }
                }

                return _HotelSubCatagoryName;

            }

        }
        public string HotelMenuName
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.HotelMenus.Where(m => m.HotelMenuID == HotelMenuID).FirstOrDefault();
                    if (Catagory == null)
                    {
                        _HotelName = "";
                    }
                    else
                    {
                        _HotelName = Catagory.DrinkName;
                    }
                }

                return _HotelName;

            }


        }


    }

    public class FavoriteModel
    {
        public int PatronID { get; set; }
        public int HotelID { get; set; }
        public int HotelCatagoryID { get; set; }
        public int HotelSubCatagoryID { get; set; }
        public int HotelMenuID { get; set; }
    }

    public class SpecialResponsemodel
    {
        private decimal? _DrinkPrice;
        private bool _IsFavourite;
        private string _Ingredient;
        private string _AlcoholPercent;
        private decimal? _DrinkSize;
        private string _DrinkName;

        public int HotelSpecialID { get; set; }
        // public string CategoryName { get; set; }
        // public string SubCategoryName { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> SubCategoryID { get; set; }
        public string DrinkName
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.HotelMenus.Where(m => m.HotelMenuID == this.HotelMenuID).FirstOrDefault();
                    if (Catagory == null)
                    {
                        _DrinkName = "";
                    }
                    else
                    {
                        _DrinkName = Catagory.DrinkName;
                    }
                }
                return _DrinkName;
            }


        }
        public Nullable<decimal> DrinkSize
        {
            get
            {

                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.HotelMenus.Where(m => m.HotelMenuID == this.HotelMenuID).FirstOrDefault();
                    if (Catagory == null)
                    {
                        _DrinkSize = 0;
                    }
                    else
                    {
                        _DrinkSize = Catagory.DrinkSize;
                    }
                }

                return _DrinkSize;
            }
        }
        public int HotelMenuID { get; set; }
        public string DescriptionOfSpecial { get; set; }
        public Nullable<decimal> NewAmount { get; set; }
        public decimal? DrinkPrice
        {

            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.HotelMenus.Where(m => m.HotelMenuID == this.HotelMenuID).FirstOrDefault();
                    if (Catagory == null)
                    {
                        _DrinkPrice = 0;
                    }
                    else
                    {
                        _DrinkPrice = Catagory.DrinkPrice;
                    }
                }

                return _DrinkPrice;
            }
        }
        public string Ingredient
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Data = _context.HotelMenus.Where(m => m.HotelMenuID == this.HotelMenuID).FirstOrDefault();
                    if (Data == null)
                    {
                        _Ingredient = "";
                    }
                    else
                    {
                        _Ingredient = Data.IngredientsForPatronsApp;
                    }
                }
                return _Ingredient;
            }

        }
        public string AlcoholPercent
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Data = _context.HotelMenus.Where(m => m.HotelMenuID == this.HotelMenuID).FirstOrDefault();
                    if (Data == null)
                    {
                        _AlcoholPercent = "";
                    }
                    else
                    {
                        _AlcoholPercent = Data.PercentAlcoholForPatronsApp;
                    }
                }
                return _AlcoholPercent;

            }

        }
        public bool IsFavourite
        {

            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {

                    var data = _context.PatronsFavourites.Where(m => m.HotelMenuID == this.HotelMenuID).FirstOrDefault();
                    if (data == null)
                    {
                        _IsFavourite = false;

                    }
                    else
                    {
                        _IsFavourite = true;
                    }
                }
                return _IsFavourite;

            }
        }

    }

    public class CouponValidation
    {
        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public int? HotelMarketingCouponID { get; set; }

    }

    public class GetCouponResponse
    {
        public int? DiscountPercent { get; set; }
        public decimal? NewAmount { get; set; }
        public Freeitem Freeitem { get; set; }


    }

    public class Freeitem
    {
        public int ItemMenuID { get; set; }
        public string DrinkName { get; set; }
        public int? FreeItemQty { get; set; }
        public decimal? DrinkSize { get; set; }
        public string DrinkUnitMlLitreUnit { get; set; }
        public string IngredientsForPatronsApp { get; set; }
        public string PercentAlcoholForPatronsApp { get; set; }

    }


}



