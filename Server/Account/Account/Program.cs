using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account
{
    class Program : Vst.Server.SlaveServer
    {
        static void Main(string[] args)
        {
            Controllers.UserController.CreateAccountDb();
            new Program().Start();
        }
    }
}
