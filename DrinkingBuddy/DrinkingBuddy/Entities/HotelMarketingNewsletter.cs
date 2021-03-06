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
    
    public partial class HotelMarketingNewsletter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HotelMarketingNewsletter()
        {
            this.HotelMarketingNewslettersPatrons = new HashSet<HotelMarketingNewslettersPatron>();
        }
    
        public int HotelMarketingNewsletterID { get; set; }
        public int HotelID { get; set; }
        public string NewsletterName { get; set; }
        public string NewsletterSubject { get; set; }
        public string NewsletterContent { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> SentBy { get; set; }
        public Nullable<System.DateTime> SentDate { get; set; }
    
        public virtual Hotel Hotel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HotelMarketingNewslettersPatron> HotelMarketingNewslettersPatrons { get; set; }
    }
}
