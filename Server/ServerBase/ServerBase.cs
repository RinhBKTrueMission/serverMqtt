using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Mvc;

namespace Vst.Server
{
    public abstract class ServerBase : Engine
    {
        public const string ConfigFileName = "config.json";
        public ShareMemory ShareMemory { get; set; }
        protected abstract void MainThread(int interval);
        protected virtual bool Wait() { return true; }
        protected virtual void Start()
        {
            Register(this, result => { });

            Console.Write("Starting " + _assemblyName.ToUpper() + " server ... ");

            try
            {
                Process proc;

                try
                {
                    proc = Json.Read<Process>(ConfigFileName);
                }
                catch
                {
                    proc = new Process();
                }

                ShareMemory = new ShareMemory(_assemblyName);
                ShareMemory.Create(proc.MemorySize << 20);

                MainThread(proc.MainThreadInterval);

                Console.WriteLine("Ready");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            while (Wait()) { }
        }
    }
}
