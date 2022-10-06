using Informer.Areas.hf.Class;
using Informer.Areas.hf.Models;
using Informer.Class;
using Informer.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Ninject;
using Telerik.Web.Mvc;

namespace Informer.Areas.hf.Controllers
{
    [InitializeSimpleMembership]
    public class QuestController : Controller
    {
        public QuestController(ILocal lokal, IDb dataBase)
        {
            Lokal = lokal;
            DataBase = dataBase;
        }
        
        [Inject]
        private ILocal Lokal { get; set; }

        [Inject]
        private IDb DataBase { get; set; }


        public ActionResult Info()
        {
            ViewBag.questInform = QuestInform("pusto", "0,1,2,3,4", 70, 84);
            return View();
        }

        [HttpPost]
        public ActionResult _searchByNameQuest(string text) // При наборе в поле поиска item выводит имена
        {
            var arg = text;
            var northwind = DataBase.Db();
            if (arg == null || arg.Trim() == "") arg = ""; arg.ToLower();
            var engCount = 0;
            var rusCount = 0;
            foreach (char c in arg)
            {
                if ((c > 'а' && c < 'я') || (c > 'А' && c < 'Я'))
                    rusCount++;
                else if ((c > 'a' && c < 'z') || (c > 'A' && c < 'Z'))
                    engCount++;
            }
            IQueryable<string> res;
            if (engCount > rusCount)
            {
                res = (from name in northwind.quest_name
                       where name.quest_names_en.Contains(arg)
                       select name.quest_names_en).Take(100);
            }
            else
            {
                res = (from name in northwind.quest_name
                       where name.quest_names.Contains(arg)
                       select name.quest_names).Take(100);
            }
            return Json(res);
        }

        [GridAction]
        public ActionResult _searchQuestInform(string quest_name, string quest_tip, int quest_min_lvl, int quest_max_lvl)
        {
          return Json(QuestInform(quest_name, quest_tip, quest_min_lvl, quest_max_lvl));
        }

