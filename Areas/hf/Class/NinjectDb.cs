using System.Web.Mvc;
using Informer.Areas.hf.Models;

namespace Informer.Areas.hf.Class
{
    public class NinjectDb : Controller, IDb
    {
        public high_five_4lUtkeEntities Db()
        {
            var db = new high_five_4lUtkeEntities();
            return db;
        }

        public string Dbstr()
        {
            return "high_five_4lUtkeEntities";
        }
    }

    public interface IDb
    {
        high_five_4lUtkeEntities Db();
        string Dbstr();
    }
}