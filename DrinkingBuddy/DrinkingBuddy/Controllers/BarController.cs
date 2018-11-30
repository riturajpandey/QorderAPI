using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using DrinkingBuddy.Models;
using DrinkingBuddy.Entities;
using DrinkingBuddy.Filter;
using DrinkingBuddy.Providers;
using DrinkingBuddy.Results;
using AutoMapper;
using System.Data.Entity;

namespace DrinkingBuddy.Controllers
{
    [APIAuthorizeAttribute]
    [RoutePrefix("api/Bar")]
    public class BarController : ApiController
    {
        DrinkingBuddyEntities _context = new DrinkingBuddyEntities();


        #region Bar&Menu

        [HttpPost]
        [Route("ShowNearByBars")]
        public IHttpActionResult ShowNearByBars(BarMapModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Provided Data is invalid");
                }
                var Hotels = _context.Mobile_Get_HotelsWithin100ks(model.Token, model.PatronID, model.CurrentLat, model.CurrentLong).ToList();
                if (Hotels.Count() == 0)
                {
                    return Ok(new ResponseModel { Message = "No Near By Hotel Found.", Status = "Failed" });
                }
                List<Mobile_Get_HotelsWithin100ks_Result> ShowHotels = new List<Mobile_Get_HotelsWithin100ks_Result>();
                foreach (var item in Hotels)
                {
                    Mobile_Get_HotelsWithin100ks_Result data = new Mobile_Get_HotelsWithin100ks_Result();
                    data.HotelID = item.HotelID;
                    if (item.HotelName == null) { data.HotelName = ""; }
                    else { data.HotelName = item.HotelName; }
                    if (item.HotelAddress1 == null) { data.HotelAddress1 = ""; }
                    else { data.HotelAddress1 = item.HotelAddress1; }
                    if (item.HotelAddress2 == null) { data.HotelAddress2 = ""; }
                    else { data.HotelAddress2 = item.HotelAddress2; }
                    if (item.HotelSuburb == null) { data.HotelSuburb = ""; }
                    else { data.HotelSuburb = item.HotelSuburb; }
                    if (item.HotelPostcode == null) { data.HotelPostcode = ""; }
                    else { data.HotelPostcode = item.HotelPostcode; }
                    if (item.StateName == null) { data.StateName = ""; }
                    else { data.StateName = item.StateName; }
                    if (item.StateNameAbbreviation == null) { data.StateNameAbbreviation = ""; }
                    else { data.StateNameAbbreviation = item.StateNameAbbreviation; }
                    if (item.HotelStateID == null) { data.HotelStateID = item.HotelStateID; }
                    else { data.HotelStateID = item.HotelStateID; }
                    if (item.HotelLat == null) { data.HotelLat = 0; }
                    else { data.HotelLat = item.HotelLat; }
                    if (item.HotelLong == null) { data.HotelLong = 0; }
                    else { data.HotelLong = item.HotelLong; }
                    if (item.DistanceAway == null) { data.DistanceAway = ""; }
                    else { data.DistanceAway = item.DistanceAway; }

                    ShowHotels.Add(data);

                }

                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = ShowHotels });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ConnectBar")]
        public IHttpActionResult ConnectBar(ConnnectBarModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Passed Data Invalid");

                }

                var patronExistinLogin = _context.PatronsHotelLogIns.Where(m => m.PatronID == model.PatronId);

                if (patronExistinLogin == null)
                {

                    return BadRequest();
                }
                var config = new MapperConfiguration(cfg =>
                {


                    cfg.CreateMap<ConnnectBarModel, PatronsHotelLogIn>();

                });

                IMapper mapper = config.CreateMapper();
                var data = mapper.Map<PatronsHotelLogIn>(model);
                data.LoginDateTime = DateTime.Now;
                _context.PatronsHotelLogIns.Add(data);
                int i = _context.SaveChanges();
                if (i == 0)
                {
                    return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });


                }

                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Data + ex.Message);
            }
        }

        [HttpGet]
        [Route("MenusCatagories")]
        public IHttpActionResult MenusCatagories(int HotelId)
        {
            try
            {
                if (HotelId == 0)
                {
                    return BadRequest("The Paramenter is not Valid");
                }

                var MenuCatagoryList = _context.HotelMenusCategories.Where(m => m.HotelID == HotelId & m.IsActive == true).ToList();
                if (MenuCatagoryList.Count == 0)
                {
                    return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });
                }
                var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<HotelMenusCategory, HotelMenuCatagoriesResponseModel>();
                        cfg.CreateMap<HotelMenuCatagoriesResponseModel, HotelMenusCategory>();

                    });

                IMapper mapper = config.CreateMapper();
                var data = mapper.Map<List<HotelMenuCatagoriesResponseModel>>(MenuCatagoryList);
                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpGet]
        [Route("MenusSubCategories")]
        public IHttpActionResult MenusSubCatagories(int HotelId, int CatagoryId)
        {
            try
            {
                if (HotelId == 0 & CatagoryId == 0)
                {
                    return BadRequest("Parameter's are not valid.");
                }

                var MenuList = _context.HotelMenusSubCategories.Where(m => m.HotelMenuCategoryID == CatagoryId & m.HotelID == HotelId & m.IsActive == true).ToList();
                if (MenuList.Count == 0)
                {
                    return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });
                }
                var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<HotelMenu, HotelMenuSubCategoryResponseModel>();
                        cfg.CreateMap<HotelMenuSubCategoryResponseModel, HotelMenu>();

                    });

                IMapper mapper = config.CreateMapper();
                var data = mapper.Map<List<HotelMenuSubCategoryResponseModel>>(MenuList);

                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }

        [HttpGet]
        [Route("Menus")]
        public IHttpActionResult Menus(int HotelId, int SubCatagoryId, int PatronID)
        {
            try
            {
                if (HotelId == 0 & SubCatagoryId == 0 & PatronID == 0)
                {

                    return BadRequest("The provided Parameter's are not valid.");
                }

                var MenuList = _context.HotelMenus.Where(m => m.HotelSubCategoryID == SubCatagoryId & m.HotelID == HotelId & m.IsActive == true).ToList();
                if (MenuList.Count == 0)
                {
                    return Ok(new ResponseModel { Message = "No Drinks found for this hotel or subcatagory", Status = "Success" });
                }
                var config = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<HotelMenu, HotelsMenuResponseModel>()
                                .ForMember(m => m.PatronID, opt => opt.Ignore())
                                .ForMember(m => m.HotelID, opt => opt.Ignore());
                                cfg.CreateMap<HotelsMenuResponseModel, HotelMenu>();

                            });

                IMapper mapper = config.CreateMapper();
                var data = mapper.Map<List<HotelsMenuResponseModel>>(MenuList);

                List<HotelsMenuResponseModel> listresponse = new List<HotelsMenuResponseModel>();

                foreach (var item in data)
                {
                    item.PatronID = PatronID;
                    item.HotelID = HotelId;

                    listresponse.Add(item);

                }


                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = listresponse });

            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }

        [HttpPost]
        [Route("Ingredient")]
        public IHttpActionResult Ingredient(int HotelMenuId)
        {
            try
            {

                var Menu = _context.HotelMenus.Where(m => m.HotelMenuID == HotelMenuId).FirstOrDefault();
                if (Menu == null)
                {
                    return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });

                }
                IngredientResponseModel data = new IngredientResponseModel();

                data.AlcoholPrecent = Menu.PercentAlcoholForPatronsApp;
                data.DrinkIngredient = Menu.IngredientsForPatronsApp;

                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }


        }

        [HttpPost]
        [Route("LeaveBar")]
        public IHttpActionResult LeaveBar(LeaveBarModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {

                    return BadRequest("Provided data is not Appropriate");
                }

                var PatronHotelLogins = _context.PatronsHotelLogIns.Where(m => m.PatronID == model.PatronID & m.LogoutDateTime == null).FirstOrDefault();
                if (PatronHotelLogins == null)
                {
                    return BadRequest("Something went wrong");
                }
                PatronHotelLogins.LogoutDateTime = DateTime.Now;
                _context.Entry(PatronHotelLogins).State = EntityState.Modified;
                int result = _context.SaveChanges();
                if (result == 0)
                {
                    return BadRequest("Something went wrong");

                }
                var PatronMemberGroup = _context.PatronsGroupsMembers.Where(m => m.MemberPatronID == model.PatronID).FirstOrDefault();
                if (PatronMemberGroup == null)
                {
                    return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success" });

                }
                //foreach (var item in PatronMemberGroup)
                //{
                //    item.DateTimeLeftGroup = DateTime.Now;
                //    PatronMemberGroup.Add(item);
                //}

                PatronMemberGroup.DateTimeLeftGroup = DateTime.Now;

                _context.Entry(PatronMemberGroup).State = EntityState.Modified;

                int result2 = _context.SaveChanges();
                if (result2 == 0)
                {
                    return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success" });
                }

                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        #endregion

        #region Coupons

        [HttpGet]
        [Route("Coupons")]
        public IHttpActionResult Coupons(int PatronsId)
        {
            try
            {
                if (PatronsId == 0)
                {
                    return NotFound();
                }

                var couponsId = _context.HotelMarketingCouponsPatrons.Where(m => m.PatronID == PatronsId).ToList();

                List<CouponResponseModel> coupons = new List<CouponResponseModel>();



                DateTime CurrentDate = DateTime.Today.Date;

                DateTime CurrentTime = DateTime.Today.ToUniversalTime();


                List<CouponValidation> listcouponValidations = new List<CouponValidation>();

                foreach (var item in couponsId)
                {
                    CouponValidation couponValidation = new CouponValidation();
                    var singlecoupon = _context.HotelMarketingCoupons.Where(m => m.HotelMarketingCouponID == item.HotelMarketingCouponID).FirstOrDefault();
                    couponValidation.StartDate = singlecoupon.CouponStartDate.Value;
                    couponValidation.StartTime = singlecoupon.CouponStartTime.Value.ToUniversalTime();
                    couponValidation.EndDate = singlecoupon.CouponEndDate.Value;
                    couponValidation.EndTime = singlecoupon.CouponEndTime.Value.ToUniversalTime();
                    couponValidation.HotelMarketingCouponID = singlecoupon.HotelMarketingCouponID;

                    listcouponValidations.Add(couponValidation);
                }


                foreach (var item in listcouponValidations)
                {

                    if (item.StartDate.Date < CurrentDate.Date)
                    {

                        if (item.EndDate.Date > CurrentDate.Date)
                        {
                            CouponResponseModel single = new CouponResponseModel();
                            var singlecoupon = _context.HotelMarketingCoupons.Where(m => m.HotelMarketingCouponID == item.HotelMarketingCouponID).FirstOrDefault();

                            single.HotelMarketingCouponId = singlecoupon.HotelMarketingCouponID;
                            single.CouponName = singlecoupon.CouponName;
                            single.CouponCode = singlecoupon.CouponCode;
                            if (singlecoupon.ItemMenuID==null)
                            {
                                single.ItemMenuID = 0;
                            }
                            single.ItemMenuID = singlecoupon.ItemMenuID;
                            if (singlecoupon.CouponEndDate==null)
                            {
                                single.CouponEndDate = DateTime.MinValue;
                            }
                            single.CouponEndDate = singlecoupon.CouponEndDate;
                            if (singlecoupon.CouponEndTime==null)
                            {
                                single.CouponEndTime = DateTime.MinValue;
                            }
                            single.CouponEndTime = singlecoupon.CouponEndTime;

                            if (singlecoupon.DiscountOrFreeItem == "Discount")
                            {
                                if (singlecoupon.DiscountAmountOrPercent == "Percent")
                                {
                                    single.CouponType = "DiscountPercent";
                                }
                                else
                                {

                                    single.CouponType = "DiscountNewAmount";
                                }

                            }
                            else
                            {
                                single.CouponType = "FreeItem";
                            }
                           

                            coupons.Add(single);
                        }
                        else if (item.EndDate.Date == CurrentDate.Date)
                        {
                            if (item.EndTime.TimeOfDay > CurrentTime.TimeOfDay)
                            {
                                CouponResponseModel single = new CouponResponseModel();
                                var singlecoupon = _context.HotelMarketingCoupons.Where(m => m.HotelMarketingCouponID == item.HotelMarketingCouponID).FirstOrDefault();

                                single.HotelMarketingCouponId = singlecoupon.HotelMarketingCouponID;
                                single.CouponName = singlecoupon.CouponName;
                                single.CouponCode = singlecoupon.CouponCode;
                                if (singlecoupon.ItemMenuID == null)
                                {
                                    single.ItemMenuID = 0;
                                }
                                single.ItemMenuID = singlecoupon.ItemMenuID;
                                if (singlecoupon.CouponEndDate == null)
                                {
                                    single.CouponEndDate = DateTime.MinValue;
                                }
                                single.CouponEndDate = singlecoupon.CouponEndDate;
                                if (singlecoupon.CouponEndTime == null)
                                {
                                    single.CouponEndTime = DateTime.MinValue;
                                }
                                single.CouponEndTime = singlecoupon.CouponEndTime;

                                if (singlecoupon.DiscountOrFreeItem == "Discount")
                                {
                                    if (singlecoupon.DiscountAmountOrPercent == "Percent")
                                    {
                                        single.CouponType = "DiscountPercent";
                                    }
                                    else
                                    {

                                        single.CouponType = "DiscountNewAmount";
                                    }

                                }
                                else
                                {
                                    single.CouponType = "FreeItem";

                                }
                               
                                coupons.Add(single);

                            }

                        }
                    }
                    else if (item.StartDate.Date == CurrentDate.Date)
                    {
                        if (item.StartTime.TimeOfDay < CurrentTime.TimeOfDay)
                        {
                            CouponResponseModel single = new CouponResponseModel();
                            var singlecoupon = _context.HotelMarketingCoupons.Where(m => m.HotelMarketingCouponID == item.HotelMarketingCouponID).FirstOrDefault();

                            single.HotelMarketingCouponId = singlecoupon.HotelMarketingCouponID;
                            single.CouponName = singlecoupon.CouponName;
                            single.CouponCode = singlecoupon.CouponCode;
                            if (singlecoupon.ItemMenuID == null)
                            {
                                single.ItemMenuID = 0;
                            }
                            single.ItemMenuID = singlecoupon.ItemMenuID;
                            if (singlecoupon.CouponEndDate == null)
                            {
                                single.CouponEndDate = DateTime.MinValue;
                            }
                            single.CouponEndDate = singlecoupon.CouponEndDate;
                            if (singlecoupon.CouponEndTime == null)
                            {
                                single.CouponEndTime = DateTime.MinValue;
                            }
                            single.CouponEndTime = singlecoupon.CouponEndTime;

                            if (singlecoupon.DiscountOrFreeItem == "Discount")
                            {
                                if (singlecoupon.DiscountAmountOrPercent == "Percent")
                                {
                                    single.CouponType = "DiscountPercent";
                                }
                                else
                                {

                                    single.CouponType = "DiscountNewAmount";
                                }

                            }
                            else
                            {
                                single.CouponType = "FreeItem";

                            }
                           
                            coupons.Add(single);

                        }

                    }

                }

                if (coupons.Count == 0)
                {
                    return Ok(new ResponseModel { Message = "No Coupons found for this Patron.", Status = "Failed" });
                }
               

                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = coupons });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }


        [HttpGet]
        [Route("GetCoupon")]
        public IHttpActionResult GetCoupon(int HotelMarketingCouponID)
        {
            if (HotelMarketingCouponID == 0)
            {
                return BadRequest();

            }

            GetCouponResponse getCouponResponse = new GetCouponResponse();
            Freeitem _freeitem = new Freeitem();

            var hotelpatroncoupons = _context.HotelMarketingCoupons.Where(m => m.HotelMarketingCouponID == HotelMarketingCouponID).FirstOrDefault();
            if (hotelpatroncoupons == null)
            {
                return Ok(new ResponseModel { Message = "No Coupon found For this patron.", Status = "Success" });
            }

            if (hotelpatroncoupons.DiscountOrFreeItem == null)
            {
                return BadRequest();
            }
            if (hotelpatroncoupons.DiscountOrFreeItem == "Discount")
            {
                if (hotelpatroncoupons.DiscountAmountOrPercent == null)
                {

                    return BadRequest();
                }

                if (hotelpatroncoupons.DiscountAmountOrPercent == "Percent")
                {
                    getCouponResponse.DiscountPercent = hotelpatroncoupons.DiscountPercent;
                    getCouponResponse.NewAmount = 0;
                    getCouponResponse.itemId = hotelpatroncoupons.ItemMenuID;
                    getCouponResponse.CouponType = "DiscountPercent";
                    return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = getCouponResponse });
                }

                getCouponResponse.NewAmount = hotelpatroncoupons.NewAmount;
                getCouponResponse.DiscountPercent = 0;
                getCouponResponse.CouponType = "DiscountNewAmount";
                getCouponResponse.itemId = hotelpatroncoupons.ItemMenuID;
                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = getCouponResponse });

            }

            getCouponResponse.NewAmount = 0;
            getCouponResponse.DiscountPercent = 0;
            getCouponResponse.CouponType = "FreeItem";
            getCouponResponse.itemId = hotelpatroncoupons.ItemMenuID;

            var freeitem = _context.HotelMenus.Where(m => m.HotelMenuID == hotelpatroncoupons.ItemMenuID).FirstOrDefault();

            if (freeitem == null)
            {
                return Ok("No item available with this ItemID");
            }

            if (freeitem.DrinkName == null)
            {
                _freeitem.DrinkName = "";
            }
            _freeitem.DrinkName = freeitem.DrinkName;
            if (freeitem.DrinkSize == null || freeitem.DrinkSize == 0)
            {
                _freeitem.DrinkSize = 0;
            }
            _freeitem.DrinkSize = freeitem.DrinkSize;
            if (freeitem.DrinkUnitMlLitreUnit == null)
            {
                _freeitem.DrinkUnitMlLitreUnit = "";

            }
            _freeitem.DrinkUnitMlLitreUnit = freeitem.DrinkUnitMlLitreUnit;
            if (freeitem.IngredientsForPatronsApp == null)
            {
                _freeitem.IngredientsForPatronsApp = "";

            }
            _freeitem.IngredientsForPatronsApp = freeitem.IngredientsForPatronsApp;
            if (freeitem.PercentAlcoholForPatronsApp == null)
            {
                _freeitem.PercentAlcoholForPatronsApp = "";
            }
            _freeitem.PercentAlcoholForPatronsApp = freeitem.PercentAlcoholForPatronsApp;
            _freeitem.FreeItemQty = hotelpatroncoupons.FreeItemQty;

            getCouponResponse.Freeitem = _freeitem;

            return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = getCouponResponse });

        }


        #endregion

        #region SpecialOffer
        [HttpGet]
        [Route("SpecialOffer")]
        public IHttpActionResult SpecialOffer(int HotelID)
        {
            try
            {
                if (HotelID == 0)
                {
                    return BadRequest("Parameter's are Invalid");

                }

                // var Data = _context.GetSpecials(HotelID, null, null, null, null, null).ToList();
                var Data = _context.HotelSpecials.Where(m => m.HotelID == HotelID).ToList();
                if (Data.Count() == 0)
                {
                    return Ok(new ResponseModel { Message = "Something Went Wrong.", Status = "Failed", });

                }
                List<HotelSpecial> Specials = new List<HotelSpecial>();

                foreach (var item in Data)
                {
                    if (item.CategoryID == null)
                    { item.CategoryID = 0; }
                    else { item.CategoryID = item.CategoryID; }
                    if (item.SubCategoryID == null)
                    { item.SubCategoryID = 0; }
                    else { item.SubCategoryID = item.SubCategoryID; }
                    if (item.NewAmount == null)
                    { item.NewAmount = 0; }
                    else { item.NewAmount = item.NewAmount; }

                    Specials.Add(item);
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<HotelSpecial, SpecialResponsemodel>();


                });

                IMapper mapper = config.CreateMapper();
                var data = mapper.Map<List<SpecialResponsemodel>>(Specials);


                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });


            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        #endregion


        #region Favorites

        [HttpGet]
        [Route("PatronsFavourites")]
        public IHttpActionResult PatronsFavourites(int PatronID, int HotelID)
        {
            try
            {
                if (PatronID == 0 & HotelID == 0)
                {
                    return BadRequest("Parameters Invalid");

                }
                var Favourite = _context.PatronsFavourites.Where(m => m.HotelID == HotelID & m.PatronID == PatronID).ToList();

                List<HotelMenu> MenuList = new List<HotelMenu>();

                foreach (var item in Favourite)
                {
                    HotelMenu _HotelMenu = new HotelMenu();
                    _HotelMenu = _context.HotelMenus.Where(m => m.HotelMenuID == item.HotelMenuID).FirstOrDefault();

                    MenuList.Add(_HotelMenu);
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<PatronsFavourite, PatronsFavouritesResponseModel>();
                    cfg.CreateMap<HotelMenu, HotelsMenuResponseModel>();

                });

                IMapper mapper = config.CreateMapper();
                var data = mapper.Map<List<HotelsMenuResponseModel>>(MenuList);

                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("AddFavourites")]
        public IHttpActionResult AddFavourites(FavoriteModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Parameters Invalid");

                }
                var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<FavoriteModel, PatronsFavourite>();

                    });

                IMapper mapper = config.CreateMapper();
                var data = mapper.Map<PatronsFavourite>(model);
                _context.PatronsFavourites.Add(data);
                int Rows = _context.SaveChanges();
                if (Rows == 0)
                {

                    return BadRequest("The Drinks Addition Failed.");
                }
                var Favourite = _context.PatronsFavourites.Where(m => m.HotelID == model.HotelID & m.PatronID == model.PatronID).FirstOrDefault();
                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = Favourite });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpGet]
        [Route("RemoveFavourites")]
        public IHttpActionResult RemoveFavourites(int PatronsID, int HotelMenuID)
        {
            try
            {

                if (PatronsID == 0 & HotelMenuID == 0)
                {
                    return BadRequest("Parameters Invalid");
                }
                var FavRecord = _context.PatronsFavourites.Where(m => m.PatronID == PatronsID & m.HotelMenuID == HotelMenuID).FirstOrDefault();

                _context.PatronsFavourites.Remove(FavRecord);
                int row = _context.SaveChanges();
                if (row == 0)
                {
                    return Ok(new ResponseModel { Message = "Removed from Favourite Failed.", Status = "Success" });
                }

                return Ok(new ResponseModel { Message = "Removed from Favourite Successfully.", Status = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        #endregion



    }
}