        private IQueryable QuestInform(string quest_name, string quest_tip, int quest_min_lvl, int quest_max_lvl)
        {
            if (quest_min_lvl == -100 && quest_name == "pusto")
                return null;
            if (quest_name == "".Trim())
                return null;

            var northwind = DataBase.Db();
            IQueryable<quest_info> res = null;

            if (quest_name == "pusto")
            {
                var questTip = quest_tip.Split(',').Select(int.Parse).ToList();
                if (Lokal.Localization(Request.Cookies["Localization"]))
                {
                    res = from questGrp in northwind.quest_grp
                          join _questName in northwind.quest_name on questGrp.quest_id equals _questName.quest_id
                          join _questComplit in northwind.quest_name on questGrp.quest_complete equals _questComplit.quest_id into v3
                          from _questComplit in v3.DefaultIfEmpty()
                          where questGrp.quest_lvl_min >= quest_min_lvl && questGrp.quest_lvl_min <= quest_max_lvl && questTip.Contains(questGrp.quest_type)
                          select new quest_info
                          {
                              quest_id = questGrp.quest_id,
                              quest_area_id = questGrp.quest_area_id,
                              quest_clan_pet = questGrp.quest_clan_pet,
                              quest_complete = _questComplit.quest_names,
                              quest_complete_id = questGrp.quest_complete,
                              quest_lvl_max = questGrp.quest_lvl_max,
                              quest_lvl_min = questGrp.quest_lvl_min,
                              quest_names = _questName.quest_names,
                              quest_type = questGrp.quest_type,
                              quest_restricions = _questName.quest_restricions
                          };
                }
                else
                {
                    res = from questGrp in northwind.quest_grp
                          join _questName in northwind.quest_name on questGrp.quest_id equals _questName.quest_id
                          join _questComplit in northwind.quest_name on questGrp.quest_complete equals _questComplit.quest_id into v3
                          from _questComplit in v3.DefaultIfEmpty()
                          where questGrp.quest_lvl_min >= quest_min_lvl && questGrp.quest_lvl_min <= quest_max_lvl && questTip.Contains(questGrp.quest_type)
                          select new quest_info
                          {
                              quest_id = questGrp.quest_id,
                              quest_area_id = questGrp.quest_area_id,
                              quest_clan_pet = questGrp.quest_clan_pet,
                              quest_complete = _questComplit.quest_names_en,
                              quest_complete_id = questGrp.quest_complete,
                              quest_lvl_max = questGrp.quest_lvl_max,
                              quest_lvl_min = questGrp.quest_lvl_min,
                              quest_type = questGrp.quest_type,
                              quest_names = _questName.quest_names_en,
                              quest_restricions = _questName.quest_restricions_en
                          };
                }
            }
            else
            {
                if (quest_name == null || quest_name.Trim() == "")
                    quest_name = "";
                quest_name.ToLower();
                int engCount = 0;
                int rusCount = 0;
                foreach (char c in quest_name)
                {
                    if ((c > 'а' && c < 'я') || (c > 'А' && c < 'Я'))
                        rusCount++;
                    else if ((c > 'a' && c < 'z') || (c > 'A' && c < 'Z'))
                        engCount++;
                }

                int namber;
                if (Int32.TryParse(quest_name.Trim(), out namber))
                {
                    int argInt = Convert.ToInt32(quest_name);
                    if (Lokal.Localization(Request.Cookies["Localization"]))
                    {
                        res = from questGrp in northwind.quest_grp
                              join _questName in northwind.quest_name on questGrp.quest_id equals _questName.quest_id
                              join _questComplit in northwind.quest_name on questGrp.quest_complete equals _questComplit.quest_id into v3
                              from _questComplit in v3.DefaultIfEmpty()
                              where questGrp.quest_id == argInt
                              select new quest_info
                              {
                                  quest_id = questGrp.quest_id,
                                  quest_area_id = questGrp.quest_area_id,
                                  quest_clan_pet = questGrp.quest_clan_pet,
                                  quest_complete = _questComplit.quest_names,
                                  quest_complete_id = questGrp.quest_complete,
                                  quest_lvl_max = questGrp.quest_lvl_max,
                                  quest_lvl_min = questGrp.quest_lvl_min,
                                  quest_names = _questName.quest_names,
                                  quest_type = questGrp.quest_type,
                                  quest_restricions = _questName.quest_restricions
                              };
                    }
                    else
                    {
                        res = from questGrp in northwind.quest_grp
                              join _questName in northwind.quest_name on questGrp.quest_id equals _questName.quest_id
                              join _questComplit in northwind.quest_name on questGrp.quest_complete equals _questComplit.quest_id into v3
                              from _questComplit in v3.DefaultIfEmpty()
                              where questGrp.quest_id == argInt
                              select new quest_info
                              {
                                  quest_id = questGrp.quest_id,
                                  quest_area_id = questGrp.quest_area_id,
                                  quest_clan_pet = questGrp.quest_clan_pet,
                                  quest_complete = _questComplit.quest_names_en,
                                  quest_complete_id = questGrp.quest_complete,
                                  quest_lvl_max = questGrp.quest_lvl_max,
                                  quest_lvl_min = questGrp.quest_lvl_min,
                                  quest_type = questGrp.quest_type,
                                  quest_names = _questName.quest_names_en,
                                  quest_restricions = _questName.quest_restricions_en
                              };
                    }
                }
                else if (engCount > rusCount)
                {
                    res = from questGrp in northwind.quest_grp
                          join _questName in northwind.quest_name on questGrp.quest_id equals _questName.quest_id
                          join _questComplit in northwind.quest_name on questGrp.quest_complete equals _questComplit.quest_id into v3
                          from _questComplit in v3.DefaultIfEmpty()
                          where _questName.quest_names_en.Contains(quest_name)
                          select new quest_info
                          {
                              quest_id = questGrp.quest_id,
                              quest_area_id = questGrp.quest_area_id,
                              quest_clan_pet = questGrp.quest_clan_pet,
                              quest_complete = _questComplit.quest_names_en,
                              quest_complete_id = questGrp.quest_complete,
                              quest_lvl_max = questGrp.quest_lvl_max,
                              quest_lvl_min = questGrp.quest_lvl_min,
                              quest_type = questGrp.quest_type,
                              quest_names = _questName.quest_names_en,
                              quest_restricions = _questName.quest_restricions_en
                          };
                }
                else
                {
                    res = from questGrp in northwind.quest_grp
                          join _questName in northwind.quest_name on questGrp.quest_id equals _questName.quest_id
                          join _questComplit in northwind.quest_name on questGrp.quest_complete equals _questComplit.quest_id into v3
                          from _questComplit in v3.DefaultIfEmpty()
                          where _questName.quest_names.Contains(quest_name)
                          select new quest_info
                          {
                              quest_id = questGrp.quest_id,
                              quest_area_id = questGrp.quest_area_id,
                              quest_clan_pet = questGrp.quest_clan_pet,
                              quest_complete = _questComplit.quest_names,
                              quest_complete_id = questGrp.quest_complete,
                              quest_lvl_max = questGrp.quest_lvl_max,
                              quest_lvl_min = questGrp.quest_lvl_min,
                              quest_names = _questName.quest_names,
                              quest_type = questGrp.quest_type,
                              quest_restricions = _questName.quest_restricions
                          };
                }
            }
            return res;
        }

