using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using Models.Domain;
using Models.Requests;
using Models.Responses;
using Services;
using Services.Security;
using Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Web.Configuration;
using System.Web.Http;

namespace Web.Controllers
{
    public class NotificationsController : ApiController
    {
        readonly INotificationsService notificationsService;

        public NotificationsController(INotificationsService notificationsService)
        {
            this.notificationsService = notificationsService;
        }

        [Route("api/notifications/{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            int userId = User.Identity.GetId().Value;
            notificationsService.Delete(id, userId);

            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }

        [Route("api/notifications/"), HttpGet]
        public HttpResponseMessage NotificationExists(string type, int userId, int senderId)
        {
            bool notificationExists = notificationsService.NotificationExists(type, userId, senderId);

            ItemResponse<bool> itemResponse = new ItemResponse<bool>();
            itemResponse.Item = notificationExists;

            return Request.CreateResponse(HttpStatusCode.OK, itemResponse);
        }
    }
}
