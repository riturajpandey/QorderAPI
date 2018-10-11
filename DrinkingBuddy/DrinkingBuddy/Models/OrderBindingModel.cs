using System;
using System.Collections.Generic;
using System.Linq;
using DrinkingBuddy.Entities;
using System.Web;

namespace DrinkingBuddy.Models
{
    public class OrderBindingModel
    {
        public int PatronID { get; set; }
        public int HotelID { get; set; }
        public List<OrderMenu> OrderMenus { get; set; }
        public double FinalAmountForOrder { get; set; }
        public string PaymentGatewayReturnID { get; set; }
        public Nullable<System.DateTime> DateTimeOfOrder { get; set; }

    }

    public class OrderMenu
    {
        public int HotelMenuItemId { get; set; }
        public int QTYOrdered { get; set; }
        public int? HotelSpecialID { get; set; }
        public decimal? AcceptedPricePerItem { get; set; }
        public string PatronsOrderDetailsNotes { get; set; }
    }


   //public class OrderModel
   // {
   //     public int HotelID { get; set; }
   //     public int PatronID { get; set; }
   //     public DateTime DateTimeOfOrder { get; set; }
   //     public Double FinalAmountForOrder { get; set; }
   //     public string PaymentGatewayReturnID { get; set; }
   //     public string PatronsOrderNotes { get; set; }

   //     public int PatronsOrdersID { get; set; }
   //     public int HotemMenuItemID { get; set; }
   //     public int QTYOrdered { get; set; }
   //     public string CategoryNameOfMenuItemAtTimeOfBuying { get; set; }
   //     public string SubCategoryNameOfMenuIteAtTimeOfBuying { get; set; }
   //     public string ItemNameAtTimeOfBuying { get; set; }
   //     public int SizeAtTimeOfBuying { get; set; }
   // }


    public class CardModel
    {
        public int PatronsID { get; set; }
        public string PaymentType { get; set; }
        public string PaymentCardholderName { get; set; }
        public string PaymentCardType { get; set; }
        public string PaymentCardNumberEncrypted { get; set; }
        public string PaymentCardCvvCodeEncrypted { get; set; }
        public string PaymentCardExpiryEncrypted { get; set; }
    }

    public class PlaceOrderResponse
    {
        public int OrderId { get; set; }
    }

    public class GroupOrderMenu
    {
        public int HotelMenuItemId { get; set; }
        public int QTYOrdered { get; set; }
        public int? HotelSpecialID { get; set; }
        public decimal? AcceptedPricePerItem { get; set; }
        public string PatronsOrderDetailsNotes { get; set; }
    }

    public class GroupOrderBindingModel
    {
        public int PatronID { get; set; }
        public int HotelID { get; set; }
        public int PatronsGroupID { get; set; }
        public List<GroupOrderMenu> OrderMenus { get; set; }
        public decimal? FinalAmountForOrder { get; set; }
        public string PaymentGatewayReturnID { get; set; }
        public DateTime? DateTimeOfOrder { get; set; }
        public Nullable<int> OpenMinutes { get; set; }
    }

}