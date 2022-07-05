using Mqtt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Vst.Server
{
    public abstract class SlaveServer : ServerBase
    {
        ShareMemory _masterMemory;
        public ShareMemory MasterMemory
        {
            get
            {
                if (_masterMemory == null)
                {
                    _masterMemory = new ShareMemory(MasterName);
                }
                return _masterMemory;
            }
        }

        protected virtual string MasterName { get; set; } = "broker";

        protected override void MainThread(int interval)
        {
            this.ShareMemory.AsyncReading<ServerContext>(interval, lst => {
                foreach (var context in lst)
                {
                    var ts = new ThreadStart(() => {
                        object response = null;
                        var request = new System.Mvc.RequestContext(context.Url);
                        var controller = _controllerMap.CreateController(request.ControllerName) as SlaveController;
                       // var data = context.Value;
                        Console.WriteLine(Json.GetString(context));

                        if (controller != null)
                        {
                            try
                            {
                                var action = controller.GetMethod(request.ActionName);
                               
                                if (action != null)
                                {
                                    controller.RequestContext = request;
                                    controller.ServerContext = context;
                                    response = action.Invoke(controller, new object[] { });
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        if (response != null)
                        {
                            MasterMemory.Open().WriteObject(response);
                        }
                    });
                    new Thread(ts).Start();
                }
            });
        }
    }

    public abstract class SlaveController : System.Mvc.Controller
    {
        static BsonData.DataBase _db;
        public BsonData.DataBase MainDb
        {
            get
            {
                if (_db == null)
                {
                    _db = new BsonData.DataBase(System.IO.Directory.GetCurrentDirectory(), "MainDB");
                }
                return _db;
            }
        }
        static Data.DeviceData _devicedb;
        public Data.DeviceData DeviceDb
        {
            get
            {
                if (_devicedb == null)
                {
                    _devicedb = new Data.DeviceData();
                }
                return _devicedb;
            }
            set => _devicedb = value;
        }
        static Data.AccountData _accountDb;
        public Data.AccountData AccountDb
        {
            get
            {
                if (_accountDb == null)
                {
                    _accountDb = new Data.AccountData();
                }
                return _accountDb;
            }
            set => _accountDb = value;
        }
        // database cho node báo cháy
        static Data.NodeData _nodedb;
        public Data.NodeData NodeDb
        {
            get
            {
                if (_nodedb == null)
                {
                    _nodedb = new Data.NodeData();
                }
                return _nodedb;
            }
            set => _nodedb = value;
        }
        //database cho toa nha
        static Data.BuildingData _buildDb;
        public Data.BuildingData BuildDb
        {
            get
            {
                if (_buildDb == null)
                {
                    _buildDb = new Data.BuildingData();
                }
                return _buildDb;
            }
            set => _buildDb = value;
        }
        public ServerContext ServerContext { get; set; }
        protected PublishContext Response(object value)
        {
            return Response(null, value, 0, null, null);
        }
        protected PublishContext Response(int code, string message, object value)
        {
            return Response(null, value, code, message, null);
        }
        protected PublishContext Response(string topic, object value)
        {
            return Response(topic, value, 0, null, null);
        }
        protected virtual PublishContext Response(string topic, object value, int code, string message, string url)
        {
            if (url == null)
            {
                url = RequestContext.Combine('_');
            }
            var context = new ResponseContext { 
                Action = url,
                Code = code,
                Message = message,
                Value = value,
            };
            return new PublishContext { 
                Topic = topic ?? (ResponseContext.DefaultTopic + ServerContext.ClientId),
                Value = context.ToJson(),
            };
        }
        protected PublishContext Error(int code, string message)
        {
            return Response(null, null, code, message, null);
        }
        protected PublishContext Error(int code)
        {
            return Response(null, null, code, null, null);
        }
        protected PublishContext STATUS(int code)
        {
            return Response(null, null, code, null, null);
        }
    }
}
