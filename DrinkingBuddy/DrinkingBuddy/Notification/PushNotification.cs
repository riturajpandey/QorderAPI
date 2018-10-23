using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using DrinkingBuddy.Entities;
using DrinkingBuddy.Models;
using AutoMapper;
using System.Web.Http;


namespace DrinkingBuddy.Notification
{
    public class PushNotification
    {
        DrinkingBuddyEntities _context = new DrinkingBuddyEntities();


        /// <summary>
        /// Sends the notification. 
        /// call this api as "api/notification/send" ajax method type: GET
        /// 
        /// </summary>
        /// <returns>The notification.</returns>
        /// <param name="deviceToken1">Device token.</param>
        /// <param name="message">message.</param>
        /// public bool SendNotification(string deviceToken1, string message)

        public bool SendNotification(List<string> deviceToken, string message) // use it, if want to send push notification on multiple device
        {
            //*******************  this is the section for multiple device ****************************

            //List<string> deviceToken = new List<string>();
            //deviceToken.Add("cgAYD30kCMw:APA91bEkBnSVjH6qekzazihDzh-0cTuumlD0Q9IFjwxypZldbCN-SURtS7pTbeSZ1e-Z1IeReFMYzi4VDGDGV-hzCuR92oI4tYoM8PEUY2yaXt8L-pummgTZOWcrQIcO503wx3jfpbSW");
            //deviceToken.Add("cgAYD30kCMw:APA91bEkBnSVjH6qekzazihDzh-0cTuumlD0Q9IFjwxypZldbCN-SURtS7pTbeSZ1e-Z1IeReFMYzi4VDGDGV-hzCuR92oI4tYoM8PEUY2yaXt8L-pummgTZOWcrQIcO503wx3jfpbSW");

            // code of push notification

            string applicationID = "AIzaSyA5p2s4Y4QiFXgsAl3rxihqaicTfAi8NgM"; // get from Firebase console
            ;
            string senderId = "312750556222"; // get from Firebase console
            string deviceId = string.Join("\",\"", deviceToken);  // if want to send notification on multiple device som please chnage the parameter here.
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = "application/json";
            var data = new
            {
                to = deviceId,
                notification = new
                {
                    body = message,
                    title = "Notifiaction",
                    sound = "Enabled"
                }
            };
            string jsonNotificationFormat = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            Byte[] byteArray = Encoding.UTF8.GetBytes(jsonNotificationFormat);
            tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
            tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
            tRequest.ContentLength = byteArray.Length;

            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            String sResponseFromServer = tReader.ReadToEnd();
                            string str = sResponseFromServer;
                        }
                    }
                }
            }
            return true;
        }

        public bool InsertNotification(NotificationBindingModel model)
        { //TODO:-A mothod used to insert notification in the notification table.
            try
            {
                if (model != null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<NotificationBindingModel, PatronsNotification>();

                    });

                    IMapper mapper = config.CreateMapper();
                    var notification = mapper.Map<PatronsNotification>(model);

                    _context.PatronsNotifications.Add(notification);
                    int row = _context.SaveChanges();
                    if (row > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;

                }

            }
            catch (Exception ex)
            {

                return false;

            }

        }

        public List<string> FindDeviceToken(List<int> PatronIDs, int PatronID)
        {
            try
            {
                List<string> devicetoken = new List<string>();
                if (PatronID > 0)
                {
                    var paton = _context.Patrons.Where(m => m.PatronsID == PatronID).FirstOrDefault();

                    string token= paton.DeviceToken;

                    devicetoken.Add(token);


                    return devicetoken;
                }

                if (PatronIDs!=null)
                {

                    foreach (var item in PatronIDs)
                    {
                        var patron = _context.Patrons.Where(m => m.PatronsID == item).FirstOrDefault();
                        devicetoken.Add(patron.DeviceToken);
                    }
                    return devicetoken;
                }

                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

    }

}



//Thpe of notification to be inserted.
//Offers
// Money Transfer
// Group Invites
// Orders
