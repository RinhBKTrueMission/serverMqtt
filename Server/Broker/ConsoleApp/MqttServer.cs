using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;
using Newtonsoft.Json.Linq;

namespace ConsoleApp
{
    class MqttServer // : MqttBroker
    {
        IMqttServer _broker;
        bool _publishing;

        bool ProcessMassage(ref MqttApplicationMessageInterceptorContext context)
        {
            if (_publishing)
            {
                _publishing = false;
                return true;
            }

            string topic = context.ApplicationMessage.Topic;
            if (topic == null) { return false; }
            if (topic.Contains('/')) { return true; }

            Console.WriteLine("[{0:HH.mm.ss.ff}] {1}", DateTime.Now, topic);
            var sm = new Vst.Server.ShareMemory(topic).Open();
            if (sm != null)
            {
                var payload = context.ApplicationMessage.Payload;
                Console.WriteLine(Encoding.ASCII.GetString(payload));
                try
                {
                    var clientId = Encoding.ASCII.GetBytes(",\"ClientId\":\"" + context.ClientId + "\"}");

                    var message = new byte[payload.Length + clientId.Length - 1];
                    payload.CopyTo(message, 0);
                    clientId.CopyTo(message, payload.Length - 1);
                    sm.WriteBytes(message);
                }
                catch
                {
                    Console.WriteLine("ERROR: " + Encoding.ASCII.GetString(payload));
                }
                return false;
            }
            return true;
        }
        public async void Start()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                 .WithConnectionBacklog(100)
                 .WithDefaultEndpointPort(1883)
                 .WithApplicationMessageInterceptor(context => {
                     context.AcceptPublish = ProcessMassage(ref context);
                 })
                 .Build();

            _broker = new MqttFactory().CreateMqttServer();

            _broker.ClientConnected += _broker_ClientConnected;
            _broker.ClientDisconnected += _broker_ClientDisconnected;

            await _broker.StartAsync(optionsBuilder);

        }

        private void _broker_ClientDisconnected(object sender, MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine("<<<<<<<<<<<<<<<<<<< " + (e.ClientId?.ToString()));
        }

        private void _broker_ClientConnected(object sender, MqttClientConnectedEventArgs e)
        {
            Console.WriteLine(">>>>>>>>>>>>>>>>> " + e.ClientId);
        }

        public MqttServer()
        {
        }
        public async void Publish(Mqtt.Models.PublishContext context)
        {
            if (context.Value != null)
            {
                _publishing = true;
                var am = new MqttApplicationMessage
                {
                    Topic = context.Topic,
                    Payload = context.Value.ToBytes(),
                    QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)context.QOS,
                    Retain = context.Retain,
                };
                await _broker.PublishAsync(am);
            }
        }
    }
}
