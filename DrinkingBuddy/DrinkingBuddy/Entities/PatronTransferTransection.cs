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
    
    public partial class PatronTransferTransection
    {
        public int PatronTransferTransectionID { get; set; }
        public Nullable<int> PatronID_Sender { get; set; }
        public Nullable<int> PatronID_Reciver { get; set; }
        public Nullable<decimal> Amount_Transfer { get; set; }
        public Nullable<System.DateTime> TransferDateTime { get; set; }
    }
}
