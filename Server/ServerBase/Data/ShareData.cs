using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.Server.Data
{
    public class MasterDB
    {
        BsonData.DataBase _db;
        ShareMemory _config;
        public BsonData.Collection GetCollection(string name)
        {
            return _db.GetCollection(name);
        }
        public BsonData.Collection GetCollection<T>()
        {
            return _db.GetCollection<T>();
        }
        public MasterDB(string name, string path)
        {
            var data = path.ToBytes();

            _config = new ShareMemory(name);
            _config.Create(data.Length);
            _config.Streaming(stream => {
                stream.Write(data, 0, data.Length);
            });

            _db = new BsonData.DataBase(path, name);
        }
        public MasterDB(string name)
        {
            _config = new ShareMemory(name);
            _config.Open()?.Streaming(stream => {
                string path = System.Text.Encoding.UTF8.GetString(stream.ToBytes());
                _db = new BsonData.DataBase(path, name);
            });
        }
    }
}
