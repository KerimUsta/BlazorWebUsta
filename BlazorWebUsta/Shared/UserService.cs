using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebUsta.Shared
{
    public class UserService
    {
        private Dictionary<string, string> users = new Dictionary<string, string>
    {
        {"kullanici1", "sifre1"},
        {"kullanici2", "sifre2"}
    };

        public bool Authenticate(string username, string password)
        {
            if (users.ContainsKey(username) && users[username] == password)
            {
                return true;
            }
            return false;
        }
    }
}
