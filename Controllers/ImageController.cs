using Informer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Informer.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload, string ckEditorFuncNum, string ckEditor, string langCode)
        {
            var uploadFolder = "/Images/UserFiles/" + User.Identity.Name;
            var fileName = Path.GetFileName(upload.FileName);
            if (fileName != null)
            {
                var path = Path.Combine(Server.MapPath(string.Format("~/{0}", uploadFolder)), fileName);

                if (System.IO.File.Exists(path))
                    return Content("<span style='color: red'>Изображение с таким названием уже есть!</span>");

                var extension = new[] { "image/gif", "image/jpeg", "image/png" };
                if (extension.All(item => upload.ContentType != item))
                    return Content("<span style='color: red'>Данный тип файла запрещён к загрузке</span>");

                System.Drawing.Image image = System.Drawing.Image.FromStream(upload.InputStream, true, true);
                if (image.Width >= 450 || image.Height >= 600) // Определяет ширину и высату картинки
                    return Content("<span style='color: red'>Максимальная размер (450x600)</span>");

                if (upload.ContentLength <= 0 || upload.ContentLength > 5000000) // Определяет размер картинки
                    return Content("<span style='color: red'>Максимальный размер 5Mb</span>");

                if (!Directory.Exists(Server.MapPath(uploadFolder)))
                {
                    Directory.CreateDirectory(Server.MapPath(uploadFolder));
                }
                upload.SaveAs(path);
            }


            if (Request.Url != null)
            {
                var url = string.Format("{0}{1}/{2}/{3}", Request.Url.GetLeftPart(UriPartial.Authority),
                                        Request.ApplicationPath == "/" ? string.Empty : Request.ApplicationPath,
                                        uploadFolder, fileName);

                // passing message success/failure
                const string message = "Изображение сохранено!";

                // since it is an ajax request it requires this string
                var output = string.Format(
                    "<html><body><script>window.parent.CKEDITOR.tools.callFunction({0}, \"{1}\", \"{2}\");</script></body></html>",
                    ckEditorFuncNum, url, message);

                return Content(output);
            }
            return Content("Не известная ошибка!");
        }

        [HttpGet]
        public ActionResult Browser(string CKEditor, string CKEditorFuncNum, string langCode)
        {
            var uploadFolder = Server.MapPath("/Images/UserFiles/" + User.Identity.Name);
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);
            var fullfilesPath = Directory.GetFiles(uploadFolder);
            var infolist = new List<ImageInfoModel>();
            foreach (var item in fullfilesPath)
            {
                var info = new ImageInfoModel();
                var fi = new FileInfo(item);
                var image = System.Drawing.Image.FromFile(item, true);
                var path = Path.GetFileName(item);
                info.Name = path;
                info.CkEditor = CKEditor;
                info.CkEditorFuncNum = CKEditorFuncNum;
                info.PatchImg = "/Images/UserFiles/" + User.Identity.Name +"/"+  path;
                info.ImgLength = (int) fi.Length;
                info.Height = image.Height;
                info.Width = image.Width;
                infolist.Add(info);
            }
            return View(infolist);
        }

    }
}
