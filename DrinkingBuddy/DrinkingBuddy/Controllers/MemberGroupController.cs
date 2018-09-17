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
namespace DrinkingBuddy.Controllers
{
    [APIAuthorizeAttribute]
    [RoutePrefix("api/MemberGroup")]
    public class MemberGroupController : ApiController
    {
        DrinkingBuddyEntities _context = new DrinkingBuddyEntities();


        #region MemberGroup


        [HttpPost]
        [Route("StartGroup")]
        public IHttpActionResult StartGroup(MemberGroupModel model)
        {
            try {

                if (ModelState.IsValid)
                {

                    using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                    {
                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<MemberGroupModel,PatronsGroup>();
                            cfg.CreateMap<PatronsGroup, MemberGroupModel>();

                        });

                        IMapper mapper = config.CreateMapper();
                        var data = mapper.Map<PatronsGroup>(model);

                        _context.PatronsGroups.Add(data);
                        int Rows=_context.SaveChanges();
                        if (Rows>0) { 

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
                    return BadRequest("something went wrong");

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