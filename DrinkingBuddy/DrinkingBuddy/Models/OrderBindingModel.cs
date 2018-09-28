﻿using System;
using System.Collections.Generic;
using System.Linq;
using DrinkingBuddy.Entities;
using System.Web;

namespace DrinkingBuddy.Models
{
    public class OrderBindingModel
    {
        public int PatronId { get; set; }
        public int HotelId { get; set; }
        public List<OrderMenu> OrderMenus { get; set; }
        public double FinalAmountForOrder { get; set; }
        public string PaymentGatewayReturnID { get; set; }
        public DateTime? DateTimeOfOrder { get; set; }
        public string PatronsOrderNotes { get; set; }
    }

    public class OrderMenu
    {
        public int PatronsOrdersID { get; set; }
        public int HotelMenuItemId { get; set; }
        public int QTYOrdered { get; set; }
        public int HotelSpecialID { get; set; }
        public double AcceptedPricePerItem { get; set; }
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

    

}