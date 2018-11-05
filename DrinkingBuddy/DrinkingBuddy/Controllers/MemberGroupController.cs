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
using DrinkingBuddy.Notification;
using AutoMapper;
using System.Data.Entity;
using System.Threading;

namespace DrinkingBuddy.Controllers
{
    [APIAuthorizeAttribute]
    [RoutePrefix("api/MemberGroup")]
    public class MemberGroupController : ApiController
    {
        DrinkingBuddyEntities _context = new DrinkingBuddyEntities();
        PushNotification push = new PushNotification();

        string Message;

        #region Master Patron

        [HttpPost]
        [Route("StartGroup")]
        public IHttpActionResult StartGroup(StartGroupBinding model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest("The Passed Parametes are not valid");

                }


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
                if (Rows == 0)
                {
                    return BadRequest("The Requested Group could not be created.");

                }
                //CODE FOR STARTGROPRESPONSE MODEL CHANGING MODEL TO "GroupByPatronResponse"

                //var Hotel = _context.Hotels.Where(m => m.HotelID == model.HotelID).FirstOrDefault();
                //var Patons = _context.Patrons.Where(m => m.PatronsID == model.MasterPatronID).FirstOrDefault();
                //var patrongroup = _context.PatronsGroups.Where(m => m.MasterPatronID == model.MasterPatronID).FirstOrDefault();
                //StartGroupResponse Response = new StartGroupResponse();
                //Response.PatronsGroupID = patrongroup.PatronsGroupID;
                //Response.HotelName = Hotel.HotelName;
                //Response.GroupMasterPatron = Patons.FirstName + " " + Patons.LastName;
                //Response.GroupStartDateTime = data.GroupStartedDateTime;

                //CODE FOR GroupByPatronResponse MODEL.

                var groupdetails = _context.PatronsGroups.Where(m => m.MasterPatronID == model.MasterPatronID & m.HotelID == model.HotelID).OrderByDescending(m => m.GroupStartedDateTime == model.GroupStartedDateTime).FirstOrDefault();
                GroupByPatronResponse Response = new GroupByPatronResponse();
                Response.GroupID = groupdetails.PatronsGroupID;
                Response.MasterPatronID = groupdetails.MasterPatronID;
                Response.IsMaster = true;
                Response.MemeberPatrons = null;


                if (Response == null)
                {
                    return Ok(new ResponseModel { Message = "Request Failed", Status = "Failed" });
                }
                //code to De-activate group after 5 hours.

