using System;
using System.Collections.Generic;
using System.Linq;
using DrinkingBuddy.Entities;
using System.Web;

namespace DrinkingBuddy.Models
{
    public class MemberGroupModel
    {
        public int HotelID { set; get; }
        public int MasterPatronID { get; set; }
        public DateTime GroupStartedDateTime { get; set; }
    }
}