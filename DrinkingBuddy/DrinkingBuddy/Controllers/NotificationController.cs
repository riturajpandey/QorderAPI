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
        public IHttpActionResult GetNotification(int PatronID)
        {
            try
            {
                if (PatronID > 0)
                {
                    var notifications = _context.PatronsNotifications.Where(m => m.PatronID == PatronID).Take(25);
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<PatronsNotification, Notifications>();

                    });

                    IMapper mapper = config.CreateMapper();
                    var dataOrder = mapper.Map<List<Notifications>>(notifications);

                    return Ok(new ResponseModel { Message = "Request Executed Successfully.", Status = "Success", Data = dataOrder });

                }
                else
                {
                    return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Success" });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        //[HttpPost]
        //[Route("SaveNotification")]//TODO:Testing purpoe only rest the method should be used instead.
        //public IHttpActionResult SaveNotification(NotificationBindingModel model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            PushNotification pushNotification = new PushNotification();
        //           bool result= pushNotification.InsertNotification(model);
        //            return Ok(new ResponseModel { Message = "Request Executed Successfully.", Status = "Success", Data = result });
        //        }
        //        else
        //        {
        //            return Ok(new ResponseModel { Message = "Request Executed Successfully.", Status = "Success"});
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }


        //}


        //[HttpGet]
        //[Route("Message")]///Testing pupose only
        //public IHttpActionResult Message()
        //{
        //    try
        //    {
        //        PushNotification pushNotification = new PushNotification();


        //        var devicetoken = pushNotification.FindDeviceToken(null,0);
        //        var response = pushNotification.SendNotification(devicetoken,"Hi Drinking Buddy");

        //        return Ok(new ResponseModel { Message = "Request Executed Successfully.", Status = "Success", Data = response });

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }


        //}

    }
}