                var autoevent = new AutoResetEvent(true);
                var timer = new Timer(
                            e => DeActivateGruopById(groupdetails.PatronsGroupID),
                    autoevent,
                     TimeSpan.FromHours(5),
                     TimeSpan.Zero);

               
                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = Response });



            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //TODO:The method will Deactivated the group and the members associated with the gruop after 5 Hours.
        private void DeActivateGruopById(int GroupID)
        {
            try
            {

                var groups = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == GroupID).ToList();

                var mester = _context.PatronsGroups.Where(m => m.PatronsGroupID == GroupID).FirstOrDefault();

                var masterpatrondetails = _context.Patrons.Where(m => m.PatronsID == mester.MasterPatronID).FirstOrDefault();

                var hotel = _context.Hotels.Where(m => m.HotelID == mester.HotelID).FirstOrDefault();


                List<string> devicetoken = new List<string>();
                string token = masterpatrondetails.DeviceToken;
                devicetoken.Add(token);

                foreach (var item in groups)
                {
                    var patron = _context.Patrons.Where(m => m.PatronsID == item.MemberPatronID).FirstOrDefault();

                    devicetoken.Add(patron.DeviceToken);

                }

                Message = "Your group of " + hotel.HotelName + "has expired.";
                push.SendNotification(devicetoken, Message);
                NotificationBindingModel notificationBindingModel = new NotificationBindingModel();
                notificationBindingModel.DateTimeSent = DateTime.Now;
                notificationBindingModel.PatronID = masterpatrondetails.PatronsID;
                notificationBindingModel.NotificationContent = Message;
                notificationBindingModel.NotificationType = "Group Invites";
                push.InsertNotification(notificationBindingModel);


                var group = _context.PatronsGroups.Where(m => m.PatronsGroupID == GroupID).FirstOrDefault();

                _context.PatronsGroups.Remove(group);
                int i = _context.SaveChanges();


                var groupmember = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == GroupID).ToList();
                List<PatronsGroupsMember> listgroupmember = new List<PatronsGroupsMember>();
                foreach (var item in groupmember)
                {
                    item.DateTimeLeftGroup = DateTime.Now;
                    listgroupmember.Add(item);
                }

                _context.PatronsGroupsMembers.RemoveRange(listgroupmember);
                int row = _context.SaveChanges();

                var groupInvitation = _context.PatronsGroupInvitations.Where(m => m.PatronsGroupID == GroupID).ToList();
                _context.PatronsGroupInvitations.RemoveRange(groupInvitation);
                int j = _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }


        //   TODO:The method will update the group as inactive and all the members associated with the group.
        [HttpGet]
        [Route("StopGroup")]
        public IHttpActionResult StopGroup(int PatronGroupID)
        {
            try
            {
                if (PatronGroupID == 0)
                {
                    return BadRequest("provided Parameters are  not Valid");

                }

                PatronsGroup data = _context.PatronsGroups.Where(m => m.PatronsGroupID == PatronGroupID).FirstOrDefault();
                if (data == null)
                {

                    return Ok(new ResponseModel { Message = "No Group Found.", Status = "Success" });
                }
                _context.PatronsGroups.Remove(data);
                int result = _context.SaveChanges();
                if (result == 0)
                {
                    return Ok(new ResponseModel { Message = "The Group deletion Failed", Status = "Success" });
                }
                var patroninvitation = _context.PatronsGroupInvitations.Where(m => m.PatronsGroupID == PatronGroupID).ToList();
                _context.PatronsGroupInvitations.RemoveRange(patroninvitation);
                int row = _context.SaveChanges();
                if (row == 0)
                {
                    return Ok(new ResponseModel { Message = "The Group has been Stop but there is no invitation", Status = "Success" });
                }
                var patronmember = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == PatronGroupID).ToList();
                _context.PatronsGroupsMembers.RemoveRange(patronmember);
                int i = _context.SaveChanges();
                if (i > 0)
                {

                    return Ok(new ResponseModel { Message = "The Group and invitaion has been deleted.", Status = "Success" });
                }

                var group = _context.PatronsGroups.Where(m => m.PatronsGroupID == PatronGroupID).FirstOrDefault();
                if (group == null)
                {
                    return BadRequest("No group found for notification.");
                }

                var groupmemebers = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == PatronGroupID).ToList();
                if (groupmemebers.Count() > 0)
                {
                    return BadRequest("No members found for notification.");
                }

                var master = _context.Patrons.Where(m => m.PatronsID == group.MasterPatronID).FirstOrDefault();
                if (master == null)
                {
                    return BadRequest("No master found for notification.");
                }

                var hotel = _context.Hotels.Where(m => m.HotelID == group.HotelID).FirstOrDefault();
                if (hotel == null)
                {
                    return BadRequest("No Hotel Found to Display");
                }

                List<string> devicetoken = new List<string>();
                foreach (var item in groupmemebers)
                {
                    var patron = _context.Patrons.Where(m => m.PatronsID == item.MemberPatronID).FirstOrDefault();

                    var token = patron.DeviceToken;
                    devicetoken.Add(token);
                }


                //Message to be sent to the all group memebers.
                Message = master.FirstName + " " + master.LastName + " have stopped the Group,Created in " + hotel.HotelName;
                push.SendNotification(devicetoken, Message);
                foreach (var item in groupmemebers)
                {
                    NotificationBindingModel notificationBindingModel = new NotificationBindingModel();
                    notificationBindingModel.DateTimeSent = DateTime.Now;
                    notificationBindingModel.PatronID = item.MemberPatronID;
                    notificationBindingModel.NotificationContent = Message;
                    notificationBindingModel.NotificationType = "Group";
                    push.InsertNotification(notificationBindingModel);
                }
                return Ok(new ResponseModel { Message = "The Group and invitation along with Group member has been deleted Successfully", Status = "Success" });
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
                if (PatronsGroupID == 0)
                {
                    return BadRequest();
                }
                var Isgroupalive = _context.PatronsGroups.Where(m => m.PatronsGroupID == PatronsGroupID).FirstOrDefault();
                if (Isgroupalive != null)
                {
                    return Ok(new ResponseModel { Message = "The Group is Active.", Status = "Success" });
                }
                else
                {
                    return Ok(new ResponseModel { Message = "The Group has been Expired.", Status = "Failed" });
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
                if (PatrongroupID == 0 & PatronsID == 0)
                {
                    return BadRequest("Passed Parameter Invalid.");

                }
                var _patroningroup = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == PatrongroupID & m.MemberPatronID == PatronsID).FirstOrDefault();
                if (_patroningroup == null)
                {
                    return BadRequest("Patron is not found in the goup.");
                }
                _context.PatronsGroupsMembers.Remove(_patroningroup);
                int i = _context.SaveChanges();
                if (i > 0)
                {
                    return Ok(new ResponseModel { Message = "Patron Removed Successfully.", Status = "Success" });
                }

                return Ok(new ResponseModel { Message = "Request Execution failed.", Status = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]//To show list of member in perticular group
        [Route("GetMemberByGroup")]
        public IHttpActionResult GetMemberByGroup(int GroupID)
        {
            try
            {
                if (GroupID == 0)
                {
                    return BadRequest("Invalid Passed Parameter");
                }
                var members = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == GroupID & m.DateTimeLeftGroup == null).ToList();
                if (members == null)
                {
                    return Ok(new ResponseModel { Message = "Patron Removed Successfully.", Status = "Success" });
                }
                List<MemberByGroupResponse> response = new List<MemberByGroupResponse>();
                foreach (var item in members)
                {
                    MemberByGroupResponse single = new MemberByGroupResponse();
                    single.PatronID = item.MemberPatronID;
                    response.Add(single);
                }
                return Ok(new ResponseModel { Message = "Request Executed Successfully.", Status = "Success", Data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]//To show a list of Patrons logged-in in a specifi hotel for invitation.
        [Route("GetLoggedInPatrons")]
        public IHttpActionResult GetLoggedInPatrons(int HotelID, int PatronID, int GroupID)
        {
            try
            {
                if (HotelID == 0 & PatronID == 0 & GroupID == 0)
                {
                    return BadRequest("Passed Parameter Invalid");
                }
                var patrons = _context.PatronsHotelLogIns.Where(m => m.HotelID == HotelID & m.PatronID != PatronID & m.LogoutDateTime == null).ToList();
                if (patrons.Count() == 0)
                {

                    return Ok(new ResponseModel { Message = "No Patron Found in This Hotel.", Status = "Success" });
                }

                List<PatronByHotelResponse> response = new List<PatronByHotelResponse>();
                foreach (var item in patrons)
                {
                    PatronByHotelResponse single = new PatronByHotelResponse();

                    var data = _context.PatronsGroupInvitations.Where(m => m.PatronsGroupID == GroupID & m.PatronID == item.PatronID).FirstOrDefault();

                    if (data == null)
                    {
                        single.InvitationStatus = "Invite";
                    }
                    else
                    {


                        if (data.IsAccepted == true)
                        {
                            single.InvitationStatus = "Accepted";
                        }
                        if (data.IsAccepted == false)
                        {
                            single.InvitationStatus = "Rejected";
                        }
                        if (data.IsAccepted == null)
                        {
                            single.InvitationStatus = "Invited";
                        }

                    }
                    single.PatronID = item.PatronID;
                    single.HotelID = item.HotelID;
                    response.Add(single);
                }

                if (response.Count() == 0)
                {
                    return Ok(new ResponseModel { Message = "No Patron Found in This Hotel.", Status = "Success" });
                }
                return Ok(new ResponseModel { Message = "Request Executed Successfully.", Status = "Success", Data = response });

            }
            catch
            (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SendInvitation")]
        public IHttpActionResult SendInvitation(SendInviteBinding model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<SendInviteBinding, PatronsGroupInvitation>();
                        cfg.CreateMap<PatronsGroupInvitation, SendInviteBinding>();

                    });

                IMapper mapper = config.CreateMapper();
                var data = mapper.Map<PatronsGroupInvitation>(model);
                data.RequestSentDateTime = DateTime.Now;
                _context.PatronsGroupInvitations.Add(data);
                int i = _context.SaveChanges();
                if (i == 0)
                {
                    return Ok(new ResponseModel { Message = "The Invitation Has been Saved.", Status = "Success" });
                }
                var Masterdetails = _context.Patrons.Where(m => m.PatronsID == model.MasterPatronID).FirstOrDefault();
                var ReciverPatron = _context.Patrons.Where(m => m.PatronsID == model.PatronID).FirstOrDefault();
                var hotel = _context.Hotels.Where(m => m.HotelID == model.HotelID).FirstOrDefault();

                List<string> devicetoken = new List<string>();
                string token = ReciverPatron.DeviceToken;
                devicetoken.Add(token);

                Message = Masterdetails.FirstName + " " + Masterdetails.LastName + " have inivited you to join his Group in " + hotel.HotelName;
                push.SendNotification(devicetoken, Message);
                NotificationBindingModel notificationBindingModel = new NotificationBindingModel();
                notificationBindingModel.DateTimeSent = DateTime.Now;
                notificationBindingModel.PatronID = ReciverPatron.PatronsID;
                notificationBindingModel.NotificationContent = Message;
                notificationBindingModel.NotificationType = "Invites";
                push.InsertNotification(notificationBindingModel);


                return Ok(new ResponseModel { Message = "The Invitation Has been Saved.", Status = "Success" });

            }
            catch (Exception ex)
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
                if (!ModelState.IsValid)
                {
                    return BadRequest("Parameter's Invalid");

                }

                var ChangeInvitestatus = _context.PatronsGroupInvitations.Where(m => m.PatronsGroupID == model.PatronsGroupID & m.PatronID == model.MemberPatronID & m.IsAccepted == null).FirstOrDefault();
                ChangeInvitestatus.IsAccepted = true;
                ChangeInvitestatus.RequestAcceptDateTime = DateTime.Now;
                _context.Entry(ChangeInvitestatus).State = EntityState.Modified;
                int Rows = _context.SaveChanges();
                if (Rows == 0)
                {
                    return BadRequest("Request Execution Failed");
                }
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<AcceptInviteModel, PatronsGroupsMember>();
                    cfg.CreateMap<PatronsGroupsMember, AcceptInviteModel>();

                });

                IMapper mapper = config.CreateMapper();
                var data = mapper.Map<PatronsGroupsMember>(model);
                _context.PatronsGroupsMembers.Add(data);

                int row = _context.SaveChanges();
                if (row == 0)
                {

                    return BadRequest("Request Execution Failed");
                }

                var groupdetails = _context.PatronsGroups.Where(m => m.PatronsGroupID == model.PatronsGroupID).FirstOrDefault();
                GroupByPatronResponse _response = new GroupByPatronResponse();
                _response.GroupID = model.PatronsGroupID;
                _response.MasterPatronID = groupdetails.MasterPatronID;
                _response.IsMaster = false;

                var memberpatrons = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == groupdetails.PatronsGroupID & m.DateTimeLeftGroup == null).ToList();

                if (memberpatrons.Count() > 0)
                {
                    _response.MemeberPatrons = null;

                    return Ok(new ResponseModel { Message = "The Member has been added Succesfully", Status = "Success", Data = _response });
                }

                List<MemberByGroupResponse> listsingle = new List<MemberByGroupResponse>();
                foreach (var item in memberpatrons)
                {
                    MemberByGroupResponse single = new MemberByGroupResponse();
                    single.PatronID = item.MemberPatronID;
                    listsingle.Add(single);
                }
                _response.MemeberPatrons = listsingle;


                var group = _context.PatronsGroups.Where(m => m.PatronsGroupID == model.PatronsGroupID).FirstOrDefault();
                var sendermaster = _context.Patrons.Where(m => m.PatronsID == group.MasterPatronID).FirstOrDefault();
                var ReciverPatron = _context.Patrons.Where(m => m.PatronsID == model.MemberPatronID).FirstOrDefault();
                var hotel = _context.Hotels.Where(m => m.HotelID == group.HotelID).FirstOrDefault();

                List<string> devicetoken = new List<string>();
                string token = sendermaster.DeviceToken;
                devicetoken.Add(token);

                Message = ReciverPatron.FirstName + " " + ReciverPatron.LastName + " have accepted your invitation in " + hotel.HotelName;
                push.SendNotification(devicetoken, Message);
                NotificationBindingModel notificationBindingModel = new NotificationBindingModel();
                notificationBindingModel.DateTimeSent = DateTime.Now;
                notificationBindingModel.PatronID = sendermaster.PatronsID;
                notificationBindingModel.NotificationContent = Message;
                notificationBindingModel.NotificationType = "Invites";
                push.InsertNotification(notificationBindingModel);


                return Ok(new ResponseModel { Message = "The Member has been added Succesfully", Status = "Success", Data = _response });


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
                if (!ModelState.IsValid)
                {
                    return BadRequest("Parameters Invalid");

                }

                var PatronGroupMember = _context.PatronsGroupsMembers.Where(m => m.MemberPatronID == model.MemberPatronID & m.PatronsGroupID == model.PatronsGroupID).FirstOrDefault();
                var patronInvitation = _context.PatronsGroupInvitations.Where(m => m.PatronsGroupID == model.PatronsGroupID & m.PatronID == model.MemberPatronID).FirstOrDefault();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<LeaveGroupModel, PatronsGroupsMember>();
                    cfg.CreateMap<PatronsGroupsMember, LeaveGroupModel>();

                });

                IMapper mapper = config.CreateMapper();
                var data = mapper.Map<PatronsGroupsMember>(PatronGroupMember);
                data.DateTimeLeftGroup = model.DateTimeLeftGroup;

                _context.Entry(data).State = EntityState.Modified;
                _context.PatronsGroupInvitations.Remove(patronInvitation);
                int Rows = _context.SaveChanges();

                if (Rows > 0)
                {
                    return BadRequest("Request Execution Failed");
                }
                var leaver = _context.Patrons.Where(m => m.PatronsID == model.MemberPatronID).FirstOrDefault();
                var group = _context.PatronsGroups.Where(m => m.PatronsGroupID == model.PatronsGroupID).FirstOrDefault();
                var master = _context.Patrons.Where(m => m.PatronsID == group.MasterPatronID).FirstOrDefault();

                List<string> devicetoken = new List<string>();
                string token = master.DeviceToken;
                devicetoken.Add(token);

                //Message to be sent to the master of the group.
                Message = leaver.FirstName + " " + leaver.LastName + " have Left the Group.";
                push.SendNotification(devicetoken, Message);

                NotificationBindingModel notificationBindingModel = new NotificationBindingModel();
                notificationBindingModel.DateTimeSent = DateTime.Now;
                notificationBindingModel.PatronID = master.PatronsID;
                notificationBindingModel.NotificationContent = Message;
                notificationBindingModel.NotificationType = "Group";
                push.InsertNotification(notificationBindingModel);


                return Ok(new ResponseModel { Message = "Request executed Successfully", Status = "Success" });

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
                if (PatronsID == 0 & HotelID == 0)
                {
                    return BadRequest("Invalid Passed Parameter.");
                }

                var _Patrons = _context.PatronsGroups.Where(m => m.MasterPatronID == PatronsID & m.HotelID == HotelID & m.IsActive == true).FirstOrDefault();

                if (_Patrons != null)
                {
                    GroupByPatronResponse _response = new GroupByPatronResponse();
                    _response.MasterPatronID = _Patrons.MasterPatronID;
                    _response.IsMaster = true;
                    _response.GroupID = _Patrons.PatronsGroupID;

                    var memberpatrons = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == _Patrons.PatronsGroupID & m.DateTimeLeftGroup == null).ToList();

                    List<MemberByGroupResponse> data = new List<MemberByGroupResponse>();
                    foreach (var item in memberpatrons)
                    {
                        MemberByGroupResponse single = new MemberByGroupResponse();
                        single.PatronID = item.MemberPatronID;
                        data.Add(single);
                    }
                    _response.MemeberPatrons = data;
                    return Ok(new ResponseModel { Message = "Patrons have one group and he is the Master", Status = "Success", Data = _response });
                }
                else
                {
                    var IsMemeber = _context.PatronsGroupsMembers.Where(m => m.MemberPatronID == PatronsID & m.DateTimeLeftGroup == null).FirstOrDefault();
                    if (IsMemeber == null)
                    {
                        return Ok(new ResponseModel { Message = "Patron deos not have any group", Status = "Success", });

                    }
                    var groupdetails = _context.PatronsGroups.Where(m => m.PatronsGroupID == IsMemeber.PatronsGroupID).FirstOrDefault();
                    GroupByPatronResponse _response = new GroupByPatronResponse();
                    _response.GroupID = groupdetails.PatronsGroupID;
                    _response.MasterPatronID = groupdetails.MasterPatronID;
                    _response.IsMaster = false;

                    var memberpatrons = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == groupdetails.PatronsGroupID & m.DateTimeLeftGroup == null).ToList();

                    List<MemberByGroupResponse> data = new List<MemberByGroupResponse>();
                    foreach (var item in memberpatrons)
                    {
                        MemberByGroupResponse single = new MemberByGroupResponse();
                        single.PatronID = item.MemberPatronID;
                        data.Add(single);
                    }
                    _response.MemeberPatrons = data;

                    return Ok(new ResponseModel { Message = "Patron have one group in the Hotel", Status = "Success", Data = _response });


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
                if (HotelID == 0)
                {
                    return BadRequest();
                }
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
            catch (Exception e)
            { return BadRequest(e.Message); }

        }



        [HttpGet]
        [Route("GetAllInvites")]
        public IHttpActionResult GetAllInvites(int PatronID, int HotelID)
        {
            try
            {
                if (PatronID == 0 & HotelID == 0)
                {
                    return BadRequest("Passed Parameter's Invalid");
                }

                var Invites = _context.PatronsGroupInvitations.Where(m => m.HotelID == HotelID & m.PatronID == PatronID & m.IsAccepted == null).ToList();
                if (Invites.Count() == 0)
                {
                    return Ok(new ResponseModel { Message = "No Invitation is Available for this patron.", Status = "Success" });
                }
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("DeclineInvite")]
        public IHttpActionResult DeclineInvite(int PatronID, int GroupID)
        {
            try
            {
                if (PatronID == 0 & GroupID == 0)
                {
                    return BadRequest("Passed Parameter's Invalid");

                }
                var request = _context.PatronsGroupInvitations.Where(m => m.PatronsGroupID == GroupID & m.PatronID == PatronID).FirstOrDefault();
                if (request == null)
                {

                    return Ok(new ResponseModel { Message = "No Request is Available.", Status = "Success" });
                }
                request.IsAccepted = false;
                _context.Entry(request).State = EntityState.Modified;
                int i = _context.SaveChanges();
                if (i == 0)
                {
                    return Ok(new ResponseModel { Message = "Status Updation Failed", Status = "Success" });
                }
                var group = _context.PatronsGroups.Where(m => m.PatronsGroupID == PatronID).FirstOrDefault();
                var sendermaster = _context.Patrons.Where(m => m.PatronsID == group.MasterPatronID).FirstOrDefault();
                var ReciverPatron = _context.Patrons.Where(m => m.PatronsID == PatronID).FirstOrDefault();


                List<string> devicetoken = new List<string>();
                string token = sendermaster.DeviceToken;
                devicetoken.Add(token);

                Message = ReciverPatron.FirstName + " " + ReciverPatron.LastName + " have rejected your invitation";
                push.SendNotification(devicetoken, Message);
                NotificationBindingModel notificationBindingModel = new NotificationBindingModel();
                notificationBindingModel.DateTimeSent = DateTime.Now;
                notificationBindingModel.PatronID = sendermaster.PatronsID;
                notificationBindingModel.NotificationContent = Message;
                notificationBindingModel.NotificationType = "Invites";
                push.InsertNotification(notificationBindingModel);


                return Ok(new ResponseModel { Message = "Status has been Updated Successfully", Status = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        #endregion
    }
}
