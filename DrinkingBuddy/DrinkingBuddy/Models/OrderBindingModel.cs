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

    public class CouponBindingModel
    {
       public int PatronsId { get; set; }
    }

   

    public class BarConnect
    {



    }
}