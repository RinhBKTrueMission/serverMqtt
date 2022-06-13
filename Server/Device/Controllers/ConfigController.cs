using Device.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                function = 0,
                location = "N001P001T001",
                status = 0,
                listData = new List<setData>()
            });    
            return Response(CID);
        }
    }
}
