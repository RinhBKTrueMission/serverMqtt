using BsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Vst.Server.Data
{
    public class BuildingData:MasterDB
    {
        const string dbname = "Builds";
        public BuildingData(string path) : base(dbname, path) { }
        public BuildingData() : base(dbname) { }
        Collection _builds;
        public Collection Builds
        {
            get
            {
                if (_builds == null)
                {
                    _builds = this.GetCollection("Build");
                }
                return _builds;
            }
        }
        public IEnumerable<object> GetAll()
        {
            return Builds.ToList<object>();
        }
        public bool CreateBuild(string BuildId, object data)
        {
            if (Builds.Contains(BuildId))
            {
                return false;
            }
            var dv = JObject.FromObject(data);
            _builds.Insert(BuildId, dv);
            return true;
        }
        public bool Update(string buildId, object data)
        {
            var vm = buildId.ToLower();
            var id = vm;
            Builds.Update(id, data);
            return true;
        }
        public object FindById(string buildId)
        {
            var vm = buildId.ToLower();
            var id = vm;
            var v = Builds.FindById(id);
            return v;
        }
    }
}
