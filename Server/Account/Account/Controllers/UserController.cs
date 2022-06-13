using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Controllers
{
    class UserController : Vst.Server.SlaveController
    {
        class LoginInfo
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
        public object Login()
        {
            var i = ServerContext.ParseObject<LoginInfo>();
            object us;

            if (!AccountDb.TryLogin(i.UserName, i.Password, out us))
            {
                return Error(-1);
            }
            return Response(us);
        }
        public static void CreateAccountDb()
        {
            var uc = new UserController();
            uc.AccountDb = new Vst.Server.Data.AccountData(uc.MainDb.PhysicalPath);
            uc.AccountDb.CreateAccount("admin", "admin", new { Role = "Admin" });
        }
    }
}
