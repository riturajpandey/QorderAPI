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
    
    public partial class HotelMenusExtraDetail
    {
        public int HotelMenusExtraDetailsID { get; set; }
        public int HotelMenuID { get; set; }
        public string IngredientsForPatronsApp { get; set; }
        public string WhereFromForPatronsApp { get; set; }
        public string PercentAlcoholForPatronsApp { get; set; }
    
        public virtual HotelMenu HotelMenu { get; set; }
    }
}
