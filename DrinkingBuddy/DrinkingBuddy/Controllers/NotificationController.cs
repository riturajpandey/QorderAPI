using System.Linq;
using System.Web;
using System.Collections.Generic;
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
using DrinkingBuddy.Notification;
using DrinkingBuddy.Results;
using AutoMapper;
using System.Data.Entity;
using DrinkingBuddy.SMS;
using System;

namespace DrinkingBuddy.Controllers
{
    [APIAuthorizeAttribute]
    [RoutePrefix("api/Notification")]
    public class NotificationController : ApiController
    {
        DrinkingBuddyEntities _context = new DrinkingBuddyEntities();

        [HttpGet]
        [Route("GetNotification")]
        public IHttpActionResult GetNotifications(int PatronID)
        {
            try
            {
                if (PatronID == 0)
                {
                    return BadRequest("Passed Parameter is not valid.");
                }
                var notifications = _context.PatronsNotifications.Where(m => m.PatronID == PatronID).OrderByDescending(m=>m.DateTimeSent).ToList();
                if (notifications.Count() == 0)
                {
                    return Ok(new ResponseModel { Message = "No Notification found for this patron.", Status = "Success" });
                }
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<PatronsNotification, Notificationsresponse>();

                });

                IMapper mapper = config.CreateMapper();
                var dataOrder = mapper.Map<List<Notificationsresponse>>(notifications);

                return Ok(new ResponseModel { Message = "Request Executed Successfully.", Status = "Success", Data = dataOrder });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("MarkRead")]
        public IHttpActionResult MarkRead(ReadNotificationRequestModel model)
        {
            try
            {
                if (model.ReadNotifications.Count() == 0)
                {
                    return BadRequest("No PatronNotificationID Passed");

                }
                List<PatronsNotification> listpatronsNotification = new List<PatronsNotification>();
                foreach (var item in model.ReadNotifications)
                {
                    PatronsNotification single = new PatronsNotification();
                    var notification = _context.PatronsNotifications.Where(m => m.PatronsNotificationID == item.NotificationId).FirstOrDefault();
                    if (notification == null)
                    {
                        return BadRequest("No Notification found for this NotificationId");
                    }
                    single = notification;
                    single.IsRead = true;
                    listpatronsNotification.Add(single);
                }
                int row = 0;
                foreach (var item in listpatronsNotification)
                {
                    _context.Entry(item).State = EntityState.Modified;
                    row = _context.SaveChanges();
                }

                if (row == 0)
                {
                    return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Success" });
                }
                return Ok(new ResponseModel { Message = "Notifications updated as Read.", Status = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Route("GetUnread")]
        public IHttpActionResult GetUnread(int PatronId)
        {
            try
            {
                if (PatronId == 0)
                {
                    return BadRequest("The PatonsID is invalid.");
                }


                var notification = _context.PatronsNotifications.Where(m => m.PatronID == PatronId & m.IsRead == false).ToList();
                if (notification.Count() == 0)
                {
                    return Ok(new ResponseModel { Message = "No Notification found for this patron.", Status = "Success", Data = 0 });

                }
                var unreadcount = notification.Count();
                return Ok(new ResponseModel { Message = "Request Executed Successfully.", Status = "Success", Data = unreadcount });


            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

    }
}
