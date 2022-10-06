using System;
using System.Web;
using System.Web.Mvc;

namespace Informer.Class
{
    public class Ninjection : Controller, ILocal
    {
        public bool Localization(HttpCookie cookie)
        {
            if (cookie != null)
            {
                return cookie.Value == "";
            }
            return true;
        }

        public int Reits(HttpCookie cookie)
        {
            if (cookie != null)
            {
                return Convert.ToInt32(cookie.Value);
            }
            return 1;
        }
    }

    public interface  ILocal
    {
        bool Localization(HttpCookie cookie);
        int Reits(HttpCookie cookie);
    }
}