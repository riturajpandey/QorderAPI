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
    
    public partial class HotelMarketingCoupon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HotelMarketingCoupon()
        {
            this.HotelMarketingCouponsPatrons = new HashSet<HotelMarketingCouponsPatron>();
        }
    
        public int HotelMarketingCouponID { get; set; }
        public int HotelID { get; set; }
        public string CouponName { get; set; }
        public string CouponCode { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> SentBy { get; set; }
        public Nullable<System.DateTime> SentDate { get; set; }
        public Nullable<System.DateTime> CouponStartDate { get; set; }
        public Nullable<System.DateTime> CouponEndDate { get; set; }
        public Nullable<System.DateTime> CouponStartTime { get; set; }
        public Nullable<System.DateTime> CouponEndTime { get; set; }
        public string DiscountOrFreeItem { get; set; }
        public string DiscountAmountOrPercent { get; set; }
        public string SpecificPatronsOrAll { get; set; }
        public Nullable<int> DiscountPercent { get; set; }
        public Nullable<decimal> NewAmount { get; set; }
        public Nullable<int> ItemMenuID { get; set; }
        public Nullable<int> FreeItemQty { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelMarketingCouponsPatron> HotelMarketingCouponsPatrons { get; set; }
    }
}
