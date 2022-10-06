using System.Data.Common;
using System.Data.Objects;
using Informer.Areas.hf.Class;
using Informer.Areas.hf.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Informer.Class;
using Informer.Filters;
using Ninject;
using Telerik.Web.Mvc;

namespace Informer.Areas.hf.Controllers
{
    [InitializeSimpleMembership]
    public class InventoryController : Controller
    {

        public InventoryController(ILocal lokal, IDb dataBase)
        {
            Lokal = lokal;
            DataBase = dataBase;
        }
        
        [Inject]
        private ILocal Lokal { get; set; }

        [Inject]
        private IDb DataBase { get; set; }

        //
        // GET: /Inventory/
        [OutputCache(Duration = 30, VaryByParam = "none")]
        public ActionResult Info()
        {
            return View();
        }

        [GridAction]
        public ActionResult _inventorySlot(string item_slot, string item_tip, int cry_tip)// Получаем итемы по слоту (веапон , армор)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (item_slot == "-100" && item_tip == "-100")
                return null;

            var northwind2 = new ObjectContext("name=" + DataBase.Dbstr())
                {
                    DefaultContainerName = DataBase.Dbstr()
                };

            string where;
            if (item_tip == "0")
            {
                if (item_slot == "18")
                {
                    where = "WHERE itemGrp.item_slot in {" + item_slot + "} and itemGrp.item_cry_tip = " + cry_tip + " and itemGrp.item_tag != 1 and itemGrp.item_tip = 23";
                }
                else
                {
                    where = "WHERE itemGrp.item_slot in {" + item_slot + "} and itemGrp.item_cry_tip = " + cry_tip + "";
                }
            }
            else if (item_tip == "50")
            {
                where = "WHERE itemGrp.item_id in {" + Resources.Views.Inventory.sqlBijaRb + "}";
            }
            else
                where = "WHERE itemGrp.item_tip in {" + item_tip + "} and itemGrp.item_cry_tip = " + cry_tip + "";


            string name, nameAdd;
            if (Lokal.Localization(Request.Cookies["Localization"]))
            {
                name = "item_name_ru";
                nameAdd = "item_add_name_ru";
            }
            else
            {
                name = "item_name_en";
                nameAdd = "item_add_name_en";
            }

            string esqlQuery =
                  " SELECT itemGrp.item_id, ItemName." + name + " as item_name, itemGrp.item_img, "
                + " itemGrp.item_cry_count, itemGrp.item_cry_tip, itemGrp.item_crafr, itemGrp.item_tradeable, "
                + " itemGrp.item_mdef, itemGrp.item_pdef, itemGrp.item_patt, itemGrp.item_matt,  itemGrp.item_price, ItemDesc." + nameAdd + " as item_add_name"
                + " FROM " + DataBase.Dbstr() + ".item_grp as itemGrp "
                + " LEFT JOIN " + DataBase.Dbstr() + ".item_name as ItemName ON itemGrp.item_id = ItemName.item_id "
                + " LEFT JOIN " + DataBase.Dbstr() + ".item_desc as ItemDesc ON itemGrp.item_id = ItemDesc.item_id "
                + where;

            var onlineOrders = new ObjectQuery<DbDataRecord>(esqlQuery, northwind2);
            return Json(onlineOrders.ToList().ConvertTo<InventoryInfoSql>().AsQueryable());
        }
    }
}
