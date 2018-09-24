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
    [RoutePrefix("api/MemberGroup")]
    public class MemberGroupController : ApiController
    {
        DrinkingBuddyEntities _context = new DrinkingBuddyEntities();

        #region MemberGroup

        [HttpPost]
        [Route("StartGroup")]
        public IHttpActionResult StartGroup(StartGroupBinding model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                    {
                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<StartGroupBinding, PatronsGroup>();
                            cfg.CreateMap<PatronsGroup, StartGroupBinding>();

                        });

                        IMapper mapper = config.CreateMapper();
                        var data = mapper.Map<PatronsGroup>(model);
                        data.IsActive = true;
                        // data.GroupStartedDateTime = DateTime.Now;
                        _context.PatronsGroups.Add(data);
                        int Rows = _context.SaveChanges();
                        if (Rows > 0)
                        {
                            var Hotel = _context.Hotels.Where(m => m.HotelID == model.HotelID).FirstOrDefault();
                            var Patons = _context.Patrons.Where(m => m.PatronsID == model.MasterPatronID).FirstOrDefault();
                            var patrongroup = _context.PatronsGroups.Where(m => m.MasterPatronID == model.MasterPatronID).FirstOrDefault();
                            StartGroupResponse Response = new StartGroupResponse();
                            Response.PatronsGroupID = patrongroup.PatronsGroupID;
                            Response.HotelName = Hotel.HotelName;
                            Response.GroupMasterPatron = Patons.FirstName + " " + Patons.LastName;
                            Response.GroupStartDateTime = data.GroupStartedDateTime;

                            if (Response != null)
                            {

                                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = Response });
                            }
                            else
                            {
                                return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });
                            }

                        }
                        else
                        {
                            return BadRequest("The Requested Group could not be created.");
                        }

                    }
                }
                else
                {
                    return BadRequest("The Passed Parametes are not valid");

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("StopGroup")]
        public IHttpActionResult StopGroup(StopGroup model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PatronsGroup data = _context.PatronsGroups.Where(m => m.MasterPatronID == model.PatronID & m.HotelID == model.HotelID & m.IsActive == true).FirstOrDefault();

                    data.GroupStopDateTime = model.GroupStopDateTime;
                    data.IsActive = false;
                    _context.Entry(data).State = EntityState.Modified;
                    int result = _context.SaveChanges();
                    if (result > 0)
                    {
                        return Ok(new ResponseModel { Message = "The Group has been Stoped Successfully", Status = "Success" });

                    }
                    else
                    {
                        return BadRequest("The Request Execution Failed");
                    }
                }
                else
                {
                    return BadRequest("provided Parameters are  not Valid");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AcceptInvite")]
        public IHttpActionResult AcceptInvite(AcceptInviteModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                    {
                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<AcceptInviteModel, PatronsGroupsMember>();
                            cfg.CreateMap<PatronsGroupsMember, AcceptInviteModel>();

                        });

                        IMapper mapper = config.CreateMapper();
                        var data = mapper.Map<PatronsGroupsMember>(model);

                        _context.PatronsGroupsMembers.Add(data);
                        int Rows = _context.SaveChanges();

                        if (Rows > 0)
                        {
                            return Ok(new ResponseModel { Message = "The Member has been Succesfully", Status = "Success" });

                        }
                        else
                        {
                            return BadRequest("Request Execution Failed");
                        }
                    }
                }
                else
                {
                    return BadRequest("The passed model is not valid.");
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