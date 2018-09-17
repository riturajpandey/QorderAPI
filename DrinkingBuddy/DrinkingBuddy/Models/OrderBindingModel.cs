using System;
using System.Collections.Generic;
using System.Linq;
using DrinkingBuddy.Entities;
using System.Web;

namespace DrinkingBuddy.Models
{
    public class OrderBindingModel
    {
        public int Id { get; set; }
        public List<OrderMenu> OrderMenus { get; set; }
        public int PatronId { get; set; }
        public int HotelId { get; set; }
        public double TotalPrice { get; set; }
        public string PaymentGatwayId { get; set; }
        public DateTime LoginDateTime { get; set; }
        public DateTime LogoutDateTime { get; set; }

    }

    public class OrderMenu
    {
        public int HotelMenuId { get; set; }
        public string HotelMenu { get; set; }
        public string SizeAtTimeOfBuying { get; set; }
        public string PatronsOrderDetailsNotes { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }


    public class OrderModel
    {
        public int HotelID { get; set; }
        public int PatronID { get; set; }
        public DateTime DateTimeOfOrder { get; set; }
        public Double FinalAmountForOrder { get; set; }
        public string PaymentGatewayReturnID { get; set; }
        public string PatronsOrderNotes { get; set; }

        public int PatronsOrdersID { get; set; }
        public int HotemMenuItemID { get; set; }
        public int QTYOrdered { get; set; }
        public string CategoryNameOfMenuItemAtTimeOfBuying { get; set; }
        public string SubCategoryNameOfMenuIteAtTimeOfBuying { get; set; }
        public string ItemNameAtTimeOfBuying { get; set; }
        public int SizeAtTimeOfBuying { get; set; }
    }


    public class CardModel
    {
        public int PatronID { get; set; }
        public string PaymentType { get; set; }
        public string PaymentCardholderName { get; set; }
        public string PaymentCardType { get; set; }
        public string PaymentCardNumberEncrypted { get; set; }
        public string PaymentCardCvvCodeEncrypted { get; set; }
        public string PaymentCardExpiryEncrypted { get; set; }


    }

}