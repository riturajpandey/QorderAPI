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
    
    public partial class HotelMarketingNewslettersPatron
    {
        public int HotelMarketingNewsletterPatronsID { get; set; }
        public int HotelMarketingNewsletterID { get; set; }
        public int PatronID { get; set; }
    
        public virtual HotelMarketingNewsletter HotelMarketingNewsletter { get; set; }
        public virtual Patron Patron { get; set; }
    }
}