        [HttpGet]
        public ActionResult details(int id) // выводит имя квеста и описание
        {
            if (id == -100)
                return null;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            var northwind = DataBase.Db();
            List<questFull> questFull;
            List<questName> questName;
            if (Lokal.Localization(Request.Cookies["Localization"]))
            {
                questFull = (from QuestFull in northwind.quest_full
                             where QuestFull.quest_id == id
                             select new questFull { quest_fulls = QuestFull.quest_ru, quest_end = QuestFull.quest_end }).ToList();
                questName = (from QuestName in northwind.quest_name
                             where QuestName.quest_id == id
                             select new questName
                             {
                                 quest_id = QuestName.quest_id,
                                 quest_names = QuestName.quest_names,
                                 quest_names_er = QuestName.quest_names_en,
                                 quest_short_desc = QuestName.quest_short_desc,
                                 quest_short_desc_er = QuestName.quest_short_desc_en
                             }).ToList();
            }
            else
            {
                questFull = (from QuestFull in northwind.quest_full
                             where QuestFull.quest_id == id
                             select new questFull { quest_fulls = QuestFull.quest_en, quest_end = QuestFull.quest_end }).ToList();
                questName = (from QuestName in northwind.quest_name
                             where QuestName.quest_id == id
                             select new questName
                             {
                                 quest_id = QuestName.quest_id,
                                 quest_names = QuestName.quest_names_en,
                                 quest_names_er = QuestName.quest_names,
                                 quest_short_desc = QuestName.quest_short_desc_en,
                                 quest_short_desc_er = QuestName.quest_short_desc
                             }).ToList();
            }
            ViewBag.QuestName = questName;
            ViewBag.QuestFull = questFull;
            return View();
        }

        public ActionResult _questItemGet(int quest_id) // Получаем по quest_id награду с квеста
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            var northwind = DataBase.Db();
            IQueryable<QuestItemInfo> res = null;

            if (Lokal.Localization(Request.Cookies["Localization"]))
                res = from QuestItem in northwind.quest_items_get
                      join itemGrp in northwind.item_grp on QuestItem.quest_items_id equals itemGrp.item_id
                      join itemIame in northwind.item_name on QuestItem.quest_items_id equals itemIame.item_id
                      where QuestItem.quest_id == quest_id && QuestItem.quest_tag == 0
                      select new QuestItemInfo
                      {
                          item_id = itemGrp.item_id,
                          item_name = itemIame.item_name_ru,
                          item_img = itemGrp.item_img,
                          quest_item_count = QuestItem.quest_items_count
                      };
            else
                res = from QuestItem in northwind.quest_items_get
                      join itemGrp in northwind.item_grp on QuestItem.quest_items_id equals itemGrp.item_id
                      join itemIame in northwind.item_name on QuestItem.quest_items_id equals itemIame.item_id
                      where QuestItem.quest_id == quest_id && QuestItem.quest_tag == 0
                      select new QuestItemInfo
                      {
                          item_id = itemGrp.item_id,
                          item_name = itemIame.item_name_en,
                          item_img = itemGrp.item_img,
                          quest_item_count = QuestItem.quest_items_count
                      };
            return Json(res);
        }

