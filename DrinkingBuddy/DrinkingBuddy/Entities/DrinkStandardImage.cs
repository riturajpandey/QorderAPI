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
    
    public partial class DrinkStandardImage
    {
        public int DrinkStandardImageID { get; set; }
        public byte[] DrinkImage { get; set; }
        public Nullable<int> DrinkImageCategoryID { get; set; }
        public Nullable<int> DrinkImageSubCategoryID { get; set; }
        public string DrinkImageDescription { get; set; }
    }
}
