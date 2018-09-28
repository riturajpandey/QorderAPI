using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrinkingBuddy.Entities;

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
        private decimal? _DrinkPrice { get; set; }
        public int HotelSpecialID { get; set; }
        public Nullable<int> HotelSpecialsMetaID { get; set; }
        public bool WillRepeat { get; set; }
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
        public decimal? DrinkPrice
        {

            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.HotelMenus.Where(m => m.DrinkName == this.DrinkName).FirstOrDefault();
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

    }
}






