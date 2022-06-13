using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using BsonData;

namespace Vst.Server.Data
{
    public class NodeData:MasterDB
    {
        const string dbname = "Nodes";
        public NodeData(string path) : base(dbname, path) { }
        public NodeData() : base(dbname) { }
        Collection _nodes;
        public Collection Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = this.GetCollection("Node");
                }
                return _nodes;
            }
        }
        public IEnumerable<object> GetAll()
        {
            return Nodes.ToList<object>();
        }
        public bool CreateNode(string nodeId, object data)
        {
            if (Nodes.Contains(nodeId))
            {
                return false;
            }
            var dv = JObject.FromObject(data);
            _nodes.Insert(nodeId, dv);
            return true;
        }
        public bool Update(string nodeId, object data)
        {
            var vm = nodeId.ToLower();
            var id = "CID(" + vm + ")";
            Nodes.Update(id, data);


            return true;
        }
        public object FindById(string nodeId)
        {
            var vm = nodeId.ToLower();
            var id = "CID(" + vm + ")";
            var v = Nodes.FindById(id);
            return v;
        }
    
    }
}
