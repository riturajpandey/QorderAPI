﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DrinkingBuddyEntities : DbContext
    {
        public DrinkingBuddyEntities()
            : base("name=DrinkingBuddyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<DrinkStandardImage> DrinkStandardImages { get; set; }
        public virtual DbSet<FAKE_MonthlySales> FAKE_MonthlySales { get; set; }
        public virtual DbSet<FAKE_MonthlySales2> FAKE_MonthlySales2 { get; set; }
        public virtual DbSet<HotelBarSale> HotelBarSales { get; set; }
        public virtual DbSet<HotelBarSalesDetail> HotelBarSalesDetails { get; set; }
        public virtual DbSet<HotelDiscountReason> HotelDiscountReasons { get; set; }
        public virtual DbSet<HotelDiscountsGiven> HotelDiscountsGivens { get; set; }
        public virtual DbSet<HotelEmployee> HotelEmployees { get; set; }
        public virtual DbSet<HotelEmployeesAvailabilityTime> HotelEmployeesAvailabilityTimes { get; set; }
        public virtual DbSet<HotelEmployeesRoster> HotelEmployeesRosters { get; set; }
        public virtual DbSet<HotelInventory> HotelInventories { get; set; }
        public virtual DbSet<HotelMarketingCoupon> HotelMarketingCoupons { get; set; }
        public virtual DbSet<HotelMarketingCouponsPatron> HotelMarketingCouponsPatrons { get; set; }
        public virtual DbSet<HotelMarketingNewsletter> HotelMarketingNewsletters { get; set; }
        public virtual DbSet<HotelMarketingNewslettersPatron> HotelMarketingNewslettersPatrons { get; set; }
        public virtual DbSet<HotelMarketingPushNotification> HotelMarketingPushNotifications { get; set; }
        public virtual DbSet<HotelMarketingPushNotificationsPatron> HotelMarketingPushNotificationsPatrons { get; set; }
        public virtual DbSet<HotelMarketingSm> HotelMarketingSms { get; set; }
        public virtual DbSet<HotelMarketingSmsPatron> HotelMarketingSmsPatrons { get; set; }
        public virtual DbSet<HotelMenu> HotelMenus { get; set; }
        public virtual DbSet<HotelMenusCategory> HotelMenusCategories { get; set; }
        public virtual DbSet<HotelMenusExtraDetail> HotelMenusExtraDetails { get; set; }
        public virtual DbSet<HotelMenusInventoryUse> HotelMenusInventoryUses { get; set; }
        public virtual DbSet<HotelMenusSubCategory> HotelMenusSubCategories { get; set; }
        public virtual DbSet<HotelOpenHour> HotelOpenHours { get; set; }
        public virtual DbSet<HotelRefundReason> HotelRefundReasons { get; set; }
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<HotelSavedOrder> HotelSavedOrders { get; set; }
        public virtual DbSet<HotelSavedOrdersDetail> HotelSavedOrdersDetails { get; set; }
        public virtual DbSet<HotelSpecial> HotelSpecials { get; set; }
        public virtual DbSet<HotelSpecialsMeta> HotelSpecialsMetas { get; set; }
        public virtual DbSet<HotelTaxGroup> HotelTaxGroups { get; set; }
        public virtual DbSet<Patron> Patrons { get; set; }
        public virtual DbSet<PatronsGroup> PatronsGroups { get; set; }
        public virtual DbSet<PatronsGroupsMember> PatronsGroupsMembers { get; set; }
        public virtual DbSet<PatronsHotelLogIn> PatronsHotelLogIns { get; set; }
        public virtual DbSet<PatronsHotelSubscription> PatronsHotelSubscriptions { get; set; }
        public virtual DbSet<PatronsOrder> PatronsOrders { get; set; }
        public virtual DbSet<PatronsOrdersDetail> PatronsOrdersDetails { get; set; }
        public virtual DbSet<PatronsOrdersFastestWay> PatronsOrdersFastestWays { get; set; }
        public virtual DbSet<PatronsOrdersRefund> PatronsOrdersRefunds { get; set; }
        public virtual DbSet<PatronsOrdersRefundsDetail> PatronsOrdersRefundsDetails { get; set; }
        public virtual DbSet<PatronsPaymentMethod> PatronsPaymentMethods { get; set; }
        public virtual DbSet<PatronsPreference> PatronsPreferences { get; set; }
        public virtual DbSet<PatronsSessionToken> PatronsSessionTokens { get; set; }
        public virtual DbSet<StatesG> StatesGs { get; set; }
        public virtual DbSet<PatronsResetPasswordToken> PatronsResetPasswordTokens { get; set; }
    
        [DbFunction("DrinkingBuddyEntities", "fnGetBestCurrentPriceForHotelMenuItem")]
        public virtual IQueryable<fnGetBestCurrentPriceForHotelMenuItem_Result> fnGetBestCurrentPriceForHotelMenuItem(Nullable<int> hotelID, Nullable<int> hotelMenuItemIDpassed)
        {
            var hotelIDParameter = hotelID.HasValue ?
                new ObjectParameter("HotelID", hotelID) :
                new ObjectParameter("HotelID", typeof(int));
    
            var hotelMenuItemIDpassedParameter = hotelMenuItemIDpassed.HasValue ?
                new ObjectParameter("hotelMenuItemIDpassed", hotelMenuItemIDpassed) :
                new ObjectParameter("hotelMenuItemIDpassed", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fnGetBestCurrentPriceForHotelMenuItem_Result>("[DrinkingBuddyEntities].[fnGetBestCurrentPriceForHotelMenuItem](@HotelID, @hotelMenuItemIDpassed)", hotelIDParameter, hotelMenuItemIDpassedParameter);
        }
    
        [DbFunction("DrinkingBuddyEntities", "fnGetNumberOfOrdersAndTotalAmountByPatron")]
        public virtual IQueryable<fnGetNumberOfOrdersAndTotalAmountByPatron_Result> fnGetNumberOfOrdersAndTotalAmountByPatron(Nullable<int> patronID, Nullable<int> hotelID, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate)
        {
            var patronIDParameter = patronID.HasValue ?
                new ObjectParameter("PatronID", patronID) :
                new ObjectParameter("PatronID", typeof(int));
    
            var hotelIDParameter = hotelID.HasValue ?
                new ObjectParameter("HotelID", hotelID) :
                new ObjectParameter("HotelID", typeof(int));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fnGetNumberOfOrdersAndTotalAmountByPatron_Result>("[DrinkingBuddyEntities].[fnGetNumberOfOrdersAndTotalAmountByPatron](@PatronID, @HotelID, @StartDate, @EndDate)", patronIDParameter, hotelIDParameter, startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<GetCounts_Result> GetCounts(Nullable<int> hotelID, Nullable<int> employeeID)
        {
            var hotelIDParameter = hotelID.HasValue ?
                new ObjectParameter("HotelID", hotelID) :
                new ObjectParameter("HotelID", typeof(int));
    
            var employeeIDParameter = employeeID.HasValue ?
                new ObjectParameter("EmployeeID", employeeID) :
                new ObjectParameter("EmployeeID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCounts_Result>("GetCounts", hotelIDParameter, employeeIDParameter);
        }
    
        public virtual ObjectResult<GetHotelMenuItemsWithCurrentPrice_Result> GetHotelMenuItemsWithCurrentPrice(Nullable<int> hotelID, Nullable<int> categoryID, Nullable<int> subCategoryID, Nullable<int> hotelSpecialsMetaID, string searchText, Nullable<int> hotelMenuItemID, Nullable<bool> quickListOnly)
        {
            var hotelIDParameter = hotelID.HasValue ?
                new ObjectParameter("HotelID", hotelID) :
                new ObjectParameter("HotelID", typeof(int));
    
            var categoryIDParameter = categoryID.HasValue ?
                new ObjectParameter("CategoryID", categoryID) :
                new ObjectParameter("CategoryID", typeof(int));
    
            var subCategoryIDParameter = subCategoryID.HasValue ?
                new ObjectParameter("SubCategoryID", subCategoryID) :
                new ObjectParameter("SubCategoryID", typeof(int));
    
            var hotelSpecialsMetaIDParameter = hotelSpecialsMetaID.HasValue ?
                new ObjectParameter("HotelSpecialsMetaID", hotelSpecialsMetaID) :
                new ObjectParameter("HotelSpecialsMetaID", typeof(int));
    
            var searchTextParameter = searchText != null ?
                new ObjectParameter("SearchText", searchText) :
                new ObjectParameter("SearchText", typeof(string));
    
            var hotelMenuItemIDParameter = hotelMenuItemID.HasValue ?
                new ObjectParameter("hotelMenuItemID", hotelMenuItemID) :
                new ObjectParameter("hotelMenuItemID", typeof(int));
    
            var quickListOnlyParameter = quickListOnly.HasValue ?
                new ObjectParameter("quickListOnly", quickListOnly) :
                new ObjectParameter("quickListOnly", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetHotelMenuItemsWithCurrentPrice_Result>("GetHotelMenuItemsWithCurrentPrice", hotelIDParameter, categoryIDParameter, subCategoryIDParameter, hotelSpecialsMetaIDParameter, searchTextParameter, hotelMenuItemIDParameter, quickListOnlyParameter);
        }
    
        public virtual ObjectResult<GetPatronsForHotelsWithNumberOfOrdersByDateRange_Result> GetPatronsForHotelsWithNumberOfOrdersByDateRange(Nullable<int> hotelID, Nullable<int> patronID, Nullable<int> minNumberOfOrders, Nullable<int> maxNumberOfOrders, Nullable<decimal> minSpend, Nullable<decimal> maxSpend, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> hotelSpecialsMetaID)
        {
            var hotelIDParameter = hotelID.HasValue ?
                new ObjectParameter("HotelID", hotelID) :
                new ObjectParameter("HotelID", typeof(int));
    
            var patronIDParameter = patronID.HasValue ?
                new ObjectParameter("PatronID", patronID) :
                new ObjectParameter("PatronID", typeof(int));
    
            var minNumberOfOrdersParameter = minNumberOfOrders.HasValue ?
                new ObjectParameter("MinNumberOfOrders", minNumberOfOrders) :
                new ObjectParameter("MinNumberOfOrders", typeof(int));
    
            var maxNumberOfOrdersParameter = maxNumberOfOrders.HasValue ?
                new ObjectParameter("MaxNumberOfOrders", maxNumberOfOrders) :
                new ObjectParameter("MaxNumberOfOrders", typeof(int));
    
            var minSpendParameter = minSpend.HasValue ?
                new ObjectParameter("MinSpend", minSpend) :
                new ObjectParameter("MinSpend", typeof(decimal));
    
            var maxSpendParameter = maxSpend.HasValue ?
                new ObjectParameter("MaxSpend", maxSpend) :
                new ObjectParameter("MaxSpend", typeof(decimal));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var hotelSpecialsMetaIDParameter = hotelSpecialsMetaID.HasValue ?
                new ObjectParameter("HotelSpecialsMetaID", hotelSpecialsMetaID) :
                new ObjectParameter("HotelSpecialsMetaID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPatronsForHotelsWithNumberOfOrdersByDateRange_Result>("GetPatronsForHotelsWithNumberOfOrdersByDateRange", hotelIDParameter, patronIDParameter, minNumberOfOrdersParameter, maxNumberOfOrdersParameter, minSpendParameter, maxSpendParameter, startDateParameter, endDateParameter, hotelSpecialsMetaIDParameter);
        }
    
        public virtual ObjectResult<GetPendingOrdersFastestWay_Result> GetPendingOrdersFastestWay(Nullable<int> hotelID, Nullable<int> employeeID)
        {
            var hotelIDParameter = hotelID.HasValue ?
                new ObjectParameter("HotelID", hotelID) :
                new ObjectParameter("HotelID", typeof(int));
    
            var employeeIDParameter = employeeID.HasValue ?
                new ObjectParameter("EmployeeID", employeeID) :
                new ObjectParameter("EmployeeID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPendingOrdersFastestWay_Result>("GetPendingOrdersFastestWay", hotelIDParameter, employeeIDParameter);
        }
    
        public virtual ObjectResult<GetSavedOrders_Result> GetSavedOrders(Nullable<int> hotelID, Nullable<int> employeeID)
        {
            var hotelIDParameter = hotelID.HasValue ?
                new ObjectParameter("HotelID", hotelID) :
                new ObjectParameter("HotelID", typeof(int));
    
            var employeeIDParameter = employeeID.HasValue ?
                new ObjectParameter("EmployeeID", employeeID) :
                new ObjectParameter("EmployeeID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetSavedOrders_Result>("GetSavedOrders", hotelIDParameter, employeeIDParameter);
        }
    
        public virtual ObjectResult<GetSpecials_Result> GetSpecials(Nullable<int> hotelID, Nullable<int> categoryID, Nullable<int> subCategoryID, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> hotelSpecialsMetaID)
        {
            var hotelIDParameter = hotelID.HasValue ?
                new ObjectParameter("HotelID", hotelID) :
                new ObjectParameter("HotelID", typeof(int));
    
            var categoryIDParameter = categoryID.HasValue ?
                new ObjectParameter("CategoryID", categoryID) :
                new ObjectParameter("CategoryID", typeof(int));
    
            var subCategoryIDParameter = subCategoryID.HasValue ?
                new ObjectParameter("SubCategoryID", subCategoryID) :
                new ObjectParameter("SubCategoryID", typeof(int));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var hotelSpecialsMetaIDParameter = hotelSpecialsMetaID.HasValue ?
                new ObjectParameter("HotelSpecialsMetaID", hotelSpecialsMetaID) :
                new ObjectParameter("HotelSpecialsMetaID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetSpecials_Result>("GetSpecials", hotelIDParameter, categoryIDParameter, subCategoryIDParameter, startDateParameter, endDateParameter, hotelSpecialsMetaIDParameter);
        }
    
        public virtual ObjectResult<string> InsertSaleWithExtraDetails(Nullable<int> hotelID, Nullable<int> employeeID, Nullable<int> hotelMenuItemID, Nullable<int> specialID, string barOrQorder, Nullable<int> orderDetailsID)
        {
            var hotelIDParameter = hotelID.HasValue ?
                new ObjectParameter("HotelID", hotelID) :
                new ObjectParameter("HotelID", typeof(int));
    
            var employeeIDParameter = employeeID.HasValue ?
                new ObjectParameter("EmployeeID", employeeID) :
                new ObjectParameter("EmployeeID", typeof(int));
    
            var hotelMenuItemIDParameter = hotelMenuItemID.HasValue ?
                new ObjectParameter("HotelMenuItemID", hotelMenuItemID) :
                new ObjectParameter("HotelMenuItemID", typeof(int));
    
            var specialIDParameter = specialID.HasValue ?
                new ObjectParameter("SpecialID", specialID) :
                new ObjectParameter("SpecialID", typeof(int));
    
            var barOrQorderParameter = barOrQorder != null ?
                new ObjectParameter("BarOrQorder", barOrQorder) :
                new ObjectParameter("BarOrQorder", typeof(string));
    
            var orderDetailsIDParameter = orderDetailsID.HasValue ?
                new ObjectParameter("OrderDetailsID", orderDetailsID) :
                new ObjectParameter("OrderDetailsID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("InsertSaleWithExtraDetails", hotelIDParameter, employeeIDParameter, hotelMenuItemIDParameter, specialIDParameter, barOrQorderParameter, orderDetailsIDParameter);
        }
    
        public virtual ObjectResult<Mobile_Get_HotelsWithin100ks_Result> Mobile_Get_HotelsWithin100ks(string sessionToken, Nullable<int> patronID, Nullable<decimal> currentLat, Nullable<decimal> currentLong)
        {
            var sessionTokenParameter = sessionToken != null ?
                new ObjectParameter("SessionToken", sessionToken) :
                new ObjectParameter("SessionToken", typeof(string));
    
            var patronIDParameter = patronID.HasValue ?
                new ObjectParameter("PatronID", patronID) :
                new ObjectParameter("PatronID", typeof(int));
    
            var currentLatParameter = currentLat.HasValue ?
                new ObjectParameter("CurrentLat", currentLat) :
                new ObjectParameter("CurrentLat", typeof(decimal));
    
            var currentLongParameter = currentLong.HasValue ?
                new ObjectParameter("CurrentLong", currentLong) :
                new ObjectParameter("CurrentLong", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Mobile_Get_HotelsWithin100ks_Result>("Mobile_Get_HotelsWithin100ks", sessionTokenParameter, patronIDParameter, currentLatParameter, currentLongParameter);
        }
    
        public virtual ObjectResult<Mobile_Patron_Login_Result> Mobile_Patron_Login(string userName, string password, Nullable<bool> fbLogin, string fbToken)
        {
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("Password", password) :
                new ObjectParameter("Password", typeof(string));
    
            var fbLoginParameter = fbLogin.HasValue ?
                new ObjectParameter("fbLogin", fbLogin) :
                new ObjectParameter("fbLogin", typeof(bool));
    
            var fbTokenParameter = fbToken != null ?
                new ObjectParameter("fbToken", fbToken) :
                new ObjectParameter("fbToken", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Mobile_Patron_Login_Result>("Mobile_Patron_Login", userNameParameter, passwordParameter, fbLoginParameter, fbTokenParameter);
        }
    
        public virtual ObjectResult<ReportGetSalesByDateRange_Result> ReportGetSalesByDateRange(Nullable<int> hotelID, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<bool> barSales, Nullable<bool> qSales, Nullable<int> categoryID, Nullable<int> subCategoryID, Nullable<int> barStaffID)
        {
            var hotelIDParameter = hotelID.HasValue ?
                new ObjectParameter("HotelID", hotelID) :
                new ObjectParameter("HotelID", typeof(int));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("EndDate", endDate) :
                new ObjectParameter("EndDate", typeof(System.DateTime));
    
            var barSalesParameter = barSales.HasValue ?
                new ObjectParameter("barSales", barSales) :
                new ObjectParameter("barSales", typeof(bool));
    
            var qSalesParameter = qSales.HasValue ?
                new ObjectParameter("qSales", qSales) :
                new ObjectParameter("qSales", typeof(bool));
    
            var categoryIDParameter = categoryID.HasValue ?
                new ObjectParameter("CategoryID", categoryID) :
                new ObjectParameter("CategoryID", typeof(int));
    
            var subCategoryIDParameter = subCategoryID.HasValue ?
                new ObjectParameter("SubCategoryID", subCategoryID) :
                new ObjectParameter("SubCategoryID", typeof(int));
    
            var barStaffIDParameter = barStaffID.HasValue ?
                new ObjectParameter("BarStaffID", barStaffID) :
                new ObjectParameter("BarStaffID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ReportGetSalesByDateRange_Result>("ReportGetSalesByDateRange", hotelIDParameter, startDateParameter, endDateParameter, barSalesParameter, qSalesParameter, categoryIDParameter, subCategoryIDParameter, barStaffIDParameter);
        }
    
        public virtual int UpdateOrderToDelivered(Nullable<int> hotelID, Nullable<int> patronID, Nullable<int> patronsOrdersID, string patronsScanDetails)
        {
            var hotelIDParameter = hotelID.HasValue ?
                new ObjectParameter("HotelID", hotelID) :
                new ObjectParameter("HotelID", typeof(int));
    
            var patronIDParameter = patronID.HasValue ?
                new ObjectParameter("PatronID", patronID) :
                new ObjectParameter("PatronID", typeof(int));
    
            var patronsOrdersIDParameter = patronsOrdersID.HasValue ?
                new ObjectParameter("PatronsOrdersID", patronsOrdersID) :
                new ObjectParameter("PatronsOrdersID", typeof(int));
    
            var patronsScanDetailsParameter = patronsScanDetails != null ?
                new ObjectParameter("PatronsScanDetails", patronsScanDetails) :
                new ObjectParameter("PatronsScanDetails", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateOrderToDelivered", hotelIDParameter, patronIDParameter, patronsOrdersIDParameter, patronsScanDetailsParameter);
        }
    }
}
