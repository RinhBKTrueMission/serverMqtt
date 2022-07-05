using Newtonsoft.Json;
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
            return Response("response/account/rinhtt",us);
        }
        public static void CreateAccountDb()
        {
            var uc = new UserController();
            uc.AccountDb = new Vst.Server.Data.AccountData(uc.MainDb.PhysicalPath);
            uc.AccountDb.CreateAccount("admin", "admin", new { Role = "Admin" });
        }
        public object CreateAccount()
        {
            var uc = new UserController();
            var payload= this.ServerContext.Value.ToString();
            var data = JsonConvert.DeserializeObject<LoginInfo>(payload);
            uc.AccountDb = new Vst.Server.Data.AccountData(uc.MainDb.PhysicalPath);
            if (data.UserName.Contains("manager")) 
            {
               if(uc.AccountDb.CreateAccount(data.UserName, data.Password, new { Role = "manager" }))
                {
                    return Response("sign_up","200");
                }
                else
                {
                    return Response("sign_up", 403);
                } 
            }
            if(data.UserName.Contains("admin"))
            {
                if (uc.AccountDb.CreateAccount(data.UserName, data.Password, new { Role = "admin" }))
                {
                    return Response("sign_up", "200");
                }
                else
                {
                    return Response("sign_up", 403);
                }
            }
            if (uc.AccountDb.CreateAccount(data.UserName, data.Password, new { Role = "user" }))
            {
                return Response("sign_up", "200");
            }
            else
            {
                return Response("sign_up", 403);
            }

        }
    }
}
