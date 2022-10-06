using System;
using Informer.Filters;
using Informer.Models;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using WebMatrix.WebData;

namespace Informer.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class MsgController : Controller
    {
        /// <summary>
        /// Личные сообшения
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        public ActionResult _LsFrom()
        {
            var id = WebSecurity.GetUserId(User.Identity.Name);
            var db = new home();
            var fromLs = from msg in db.lsMessages
                         join users in db.UserProfile on msg.to_id equals users.UserId
                         where msg.from_id == id && msg.status >= 0
                         select new MsgLsFrom
                         {
                             Id = msg.msg_id,
                             UserName = users.UserName,
                             MsgData = msg.date,
                             Status = msg.status,
                             SubJ = msg.subj,
                             UserId = users.UserId,
                             Title = msg.title
                         };
            return Json(fromLs);
        }

        [GridAction]
        public ActionResult _LsTo()
        {
            var id = WebSecurity.GetUserId(User.Identity.Name);
            var db = new home();
            var fromLs = from msg in db.lsMessages
                         join users in db.UserProfile on msg.from_id equals users.UserId
                         join usersFrom in db.UserProfile on msg.to_id equals usersFrom.UserId
                         where msg.to_id == id
                         select new MsgLsFrom
                         {
                             Id = msg.msg_id,
                             UserName = users.UserName,
                             MsgData = msg.date,
                             Status = msg.status,
                             SubJ = msg.subj,
                             UserId = users.UserId,
                             Title = msg.title
                         };
            return Json(fromLs);
        }

        [GridAction]
        public ActionResult _LsDell()
        {
            var id = WebSecurity.GetUserId(User.Identity.Name);
            var db = new home();
            var fromLs = from msg in db.lsMessages
                         join users in db.UserProfile on msg.to_id equals users.UserId
                         where msg.from_id == id && msg.status == -1
                         select new MsgLsFrom
                         {
                             Id = msg.msg_id,
                             UserName = users.UserName,
                             MsgData = msg.date,
                             Status = msg.status,
                             SubJ = msg.subj,
                             UserId = users.UserId,
                             Title = msg.title
                         };
            return Json(fromLs);
        }

        [GridAction]
        public ActionResult _LsIgnorUser()
        {
            var id = WebSecurity.GetUserId(User.Identity.Name);
            var db = new home();
            var fromLs = from userUgnor in db.user_ignore
                         join users in db.UserProfile on userUgnor.userIdIgnor equals users.UserId
                         where userUgnor.userId == id
                         select new UserIgnore
                             {
                                 UserName = users.UserName,
                                 UserId = users.UserId
                             };
            return Json(fromLs);
        }

        public PartialViewResult _ReadLsFrom(int id)
        {
            var userid = WebSecurity.GetUserId(User.Identity.Name);
            var db = new home();
            var fromLs = from lsMsg in db.lsMessages
                         join users in db.UserProfile on lsMsg.to_id equals users.UserId
                         where lsMsg.msg_id == id && lsMsg.from_id == userid
                         select new MsgLsFrom
                         {
                             UserName = users.UserName,
                             UserId = users.UserId,
                             Id = lsMsg.msg_id,
                             MsgData = lsMsg.date,
                             Status = lsMsg.status,
                             SubJ = lsMsg.subj,
                             Title = lsMsg.title
                         };
            var firstOrDefault = db.lsMessages.FirstOrDefault(x => x.msg_id == id && x.from_id == userid);
            if (firstOrDefault != null)
                firstOrDefault.status = 1;
            db.SaveChanges();
            return PartialView("_lsReadPartial", fromLs.FirstOrDefault());
        }

        public PartialViewResult _ReadLsTo(int id)
        {
            var userid = WebSecurity.GetUserId(User.Identity.Name);
            var db = new home();
            var fromLs = from lsMsg in db.lsMessages
                         join users in db.UserProfile on lsMsg.to_id equals users.UserId
                         where lsMsg.msg_id == id && lsMsg.to_id == userid
                         select new MsgLsFrom
                         {
                             UserName = users.UserName,
                             UserId = users.UserId,
                             Id = lsMsg.msg_id,
                             MsgData = lsMsg.date,
                             Status = lsMsg.status,
                             SubJ = lsMsg.subj,
                             Title = lsMsg.title
                         };
            return PartialView("_lsReadPartial", fromLs.FirstOrDefault());
        }


        public PartialViewResult _ReadLsDell(int id)
        {
            var userid = WebSecurity.GetUserId(User.Identity.Name);
            var db = new home();
            var fromLs = from lsMsg in db.lsMessages
                         join users in db.UserProfile on lsMsg.to_id equals users.UserId
                         where lsMsg.msg_id == id && lsMsg.from_id == userid
                         select new MsgLsFrom
                         {
                             UserName = users.UserName,
                             UserId = users.UserId,
                             Id = lsMsg.msg_id,
                             MsgData = lsMsg.date,
                             Status = lsMsg.status,
                             SubJ = lsMsg.subj,
                             Title = lsMsg.title
                         };
            return PartialView("_lsReadPartial", fromLs.FirstOrDefault());
        }

        public PartialViewResult PartialTo()
        {
            return PartialView("_lsToPartial");
        }

        public PartialViewResult PartialDell()
        {
            return PartialView("_lsDellPartial");
        }

        public PartialViewResult PartialUserIgnor()
        {
            return PartialView("_LsUserIgnorPartial");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult _AddLs(string title, string subj, string toName)
        {
            if(string.IsNullOrEmpty(title.Trim()))
                return Content(@"Пустое название!");
            if (string.IsNullOrEmpty(subj.Trim()))
                return Content(@"Пустое содержание!");
            var db = new home();
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            var userIdTo = WebSecurity.GetUserId(toName);
            if (db.user_ignore.Any(x => x.userId == userIdTo && x.userIdIgnor == userId))
                return Content(@"Пользователь вас игнорирует!"); 
            if (db.UserProfile.Any(x => x.UserId == userIdTo))
            {
                var ls = new lsMessages
                    {
                        title = HtmlScrubber.Clean(title,false, false),
                        subj = HtmlScrubber.Clean(subj, false, false),
                        from_id = userIdTo,
                        to_id = userId,
                        status = 0,
                        date = DateTime.Today
                    };
                db.lsMessages.Add(ls);
                db.SaveChanges();
                return Content(@"<script type=""text/javascript"">WindowClose()</script>");
            }
            return Content(@"Пользователь не найден!");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult _EditLs(int idLs, string subjEdit)
        {
            var db = new home();
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            var firstOrDefault = db.lsMessages.FirstOrDefault(x => x.msg_id == idLs && (x.from_id == userId || x.to_id == userId));
            if (firstOrDefault != null)
            {
                var userIdTo = firstOrDefault.from_id == userId ? firstOrDefault.to_id : firstOrDefault.from_id;
                if (db.user_ignore.Any(x => x.userId == userIdTo && x.userIdIgnor == userId))
                    return Content(@"Пользователь вас игнорирует!");
                var ls = db.lsMessages.FirstOrDefault(x => x.msg_id == idLs);
                if (ls != null)
                {
                    var start = "<blockquote><span class='quoteUser'>" + User.Identity.Name + "</span> <span class='quoteData'>(" + DateTime.Today.ToShortDateString() + ")</span> <hr>";
                    const string end = "</blockquote>";
                    int from ;
                    int to ;
                    if (firstOrDefault.from_id == userId)
                    {
                         from = ls.from_id;
                         to = ls.to_id;
                    }
                    else
                    {
                        from = ls.to_id;
                        to = ls.from_id;
                    }
                    ls.subj = HtmlScrubber.Clean( start + subjEdit + end + ls.subj, false, false);
                    ls.status = 0;
                    ls.from_id = to;
                    ls.to_id = from;
                }
                db.SaveChanges();
                return Content(@"<script type=""text/javascript"">WindowCloseEdit()</script>");
            }

            return Content(@"Сообщение не найдено!");
        }

        public void _ReadLs(int[] id)
        {
            var db = new home();
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            var toLs = (from msg in db.lsMessages
                        where (id.Contains(msg.msg_id))
                        select msg).ToArray();
            foreach (var item in toLs)
            {
                if (item.from_id == userId)
                    item.status = 1;
                else
                    break;
            }
            db.SaveChanges();
        }

        public void _DelLs(int[] id)
        {
            var db = new home();
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            var toLs = (from msg in db.lsMessages
                        where (id.Contains(msg.msg_id))
                        select msg).ToArray();
            foreach (var item in toLs)
            {
                if (item.from_id == userId)
                    item.status = -1;
                else
                    break;
            }
            db.SaveChanges();
        }

        public void _DellIgnor(string id)
        {
            var db = new home();
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            var userIgnorId = WebSecurity.GetUserId(id);
            var res = db.user_ignore.FirstOrDefault(p => p.userId == userId && p.userIdIgnor == userIgnorId);
            db.user_ignore.Remove(res);
            db.SaveChanges();
        }

        public void _AddIgnor(int[] id)
        {
            var db = new home();
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            foreach (var i in id)
            {
                var firstOrDefault = db.lsMessages.FirstOrDefault(x => x.msg_id == i);
                if (firstOrDefault != null)
                {
                    if (db.user_ignore.Any(x => x.userId == userId && x.userIdIgnor == firstOrDefault.to_id))
                        continue;
                    var userIgnorId = firstOrDefault.to_id;
                    var res = new user_ignore { userId = userId, userIdIgnor = userIgnorId };
                    db.user_ignore.Add(res);
                    db.SaveChanges();

                }
            }
        }

    }
}
