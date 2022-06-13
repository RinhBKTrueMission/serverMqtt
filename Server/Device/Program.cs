using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    class Program : Vst.Server.SlaveServer
    {
        static void Main(string[] args)
        {
            new Program().Start();
        }
    }
}
