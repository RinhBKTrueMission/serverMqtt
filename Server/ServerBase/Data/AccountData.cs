using BsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Vst.Server.Data
{
    public class AccountData : MasterDB
    {
        const string dbname = "Accounts";
        const string pw = "Password";
        public AccountData(string path) : base(dbname, path) { }
        public AccountData() : base(dbname) { }

        Collection _accounts;
        Collection _tokens;

        public Collection Accounts
        {
            get
            {
                if (_accounts == null)
                {
                    _accounts = this.GetCollection("Users");
                }
                return _accounts;
            }
        }
        public Collection Tokens
        {
            get
            {
                if (_tokens == null)
                {
                    _tokens = this.GetCollection("Tokens");
                }
                return _tokens;
            }
        }
        public bool CreateAccount(string userName, string password, object data)
        {
            if (Accounts.Contains(userName))
            {
                return false;
            }
            var acc = JObject.FromObject(data);
            acc.Remove(pw);

            var id = userName.ToLower();
            acc.Add(pw, (id + password).ToMD5());

            _accounts.Insert(id, acc);
            return true;
        }
        public bool TryLogin(string userName, string password, out object userInfo)
        {
            var id = userName.ToLower();
            var acc = Accounts.FindById(id);

            if (acc != null)
            {
                var pass = acc.Get(pw);
                if (password.Equals(pass))
                {
                    acc.Remove(pw);

                    var time = DateTime.Now;
                    string token = (id + time.Ticks).ToMD5();

                    var us = JObject.FromObject(new { Time = time, Name = id, Account = acc });
                    Tokens.Insert(token, us);

                    us.Add("Token", token);

                    userInfo = us;
                    return true;
                }
            }

            userInfo = null;
            return false;
        }
    }
}
