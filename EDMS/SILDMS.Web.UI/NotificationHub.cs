using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using SILDMS.Web.UI.Models;
using System.Security.Claims;

namespace SILDMS.Web.UI
{
    public class NotificationHub : Hub
    {
        private readonly NotificationManager _notificationManager;

        public NotificationHub() :
            this(NotificationManager.Instance)
        {

        }

        public NotificationHub(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }
        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;

            if (name != null && name != "")
            {
               _notificationManager.AddConnection(name, this.Context.ConnectionId);

            }
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        { 
            string name = Context.User.Identity.Name;
        
            if (name != null && name != "")
            {              
               _notificationManager.RemoveConnection(name);               
            }    
            return base.OnDisconnected(stopCalled);
        }
    }
}