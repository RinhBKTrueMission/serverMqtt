using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using BsonData;

namespace Vst.Server.Data
{
    public class DeviceData : MasterDB
    {
       
      
        const string dbname = "Devices";
        public DeviceData(string path) : base(dbname, path) { }
        public DeviceData() : base(dbname) { }
        Collection _devices;
        public Collection Devices
        {
            get
            {
                if (_devices == null)
                {
                    _devices = this.GetCollection("Device");
                }
                return _devices;
            }
        }
       public IEnumerable<object> GetAll()
        {
            return Devices.ToList<object>();
        }
        public bool CreateDevice(string deviceId,object data)
        {
            if (Devices.Contains(deviceId))
            {
                return false;
            }
            var dv = JObject.FromObject(data);
            _devices.Insert(deviceId, dv);
            return true;
        }
        public bool Update(string deviceId,object data)
        {
            var vm = deviceId.ToLower();
            var id = "CID(" + vm + ")";
            Devices.Update(id,data);
            
           
            return true;
        }
        public object FindById(string deviceId)
        {
            var vm = deviceId.ToLower();
            var id = "CID(" + vm + ")";
            var v=Devices.FindById(id);
            return v;
        }
    }
}
