using System;
using System.Collections.Generic;
using System.Linq;
using System.Mvc;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace Mqtt.Client
{
    public class Controller : System.Mvc.Controller
    {
        static MqttClient _mqttClient;
        static Mqtt.Models.ClientStatus _status;

        public MqttClient Client => _mqttClient;
        public Mqtt.Models.ClientStatus ClientStatus
        {
            get
            {
                if (_status == null)
                {
                    _status = new Models.ClientStatus { 
                        //Host = "system.aks.vn",
                        Host = "localhost",
                    };
                }
                return _status;
            }
        }
        public Mqtt.Models.ResponseContext Response { get; set; }
        protected virtual void OnClientConnected() { }
        protected virtual void OnConnectError(Exception e) 
        {
            Console.WriteLine(e.Message);
        }
        protected void Connect(string clientId, int checkConnectionSeconds = 0)
        {
            if (_mqttClient != null && _mqttClient.IsConnected) { return; }

            Console.Write("Connect to " + ClientStatus.Host + "...");
            try
            {
                ClientStatus.SetConnectionState(false);
                _mqttClient = new MqttClient(
                    _status.Host,
                    _status.Port,
                    false,
                    MqttSslProtocols.None,
                    null,
                    null
                );
                _mqttClient.MqttMsgPublishReceived += (s, e) =>
                {
                    var context = e.Message.ToObject<Mqtt.Models.ResponseContext>();
                    var request = new RequestContext(e.Topic);

                    var c = Engine.GetController<Controller>(request.ControllerName);
                    if (c != null)
                    {
                        var action = c.GetMethod(request.ActionName);
                        if (action != null)
                        {
                            c.Response = context;
                            AsyncEngine.CreateThread(() => action.Invoke(c, new object[] { }));
                        }
                    }
                };
                _mqttClient.ConnectionClosed += (s, e) => ClientStatus.SetConnectionState(false);

                if (clientId == null)
                {
                    clientId = Guid.NewGuid().ToString();
                }
                _mqttClient.Connect(clientId);
                if (_mqttClient.IsConnected)
                {
                    OnClientConnected();

                    _status.SetConnectionState(true);
                    _mqttClient.Subscribe(new string[] { Mqtt.Models.ResponseContext.DefaultTopic + _mqttClient.ClientId }, new byte[] { 0 });
                }
            }
            catch (Exception e)
            {
                OnConnectError(e);
            }
            if (checkConnectionSeconds > 0)
            {
                int interval = checkConnectionSeconds * 1000;
                AsyncEngine.CreateThread(() => {
                    while (true)
                    {
                        System.Threading.Thread.Sleep(interval);
                        Connect(clientId);
                    }
                });
            }
        }
        protected void Disconnect()
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                _mqttClient.Disconnect();
            }
        }
        protected void Publish(string topic, string url, object value)
        {
            var context = new Mqtt.Models.ServerContext { 
                Url = url,
                Value = value,
            };
            _mqttClient.Publish(topic, context.ToBytes());
        }
        protected void Publish(Mqtt.Models.PublishContext context)
        {
            _mqttClient.Publish(
                context.Topic, 
                context.Value.ToBytes(), 
                context.QOS, 
                context.Retain
            );
        }
    }
}
