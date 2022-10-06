using Informer.Areas.hf.Class;
using Informer.Areas.hf.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Informer.Class;
using Informer.Filters;
using Ninject;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Informer.Areas.hf.Controllers
{
    [InitializeSimpleMembership]
    public class SkilsController : Controller
    {
        public SkilsController(ILocal lokal, IDb dataBase)
        {
            Lokal = lokal;
            DataBase = dataBase;
        }
        
        [Inject]
        private ILocal Lokal { get; set; }

        [Inject]
        private IDb DataBase { get; set; }

        //
        // GET: /Skils/
        [OutputCache(Duration = 30, VaryByParam = "none")]
        public ActionResult Info()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _CharLoading(string char_id, bool char_bool) // Получаем по id skil список классов чара
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            IEnumerable res;
            var templ = char_id.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            var northwind = DataBase.Db();
            if (Lokal.Localization(Request.Cookies["Localization"]))
            {
                if (char_bool)
                    res = from clasList in northwind.class_list
                          join CharTempl in northwind.class_templates on clasList.class_id equals CharTempl.class_id
                          where templ.Contains(clasList.class_id)
                          select new TreeViewItemModel
                          {
                              Text = CharTempl.class_name_ru,
                              Value = SqlFunctions.StringConvert((double)clasList.class_id),
                              LoadOnDemand = SqlFunctions.StringConvert((double)clasList.class_id) != null,
                              ImageUrl = "/Images/class/" + SqlFunctions.StringConvert((double)clasList.class_id).Trim() + ".png",
                              Enabled = true
                          };
                else
                {
                    int id = Convert.ToInt32(char_id);
                    res = from clasList in northwind.class_list
                          join charTempl in northwind.class_templates on clasList.class_id equals charTempl.class_id
                          where clasList.class_parent_id == id
                          select new TreeViewItemModel
                          {
                              Text = charTempl.class_name_ru,
                              Value = SqlFunctions.StringConvert((double)charTempl.class_id),
                              LoadOnDemand = clasList.class_max_prof == 0,
                              ImageUrl = "/Images/class/" + SqlFunctions.StringConvert((double)clasList.class_id).Trim() + ".png",
                              Enabled = true
                          };
                }
            }
            else
                if (char_bool)
                    res = from clasList in northwind.class_list
                          join charTempl in northwind.class_templates on clasList.class_id equals charTempl.class_id
                          where templ.Contains(clasList.class_id)
                          select new TreeViewItemModel
                          {
                              Text = charTempl.class_name_en,
                              Value = SqlFunctions.StringConvert((double)clasList.class_id),
                              LoadOnDemand = SqlFunctions.StringConvert((double)clasList.class_id) != null,
                              ImageUrl = "/Images/class/" + SqlFunctions.StringConvert((double)clasList.class_id).Trim() + ".png",
                              Enabled = true
                          };
                else
                {
                    int id = Convert.ToInt32(char_id);
                    res = from clasList in northwind.class_list
                          join charTempl in northwind.class_templates on clasList.class_id equals charTempl.class_id
                          where clasList.class_parent_id == id
                          select new TreeViewItemModel
                          {
                              Text = charTempl.class_name_en,
                              Value = SqlFunctions.StringConvert((double)charTempl.class_id),
                              LoadOnDemand = clasList.class_max_prof == 0,
                              ImageUrl = "/Images/class/" + SqlFunctions.StringConvert((double)clasList.class_id).Trim() + ".png",
                              Enabled = true
                          };
                }
            return new JsonResult { Data = res };


        }

        [HttpPost]
        public ActionResult _SkilsMinLvl(int skil_class) // Получаем при выборе класса чара список его уровней повышения скилов
        {
            if (skil_class == -100)
                return null;
            var northwind2 = new ObjectContext("name=" + DataBase.Dbstr())
                {
                    DefaultContainerName = DataBase.Dbstr() 
                };
            var esqlQuery = "select Distinct(SkilTree.skil_min_level) as skil_min_level from " + DataBase.Dbstr() + ".skils_tree as SkilTree Where SkilTree.skil_class_id=" + skil_class;
            var onlineOrders = new ObjectQuery<DbDataRecord>(esqlQuery, northwind2);
            var res = onlineOrders.ToList().ConvertTo<skils_tree>().ToList();
            ViewBag.skilsTree = res;
            ViewBag.skilClass = skil_class;
            return PartialView("_SkilMinLvlPartial");
        }

        [GridAction]
        public ActionResult _GetScilsGrid(int skil_class, int skil_min_lvl) // выводит скилы класса в грид 
        {
            if (skil_class == -100)
                return null;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            var northwind = DataBase.Db();
            IQueryable<SkilTree> res;
            if (Lokal.Localization(Request.Cookies["Localization"]))
                res = from skilTree in northwind.skils_tree
                      join skilsDesc in northwind.skils_desc_ru on skilTree.skil_id equals skilsDesc.skil_id
                      join skilGrp in northwind.skill_grp on skilTree.skil_id equals skilGrp.skil_id
                      join skilOperN in northwind.skils_oper_name on skilGrp.skil_oper_type equals skilOperN.skil_oper_type
                      where skilTree.skil_class_id == skil_class && skilTree.skil_min_level == skil_min_lvl
                      && skilGrp.skil_lvl == skilTree.skil_lvl && skilTree.skil_lvl == skilsDesc.skil_lvl
                      select new SkilTree
                      {
                          skil_cost = skilTree.skil_cost,
                          skil_desc = skilsDesc.skil_desc,
                          skil_id = skilTree.skil_id,
                          skil_img = skilGrp.skil_img,
                          skil_item_id = skilTree.skil_item_id,
                          skil_lvl = skilTree.skil_lvl,
                          skil_name = skilGrp.skil_name_ru,
                          skil_oper_type = skilOperN.skil_oper_name_ru
                      };
            else
                res = from skilTree in northwind.skils_tree
                      join skilsDesc in northwind.skils_desc_ru on skilTree.skil_id equals skilsDesc.skil_id
                      join skilGrp in northwind.skill_grp on skilTree.skil_id equals skilGrp.skil_id
                      join skilOperN in northwind.skils_oper_name on skilGrp.skil_oper_type equals skilOperN.skil_oper_type
                      where skilTree.skil_class_id == skil_class && skilTree.skil_min_level == skil_min_lvl
                      && skilGrp.skil_lvl == skilTree.skil_lvl && skilTree.skil_lvl == skilsDesc.skil_lvl
                      select new SkilTree
                      {
                          skil_cost = skilTree.skil_cost,
                          skil_desc = skilsDesc.skil_desc,
                          skil_id = skilTree.skil_id,
                          skil_img = skilGrp.skil_img,
                          skil_item_id = skilTree.skil_item_id,
                          skil_lvl = skilTree.skil_lvl,
                          skil_name = skilGrp.skil_name_en,
                          skil_oper_type = skilOperN.skil_oper_name_ru
                      };
            return Json(res);
        }

        [HttpGet]
        public ActionResult details(int id)// Получаем детали скила по id
        {
            ViewBag.SkilId = id;

            var northwind = DataBase.Db();
            List<SkilDetail> res;
            if (Lokal.Localization(Request.Cookies["Localization"]))
                res = (from skilGrp in northwind.skill_grp
                      join skilsDesc in northwind.skils_desc_ru on skilGrp.skil_id equals skilsDesc.skil_id
                      join skilOperN in northwind.skils_oper_name on skilGrp.skil_oper_type equals skilOperN.skil_oper_type
                       where skilGrp.skil_id == id && skilGrp.skil_lvl == skilsDesc.skil_lvl && skilsDesc.skil_id == skilGrp.skil_id
                       select new SkilDetail
                      {
                          skil_desc = skilsDesc.skil_desc,
                          skil_desc_add1 = skilsDesc.skil_desc_add1,
                          skil_desc_add2 = skilsDesc.skil_desc_add2,
                          skil_id = skilGrp.skil_id,
                          skil_img = skilGrp.skil_img,
                          skil_lvl = skilGrp.skil_lvl,
                          skil_name = skilGrp.skil_name_ru,
                          skil_name_er = skilGrp.skil_name_en,
                          skil_oper_type = skilOperN.skil_oper_name_ru
                      }).ToList();
            else
                res = (from skilGrp in northwind.skill_grp
                      join skilsDesc in northwind.skils_desc_en on skilGrp.skil_id equals skilsDesc.skil_id
                      join skilOperN in northwind.skils_oper_name on skilGrp.skil_oper_type equals skilOperN.skil_oper_type
                       where skilGrp.skil_id == id && skilGrp.skil_lvl == skilsDesc.skil_lvl && skilsDesc.skil_id == skilGrp.skil_id
                       select new SkilDetail
                      {
                          skil_desc = skilsDesc.skil_desc,
                          skil_desc_add1 = skilsDesc.skil_desc_add1,
                          skil_desc_add2 = skilsDesc.skil_desc_add2,
                          skil_id = skilGrp.skil_id,
                          skil_img = skilGrp.skil_img,
                          skil_lvl = skilGrp.skil_lvl,
                          skil_name = skilGrp.skil_name_en,
                          skil_name_er = skilGrp.skil_name_ru,
                          skil_oper_type = skilOperN.skil_oper_name_en
                      }).ToList();

           ViewBag.SkilViev = res;

           return View(res);
        }
    }
}
