using System;
using System.Web;
using System.Web.Mvc;
using Informer.Areas.hf.Class;
using Informer.Class;
using Informer.Filters;
using Ninject;
using Telerik.Web.Mvc;
using Informer.Areas.hf.Models;

namespace Informer.Areas.hf.Controllers
{
    [InitializeSimpleMembership]
    public class NpcController : Controller
    {
        private readonly NpcModel _npcModel;

        public NpcController(ILocal lokal, NpcModel npcModel)
        {
            Lokal = lokal;
            _npcModel = npcModel;
        }

        [Inject]
        private ILocal Lokal { get; set; }

        // GET: /NpcInfo/
        [OutputCache(Duration = 30, VaryByParam = "none")]
        public ActionResult Info()
        {
            return View();
        }

        [HttpGet]
        public ActionResult details(string id)// Получаем детали конкретного npc по id
        {
            int npcId;
            if (Int32.TryParse(id.Trim(), out npcId) == false)
            {
                npcId = -100;
            }
            var npcLocalization = Lokal.Localization(Request.Cookies["Localization"]);
            var npcReits = Convert.ToInt32(Lokal.Reits(Request.Cookies["Reits"]));
            ViewBag.details = _npcModel.GetNpcDetails(npcId, npcLocalization);
            ViewBag.detailSkil = _npcModel.GetNpcDetailsSkils(npcId, npcLocalization);
            ViewBag.descRb = _npcModel.GetNpcRbDesc(npcId, npcLocalization);
            ViewBag.minion = _npcModel.GetNpcMinion(npcId, npcLocalization);
            ViewBag.Npcdrop = _npcModel.GetNpcdrop(npcId, npcLocalization, npcReits);
            ViewBag.NpcSpoil = _npcModel.GetNpcSpoil(npcId, npcLocalization, npcReits);
            ViewBag.NpcQuest = _npcModel.GetNpcQuest(npcId, npcLocalization);
            return View();
        }

        [GridAction]
        public ActionResult _Select_Npcdrop(int npc_id) // Получаем дроп и спомл мобов
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return Json(_npcModel.GetNpcdrop(npc_id, Lokal.Localization(Request.Cookies["Localization"]), Convert.ToInt32(Lokal.Reits(Request.Cookies["Reits"]))));
        }

        [GridAction]
        public ActionResult _Select_NpcSpoil(int npc_id) // Получаем дроп и спомл мобов
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return Json(_npcModel.GetNpcSpoil(npc_id, Lokal.Localization(Request.Cookies["Localization"]), Convert.ToInt32(Lokal.Reits(Request.Cookies["Reits"]))));
        }

        [HttpPost]
        public ActionResult _searchByNameNpc(string text) // При наборе в поле поиска мобов выводит имена
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return Json(_npcModel.GetNpcNameStr(text.Trim()));
        }

        [HttpPost]
        public string SpawnNpc(int id) // Вывод спавна npc
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return _npcModel.Spawn(id);
        }

        [GridAction]
        public ActionResult _searchNpcInform(string npc_name, string tip_npc, int npc_vulnerability, int slider_min, int slider_max, string npc_raite) // Получаем список мобов в грид NpcInform
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return Json(_npcModel.GetNpcInform(npc_name, tip_npc, npc_vulnerability, slider_min, slider_max, npc_raite, Lokal.Localization(Request.Cookies["Localization"])));
        }

    }
}
