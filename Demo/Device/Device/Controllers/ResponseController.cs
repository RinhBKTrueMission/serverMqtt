using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Controllers
{
    class ResponseController : Mqtt.Client.Controller
    {
        public void Default()
        {
            var action = GetMethod(Response.Action);
            action?.Invoke(this, new object[] { });
        }

        public void config_deviceid()
        {
            string v = this.Response.Value.ToString();
            int i = v.IndexOf('(');
            int j = v.IndexOf(')');

            string id = v.Substring(i + 1, j - i - 1);
            HomeController.Device.Id = id;
            Disconnect();
            System.Threading.Thread.Sleep(1000);
            System.Mvc.AsyncEngine.CreateThread(() => Connect(id));
        }
        public void config_time()
        {

        }
        
    }
}
