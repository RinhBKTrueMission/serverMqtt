using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Device.Models;
using Newtonsoft.Json;
namespace Device.Controllers
{
    class ManageController : Vst.Server.SlaveController
    {
        public object DeviceList()
        {
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var lstDevice = uc.NodeDb.GetAll();           
            return Response(lstDevice);

        }
        public object ControlDevice()
        {

            var data = this.ServerContext.Value.ToString();
            List<string> cs = data.Split('/').ToList<string>();
            var clientId = cs[0];
            var SetDivce = cs[1];
            var topic = "response/default/" + clientId;
            return Response(topic, SetDivce);
        }
        public object UpdateDevice()
        {
            var d = this.ServerContext.Value;
            var data = Json.Convert<DeviceModel>(d);
            var uc = new ManageController();
            uc.DeviceDb = new Vst.Server.Data.DeviceData(uc.MainDb.PhysicalPath);
            uc.DeviceDb.Update(data.Id, data);
            return Response(null);
        }
        public object Find()
        {
            var Id = this.ServerContext.Value.ToString();
            var uc = new ManageController();
            uc.DeviceDb = new Vst.Server.Data.DeviceData(uc.MainDb.PhysicalPath);
            var data = uc.DeviceDb.FindById(Id);
            var vm = Json.Convert<DeviceModel>(data);
            vm.Name = "Success";
            return Response(vm);
        }

        public object NodeSum()
        {
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var lstDevice = uc.NodeDb.GetAll();
            var sum = new nodeSum();
            
            foreach(NodeModel device in lstDevice)
            {
                var lst = device.listData.ToArray();
                switch (device.function)
                {
                    case 0:
                        sum.nhiet_doi += lst[lst.Length-1].value;
                        break;
                    case 1:
                        sum.gas += lst[lst.Length - 1].value;
                        break;
                    case 2:
                        sum.khoi += lst[lst.Length - 1].value;
                        break;
                    case 3:
                        sum.do_am += lst[lst.Length - 1].value;
                        break;

                }
            }
            
            return Response("Tong quat",Json.Convert<nodeSum>(sum));

        }
        public object NodeList()
        {
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var lstDevice = uc.NodeDb.GetAll();
            return Response(lstDevice);
        }
        public object Alert()
        {
            var node = this.ServerContext.Value;
            var data = Json.Convert<NodeModel>(node);
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            uc.NodeDb.Update(data.Id, data);

            return Response("alert",data);
        }

    }
}
