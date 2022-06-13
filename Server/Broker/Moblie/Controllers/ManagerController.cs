using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moblie.Controllers
{
    class ManagerController: Vst.Server.SlaveController
    {

        public object ListDevice()
        {
           
          //  var uc = new ManagerController();
          //  uc.DeviceDb = new Vst.Server.Data.DeviceData(uc.MainDb.PhysicalPath);
          //var lstDevice= uc.DeviceDb.GetAll();
            return Response(null);
        }
        public object SettingDevice()
        {
           
            var data = this.ServerContext.Value.ToString();
            List<string> cs=data.Split('/').ToList<string>();
            var clientId = cs[0];
            var SetDivce = cs[1];
            var topic = "response/default/" + clientId;
            return Response(topic,SetDivce);
        }
        public object UpdateDevice()
        {
            var data=this.ServerContext.Value.ToString();
      
            return Response(data);
        }
    }
}
