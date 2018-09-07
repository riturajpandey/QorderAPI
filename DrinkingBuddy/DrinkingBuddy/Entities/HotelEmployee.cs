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
    
    public partial class HotelEmployee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HotelEmployee()
        {
            this.HotelDiscountsGivens = new HashSet<HotelDiscountsGiven>();
            this.HotelEmployeesAvailabilityTimes = new HashSet<HotelEmployeesAvailabilityTime>();
            this.HotelEmployeesRosters = new HashSet<HotelEmployeesRoster>();
            this.HotelMarketingPushNotifications = new HashSet<HotelMarketingPushNotification>();
            this.HotelMarketingPushNotifications1 = new HashSet<HotelMarketingPushNotification>();
            this.HotelMarketingSms = new HashSet<HotelMarketingSm>();
            this.HotelMarketingSms1 = new HashSet<HotelMarketingSm>();
            this.HotelSavedOrders = new HashSet<HotelSavedOrder>();
        }
    
        public int HotelEmployeeID { get; set; }
        public Nullable<System.Guid> HotelEmployeeGUID { get; set; }
        public Nullable<int> HotelID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string PostCode { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<int> SecretPin { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public string MasterPassword { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelDiscountsGiven> HotelDiscountsGivens { get; set; }
        public virtual Hotel Hotel { get; set; }
        public virtual StatesG StatesG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelEmployeesAvailabilityTime> HotelEmployeesAvailabilityTimes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelEmployeesRoster> HotelEmployeesRosters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelMarketingPushNotification> HotelMarketingPushNotifications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelMarketingPushNotification> HotelMarketingPushNotifications1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelMarketingSm> HotelMarketingSms { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelMarketingSm> HotelMarketingSms1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelSavedOrder> HotelSavedOrders { get; set; }
    }
}
