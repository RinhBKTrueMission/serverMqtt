using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vst.Server;
using MVC = System.Mvc.Engine;

namespace ConsoleApp
{
    class Program : MasterServer
    {
        MqttServer _broker;
        protected override void MainThread(int interval)
        {
            _broker = new MqttServer();
            _broker.Start();

            base.MainThread(interval);
        }
        protected override void ProcessResponse(Mqtt.Models.PublishContext context)
        {
            Console.WriteLine("[{0:HH.mm.ss.ff}] {1}", DateTime.Now, context.Value);
            _broker.Publish(context);
        }
        protected override bool Wait()
        {
            Thread.Sleep(100);
            return true;
        }
        static void Main(string[] args)
        {
            new Program().Start(); // start server with 16MB RAM and 100 ms reading duration
        }
    }
}
