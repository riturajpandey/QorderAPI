//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrinkingBuddy.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class PatronsOrdersFastestWay
    {
        public int PatronsOrdersFastestWayID { get; set; }
        public int HotelID { get; set; }
        public int EmployeeID { get; set; }
        public int PatronsOrderID { get; set; }
        public int PatronsOrderDetailID { get; set; }
        public int MenuItemID { get; set; }
        public Nullable<int> QtyOrdered { get; set; }
        public bool StartedG { get; set; }
        public bool CompletedG { get; set; }
    }
}
