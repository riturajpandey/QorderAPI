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
    
    public partial class HotelDiscountsGiven
    {
        public int HotelDiscountsGivenID { get; set; }
        public int HotelID { get; set; }
        public int HotelEmployeeID { get; set; }
        public int HotelDiscountReasonID { get; set; }
    
        public virtual HotelDiscountReason HotelDiscountReason { get; set; }
        public virtual HotelEmployee HotelEmployee { get; set; }
        public virtual Hotel Hotel { get; set; }
    }
}
