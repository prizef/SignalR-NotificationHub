using Microsoft.AspNet.SignalR;
using Models.Domain;
using Services;
using Services.Interfaces;
using Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web
{
    public class NotificationSender : INotificationSender
    {
        public void SendNotificationsToUser(int userId, string notifications)
        {
            NotificationHub.SendNotification(userId, notifications);
        }
    }
}