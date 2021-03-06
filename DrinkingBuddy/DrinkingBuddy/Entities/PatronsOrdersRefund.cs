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
    
    public partial class PatronsOrdersRefund
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PatronsOrdersRefund()
        {
            this.PatronsOrdersRefundsDetails = new HashSet<PatronsOrdersRefundsDetail>();
        }
    
        public int PatronsOrdersRefundID { get; set; }
        public int HotelID { get; set; }
        public int PatronID { get; set; }
        public Nullable<int> HotelRefundReasonID { get; set; }
        public Nullable<System.DateTime> DateTimeOfRefund { get; set; }
        public Nullable<int> EmployeeIDdoingRefund { get; set; }
        public bool BarCompletedOrder { get; set; }
        public Nullable<bool> OrderCollected { get; set; }
        public Nullable<System.DateTime> DateTimeCollected { get; set; }
        public Nullable<decimal> FinalAmountForOrder { get; set; }
        public decimal RefundAmount { get; set; }
        public bool Refunded { get; set; }
        public string PaymentGatewayReturnID { get; set; }
    
        public virtual HotelRefundReason HotelRefundReason { get; set; }
        public virtual Hotel Hotel { get; set; }
        public virtual Patron Patron { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PatronsOrdersRefundsDetail> PatronsOrdersRefundsDetails { get; set; }
    }
}
