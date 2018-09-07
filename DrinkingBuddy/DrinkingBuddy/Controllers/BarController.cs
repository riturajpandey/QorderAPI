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
using DrinkingBuddy.Providers;
using DrinkingBuddy.Results;
using AutoMapper;

namespace DrinkingBuddy.Controllers
{
    [RoutePrefix("api/Bar")]
    public class BarController : ApiController
    {
        #region Bar&Menu

        [HttpGet]
        [Route("Bars")]
        public IHttpActionResult Bars()
        {
            try
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var BarList = _context.Hotels.ToList();
                    if (BarList.Count != 0)
                    {
                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<Hotel, HotelResponseModel>();

                        });

                        IMapper mapper = config.CreateMapper();
                        var data = mapper.Map<List<HotelResponseModel>>(BarList);

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
        [Route("MenusCatagories")]
        public IHttpActionResult MenusCatagories()
        {
            try
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var MenuCatagoryList = _context.HotelMenusCategories.ToList();
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }


        }

        [HttpPost]
        [Route("Menus")]
        public IHttpActionResult Menus(int id)
        {
            try
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var MenuList = _context.HotelMenus.Where(m => m.HotelCategoryID == id).ToList();
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }

        [HttpPost]
        [Route("Ingredient")]
        public IHttpActionResult Ingredient(int id)
        {

            try
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Menu = _context.HotelMenus.Where(m => m.HotelMenuID == id).FirstOrDefault();
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

       
        #endregion

        #region Orders

        #endregion
    }
}