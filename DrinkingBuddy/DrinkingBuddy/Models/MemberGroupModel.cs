using System;
using System.Collections.Generic;
using System.Linq;
using DrinkingBuddy.Entities;
using System.Web;

namespace DrinkingBuddy.Models
{
    public class StartGroupBinding
    {
        public int HotelID { set; get; }
        public int MasterPatronID { get; set; }
        public Nullable<System.DateTime> GroupStartedDateTime { get; set; }

    }

    public class StartGroupResponse
    {
       public int PatronsGroupID { get; set; }
       public string HotelName { get; set; }
       public DateTime? GroupStartDateTime { get; set; }
       public string GroupMasterPatron { get; set; }

    }

    public class StopGroup
    {
        public int PatronID { get; set; }
        public int HotelID { get; set; }
       public DateTime GroupStopDateTime { get; set; }
    }
}