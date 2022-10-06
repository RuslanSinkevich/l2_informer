using System.Collections.Generic;
using Informer.App_Start;
using Informer.Areas.hf.Class;
using Informer.Class;
using Informer.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Ninject;
using Ninject.Parameters;

namespace Informer
{
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            IKernel kernal = new StandardKernel();
            kernal.Bind<ILocal>().To<Ninjection>();
            kernal.Bind<IDb>().To<NinjectDb>();
            DependencyResolver.SetResolver(new MyDependetOnresolved(kernal));

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();


        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (User != null)
            {
                var db = new home();
                if (db.UserProfile.Where(p => p.UserName == User.Identity.Name).Any(x => x.baned == 1))
                {
                   FormsAuthentication.SignOut();
                   Response.Redirect("/Home/bann");
                }
            }
        }



         //protected void Application_Error(object sender, EventArgs e) // При ошибке перенаправляет на страницу ошибки
         //{
         //   HttpContext ctx = HttpContext.Current;
         //   Exception ex = ctx.Server.GetLastError();
         //   ctx.Response.Clear();
         //
         //   RequestContext rc = ((MvcHandler)ctx.CurrentHandler).RequestContext;
         //   IController controller = new HomeController();
         //   var context = new ControllerContext(rc, (ControllerBase)controller);
         //
         //   var viewResult = new ViewResult();
         //
         //   var httpException = ex as HttpException;
         //   if (httpException != null)
         //   {
         //       switch (httpException.GetHttpCode())
         //       {
         //           case 404:
         //               viewResult.ViewName = "Error404";
         //               break;
         //           
         //           case 500:
         //               viewResult.ViewName = "Error500";
         //               break;
         //
         //           default:
         //               viewResult.ViewName = "Error";
         //               break;
         //       }
         //   }
         //   else
         //   {
         //       viewResult.ViewName = "Error";
         //   }
         //
         //   viewResult.ViewData.Model = new HandleErrorInfo(ex, context.RouteData.GetRequiredString("controller"), context.RouteData.GetRequiredString("action"));
         //   viewResult.ExecuteResult(context);
         //   ctx.Server.ClearError();
         //}
    }

    public  class MyDependetOnresolved : IDependencyResolver
    {
        private readonly IKernel _ikernel;

        public MyDependetOnresolved(IKernel ikernel)
        {
            _ikernel = ikernel;
        }

        public object GetService(Type serviceType)
        {
            return _ikernel.TryGet(serviceType, new IParameter[0]);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _ikernel.GetAll(serviceType, new IParameter[0]);

        }
    }

}