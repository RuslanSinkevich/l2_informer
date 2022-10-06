using System.Globalization;
using System.Web;
using Informer.Filters;
using Informer.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using Calabonga.Mvc.PagedListExt;

namespace Informer.Controllers
{
    [InitializeSimpleMembership]
    public class HomeController : Controller
    {
        public ActionResult Index(int? id)
        {
            var cookie = Request.Cookies["CatId"];
            if (cookie != null)
            {
                cookie.Value = "0";
                ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            else
            {
                var cookies = new HttpCookie("CatId") { Value = "0" };
                ControllerContext.HttpContext.Response.Cookies.Add(cookies);
            }
            var db = new home();
            var newsList = (from news in db.home_news
                            join user in db.UserProfile on news.autor_id equals user.UserId
                            join category in db.category_news on news.category_id equals category.cat_id
                            join role in db.webpages_UsersInRoles on news.autor_id equals role.UserId into v3
                            from role in v3.DefaultIfEmpty()
                            where news.adm_edit <= 2
                            orderby news.id descending
                            select new news_model
                            {
                                news_autor_id = news.autor_id,
                                news_content = news.content,
                                news_data = news.data,
                                news_id = news.id,
                                news_preview = news.preview,
                                news_rating = news.rating,
                                news_title = news.title,
                                user_id = user.UserId,
                                user_name = user.UserName,
                                user_avatar = user.Avatar,
                                category_name = category.cat_name,
                                category_id = category.cat_id,
                                adm_edit = news.adm_edit,
                                roles_id = (role == null ? 0 : role.RoleId)
                            });
            IQueryable<news_model> res = User.IsInRole("Admin") ? newsList : newsList.Where(x => x.adm_edit == 0);
            return View(res.ToPagedList(id ?? 1));
        }

        public ActionResult Category(int? id)
        {
            var cookie = Request.Cookies["CatId"];
            if (cookie != null)
            {
                cookie.Value = id.ToString();
                ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            else
            {
                var cookies = new HttpCookie("CatId") { Value = id.ToString() };
                ControllerContext.HttpContext.Response.Cookies.Add(cookies);
            }
            var db = new home();
            var newsList = (from news in db.home_news
                            join user in db.UserProfile on news.autor_id equals user.UserId
                            join category in db.category_news on news.category_id equals category.cat_id
                            join role in db.webpages_UsersInRoles on news.autor_id equals role.UserId into v3
                            from role in v3.DefaultIfEmpty()
                            where category.cat_id == id
                            orderby news.id descending
                            select new news_model
                            {
                                news_autor_id = news.autor_id,
                                news_content = news.content,
                                news_data = news.data,
                                news_id = news.id,
                                news_preview = news.preview,
                                news_rating = news.rating,
                                news_title = news.title,
                                user_id = user.UserId,
                                user_name = user.UserName,
                                user_avatar = user.Avatar,
                                category_name = category.cat_name,
                                category_id = category.cat_id,
                                adm_edit = news.adm_edit,
                                roles_id = (role == null ? 0 : role.RoleId)
                            });
            IQueryable<news_model> res = User.IsInRole("Admin") ? newsList : newsList.Where(x => x.adm_edit == 0);
            return View("Index", res.ToPagedList(id ?? 1));
        }

        public ActionResult News(int id)
        {
            var db = new home();
            var cookie = Request.Cookies["CatId"];
            if (cookie != null)
            {
                var homeNews = db.home_news.FirstOrDefault(x => x.id == id);
                if (homeNews != null)
                {
                    var categoryId = homeNews.category_id;
                    cookie.Value = categoryId.ToString(CultureInfo.InvariantCulture);
                }
                ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            else
            {
                var cookies = new HttpCookie("CatId") { Value = id.ToString(CultureInfo.InvariantCulture) };
                ControllerContext.HttpContext.Response.Cookies.Add(cookies);
            }

            var newsList = (from news in db.home_news
                            join user in db.UserProfile on news.autor_id equals user.UserId
                            join category in db.category_news on news.category_id equals category.cat_id
                            join role in db.webpages_UsersInRoles on news.autor_id equals role.UserId into v3
                            from role in v3.DefaultIfEmpty()
                            where news.id == id
                            orderby news.id descending
                            select new news_model
                            {
                                news_autor_id = news.autor_id,
                                news_content = news.content,
                                news_data = news.data,
                                news_id = news.id,
                                news_preview = news.preview,
                                news_rating = news.rating,
                                news_title = news.title,
                                user_id = user.UserId,
                                user_name = user.UserName,
                                user_avatar = user.Avatar,
                                category_name = category.cat_name,
                                category_id = category.cat_id,
                                adm_edit = news.adm_edit,
                                roles_id = (role == null ? 0 : role.RoleId)
                            }).ToList();
            if (User.IsInRole("Admin"))
                return View(newsList);
            if (newsList.Any(x => x.adm_edit == 0))
                return View(newsList);
            return Redirect(@"\");
        }

        [Authorize]
        public ActionResult AddNews()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNews(home_news newMod)
        {
            var cookie = Request.Cookies["Area"];
            try
            {
                var db = new home();
                if (newMod.title == null)
                {
                    ViewBag.StatusMessage = "Ошибка! отсутствует название статьи";
                    return View(newMod);
                }
                if (newMod.preview == null || newMod.content == null)
                {
                    ViewBag.StatusMessage = "Ошибка! краткое содержание или полная новость пустые";
                    return View(newMod);
                }
                if (db.home_news.Any(x => x.title == newMod.title))
                {
                    ViewBag.StatusMessage = "Ошибка! статья с таким именем уже существует";
                    return View(newMod);
                }
                if (newMod.category_id == 0)
                {
                    ViewBag.StatusMessage = "Ошибка! вы не указали категорию";
                    return View(newMod);
                }
                if (newMod.preview.Trim() == "" || newMod.content.Trim() == "")
                {
                    ViewBag.StatusMessage = "Ошибка! краткое содержание или полная новость пустые";
                    return View(newMod);
                }
                if (newMod.title.Trim().Length < 6)
                {
                    ViewBag.StatusMessage = "Ошибка! короткое название, минимум 6 знаков";
                    return View(newMod);
                }
                newMod.autor_id = db.UserProfile.Where(x => x.UserName == User.Identity.Name).Select(x => x.UserId).FirstOrDefault();
                newMod.id = db.home_news.Max(x => x.id) + 1;
                newMod.data = DateTime.Today;
                newMod.rating = 0;
                newMod.adm_edit = 1;
                if (cookie == null)
                    newMod.content = ConvertTagHtml.ConvertHf(HtmlScrubber.Clean(newMod.content, false, true));
                else if (cookie.Value == "hf")
                    newMod.content = ConvertTagHtml.ConvertHf(HtmlScrubber.Clean(newMod.content, false, true));
                newMod.preview = HtmlScrubber.RemoveAllTags(newMod.preview);
                if (ModelState.IsValid)
                {
                    db.home_news.Add(newMod);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                ViewBag.StatusMessage = "Ошибка!";
            }
            return View(newMod);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Admin4356()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Страница описания приложения.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Страница контактов.";

            return View();
        }

        public ActionResult Bann()
        {
            return View();
        }

    }
}
