using System;
using System.Linq;
using System.Web.Http;

namespace Salus.Rest
{
    public class ApiController : System.Web.Http.ApiController
    {
        [AcceptVerbs("GET")]
        public object Help()
        {
            return "This is test WEB API. Supported methods are ../api/MyWebAPI/Help, ../api/MyWebAPI/Square/{number}";
        }

        [AcceptVerbs("GET")]
        public object Account(string id)
        {
            PasswordSelector a = new PasswordSelector(id);
            PasswordEntry entry = PasswordEntryManager.Instance.Passwords.FirstOrDefault(a.Select);
            if (entry != null)
            {
                return new UnsecurePasswordEntry(entry);
            }
            return null;
        }
        
        class PasswordSelector
        {
            private readonly string id;

            public PasswordSelector(string _id)
            {
                id = _id;
            }

            public bool Select(PasswordEntry o)
            {
                if (string.IsNullOrEmpty(o.Website))
                    return false;

                Uri uri;
                try
                {
                    uri = new Uri(o.Website);
                }
                catch
                {
                    return false;
                }

                return uri.Host.Contains(id);
            }
        }
    }
}
