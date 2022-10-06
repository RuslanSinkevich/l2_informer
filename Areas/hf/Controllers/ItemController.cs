using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Informer.Areas.hf.Class;
using Informer.Filters;
using Ninject;
using Telerik.Web.Mvc;
using System.Text.RegularExpressions;
using Informer.Areas.hf.Models;
using Informer.Class;

namespace Informer.Areas.hf.Controllers
{
    [InitializeSimpleMembership]
    public class ItemController : Controller
    {
        //
        // GET: /Item/
        private readonly ItemModel _itemModel;

        public ItemController(ILocal lokal, IDb dataBase, ItemModel itemModel)
        {
            Lokal = lokal;
            DataBase = dataBase;
            _itemModel = itemModel;
        }
        
        [Inject]
        private ILocal Lokal { get; set; }

        [Inject]
        private IDb DataBase { get; set; }

        [OutputCache(Duration = 30, VaryByParam = "none")]
        public ActionResult Info()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _searchByNameItem(string text) // При наборе в поле поиска item выводит имена
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return Json(_itemModel.GetItemNameStr(text).ToList());
        }

        [GridAction]
        public ActionResult _searchItemInform(string item_name, int item_tip) // Получаем список item в грид ItemInform
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return Json(_itemModel.GetItemInform(item_name, item_tip, Lokal.Localization(Request.Cookies["Localization"])));
        }

        [GridAction]
        public ActionResult _Select_ItemDrop(int item_id) // Получаем дроп  мобов
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (item_id == -100)
                return null;
            return Json(_itemModel.GetItemDrop(item_id, Lokal.Localization(Request.Cookies["Localization"]), Lokal.Reits(Request.Cookies["Reits"])).OrderBy(z => z.item_max), JsonRequestBehavior.AllowGet);
        }

        [GridAction]
        public ActionResult _Select_ItemSpoil(int item_id) // Получаем споил мобов
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (item_id == -100)
                return null;

            return Json(_itemModel.GetItemSpoil(item_id, Lokal.Localization(Request.Cookies["Localization"]), Lokal.Reits(Request.Cookies["Reits"])), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult details(string id)// Получаем детали конкретного item по id
        {
            int itemId;
            if (Int32.TryParse(id.Trim(), out itemId) == false)
            {
                itemId = -100;
            }
            ViewBag.ItemGrp = _itemModel.GetItemDetailsGrp(itemId);
            ViewBag.ItemName = _itemModel.GetItemDetailsName(itemId, Lokal.Localization(Request.Cookies["Localization"]));
            ViewBag.ItemDrop = _itemModel.GetItemDrop(itemId, Lokal.Localization(Request.Cookies["Localization"]), Lokal.Reits(Request.Cookies["Reits"])).OrderBy(z => z.item_max);
            ViewBag.ItemSpoil = _itemModel.GetItemSpoil(itemId, Lokal.Localization(Request.Cookies["Localization"]), Lokal.Reits(Request.Cookies["Reits"])).OrderBy(z => z.item_max);
            ViewBag.ItemQuest = _itemModel.GetItemQuest(itemId, Lokal.Localization(Request.Cookies["Localization"]));
            List<ItemCraft> rec;
            using (var northwind = DataBase.Db())
            {
                if (Lokal.Localization(Request.Cookies["Localization"]))
                    rec = (from itemRec in northwind.item_recipe
                          join itemName in northwind.item_name on itemRec.rec_id_recipe equals itemName.item_id
                          join itemGrp in northwind.item_grp on itemRec.rec_id_recipe equals itemGrp.item_id
                           where itemRec.rec_id_item == itemId
                           select new ItemCraft
                               {
                                   item_id = itemRec.rec_id_recipe, 
                                   item_name = itemName.item_name_ru, 
                                   item_img = itemGrp.item_img
                               }).ToList();
                else
                    rec = (from itemRec in northwind.item_recipe
                          join itemName in northwind.item_name on itemRec.rec_id_recipe equals itemName.item_id
                          join itemGrp in northwind.item_grp on itemRec.rec_id_recipe equals itemGrp.item_id
                           where itemRec.rec_id_item == itemId
                           select new ItemCraft
                               {
                                   item_id = itemRec.rec_id_recipe, 
                                   item_name = itemName.item_name_en, 
                                   item_img = itemGrp.item_img
                               }).ToList();
            }
            ViewBag.ItemCraft = rec;
            return View();

        }

        [HttpGet]
        public PartialViewResult _ItemDetailPartial(int item_id) // При клике выводит дроп и споил определённого item в detail
        {
            return PartialView("_ItemDetailPartial", item_id);
        }

        [HttpGet]
        public PartialViewResult _ItemLikePartial(string item_name) // При клике выводит похожие итемы по имени
        {
            return PartialView("_ItemLikePartial", Regex.Replace(item_name, "- PvP", "").Trim());
        }

        [HttpGet]
        public PartialViewResult itemTooltip(int item_id) // тултип 
        {
            ViewBag.ItemGrp = _itemModel.GetItemDetailsGrp(item_id);
            ViewBag.ItemName = _itemModel.GetItemDetailsName(item_id, Lokal.Localization(Request.Cookies["Localization"]));
            return PartialView("itemTooltip", item_id);
        }
    }
}
