using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Linq;
using Informer.Areas.hf.Class;
using Informer.Areas.hf.Models;
using System;
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
    public class CraftController : Controller
    {
        public CraftController(ILocal lokal, IDb dataBase)
        {
            Lokal = lokal;
            DataBase = dataBase;
        }
        
        [Inject]
        private ILocal Lokal { get; set; }

        [Inject]
        private IDb DataBase { get; set; }

        //
        // GET: /Craft/
        [OutputCache(Duration = 30, VaryByParam = "none")]
        public ActionResult Info()
        {
            return View();
        }

        [GridAction]
        public ActionResult _SelectCraftRec(int craft_lvl, int cRaft_tip, int cRaft_chance) // Получаем список рецептов в grid Craft
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (craft_lvl == -100 && cRaft_tip == -100 && cRaft_chance == -100)
                return null;

            var northwind2 = new ObjectContext("name=" + DataBase.Dbstr())
                {
                    DefaultContainerName = DataBase.Dbstr()
                };

            string where;

            if (cRaft_chance == -100)
            {
                if (cRaft_tip == -100 || cRaft_tip == 100)
                    where = "WHERE itemRec.rec_level_recipe = " + craft_lvl + "";
                else
                    where = "WHERE itemRec.rec_level_recipe = " + craft_lvl + " and itemGrp.item_tag = " + cRaft_tip + "";
            }
            else
            {
                if (cRaft_tip == -100 || cRaft_tip == 100)
                    where = "WHERE itemRec.rec_success_rate = " + cRaft_chance + " and itemRec.rec_level_recipe = " + craft_lvl + "";
                else
                    where = "WHERE itemRec.rec_success_rate = " + cRaft_chance + " and itemRec.rec_level_recipe = " + craft_lvl + " and itemGrp.item_tag = " + cRaft_tip + "";
            }

            var name = Lokal.Localization(Request.Cookies["Localization"]) ? "item_name_ru" : "item_name_en";

            var esqlQuery =
                  " SELECT itemRec.rec_id_recipe as item_id, ItemName." + name + " as item_name, itemGrp2.item_img,  itemGrp.item_cry_tip, itemGrp.item_price "
                + " FROM " + DataBase.Dbstr() + ".item_recipe as itemRec "
                + " LEFT JOIN " + DataBase.Dbstr() + ".item_name as ItemName ON ItemName.item_id = itemRec.rec_id_recipe "
                + " LEFT JOIN " + DataBase.Dbstr() + ".item_grp as itemGrp ON itemGrp.item_id = itemRec.rec_id_item "
                + " LEFT JOIN " + DataBase.Dbstr() + ".item_grp as itemGrp2 ON itemGrp2.item_id = itemRec.rec_id_recipe "
                + where;

            var onlineOrders = new ObjectQuery<DbDataRecord>(esqlQuery, northwind2);
            return Json(onlineOrders.ToList().ConvertTo<CraftSql>().AsQueryable());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _CraftLoading(int rec_id) // Получаем по id рецепта список ингридиентов
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            var northwind = DataBase.Db();
            IEnumerable res;
            if (Lokal.Localization(Request.Cookies["Localization"]))
                res = from itemName in northwind.item_name
                      join itemGrp in northwind.item_grp on itemName.item_id equals itemGrp.item_id
                      join itemRecing in northwind.item_recipe_ingr on itemName.item_id equals itemRecing.rec_ingredient_id
                      join itemRec in northwind.item_recipe on itemName.item_id equals itemRec.rec_id_item into gd
                      from itemRec in gd.DefaultIfEmpty()
                      where itemRecing.rec_id_recipe == rec_id
                      select new TreeViewItemModel
                      {
                          Text = SqlFunctions.StringConvert((double)itemRecing.rec_ingredient_count) + " - " + itemName.item_name_ru,
                          Value = SqlFunctions.StringConvert((double)itemRec.rec_id_recipe),
                          LoadOnDemand = itemGrp.item_crafr > 0,
                          ImageUrl = "/Images/icon/" + itemGrp.item_img + ".jpg",
                          NavigateUrl = "/hf/Item/details/" + SqlFunctions.StringConvert((double)itemGrp.item_id).Trim(),
                          Enabled = true,
                      };
            else
                res = from itemName in northwind.item_name
                      join itemGrp in northwind.item_grp on itemName.item_id equals itemGrp.item_id
                      join itemRecing in northwind.item_recipe_ingr on itemName.item_id equals itemRecing.rec_ingredient_id
                      join itemRec in northwind.item_recipe on itemName.item_id equals itemRec.rec_id_item into gd
                      from itemRec in gd.DefaultIfEmpty()
                      where itemRecing.rec_id_recipe == rec_id
                      select new TreeViewItemModel
                      {
                          Text = SqlFunctions.StringConvert((double)itemRecing.rec_ingredient_count).Trim() + " - " + itemName.item_name_en,
                          Value = SqlFunctions.StringConvert((double)itemRec.rec_id_recipe).Trim(),
                          LoadOnDemand = itemGrp.item_crafr > 0,
                          ImageUrl = "/Images/icon/" + itemGrp.item_img + ".jpg",
                          NavigateUrl = "/hf/Item/details/" + SqlFunctions.StringConvert((double)itemGrp.item_id).Trim(),
                          Enabled = true
                      };
            return Json(res);
             
        }

        [HttpGet]
        public ActionResult details(string id)// Получаем детали крафта конкретного item по id
        {
            int rec_id;
            if (Int32.TryParse(id.Trim(), out rec_id) == false)
            {
                rec_id = -100;
            }

            ViewBag.CraftItemId = rec_id;
            ViewBag.ItemCaftName = GetCraftItemName(rec_id, Lokal.Localization(Request.Cookies["Localization"]));

            return View();
        }
       
        [HttpPost]
        public PartialViewResult _CraftItemName(int rec_id) // Получаем при выборе рецепта имя получаемого ингридиента и картинку по скрипту
        {
            ViewBag.CraftitemName = GetCraftItemName(rec_id, Lokal.Localization(Request.Cookies["Localization"]));
            return PartialView("_CraftItemNamePartial");
        }

        /// <summary>
        /// Получаем по id рецепта описания предмета полученого после крафта
        /// </summary>
        /// <param name="id"></param>
        /// <param name="localization"></param>
        /// <returns></returns>
        private List<ItemCraft> GetCraftItemName(int id, bool localization)
        {
            using (var northwind = DataBase.Db())
            {
                if (localization)
                    return (from itemRec in northwind.item_recipe
                            join itemGrp in northwind.item_grp on itemRec.rec_id_item equals itemGrp.item_id
                            join itemName in northwind.item_name on itemRec.rec_id_item equals itemName.item_id
                            where itemRec.rec_id_recipe == id
                            select new ItemCraft { item_id = itemName.item_id, item_img = itemGrp.item_img, item_name = itemName.item_name_ru }).ToList();
                return (from itemRec in northwind.item_recipe
                        join itemGrp in northwind.item_grp on itemRec.rec_id_item equals itemGrp.item_id
                        join itemName in northwind.item_name on itemRec.rec_id_item equals itemName.item_id
                        where itemRec.rec_id_recipe == id
                        select new ItemCraft { item_id = itemName.item_id, item_img = itemGrp.item_img, item_name = itemName.item_name_en }).ToList();
            }
        }
    }
}
