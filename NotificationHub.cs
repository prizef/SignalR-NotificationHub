using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Data.Providers;
using Models.Domain;
using Models.Responses;
using Services;
using Services.Security;

namespace Web.App_Start
{
    public class NotificationHub : Hub
    {
        readonly INotificationsService notificationsService;

        public NotificationHub(INotificationsService notificationsService)
        {
            this.notificationsService = notificationsService;
        }

        static Dictionary<int, HashSet<string>> userIdToConnectionIds = new Dictionary<int, HashSet<string>>();

        public override Task OnConnected()
        {
            int userId = Context.User.Identity.GetId().Value;
            string connectionId = Context.ConnectionId;

            string notifications = notificationsService.GetNotificationByUserId(userId);
            Clients.Caller.addNotification(notifications);

            lock (userIdToConnectionIds)
            {
                if (!userIdToConnectionIds.TryGetValue(userId, out HashSet<string> connections))
                {
                    connections = new HashSet<string>();
                    userIdToConnectionIds.Add(userId, connections);
                }
                connections.Add(connectionId);
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            int userId = Context.User.Identity.GetId().Value;
            string connectionId = Context.ConnectionId;

            lock (userIdToConnectionIds)
            {
                HashSet<string> connections = userIdToConnectionIds[userId];
                connections.Remove(connectionId);

                if (connections.Count == 0)
                {
                    userIdToConnectionIds.Remove(userId);
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        public static string[] GetConnections(int key)
        {
            HashSet<string> connections;
            if (userIdToConnectionIds.TryGetValue(key, out connections))
            {
                return connections.ToArray();
            }

            return new string[0];
        }

        public static void SendNotification(int userId, string items)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            
            lock (userIdToConnectionIds)
            {
                string[] connections = GetConnections(userId);
                foreach (var connectionId in connections)
                {
                    hub.Clients.Client(connectionId).SendNotificationsToUser(items);
                }
            }
        }
    }
}