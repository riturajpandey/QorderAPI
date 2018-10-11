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


        #region Master Patron

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
                        //setting the Stop Time right after 5 hours.

                        DateTime stoptime = (DateTime)data.GroupStartedDateTime;
                        data.GroupStopDateTime = stoptime.AddHours(5);

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

        [HttpGet]
        [Route("IsGroupActive")]
        public IHttpActionResult IsGroupActive(int PatronsGroupID)
        {
            try
            {
                if (PatronsGroupID != 0)
                {
                    var Isgroupalive = _context.PatronsGroups.Where(m => m.PatronsGroupID == PatronsGroupID).FirstOrDefault();
                    if (Isgroupalive.GroupStopDateTime > DateTime.Now)
                    {
                        return Ok(new ResponseModel { Message = "The Group is Active.", Status = "Success" });
                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "The Group has been Expired.", Status = "Failed" });
                    }
                }
                else
                {
                    return BadRequest();

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpGet]
        [Route("RemovePatron")]
        public IHttpActionResult RemovePatron(int PatrongroupID, int PatronsID)
        {
            try
            {
                if (PatrongroupID != null & PatronsID != null)
                {
                    var _patroningroup = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == PatrongroupID & m.MemberPatronID == PatronsID).FirstOrDefault();
                    if (_patroningroup != null)
                    {
                        _context.PatronsGroupsMembers.Remove(_patroningroup);
                        int i = _context.SaveChanges();
                        if (i > 0)
                        {
                            return Ok(new ResponseModel { Message = "Patron Removed Successfully.", Status = "Success" });
                        }
                        else
                        {
                            return Ok(new ResponseModel { Message = "Request Execution failed.", Status = "Success" });
                        }
                    }
                    else
                    {
                        return BadRequest("Patron is not found in the goup.");
                    }
                }
                else
                {
                    return BadRequest("Passed Parameter Invalid.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpGet]//To show list of member in perticular group
        [Route("GetMemberByGroup")]
        public IHttpActionResult GetMemberByGroup(int GroupID)
        {
            try
            {
                if (GroupID != null)
                {
                    var members = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == GroupID).ToList();
                    if (members != null)
                    {

                        List<MemberByGroupResponse> response = new List<MemberByGroupResponse>();
                        foreach (var item in members)
                        {
                            MemberByGroupResponse single = new MemberByGroupResponse();
                            single.PatronID = item.MemberPatronID;
                            response.Add(single);
                        }
                        return Ok(new ResponseModel { Message = "Request Executed Successfully.", Status = "Success", Data = response });

                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "Patron Removed Successfully.", Status = "Success" });

                    }
                }
                else
                {
                    return BadRequest("Invalid Passed Parameter");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]//To show a list of Patrons logged-in in a specifi hotel for invitation.
        [Route("GetLoggedInPatrons")]
        public IHttpActionResult GetLoggedInPatrons(int HotelID)
        {
            try
            {
                if (HotelID > 0)
                {
                    var patrons = _context.PatronsHotelLogIns.Where(m => m.HotelID == HotelID & m.LogoutDateTime == null).ToList();

                    List<MemberByGroupResponse> response = new List<MemberByGroupResponse>();
                    foreach (var item in patrons)
                    {
                        MemberByGroupResponse single = new MemberByGroupResponse();
                        single.PatronID = item.PatronID;
                        response.Add(single);
                    }
                    if (response != null)
                    {
                        return Ok(new ResponseModel { Message = "Request Executed Successfully.", Status = "Success", Data = response });
                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "No Patron Found in This Hotel.", Status = "Success" });
                    }
                }
                else
                {

                    return BadRequest("Passed Parameter Invalid");
                }
            }
            catch
            (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        #endregion

        #region Member Patron

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
                            return Ok(new ResponseModel { Message = "The Member has been added Succesfully", Status = "Success" });

                        }
                        else
                        {
                            return BadRequest("Request Execution Failed");
                        }
                    }
                }
                else
                {
                    return BadRequest("Parameter's Invalid");
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("LeaveGroup")]
        public IHttpActionResult LeaveGroup(LeaveGroupModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                    {
                        var PatronGroupMember = _context.PatronsGroupsMembers.Where(m => m.MemberPatronID == model.MemberPatronID & m.PatronsGroupID == model.PatronsGroupID).FirstOrDefault();

                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<LeaveGroupModel, PatronsGroupsMember>();
                            cfg.CreateMap<PatronsGroupsMember, LeaveGroupModel>();

                        });

                        IMapper mapper = config.CreateMapper();
                        var data = mapper.Map<PatronsGroupsMember>(PatronGroupMember);
                        data.DateTimeLeftGroup = model.DateTimeLeftGroup;

                        _context.Entry(data).State = EntityState.Modified;
                        int Rows = _context.SaveChanges();

                        if (Rows > 0)
                        {
                            return Ok(new ResponseModel { Message = "Request executed Successfully", Status = "Success" });

                        }
                        else
                        {
                            return BadRequest("Request Execution Failed");

                        }
                    }
                }
                else
                {
                    return BadRequest("Parameters Invalid");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]//To check if the person have gorup and status in the group.
        [Route("IsExistInGroup")]
        public IHttpActionResult IsExistInGroup(int PatronsID, int HotelID)
        {
            try
            {
                if (PatronsID > 0 & HotelID > 0)
                {
                    var _Patrons = _context.PatronsGroups.Where(m => m.MasterPatronID == PatronsID & m.HotelID == HotelID & m.IsActive == true).FirstOrDefault();

                    if (_Patrons != null)
                    {
                        GroupByPatronResponse _response = new GroupByPatronResponse();
                        _response.MasterPatronID = _Patrons.MasterPatronID;
                        _response.IsMaster = true;
                        _response.GroupID = _Patrons.PatronsGroupID;

                        var memberpatrons = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == _Patrons.PatronsGroupID & m.DateTimeLeftGroup == null).ToList();
                        foreach (var item in memberpatrons)
                        {
                            MemberByGroupResponse data = new MemberByGroupResponse();
                            data.PatronID = item.MemberPatronID;
                            _response.MemeberPatrons.Add(data);
                        }
                        return Ok(new ResponseModel { Message = "Patrons have one group and he is a Master", Status = "Success", Data = _response });
                    }
                    else
                    {
                        var IsMemeber = _context.PatronsGroupsMembers.Where(m => m.MemberPatronID == PatronsID).FirstOrDefault();
                        if (IsMemeber != null)
                        {
                            var groupdetails = _context.PatronsGroups.Where(m => m.PatronsGroupID == IsMemeber.PatronsGroupID).FirstOrDefault();
                            GroupByPatronResponse _response = new GroupByPatronResponse();
                            _response.GroupID = groupdetails.PatronsGroupID;
                            _response.MasterPatronID = groupdetails.MasterPatronID;
                            _response.IsMaster = false;
                            return Ok(new ResponseModel { Message = "Patron have one group in the Hotel", Status = "Success", Data = _response });
                        }
                        else
                        {
                            return Ok(new ResponseModel { Message = "Patron deos not have any group", Status = "Success", });
                        }

                    }
                }
                else
                {
                    return BadRequest("Invalid Passed Parameter.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }


        [HttpGet]
        [Route("GetDeviceWithToken")]
        public IHttpActionResult GetDeviceWithToken(int HotelID)
        {
            try
            {
                if (HotelID != 0)
                {
                    var Patrons = _context.PatronsHotelLogIns.Where(m => m.HotelID == HotelID & m.LogoutDateTime == null).ToList();

                    List<DeviceTokenResponse> _DeviceTokenResponse = new List<DeviceTokenResponse>();
                    foreach (var item in Patrons)
                    {
                        DeviceTokenResponse DeviceTokenResponse = new DeviceTokenResponse();
                        var singlepatron = _context.Patrons.Where(m => m.PatronsID == item.PatronID).FirstOrDefault();
                        DeviceTokenResponse.FirstName = singlepatron.FirstName;
                        DeviceTokenResponse.LastName = singlepatron.LastName;
                        DeviceTokenResponse.DeviceToken = singlepatron.DeviceToken;
                        DeviceTokenResponse.DeviceType = singlepatron.DeviceType;

                        _DeviceTokenResponse.Add(DeviceTokenResponse);

                    }
                    return Ok(new ResponseModel { Message = "Request completed Successfully.", Status = "Success", Data = _DeviceTokenResponse });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            { return BadRequest(e.Message); }

        }

        #endregion

        #region Invitation

        [HttpGet]
        [Route("GetAllInvites")]
        public IHttpActionResult GetAllInvites(int PatronID, int HotelID)
        {
            try
            {
                if (PatronID !=0 & HotelID !=0)
                {
                    var Invites = _context.PatronsGroupInvitations.Where(m => m.HotelID== HotelID & m.PatronID== PatronID).ToList();
                    if (Invites!=null)
                    {
                        List<InvitationResponse> invitationResponse = new List<InvitationResponse>();
                        foreach (var item in Invites)
                        {
                            InvitationResponse invitation = new InvitationResponse();
                            invitation.PatronsGroupID = item.PatronsGroupID;
                            invitation.HotelID = item.HotelID;
                            invitation.MasterPatornID = item.MasterPatronID;

                            invitationResponse.Add(invitation);
                        }
                        return Ok(new ResponseModel { Message = "Requeset Executed Successfully", Status = "Success", Data = invitationResponse });
                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "No Invitation is Available for this patron.", Status = "Success"});
                    }
                }
                else
                {
                    return BadRequest("Passed Parameter's Invalid");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("DeclineInvite")]
        public IHttpActionResult DeclineInvite(int PatronID, int HotelID)
        {
            try
            {
                if (PatronID!=0& HotelID!=0)
                {
                    var request = _context.PatronsGroupInvitations.Where(m => m.HotelID == HotelID & m.PatronID == PatronID).FirstOrDefault();
                    if (request!=null)
                    {
                        request.IsAccepted = false;
                        _context.Entry(request).State = EntityState.Modified;
                        int i=_context.SaveChanges();
                        if (i>0)
                        {
                            return Ok(new ResponseModel { Message = "Status has been Updated Successfully", Status = "Success" });
                        }
                        else
                        {
                            return Ok(new ResponseModel { Message = "Status Updation Failed", Status = "Success" });

                        }
                        
                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "No Request is Available.", Status = "Success"});
                    }
                }
                else
                {
                    return BadRequest("Passed Parameter's Invalid");
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