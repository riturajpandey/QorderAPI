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
    
    public partial class HotelSpecialsMeta
    {
        public int HotelSpecialsMetaID { get; set; }
        public int HotelSpecialID { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string RepeatInterval { get; set; }
        public Nullable<int> RepeatEveryXWeeks { get; set; }
        public bool RepeatEveryXWeeksMonday { get; set; }
        public bool RepeatEveryXWeeksTuesday { get; set; }
        public bool RepeatEveryXWeeksWednesday { get; set; }
        public bool RepeatEveryXWeeksThursday { get; set; }
        public bool RepeatEveryXWeeksFriday { get; set; }
        public bool RepeatEveryXWeeksSaturday { get; set; }
        public bool RepeatEveryXWeeksSunday { get; set; }
        public Nullable<int> RepeatMonthlyDay { get; set; }
        public Nullable<System.DateTime> RepeatYearlyDate { get; set; }
        public Nullable<bool> RepeatDailyEveryDay { get; set; }
    
        public virtual HotelSpecial HotelSpecial { get; set; }
    }
}
