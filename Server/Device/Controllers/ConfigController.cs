using Device.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Device.Controllers
{
    class ConfigController : Vst.Server.SlaveController
    {
        public object Time()
        {
            return Response(string.Format("TIME({0:yyyy,M,d,H,m,s})", DateTime.Now));
        }
        public object NodeId()
        {
            var uc = new ConfigController();
            var CID = string.Format("CID({0:yyMMddHHmmss})", DateTime.Now);
            int i = CID.IndexOf('(');
            int j = CID.IndexOf(')');

            string id = CID.Substring(i + 1, j - i - 1);
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            uc.NodeDb.CreateNode(CID, new NodeModel
            {
                Id = id,
                function = 3,
                location = "N001P001T001",
                status = 0,
                listData = new List<setData>()
            });    
            return Response(CID);
        }
        public object getbuild()
        {
            var data = this.ServerContext.Value;
            var uc = new ConfigController();
            uc.BuildDb = new Vst.Server.Data.BuildingData(uc.MainDb.PhysicalPath);
           var value = uc.BuildDb.FindById(data.ToString());
            if (value == null)
            {
                return STATUS(401);
            }
            return Response(value);
        }
        public object Insertbuild()
        {
            var data = Json.Convert<Building>(this.ServerContext.Value);

            var uc = new ConfigController();
            uc.BuildDb = new Vst.Server.Data.BuildingData(uc.MainDb.PhysicalPath);
            var value = uc.BuildDb.FindById(data.ToString());
            if (value == null)
            {
                uc.BuildDb.CreateBuild(data.Id, data);
                return STATUS(200);

            }
            return STATUS(403);
        }
        public object Updatebuild()
        {
            var data = Json.Convert<Building>(this.ServerContext.Value);

            var uc = new ConfigController();
            uc.BuildDb = new Vst.Server.Data.BuildingData(uc.MainDb.PhysicalPath);
            var value = uc.BuildDb.FindById(data.Id);
            if (value != null)
            {
                uc.BuildDb.Update(data.Id, data);
                return STATUS(200);

            }
            return STATUS(403);
        }
        public object FindNode()
        {
            var Id = this.ServerContext.Value.ToString();
            var uc = new ManageController();
            uc.NodeDb = new Vst.Server.Data.NodeData(uc.MainDb.PhysicalPath);
            var data = uc.NodeDb.FindById(Id);
            var vm = Json.Convert<NodeModel>(data);
            return Response(vm);
        }
    }
}
