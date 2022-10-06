using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Informer.Filters;
using Informer.Models;
using System.IO;
using Microsoft.Web.Helpers;

namespace Informer.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            using (new home())
            {

                if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
                {
                    return RedirectToAction("Manage", "Account");
                }
                ModelState.AddModelError("", @"Имя пользователя или пароль указаны неверно.");
                return View(model);
            }
            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (!ReCaptcha.Validate(privateKey: "6LfsNuISAAAAAIqCESDaS2sp-P8PoMk4xx9koWX- "))
            {
                ModelState.AddModelError("", @"Ошибка капчи");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                // Попытка зарегистрировать пользователя
                try
                {
                    // Добавление нового пользователя в базу данных
                    using (var db = new home())
                    {
                        if (db.UserProfile.Any(x => x.Email == model.Email)) // Проверка на совпадение почты
                        {
                            ModelState.AddModelError("", @"Имя адреса электронной почты уже существует. Введите другой адрес электронной почты.");
                            return View(model);
                        }

                    }
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new {model.Email });
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Manage", "Account");

                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Удалять связь учетной записи, только если текущий пользователь — ее владелец
            if (ownerAccount == User.Identity.Name)
            {
                // Транзакция используется, чтобы помешать пользователю удалить учетные данные последнего входа
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Пароль изменен."
                : message == ManageMessageId.SetPasswordSuccess ? "Пароль задан."
                : message == ManageMessageId.RemoveLoginSuccess ? "Внешняя учетная запись удалена."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            var db = new home();
            ViewBag.Users = db.UserProfile.Where(x => x.UserName == User.Identity.Name).AsEnumerable().FirstOrDefault();
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            var db = new home();
            ViewBag.Users = db.UserProfile.Where(x => x.UserName == User.Identity.Name).AsEnumerable().FirstOrDefault();
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // В ряде случаев при сбое ChangePassword породит исключение, а не вернет false.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    ModelState.AddModelError("", @"Неправильный текущий пароль или недопустимый новый пароль.");
                }
            }
            else
            {
                // У пользователя нет локального пароля, уберите все ошибки проверки, вызванные отсутствующим
                // полем OldPassword
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }


        public ActionResult Users(String id)
        {
            var db = new home();
            var userInfo = db.UserProfile.FirstOrDefault(x => x.UserName == id);
            var userLsCount = db.lsMessages.Count(x => x.from_id == userInfo.UserId && x.status == 0);
            ViewBag.userInfo = userInfo;
            ViewBag.userLsCount = userLsCount;
            return View();
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        /// <summary>
        /// Сохранение Icq Skype country city signs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult UploadInfoUser(string Icq, string Skype, string country, string city, string signs)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            if (hasLocalAccount)
            {
                try
                {
                    if (signs.Length < 500)
                    {
                        using (var db = new home())
                        {
                            var avtarAdd = db.UserProfile.FirstOrDefault(x => x.UserName == User.Identity.Name);
                            if (avtarAdd != null)
                            {
                                avtarAdd.Icq = HtmlScrubber.Clean(Icq, true, true);
                                avtarAdd.Skype = HtmlScrubber.Clean(Skype, true, true);
                                avtarAdd.country = HtmlScrubber.Clean(country, true, true);
                                avtarAdd.city = HtmlScrubber.Clean(city, true, true);
                                avtarAdd.signs = HtmlScrubber.Clean(signs, true, true);
                            }
                            db.SaveChanges();
                            return RedirectToAction("Manage", "Account");
                        }
                    }
                    TempData["Error"] = "Ошибка! не верно заполнены поля, либо подпись превышает 500 знаков";
                    return RedirectToAction("Manage", "Account");
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Ошибка! " + e.Message;
                    return RedirectToAction("Manage", "Account");
                }
            }
            TempData["Error"] = "Не известная Ошибка!";
            return RedirectToAction("Manage", "Account");
        }

        /// <summary>
        /// Загрузка аватара
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UploadAvatar(HttpPostedFileBase file)
        {
            if (file == null) // Если не выбран файл
            {
                TempData["Error"] = "Не выбрано фото аватара";
                return RedirectToAction("Manage", "Account");
            }
            try
            {
                if (file.ContentType == "image/gif" || file.ContentType == "image/jpeg" || file.ContentType == "image/png") // Определяет тип картинки
                {
                    if (file.ContentLength > 0 && file.ContentLength <= 30000) // Определяет размер картинки
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(file.InputStream, true, true);
                        if (image.Width <= 64 && image.Height <= 64) // Определяет ширину и высату картинки
                        {
                            var rng = new Random();
                            string ext = file.FileName.Substring(file.FileName.LastIndexOf('.')); // Раширение файла
                            string fileName = Path.GetFileName(file.FileName); // имя файла
                            int rand = rng.Next(999999); // 6-ти значиное рандомное число от дубликатов
                            if (fileName.Length >= 8) // Обрезает имя картинки до 8-ми значеной еси оно длинное
                                fileName = fileName.Substring(0, 8);
                            var path = Path.Combine(Server.MapPath("~/Images/avatar/"), fileName + "_" + rand + ext);
                            string img = fileName + "_" + rand + ext;
                            file.SaveAs(path);
                            using (var db2 = new home())
                            {
                                var avtarAdd = db2.UserProfile.FirstOrDefault(x => x.UserName == User.Identity.Name);
                                avtarAdd.Avatar = img;
                                db2.SaveChanges();
                            }
                            TempData["avatar"] = fileName + "_" + rand + ext;
                            return RedirectToAction("Manage", "Account");
                        }
                        TempData["Error"] = "Аватар имеет не правильный размер, допустимо (64x64)";
                        return RedirectToAction("Manage", "Account");
                    }
                    TempData["Error"] = "Аватар превышает максимальный размер, допустимо (30кб)";
                    return RedirectToAction("Manage", "Account");
                }
                TempData["Error"] = "Не правильный формат аватара, разрешены (gif jpeg png)";
                return RedirectToAction("Manage", "Account");
            }
            catch
            {
                TempData["Error"] = "Неизвестная ошибка!";
                return RedirectToAction("Manage", "Account");
            }
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // Если текущий пользователь вошел в систему, добавляется новая учетная запись
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            // Новый пользователь, запрашиваем желаемое имя участника
            string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider;
            string providerUserId;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Добавление нового пользователя в базу данных
                using (var db = new home())
                {
                    UserProfile user = db.UserProfile.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Проверка наличия пользователя в базе данных
                    if (user == null)
                    {
                        if (db.UserProfile.Any(x => x.Email == model.Email)) // Проверка на совпадение почты
                        {
                            ModelState.AddModelError("", @"Имя адреса электронной почты уже существует. Введите другой адрес электронной почты.");
                            return View(model);
                        }
                        db.UserProfile.Add(new UserProfile { Email = model.Email, UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);
                        return RedirectToAction("Manage", "Account");
                    }
                    ModelState.AddModelError("UserName", @"Имя пользователя уже существует. Введите другое имя пользователя.");
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            var externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }


        #region Вспомогательные методы
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }



        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Полный список кодов состояния см. по адресу http://go.microsoft.com/fwlink/?LinkID=177550
            //.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Имя пользователя уже существует. Введите другое имя пользователя.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Имя пользователя для данного адреса электронной почты уже существует. Введите другой адрес электронной почты.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Указан недопустимый пароль. Введите допустимое значение пароля.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Указан недопустимый адрес электронной почты. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Указан недопустимый ответ на вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Указан недопустимый вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Указано недопустимое имя пользователя. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.ProviderError:
                    return "Поставщик проверки подлинности вернул ошибку. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                case MembershipCreateStatus.UserRejected:
                    return "Запрос создания пользователя был отменен. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                default:
                    return "Произошла неизвестная ошибка. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";
            }
        }
        #endregion
    }
}
