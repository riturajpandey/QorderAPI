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
    
    public partial class PatronsHotelSubscription
    {
        public int PatronsHotelSubscriptionID { get; set; }
        public int PatronID { get; set; }
        public int HotelID { get; set; }
        public bool SubscribeSMS { get; set; }
        public bool SubsribeEmail { get; set; }
        public bool SubsribePushNotifications { get; set; }
    
        public virtual Hotel Hotel { get; set; }
        public virtual Patron Patron { get; set; }
    }
}
