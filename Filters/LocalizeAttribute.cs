using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace Informer.Filters
{

    /// <summary>
    /// Определяет ресурсы какого языка подгружать
    /// </summary>
    public class LocalizeAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string culture = (filterContext.HttpContext.Session["culture"] != null)
                ? filterContext.HttpContext.Session["culture"].ToString()
                : "ru-ru";

            CultureInfo cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            base.OnActionExecuting(filterContext);
        }
    }
}