        // ----------------------------------------- ADMIN -----------------------------------//

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public void quest_save_end(int id) // сохроняет что квест готов
        {
            using (var context = DataBase.Db())
            {
                context.Database.ExecuteSqlCommand("UPDATE quest_full SET quest_end = 1 WHERE quest_id = " + id);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        public void quest_save_check(int id) // сохроняет что квест на проверке для модераторов
        {
            using (var context = DataBase.Db())
            {
                context.Database.ExecuteSqlCommand("UPDATE quest_full SET quest_end = -1 WHERE quest_id = " + id);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public void quest_save_off(int id) // сохроняет что квест Не готов
        {
            using (var context =DataBase.Db())
            {
                context.Database.ExecuteSqlCommand("UPDATE quest_full SET quest_end = 0 WHERE quest_id = " + id);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public void quest_user_off(int id) // удолить юзера с занятого им квеста
        {
            using (var context = DataBase.Db())
            {
                context.Database.ExecuteSqlCommand("UPDATE quest_full SET quest_name_editor = '' WHERE quest_id = " + id);
            }
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateInput(false)]
        public void quest_save(int id, string quest_full, int quest_end) // сохроняет квесты
        {
            var northwind = DataBase.Db();
            var questFull4 = northwind.quest_full.Where(x => x.quest_id == id).FirstOrDefault();
            if (questFull4 != null)
            {
                if (questFull4.quest_name_editor != "" && questFull4.quest_name_editor != "Admin")
                {
                    if (questFull4.quest_name_editor != User.Identity.Name)
                    {
                        return;
                    }
                }
            }
            try
            {
                using (var context = DataBase.Db())
                {
                    if (!northwind.quest_full.Any(x => x.quest_id == id))
                        context.Database.ExecuteSqlCommand("INSERT INTO quest_full VALUES (" + id + ",'','','','','')");
                }

            }
            catch { }

            for (int h = 0; h < 2; h++)
            {
                string quest = quest_full;

                if (Regex.IsMatch(quest, @"\[npc\{"))
                {
                    Match[] npc_mas = Regex.Matches(quest, @"npc\{(.+?)\}").Cast<Match>().Distinct().ToArray();

                    using (var context = DataBase.Db())
                    {
                        context.Database.ExecuteSqlCommand("DELETE FROM quest_npc WHERE quest_id = " + id);
                    }
                    for (int z = 0; z < npc_mas.Length; z++)
                    {
                        string[] mas_npc_id_replace = npc_mas[z].Groups[1].Value.Split(',').ToArray();

                        int id_npc = Convert.ToInt32(mas_npc_id_replace[0]);
                        using (var context = DataBase.Db())
                        {
                            context.Database.ExecuteSqlCommand("INSERT INTO quest_npc VALUES (" + id + "," + id_npc + ")");
                        }
                        string _npc_name, _npc_titl;
                        if (h == 0)
                        {
                            _npc_name = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_name_ru).FirstOrDefault();
                            _npc_titl = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_title_ru).FirstOrDefault();
                        }
                        else
                        {
                            _npc_name = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_name_en).FirstOrDefault();
                            _npc_titl = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_title_en).FirstOrDefault();
                        }
                        string map = "";
                        if (mas_npc_id_replace.Length > 1)
                            map = @"<a class='toolNpcDetail'  title=""" + _npc_name + " " + _npc_titl + @""" href=""/hf/Npc/SpawnNpc/" + id_npc + @""" data-ajax-success=""_Map"" data-ajax-method=""POST"" data-ajax-begin=""MapName('" + _npc_name + " " + _npc_titl + @"', false)"" data-ajax=""true""> </a>";
                        string name_npc = "<strong><a href='/hf/Npc/details/" + id_npc + "'><span>" + _npc_name + " " + _npc_titl + "</span></a></strong> " + map;
                        string replace = @"\[npc\{" + npc_mas[z].Groups[1].Value + @"\}(.+?)\]";
                        quest = Regex.Replace(quest, replace, name_npc);
                    }

                }
                if (Regex.IsMatch(quest, @"\[ru_en\{"))
                {
                    MatchCollection race_mas = Regex.Matches(quest, @"ru_en\{(.+?)\}");

                    for (int z = 0; z < race_mas.Count; z++)
                    {
                        string[] race_en_ru = race_mas[z].Groups[1].Value.Split(':').ToArray();

                        string race_name;
                        if (h == 0)
                            race_name = race_en_ru[0];
                        else
                            race_name = race_en_ru[1];

                        string name_npc = "<span>" + race_name + "</span>";
                        string replace = @"\[ru_en\{" + race_en_ru[0] + @"(.+?)\]";
                        quest = Regex.Replace(quest, replace, name_npc);
                    }

                }
                if (Regex.IsMatch(quest, @"\[tip\{"))
                {
                    MatchCollection race_mas = Regex.Matches(quest, @"tip\{(.+?)\}");

                    for (int z = 0; z < race_mas.Count; z++)
                    {
                        string tip = "";
                        if (race_mas[z].Groups[1].Value == "0")
                            tip = Informer.Resources.Views.Quest.img_0;
                        else if (race_mas[z].Groups[1].Value == "1")
                            tip = Informer.Resources.Views.Quest.img_1;
                        else if (race_mas[z].Groups[1].Value == "2")
                            tip = Informer.Resources.Views.Quest.img_2;
                        else
                            tip = Informer.Resources.Views.Quest.img_3;
                        string name_npc = @"<img class=""toolNpcDetail"" title=""" + tip + @""" style='vertical-align: middle;width:22px;height:11px;' src='" + Url.Content("~/Images/quest/") + race_mas[z].Groups[1].Value + ".jpg' />";
                        string replace = @"\[tip\{" + race_mas[z].Groups[1].Value + @"(.+?)\]";
                        quest = Regex.Replace(quest, replace, name_npc);
                    }

                }
                if (Regex.IsMatch(quest, @"\[map_x\{"))
                {
                    Match[] map_mas = Regex.Matches(quest, @"map_x\{(.+?)\}").Cast<Match>().Distinct().ToArray();

                    for (int z = 0; z < map_mas.Length; z++)
                    {
                        MatchCollection map_npcId = Regex.Matches(map_mas[z].Groups[0].Value, @"\=(.+?)\}");
                        MatchCollection map_XY = Regex.Matches(map_mas[z].Groups[0].Value, @"\{(.+?)\=");

                        string _npc_name = "", _npc_titl = "", map = "";
                        if (map_npcId.Count > 0)
                        {
                            int id_npc = Convert.ToInt32(map_npcId[0].Groups[1].Value);

                            if (h == 0)
                            {
                                _npc_name = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_name_ru).FirstOrDefault();
                                _npc_titl = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_title_ru).FirstOrDefault();
                            }
                            else
                            {
                                _npc_name = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_name_en).FirstOrDefault();
                                _npc_titl = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_title_en).FirstOrDefault();
                            }

                            map = @"<img title=""" + _npc_name + " " + _npc_titl + @""" class=""toolNpcDetail"" onclick='MapQuest(""" + _npc_name + " " + _npc_titl + @""",""" + map_XY[0].Groups[1].Value + @""")' src='/Images/map.png'>";
                        }
                        else
                            map = @"<img class='toolNpcQ' onclick='MapQuest("""",""" + map_mas[z].Groups[1].Value + @""")' src='/Images/map.png'>";

                        string replace = @"\[map_x\{" + map_mas[z].Groups[1].Value + @"\}\]";
                        quest = Regex.Replace(quest, replace, map);
                    }
                }
                if (Regex.IsMatch(quest, @"\[map\{"))
                {
                    Match[] map_mas = Regex.Matches(quest, @"map\{(.+?)\}").Cast<Match>().Distinct().ToArray();

                    for (int z = 0; z < map_mas.Length; z++)
                    {

                        string _npc_name = "", _npc_titl = "", map = "";

                        string[] mas_npc_ = map_mas[z].Groups[1].Value.Split(',').ToArray();
                        int id_npc;
                        if (mas_npc_.Length > 1)
                            id_npc = Convert.ToInt32(mas_npc_[0]);
                        else
                            id_npc = Convert.ToInt32(map_mas[z].Groups[1].Value);

                        if (h == 0)
                        {
                            _npc_name = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_name_ru).FirstOrDefault();
                            _npc_titl = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_title_ru).FirstOrDefault();
                        }
                        else
                        {
                            _npc_name = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_name_en).FirstOrDefault();
                            _npc_titl = (from NpcName in northwind.npc_name where NpcName.npc_id == id_npc select NpcName.npc_title_en).FirstOrDefault();
                        }
                        if (mas_npc_.Length > 1)
                            map = @"<a class='toolNpcDetail'  title=""" + _npc_name + " " + _npc_titl + @""" href=""/hf/Npc/SpawnNpc/" + id_npc + @""" data-ajax-success=""_Map"" data-ajax-method=""POST"" data-ajax-begin=""MapName('" + _npc_name + " " + _npc_titl + @"', true)"" data-ajax=""true""> </a>";
                        else
                            map = @"<a class='toolNpcDetail'  title=""" + _npc_name + " " + _npc_titl + @""" href=""/hf/Npc/SpawnNpc/" + id_npc + @""" data-ajax-success=""_Map"" data-ajax-method=""POST"" data-ajax-begin=""MapName('" + _npc_name + " " + _npc_titl + @"', false)"" data-ajax=""true""> </a>";



                        string replace = @"\[map\{" + map_mas[z].Groups[1].Value + @"(.+?)\]";
                        quest = Regex.Replace(quest, replace, map);
                    }
                }
                if (Regex.IsMatch(quest, @"\[q_end\{"))
                {
                    Match[] map_mas = Regex.Matches(quest, @"q_end\{(.+?)\}").Cast<Match>().ToArray();

                    for (int z = 0; z < map_mas.Length; z++)
                    {

                        string _quest_name = "", res = "";
                        int quest_id = Convert.ToInt32(map_mas[z].Groups[1].Value);

                        if (h == 0)
                        {
                            _quest_name = (from QuestName in northwind.quest_name where QuestName.quest_id == quest_id select QuestName.quest_names).FirstOrDefault();
                        }
                        else
                        {
                            _quest_name = (from QuestName in northwind.quest_name where QuestName.quest_id == quest_id select QuestName.quest_names_en).FirstOrDefault();
                        }
                        res = @"<a href=""/hf/Quest/details/" + quest_id + @""">" + _quest_name + "</a>";
                        string replace = @"\[q_end\{" + map_mas[z].Groups[1].Value + @"(.+?)\]";
                        quest = Regex.Replace(quest, replace, res);
                    }
                }
                if (Regex.IsMatch(quest, @"\[item\{"))
                {
                    Match[] map_mas = Regex.Matches(quest, @"item\{(.+?)\}").Cast<Match>().Distinct().ToArray();
                    using (var context = DataBase.Db())
                    {
                        if (h == 0)
                            context.Database.ExecuteSqlCommand("DELETE FROM quest_items WHERE quest_id = " + id);
                    }
                    for (int z = 0; z < map_mas.Length; z++)
                    {
                        string[] item_id_count = map_mas[z].Groups[1].Value.Split(',').ToArray();

                        string _item_name = "", img = "", full_ = "";
                        int id_item = 0, count = 0;

                        if (item_id_count.Length > 1)
                        {
                            id_item = Convert.ToInt32(item_id_count[0]);
                            count = Convert.ToInt32(item_id_count[1]);
                            int QuestItems = (from questItems in northwind.quest_items where questItems.quest_id == id && questItems.quest_items_id == id_item && questItems.quest_items_count == count select questItems.quest_id).FirstOrDefault();

                            using (var context = DataBase.Db())
                            {
                                if (h == 0 && QuestItems == 0)
                                    context.Database.ExecuteSqlCommand("INSERT INTO quest_items VALUES (" + id + "," + id_item + "," + count + ")");
                            }
                        }
                        else
                        {
                            int QuestItems = (from questItems in northwind.quest_items where questItems.quest_id == id && questItems.quest_items_id == id_item && questItems.quest_items_count == count select questItems.quest_id).FirstOrDefault();

                            id_item = Convert.ToInt32(map_mas[z].Groups[1].Value);
                            using (var context = DataBase.Db())
                            {
                                if (h == 0 && QuestItems == 0)
                                    context.Database.ExecuteSqlCommand("INSERT INTO quest_items VALUES (" + id + "," + id_item + ",1)");
                            }
                        }

                        img = (from _ItemGrp in northwind.item_grp where _ItemGrp.item_id == id_item select _ItemGrp.item_img).FirstOrDefault();

                        if (h == 0)
                        {
                            _item_name = (from _ItemName in northwind.item_name where _ItemName.item_id == id_item select _ItemName.item_name_ru).FirstOrDefault();
                        }
                        else
                        {
                            _item_name = (from _ItemName in northwind.item_name where _ItemName.item_id == id_item select _ItemName.item_name_en).FirstOrDefault();
                        }
                        string tooltip = "";
                        //if (z == 0)
                        //tooltip = @"onload=""NpcdropOnDataBound()""";
                        if (item_id_count.Length == 3) // если трю то ставим перенос
                        {
                            if (item_id_count.Length > 1)// если трю то ставим количество штук
                                full_ = @"<img src=""/Images/icon/" + img + @".jpg"" " + tooltip + @" onerror=""NpcdroploadImage(this)"" itemid=""" + id_item + @""" title="""" class=""toltipDrop"">" +
                                    @"<a href=""/hf/Item/details/" + id_item + @"""><span>" + _item_name + @"</span></a> - " + count.ToString("### ### ### ### ###") + "</br>";
                            else
                                full_ = @"<img src=""/Images/icon/" + img + @".jpg"" " + tooltip + @" onerror=""NpcdroploadImage(this)"" itemid=""" + id_item + @""" title="""" class=""toltipDrop"">" +
                                @"<a href=""/hf/Item/details/" + id_item + @"""><span>" + _item_name + @"</span></a></br>";
                        }
                        else
                        {
                            if (item_id_count.Length > 1)
                                full_ = @"<img src=""/Images/icon/" + img + @".jpg"" " + tooltip + @" onerror=""NpcdroploadImage(this)"" itemid=""" + id_item + @""" title="""" class=""toltipDrop"">" +
                                    @"<a href=""/hf/Item/details/" + id_item + @"""><span>" + _item_name + @"</span></a> - " + count.ToString("### ### ### ### ###");
                            else
                                full_ = @"<img src=""/Images/icon/" + img + @".jpg"" " + tooltip + @" onerror=""NpcdroploadImage(this)"" itemid=""" + id_item + @""" title="""" class=""toltipDrop"">" +
                                @"<a href=""/hf/Item/details/" + id_item + @"""><span>" + _item_name + @"</span></a>";
                        }
                        string replace = @"\[item\{" + map_mas[z].Groups[1].Value + @"(.+?)\]";
                        quest = Regex.Replace(quest, replace, full_);
                    }
                }
                var questFull = northwind.quest_full.Where(x => x.quest_id == id).FirstOrDefault();
                if (h == 0)
                    questFull.quest_ru = quest;
                else
                    questFull.quest_en = quest;
                if (questFull.quest_end == -1)
                    questFull.quest_end = -1;
                else
                    questFull.quest_end = quest_end;

                northwind.SaveChanges();
                var questFull2 = northwind.quest_full.Where(x => x.quest_id == id).FirstOrDefault();

                    if (questFull.quest_name_editor != User.Identity.Name)
                        questFull.quest_name_editor = User.Identity.Name;
                    else
                        questFull.quest_name_editor = questFull.quest_name_editor;
                
                if (quest_full != null)
                    questFull2.quest_html = quest_full;
                northwind.SaveChanges();
            }
        }

        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Info_adm() // Выводит информацию о готовности квестов
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult details_adm(int id) // выводит имя квеста и описание
        {

            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            var northwind = DataBase.Db();
            Thread.Sleep(1000);

            ViewBag.QuestName = (from QuestName in northwind.quest_name
                                 where QuestName.quest_id == id
                                 select QuestName).ToList();
            ViewBag.QuestGrp = (from QuestGrp in northwind.quest_grp
                                where QuestGrp.quest_id == id
                                select QuestGrp).ToList();
            ViewBag.QuestDesc = (from QuestDesc in northwind.quest_desc
                                 where QuestDesc.quest_id == id
                                 select QuestDesc).ToList();
            ViewBag.QuestItem1 = (from QuestItem in northwind.quest_items_get
                                  where QuestItem.quest_id == id && QuestItem.quest_tag == 1
                                  select QuestItem).ToList();
            ViewBag.QuestItem0 = (from QuestItem in northwind.quest_items_get
                                  where QuestItem.quest_id == id && QuestItem.quest_tag == 0
                                  select QuestItem).ToList();
            ViewBag.QuestFull = (from QuestFull in northwind.quest_full
                                 where QuestFull.quest_id == id
                                 select new questFull { quest_fulls = QuestFull.quest_html, quest_end = QuestFull.quest_end, quest_id = QuestFull.quest_id, quest_name_editor = QuestFull.quest_name_editor }).ToList();
            return View();
        }

        [GridAction]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult _QuestName()
        {


            var northwind = DataBase.Db();
            IQueryable<questName> res = from _questName in northwind.quest_name
                                        join _questFull in northwind.quest_full on _questName.quest_id equals _questFull.quest_id into v3
                                        from _questFull in v3.DefaultIfEmpty()
                                        select new questName { quest_id = _questName.quest_id, quest_names = _questName.quest_names, quest_end = (_questFull.quest_end == null ? 0 : _questFull.quest_end), quest_name_editor = _questFull.quest_name_editor };
            return Json(res);
        }
    }
}
