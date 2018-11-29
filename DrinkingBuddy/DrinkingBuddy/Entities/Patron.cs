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
    
    public partial class Patron
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Patron()
        {
            this.HotelMarketingCouponsPatrons = new HashSet<HotelMarketingCouponsPatron>();
            this.HotelMarketingNewslettersPatrons = new HashSet<HotelMarketingNewslettersPatron>();
            this.HotelMarketingPushNotificationsPatrons = new HashSet<HotelMarketingPushNotificationsPatron>();
            this.HotelMarketingSmsPatrons = new HashSet<HotelMarketingSmsPatron>();
            this.PatronsHotelLogIns = new HashSet<PatronsHotelLogIn>();
            this.PatronsHotelSubscriptions = new HashSet<PatronsHotelSubscription>();
            this.PatronsOrders = new HashSet<PatronsOrder>();
            this.PatronsOrdersRefunds = new HashSet<PatronsOrdersRefund>();
            this.PatronsPaymentMethods = new HashSet<PatronsPaymentMethod>();
            this.PatronsPreferences = new HashSet<PatronsPreference>();
            this.PatronsResetPasswordTokens = new HashSet<PatronsResetPasswordToken>();
            this.PatronsSessionTokens = new HashSet<PatronsSessionToken>();
            this.TrackGroupOrders = new HashSet<TrackGroupOrder>();
        }
    
        public int PatronsID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneMobile { get; set; }
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string PostCode { get; set; }
        public Nullable<int> StateID { get; set; }
        public bool OptInForMarketing { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string Gassword { get; set; }
        public bool IsActive { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
        public Nullable<System.DateTime> RegisterOn { get; set; }
        public Nullable<System.DateTime> LastLogOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelMarketingCouponsPatron> HotelMarketingCouponsPatrons { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelMarketingNewslettersPatron> HotelMarketingNewslettersPatrons { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelMarketingPushNotificationsPatron> HotelMarketingPushNotificationsPatrons { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelMarketingSmsPatron> HotelMarketingSmsPatrons { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatronsHotelLogIn> PatronsHotelLogIns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatronsHotelSubscription> PatronsHotelSubscriptions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatronsOrder> PatronsOrders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatronsOrdersRefund> PatronsOrdersRefunds { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatronsPaymentMethod> PatronsPaymentMethods { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatronsPreference> PatronsPreferences { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatronsResetPasswordToken> PatronsResetPasswordTokens { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatronsSessionToken> PatronsSessionTokens { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrackGroupOrder> TrackGroupOrders { get; set; }
    }
}
