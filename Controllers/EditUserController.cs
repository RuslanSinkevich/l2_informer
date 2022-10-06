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
    public class EditUserController : Controller
    {
        #region Пользователи 
        [GridAction]
        [Authorize(Roles = "Admin")]
        public ActionResult _SelectUser()
        {
            var db = new home();
            return View(new GridModel(db.UserProfile));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = "Admin")]
        public ActionResult _SaveUser(int id)
        {
            var db = new home();
            var res = db.UserProfile.FirstOrDefault(p => p.UserId == id);
            TryUpdateModel(res);
            db.SaveChanges();
            return View(new GridModel(db.UserProfile));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = "Admin")]
        public ActionResult _InsertUser()
        {
            var db = new home();
            var up = new UserProfile();
            if (TryUpdateModel(up))
            {
                up.UserId = db.UserProfile.OrderByDescending(p => p.UserId).First().UserId + 1;
                db.UserProfile.Add(up);
                db.SaveChanges();
            }
            return View(new GridModel(db.UserProfile));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = "Admin")]
        public ActionResult _DeleteUser(int id)
        {

            var db = new home();
            var res = db.UserProfile.FirstOrDefault(p => p.UserId == id);
            if (res != null)
            {
                db.UserProfile.Remove(res);
            }
            var res2 = db.webpages_OAuthMembership.FirstOrDefault(p => p.UserId == id);
            if (res2 != null)
            {
                db.webpages_OAuthMembership.Remove(res2);
            }
            var res3 = db.webpages_Membership.FirstOrDefault(p => p.UserId == id);
            if (res3 != null)
            {
                db.webpages_Membership.Remove(res3);
            }
            db.SaveChanges();
            return View(new GridModel(db.UserProfile));
        }
        #endregion 

        #region Роли пользователей
        [GridAction]
        [Authorize(Roles = "Admin")]
        public ActionResult _SelectUserRole()
        {
            var db = new home();
            return View(new GridModel(db.webpages_UsersInRoles));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = "Admin")]
        public ActionResult _InsertUserRole()
        {
            var db = new home();
            var up = new webpages_UsersInRoles();
            if (TryUpdateModel(up))
            {
                db.webpages_UsersInRoles.Add(up);
                db.SaveChanges();
            }
            return View(new GridModel(db.webpages_UsersInRoles));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = "Admin")]
        public ActionResult _DeleteUserRole(int id)
        {

            var db = new home();
            var res = db.webpages_UsersInRoles.FirstOrDefault(p => p.UserId == id);
            db.webpages_UsersInRoles.Remove(res);
            db.SaveChanges();
            return View(new GridModel(db.webpages_UsersInRoles));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = "Admin")]
        public ActionResult _SaveUserRole(int id)
        {

            var db = new home();
            var res = db.webpages_UsersInRoles.FirstOrDefault(p => p.UserId == id);
            TryUpdateModel(res);
            db.SaveChanges();
            return View(new GridModel(db.webpages_UsersInRoles));
        }
        #endregion

        #region Новости 
        [GridAction]
        [Authorize]
        public ActionResult _SelectNewsUserId(int id)
        {
            var db = new home();
            return View(new GridModel(db.home_news.Where(x => x.autor_id == id)));
        }

        [GridAction]
        [Authorize(Roles = "Admin")]
        public ActionResult _SelectNews()
        {
            var db = new home();
            return View(new GridModel(db.home_news));
        }

        [Authorize]
        public ActionResult EditNews(int id)
        {
            var db = new home();
            var memberId = WebSecurity.GetUserId(User.Identity.Name);
            var newsEdit = db.home_news.FirstOrDefault(x => x.id == id);
            if (newsEdit != null)
            {
                if (newsEdit.adm_edit == 1 && memberId == newsEdit.autor_id && User.IsInRole("Admin") == false)
                    return View(newsEdit);
                if (User.IsInRole("Admin"))
                    return View(newsEdit);
                return RedirectToAction("AddNews", "Home");
            }
            return RedirectToAction("AddNews", "Home");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult _SaveNews(int id, string title, int autorId, int categoryId, string preview, string content, int admEdit = 0)
        {
            var db = new home();
            if (preview.Trim() == "" || content.Trim() == "" || title.Trim() == "")
            {
                TempData["Error"] = "Ошибка! краткое содержание или полная новость или названае статьи пустые";
                return RedirectToAction("EditNews", "EditUser", new { id });
            }
            if (title.Trim().Length < 6)
            {
                TempData["Error"]= "Ошибка! короткое название, минимум 6 знаков";
                return RedirectToAction("EditNews", "EditUser", new { id });
            }
            try
            {
            var cookie = Request.Cookies["Area"];
            var res = db.home_news.FirstOrDefault(p => p.id == id);
            var memberId = WebSecurity.GetUserId(User.Identity.Name);
            if (res != null)
            {
                if (res.adm_edit == 1 && memberId == res.autor_id && User.IsInRole("Admin") == false)
                {
                    res.title = HtmlScrubber.RemoveAllTags(title);
                    res.preview = HtmlScrubber.RemoveAllTags(preview);
                    res.content = ConvertTagHtml.ConvertHf(HtmlScrubber.Clean(content, false, true));
                    db.SaveChanges();
                }
                if (User.IsInRole("Admin"))
                {
                    res.title = HtmlScrubber.Clean(title, true, true);
                    res.autor_id = autorId;
                    res.adm_edit = admEdit;
                    res.category_id = categoryId;
                    res.preview = HtmlScrubber.RemoveAllTags(preview);
                    content = HtmlScrubber.Clean(content, false, true);
                    if (cookie == null)
                        res.content = ConvertTagHtml.ConvertHf(HtmlScrubber.Clean(content, false, true));
                    else if (cookie.Value == "hf")
                        res.content = ConvertTagHtml.ConvertHf(HtmlScrubber.Clean(content, false, true));

                        db.SaveChanges();
                }

            }
            }
            catch
            {
                TempData["Error"] = "Ошибка!";
            }
            return RedirectToAction("EditNews", "EditUser", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateNews(int id)
        {
            var db = new home();
            var res = db.home_news.FirstOrDefault(p => p.id == id);
            TryUpdateModel(res);
            db.SaveChanges();
            return View(new GridModel(db.home_news));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = "Admin")]
        public ActionResult _DeleteNews(int id)
        {
            var db = new home();
            var res = db.home_news.FirstOrDefault(p => p.id == id);
            db.home_news.Remove(res);
            db.SaveChanges();
            return View(new GridModel(db.home_news));
        }

        [Authorize]
        public ActionResult _AddModer(int newsId)
        {
            ModerEdit(newsId);
            return RedirectToAction("AddNews", "Home");
        }

        [Authorize]
        public ActionResult _ifModer(int newsId)
        {
            ModerEdit(newsId);
            return RedirectToAction("AddNews", "Home");
        }

        private void ModerEdit(int newsId)
        {
            var db = new home();
            var res = db.home_news.FirstOrDefault(p => p.id == newsId);
            var memberId = WebSecurity.GetUserId(User.Identity.Name);
            if (res != null && (res.adm_edit == 1 && memberId == res.autor_id && User.IsInRole("Admin") == false))
            {
                res.adm_edit = 2;
                db.SaveChanges();
            }
            if (User.IsInRole("Admin"))
            {
                if (res != null) res.adm_edit = 2;
                db.SaveChanges();
            }
        }
        #endregion

        #region Катогории новостей
        [GridAction]
        public ActionResult _SelectСatNews()
        {
            var db = new home();
            return View(new GridModel(db.category_news));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveСatNews(int id)
        {
            var db = new home();
            var res = db.category_news.FirstOrDefault(p => p.cat_id == id);
            TryUpdateModel(res);
            db.SaveChanges();
            return View(new GridModel(db.category_news));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertСatNews()
        {
            var db = new home();
            var up = new category_news();
            if (TryUpdateModel(up))
            {
                up.cat_id = db.category_news.OrderByDescending(p => p.cat_id).First().cat_id + 1;
                db.category_news.Add(up);
                db.SaveChanges();
            }
            return View(new GridModel(db.category_news));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteСatNews(int id)
        {

            var db = new home();
            var res = db.category_news.FirstOrDefault(p => p.cat_id == id);
            if (res != null)
            {
                db.category_news.Remove(res);
            }
            return View(new GridModel(db.category_news));
        }
        #endregion 
    }
}
