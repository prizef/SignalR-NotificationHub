using Data.Providers;
using Models.Domain;
using Models.Requests;
using Services.Interfaces;
using Services.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Services
{
    public class NotificationsService : INotificationsService
    {
        readonly IDataProvider dataProvider;
        readonly INotificationSender notificationSender;

        public NotificationsService(IDataProvider dataProvider, INotificationSender notificationSender)
        {
            this.dataProvider = dataProvider;
            this.notificationSender = notificationSender;
        }

        public string GetNotificationByUserId(int userId)
        {
            var jsonResult = new StringBuilder();
            dataProvider.ExecuteCmd(
                "Notifications_GetByUserId",
                inputParamMapper: parameters =>
                {
                    parameters.AddWithValue("@UserId", userId);
                },
                singleRecordMapper: (reader, resultSetId) =>
                {
                    jsonResult.Append(reader.GetString(0));
                });
            return jsonResult.ToString();
        }

        public void Delete(int id, int userId)
        {
            dataProvider.ExecuteNonQuery(
                "Notifications_Delete",
                inputParamMapper: (parameters) =>
                {
                    parameters.AddWithValue("@Id", id);
                    parameters.AddWithValue("@UserId", userId);
                },
                returnParameters: null);

            string notifications = GetNotificationByUserId(userId);
            notificationSender.SendNotificationsToUser(userId, notifications);
        }

        public int Create(NotificationCreateRequest model)
        {
            int id = 0;
            dataProvider.ExecuteNonQuery(
                "Notifications_Create",
                inputParamMapper: (parameters) =>
                {
                    parameters.AddWithValue("@UserId", model.UserId);
                    parameters.AddWithValue("@NotificationJson", model.NotificationJson);
                    parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                },
                returnParameters: (parameters) =>
                {
                    id = (int)parameters["@Id"].Value;
                });

            string notifications = GetNotificationByUserId(Convert.ToInt32(model.UserId));
            notificationSender.SendNotificationsToUser(Convert.ToInt32(model.UserId), notifications);

            return id;
        }

        public bool NotificationExists(string type, int userId, int senderId)
        {
            List<Notification> notifications = new List<Notification>();

            dataProvider.ExecuteCmd(
                "Notifications_NotificationExists",
                inputParamMapper: parameters =>
                {
                    parameters.AddWithValue("@Type", type);
                    parameters.AddWithValue("@UserId", userId);
                    parameters.AddWithValue("@SenderId", senderId);
                },
                singleRecordMapper: (reader, resultSetId) =>
                {
                    Notification notification = new Notification();
                    notification.Id = (int)reader["Id"];
                    notification.UserId = (int)reader["UserId"];
                    notification.NotificationJson = (string)reader["NotificationJson"];

                    notifications.Add(notification);
                }
                );

            if (notifications.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
