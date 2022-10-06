using Informer.Areas.hf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Informer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public ActionResult add_map()
        {
            var document = new XmlDocument();
            document.Load(Server.MapPath("../sitemap.xml"));
            int i = 0;
            XmlNode element = document.DocumentElement;
            document.RemoveChild(element);
            XmlNode urlset = document.CreateElement("urlset");
            document.AppendChild(urlset); // указываем родителя
            foreach (var item_id in FH_Item_map())
            {
                XmlNode url = document.CreateElement("url"); // даём имя
                XmlElement loc = document.CreateElement("loc");
                urlset.AppendChild(url); // и указываем кому принадлежит
                loc.InnerText = "http://l2mega.net/hf/Item/details/" + item_id; // и значение
                url.AppendChild(loc); // и указываем кому принадлежит
                i++;
            }
            foreach (var npc_id in FH_Npc_map())
            {
                XmlNode url = document.CreateElement("url"); // даём имя
                XmlElement loc = document.CreateElement("loc");
                urlset.AppendChild(url); // и указываем кому принадлежит
                loc.InnerText = "http://l2mega.net/hf/Npc/details/" + npc_id; // и значение
                url.AppendChild(loc); // и указываем кому принадлежит
                i++;
            }
            foreach (var Quest_id in FH_Quest_map())
            {
                XmlNode url = document.CreateElement("url"); // даём имя
                XmlElement loc = document.CreateElement("loc");
                urlset.AppendChild(url); // и указываем кому принадлежит
                loc.InnerText = "http://l2mega.net/hf/Quest/details/" + Quest_id; // и значение
                url.AppendChild(loc); // и указываем кому принадлежит
                i++;
            }
            foreach (var Skils_id in FH_Skils_map())
            {
                XmlNode url = document.CreateElement("url"); // даём имя
                XmlElement loc = document.CreateElement("loc");
                urlset.AppendChild(url); // и указываем кому принадлежит
                loc.InnerText = "http://l2mega.net/hf/Skils/details/" + Skils_id; // и значение
                url.AppendChild(loc); // и указываем кому принадлежит
                i++;
            }
            XmlAttribute attribute = document.CreateAttribute("xmlns"); // создаём атрибут
            attribute.Value = "http://www.sitemaps.org/schemas/sitemap/0.9"; // устанавливаем значение атрибута
            urlset.Attributes.Append(attribute); // добавляем атрибут
            XmlNode count = document.CreateElement("loc"); // даём имя
            count.InnerText = "href='количество'" + i; // и значение
            document.DocumentElement.AppendChild(count); // и указываем кому принадлежит
            document.Save(Server.MapPath("../sitemap.xml"));
            return RedirectToAction("Admin4356", "Home");
        }

        public List<int> FH_Item_map() // Возрашяет все id итемов
        {
            var northwind = new high_five_4lUtkeEntities();
            return (from itemId in northwind.item_grp select itemId.item_id).ToList();
        }

        public List<int> FH_Npc_map() // Возрашяет все id итемов
        {
            var northwind = new high_five_4lUtkeEntities();
            return (from NpcId in northwind.npc_grp select NpcId.npc_id).ToList();
        }


        public List<int> FH_Quest_map() // Возрашяет все id итемов
        {
            var northwind = new high_five_4lUtkeEntities();
            return (from QuestId in northwind.quest_desc select QuestId.quest_id).ToList();
        }

        public List<int> FH_Skils_map() // Возрашяет все id итемов
        {
            var northwind = new high_five_4lUtkeEntities();
            return (from SkilsId in northwind.skill_grp where SkilsId.skil_lvl == 1 select SkilsId.skil_id).ToList();
        }
    }
}
