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
}