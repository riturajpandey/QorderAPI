using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrinkingBuddy.Models
{
    public class Notifications
    {
        public int PatronsNotificationID { get; set; }
        public string NotificationContent { get; set; }
        public Nullable<System.DateTime> DateTimeSent { get; set; }
        public string NotificationType { get; set; }
    }

    public class NotificationBindingModel
    {
        public Nullable<int> PatronID { get; set; }
        public string NotificationContent { get; set; }
        public Nullable<System.DateTime> DateTimeSent { get; set; }
        public string NotificationType { get; set; }
    }


    public class Notificationsresponse
    {
        public int PatronsNotificationID { get; set; }
        public string NotificationContent { get; set; }
        public Nullable<System.DateTime> DateTimeSent { get; set; }
        public string NotificationType { get; set; }
        public bool IsRead { get; set; }
    }

    public class NotificationMember
    {
        public int PatronID { get; set; }
        public int PatronGroupID { get; set; }

    }
    public class ReadNotificationRequestModel
    {
        public List<ReadNotification> ReadNotifications { get; set; }
    }
    public class ReadNotification
    {
        public int NotificationId { get; set; }
    }
}