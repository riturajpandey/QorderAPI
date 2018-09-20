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
                if (ModelState.IsValid)
                {

                    var Hotels = _context.Mobile_Get_HotelsWithin100ks(model.Token, model.PatronID, model.CurrentLat, model.CurrentLong).ToList();
                    if (Hotels != null)
                    {
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
                    else
                    {
                        return Ok(new ResponseModel { Message = "Something Went Wrong.", Status = "Failed" });

                    }

                }
                else
                {
                    return BadRequest("Provided Data is invalid");

                }
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
                if (ModelState.IsValid)
                {
                    using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                    {
                        var patronExistinLogin = _context.PatronsHotelLogIns.Where(m => m.PatronID == model.PatronId);

                        if (patronExistinLogin != null)
                        {

                            var config = new MapperConfiguration(cfg =>
                           {

                               cfg.CreateMap<ConnnectBarModel, PatronsHotelLogIn>();

                           });

                            IMapper mapper = config.CreateMapper();
                            var data = mapper.Map<PatronsHotelLogIn>(model);
                            data.LoginDateTime = DateTime.Now;
                            _context.PatronsHotelLogIns.Add(data);
                            int i = _context.SaveChanges();
                            if (i != 0)
                            {
                                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", });

                            }
                            else
                            {
                                return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });
                            }


                        }
                        else
                        {
                            return BadRequest();

                        }
                    }


                }
                else
                {
                    return BadRequest("Passed Data Invalid");

                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Data + ex.Message);
            }
        }

        //[HttpGet]
        //[Route("Bars")]
        //public IHttpActionResult Bars()
        //{
        //    try
        //    {
        //        using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
        //        {
        //            var BarList = _context.Hotels.ToList();
        //            if (BarList.Count != 0)
        //            {
        //                var config = new MapperConfiguration(cfg =>
        //                {
        //                    cfg.CreateMap<Hotel, HotelResponseModel>();

        //                });

        //                IMapper mapper = config.CreateMapper();
        //                var data = mapper.Map<List<HotelResponseModel>>(BarList);

        //                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });
        //            }
        //            else
        //            {
        //                return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);

        //    }
        //}

        [HttpGet]
        [Route("MenusCatagories")]
        public IHttpActionResult MenusCatagories(int HotelId)
        {
            try
            {
                if (HotelId != 0)
                {
                    using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                    {
                        var MenuCatagoryList = _context.HotelMenusCategories.Where(m => m.HotelID == HotelId & m.IsActive == true).ToList();
                        if (MenuCatagoryList.Count != 0)
                        {
                            var config = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<HotelMenusCategory, HotelMenuCatagoriesResponseModel>();
                                cfg.CreateMap<HotelMenuCatagoriesResponseModel, HotelMenusCategory>();

                            });

                            IMapper mapper = config.CreateMapper();
                            var data = mapper.Map<List<HotelMenuCatagoriesResponseModel>>(MenuCatagoryList);

                            return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });
                        }
                        else
                        {
                            return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });
                        }
                    }

                }
                else
                {
                    return BadRequest("The Paramenter is not Valid");
                }
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
                if (HotelId != 0 & CatagoryId != 0)
                {
                    using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                    {
                        var MenuList = _context.HotelMenusSubCategories.Where(m => m.HotelMenuCategoryID == CatagoryId & m.HotelID == HotelId & m.IsActive == true).ToList();
                        if (MenuList.Count != 0)
                        {
                            var config = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<HotelMenu, HotelMenuSubCategoryResponseModel>();
                                cfg.CreateMap<HotelMenuSubCategoryResponseModel, HotelMenu>();

                            });

                            IMapper mapper = config.CreateMapper();
                            var data = mapper.Map<List<HotelMenuSubCategoryResponseModel>>(MenuList);

                            return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });
                        }
                        else
                        {
                            return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });
                        }
                    }
                }
                else
                {
                    return BadRequest("Parameter's are not valid.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }

        [HttpGet]
        [Route("Menus")]
        public IHttpActionResult Menus(int HotelId, int SubCatagoryId)
        {
            try
            {
                if (HotelId != 0 & SubCatagoryId != 0)
                {
                    using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                    {
                        var MenuList = _context.HotelMenus.Where(m => m.HotelSubCategoryID == SubCatagoryId & m.HotelID == HotelId & m.IsActive == true).ToList();
                        if (MenuList.Count != 0)
                        {
                            var config = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<HotelMenu, HotelsMenuResponseModel>();
                                cfg.CreateMap<HotelsMenuResponseModel, HotelMenu>();

                            });

                            IMapper mapper = config.CreateMapper();
                            var data = mapper.Map<List<HotelsMenuResponseModel>>(MenuList);

                            return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });
                        }
                        else
                        {
                            return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });
                        }
                    }
                }
                else
                {
                    return BadRequest("The provided Parameter's are not valid.");
                }
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
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Menu = _context.HotelMenus.Where(m => m.HotelMenuID == HotelMenuId).FirstOrDefault();
                    if (Menu != null)
                    {
                        IngredientResponseModel data = new IngredientResponseModel();

                        data.AlcoholPrecent = Menu.PercentAlcoholForPatronsApp;
                        data.DrinkIngredient = Menu.IngredientsForPatronsApp;


                        return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });
                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }


        }


        [HttpGet]
        [Route("Coupons")]
        public IHttpActionResult Coupons(int PatronsId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                    {
                        var couponsId = _context.HotelMarketingCouponsPatrons.Where(m => m.PatronID == PatronsId).ToList();

                        List<HotelMarketingCoupon> coupons = new List<HotelMarketingCoupon>();

                        foreach (var item in couponsId)
                        {
                            var singlecoupon = _context.HotelMarketingCoupons.Where(m => m.HotelMarketingCouponID == item.HotelMarketingCouponID).FirstOrDefault();

                            coupons.Add(singlecoupon);
                        }

                        if (coupons.Count != 0)
                        {
                            var config = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<HotelMarketingCoupon, CouponResponseModel>();
                                cfg.CreateMap<CouponResponseModel, HotelMarketingCoupon>();

                            });

                            IMapper mapper = config.CreateMapper();
                            var data = mapper.Map<List<CouponResponseModel>>(coupons);

                            return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });
                        }
                        else
                        {
                            return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Failed" });
                        }

                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }

        //[HttpPost]
        //[Route("CouponUsed")]
        //public IHttpActionResult CouponUsed(CouponBindingModel model)
        //{
        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
        //            {
        //                var config = new MapperConfiguration(cfg =>
        //                {
        //                    cfg.CreateMap<HotelMarketingCouponsPatron, CouponBindingModel>();
        //                    cfg.CreateMap<CouponBindingModel, HotelMarketingCoupon>();

        //                });

        //                IMapper mapper = config.CreateMapper();
        //                var data = mapper.Map<HotelMarketingCouponsPatron>(model);

        //                _context.HotelMarketingCouponsPatrons.Add(data);
        //                int _result = _context.SaveChanges();
        //                if (_result == 0)
        //                {
        //                    return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = data });
        //                }
        //                else
        //                {
        //                    return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Failed" });
        //                }


        //            }
        //        }
        //        else
        //        {
        //            return BadRequest("Parameter's Invalid");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}

        [HttpGet]
        [Route("SpecialOffer")]
        public IHttpActionResult SpecialOffer(int id)
        {
            try
            {
                if (id != 0)
                {
                    var Specials = _context.GetSpecials(id, null, null, null, null, null).ToList();
                    if (Specials != null)
                    {
                        return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = Specials });
                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "Something Went Wrong.", Status = "Failed", });

                    }

                }
                else
                {
                    return BadRequest("Parameter's are Invalid");
                }

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
                if (ModelState.IsValid)
                {
                    var PatronHotelLogins = _context.PatronsHotelLogIns.Where(m => m.PatronID == model.PatronID).FirstOrDefault();
                    var PatronMemberGroup = _context.PatronsGroupsMembers.Where(m => m.MemberPatronID == model.PatronID).ToList();
                    if (PatronMemberGroup != null) {

                        foreach (var item in PatronMemberGroup)
                        {
                            item.DateTimeJoinedGroup = model.LogoutDateTime;
                            PatronMemberGroup.Add(item);
                        }
                        PatronHotelLogins.LogoutDateTime = model.LogoutDateTime;
                        _context.Entry(PatronMemberGroup).State = EntityState.Modified;
                        _context.Entry(PatronHotelLogins).State = EntityState.Modified;
                        int result = _context.SaveChanges();
                        if (result > 0)
                        {
                            return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success" });
                        }
                        else
                        {
                            return BadRequest("Something went wrong");
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest("Provided data is not Appropriate");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}