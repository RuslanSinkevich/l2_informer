using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Objects;
using System.Data.Common;
using System.Text;
using Informer.Areas.hf.Class;
using Ninject;


namespace Informer.Areas.hf.Models
{
    public class NpcModel
    {
        public NpcModel(IDb dataBase)
        {
            DataBase = dataBase;
        }

        [Inject]
        private IDb DataBase { get; set; }

        public IQueryable<NpcDropSpoil> GetNpcdrop(int npc_id, bool npcLocalization, int rate) // Получаем дроп мобов
        {
            if (npc_id == -100)
                return null;
            var northwind = DataBase.Db();
            if (npcLocalization)
                return from npcGrp in northwind.npc_grp
                       join npcDrop in northwind.npc_drop on npcGrp.npc_id equals npcDrop.npc_id
                       join itemName in northwind.item_name on npcDrop.npc_item_id equals itemName.item_id
                       where npcDrop.category == 1
                       join itemGrp in northwind.item_grp on itemName.item_id equals itemGrp.item_id
                       where npcGrp.npc_id == npc_id
                       select new NpcDropSpoil
                       {
                           npc_id = npcDrop.npc_id,
                           category = npcDrop.category,
                           item_id = itemName.item_id,
                           item_cry_tip = itemGrp.item_cry_tip,
                           item_img = itemGrp.item_img,
                           item_name = itemName.item_name_ru,
                           npc_min = npcDrop.npc_min,
                           npc_max = npcDrop.npc_max,
                           npc_chance = npcDrop.npc_chance * rate
                       };
            return from npcGrp in northwind.npc_grp
                   join npcDrop in northwind.npc_drop on npcGrp.npc_id equals npcDrop.npc_id
                   join itemName in northwind.item_name on npcDrop.npc_item_id equals itemName.item_id
                   where npcDrop.category == 1
                   join itemGrp in northwind.item_grp on itemName.item_id equals itemGrp.item_id
                   where npcGrp.npc_id == npc_id
                   select new NpcDropSpoil
                       {
                           npc_id = npcDrop.npc_id,
                           category = npcDrop.category,
                           item_id = itemName.item_id,
                           item_cry_tip = itemGrp.item_cry_tip,
                           item_img = itemGrp.item_img,
                           item_name = itemName.item_name_en,
                           npc_min = npcDrop.npc_min,
                           npc_max = npcDrop.npc_max,
                           npc_chance = npcDrop.npc_chance * rate
                       };
        }

        public IQueryable<NpcDropSpoil> GetNpcSpoil(int npc_id, bool npcLocalization, int rate) // Получаем споил мобов
        {
            if (npc_id == -100)
                return null;
            var northwind = DataBase.Db();
            if (npcLocalization)
                return from npcGrp in northwind.npc_grp
                       join npcDrop in northwind.npc_drop on npcGrp.npc_id equals npcDrop.npc_id into v1
                       from npcDrop in v1
                       join itemName in northwind.item_name on npcDrop.npc_item_id equals itemName.item_id into v2
                       from itemName in v2
                       where npcDrop.category == -1
                       join itemGrp in northwind.item_grp on itemName.item_id equals itemGrp.item_id into v3
                       from itemGrp in v3
                       where npcGrp.npc_id == npc_id
                       select new NpcDropSpoil
                       {
                           npc_id = npcDrop.npc_id,
                           category = npcDrop.category,
                           item_id = itemName.item_id,
                           item_cry_tip = itemGrp.item_cry_tip,
                           item_img = itemGrp.item_img,
                           item_name = itemName.item_name_ru,
                           npc_min = npcDrop.npc_min,
                           npc_max = npcDrop.npc_max,
                           npc_chance = npcDrop.npc_chance * rate
                       };
            return from npcGrp in northwind.npc_grp
                   join npcDrop in northwind.npc_drop on npcGrp.npc_id equals npcDrop.npc_id into v1
                   from npcDrop in v1
                   join itemName in northwind.item_name on npcDrop.npc_item_id equals itemName.item_id into v2
                   from itemName in v2
                   where npcDrop.category == -1
                   join itemGrp in northwind.item_grp on itemName.item_id equals itemGrp.item_id into v3
                   from itemGrp in v3
                   where npcGrp.npc_id == npc_id
                   select new NpcDropSpoil
                       {
                           npc_id = npcDrop.npc_id,
                           category = npcDrop.category,
                           item_id = itemName.item_id,
                           item_cry_tip = itemGrp.item_cry_tip,
                           item_img = itemGrp.item_img,
                           item_name = itemName.item_name_en,
                           npc_min = npcDrop.npc_min,
                           npc_max = npcDrop.npc_max,
                           npc_chance = npcDrop.npc_chance * rate
                       };
        }

        public IQueryable<string> GetNpcNameStr(string arg) // При наборе в поле поиска мобов выводит имена
        {
            var northwind = DataBase.Db();
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            if (arg == null || arg.Trim() == "") arg = ""; arg.ToLower();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            int engCount = 0;
            int rusCount = 0;
            foreach (char c in arg)
            {
                if ((c > 'а' && c < 'я') || (c > 'А' && c < 'Я'))
                    rusCount++;
                else if ((c > 'a' && c < 'z') || (c > 'A' && c < 'Z'))
                    engCount++;
            }
            if (engCount > rusCount)
            {
                return (from name in northwind.npc_name
                        where name.npc_name_en.Contains(arg)
                        select name.npc_name_en).Take(150);
            }
            return (from name in northwind.npc_name
                    where name.npc_name_ru.Contains(arg)
                    select name.npc_name_ru).Take(150);
        }

        public IQueryable GetNpcInform(string name, string class_tip, int npc_vulnerability, int slider_min, int slider_max, string npc_raite, bool npcLocalization) // Получаем список мобов в грид NpcInform
        {
            if (class_tip == "-100" && name == "pusto")
                return null;
            if (name == "".Trim())
                return null;
            var northwind2 = new ObjectContext("name=" + DataBase.Dbstr())
                {
                    DefaultContainerName = DataBase.Dbstr()
                };

            string joinSkil = " LEFT JOIN " + DataBase.Dbstr() + ".npc_skilss as npcSkil ON npcSkil.npc_id = npcGrp.npc_id ";
            if (name == "pusto")
            {
                string where;
                if (npc_vulnerability != -100 && npc_raite != "-100")
                {
                    where = joinSkil + " where npcGrp.npc_class = " + class_tip + " and" +
                        " npcGrp.npc_reit in {" + npc_raite + "} and" +
                        " npcSkil.npc_skil_id = " + npc_vulnerability + " and" +
                        " npcGrp.npc_level >= " + slider_min + " and" +
                        " npcGrp.npc_level <= " + slider_max;
                }
                else if (npc_vulnerability == -100 && npc_raite != "-100")
                {
                    where = " where npcGrp.npc_class = " + class_tip + " and" +
                        " npcGrp.npc_reit in {" + npc_raite + "} and" +
                        " npcGrp.npc_level >= " + slider_min + " and" +
                        " npcGrp.npc_level <= " + slider_max;
                }
                else if (npc_vulnerability != -100 && npc_raite == "-100")
                {
                    where = joinSkil + " where npcGrp.npc_class = " + class_tip + " and" +
                        " npcSkil.npc_skil_id = " + npc_vulnerability + " and" +
                        " npcGrp.npc_level >= " + slider_min + " and" +
                        " npcGrp.npc_level <= " + slider_max;
                }
                else
                {
                    where = " where npcGrp.npc_class = " + class_tip + " and" +
                        " npcGrp.npc_level >= " + slider_min + " and" +
                        " npcGrp.npc_level <= " + slider_max;
                }

                string select;
                if (npcLocalization)
                    select = "SELECT npcName.npc_id, npcGrp.npc_level, npcName.npc_name_ru as npc_name, npcName.npc_title_ru as npc_title, npcGrp.npc_spawn, npcGrp.npc_img  FROM " + DataBase.Dbstr() + ".npc_name as npcName";
                else
                    select = "SELECT npcName.npc_id, npcGrp.npc_level, npcName.npc_name_en as npc_name, npcName.npc_title_en as npc_title, npcGrp.npc_spawn, npcGrp.npc_img  FROM " + DataBase.Dbstr() + ".npc_name as npcName";

                var esqlQuery = select
                    + " LEFT JOIN " + DataBase.Dbstr() + ".npc_grp as npcGrp ON npcGrp.npc_id = npcName.npc_id "
                    + where + " ORDER BY npcGrp.npc_level ASC";

                var onlineOrders = new ObjectQuery<DbDataRecord>(esqlQuery, northwind2);
                return onlineOrders.ToList().ConvertTo<NpcNmGpSk>().AsQueryable();
            }
            var northwind = DataBase.Db();
            if (name == null || name.Trim() == "") name = "";
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            name.ToLower();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            var engCount = 0;
            var rusCount = 0;
            foreach (var c in name)
            {
                if ((c > 'а' && c < 'я') || (c > 'А' && c < 'Я'))
                    rusCount++;
                else if ((c > 'a' && c < 'z') || (c > 'A' && c < 'Z'))
                    engCount++;
            }

            int namber;
            if (Int32.TryParse(name.Trim(), out namber))
            {
                var argInt = Convert.ToInt32(name);
                if (npcLocalization)
                {
                    return from npcName in northwind.npc_name
                           join npcGrp in northwind.npc_grp on npcName.npc_id equals npcGrp.npc_id into v3
                           from npcGrp in v3
                           where npcName.npc_id == argInt
                           select new { npc_name = npcName.npc_name_ru, npcGrp.npc_level, npc_title = npcName.npc_title_ru, npcName.npc_id, npcGrp.npc_spawn };
                }
                return from npcName in northwind.npc_name
                       join npcGrp in northwind.npc_grp on npcName.npc_id equals npcGrp.npc_id into v3
                       from npcGrp in v3
                       where npcName.npc_id == argInt
                       select new { npc_name = npcName.npc_name_en, npcGrp.npc_level, npc_title = npcName.npc_title_en, npcName.npc_id, npcGrp.npc_spawn };
            }
            else if (engCount > rusCount)
            {
                return from npcName in northwind.npc_name
                       join npcGrp in northwind.npc_grp on npcName.npc_id equals npcGrp.npc_id into v3
                       from npcGrp in v3
                       where npcName.npc_name_en.Contains(name)
                       select new { npc_name = npcName.npc_name_en, npcGrp.npc_level, npc_title = npcName.npc_title_en, npcName.npc_id, npcGrp.npc_spawn };
            }
            else
            {
                var r2 = from npcName in northwind.npc_name
                         join npcGrp in northwind.npc_grp on npcName.npc_id equals npcGrp.npc_id into v3
                         from npcGrp in v3
                         where npcName.npc_name_ru.Contains(name)
                         select new { npc_name = npcName.npc_name_ru, npcGrp.npc_level, npc_title = npcName.npc_title_ru, npcName.npc_id, npcGrp.npc_spawn };
                return r2;
            }
        }

        public string Spawn(int npc_id_spawn) // Получаем спавн npc закодированный в str64
        {

            var northwind = DataBase.Db();

            var spawn = from npcDrop in northwind.npc_spawn
                        where npcDrop.npc_id == npc_id_spawn
                        select new { npcDrop.npc_x, npcDrop.npc_y };
            var rez = "";
            var z = true;
            foreach (var item in spawn)
            {
                if (z)
                {
                    rez += item.npc_x + "," + item.npc_y;
                    z = false;
                }
                else
                    rez += "," + item.npc_x + "," + item.npc_y;
            }
            var buffer = Encoding.ASCII.GetBytes(rez);
            return Convert.ToBase64String(buffer);

        }

        public List<NpcDetails> GetNpcDetails(int npc_id, bool npcLocalization) // Получаем детали о конкретном npc
        {
            var northwind = DataBase.Db();
            if (npcLocalization)
                return (from npcGrp in northwind.npc_grp
                        join npcName in northwind.npc_name on npcGrp.npc_id equals npcName.npc_id into v1
                        from npcName in v1
                        where npcGrp.npc_id == npc_id
                        select new NpcDetails
                        {
                            npc_id = npcGrp.npc_id,
                            npc_class = npcGrp.npc_class,
                            npc_img = npcGrp.npc_img,
                            npc_level = npcGrp.npc_level,
                            npc_hp = npcGrp.npc_hp,
                            npc_exp = npcGrp.npc_exp,
                            npc_sp = npcGrp.npc_sp,
                            npc_reit = npcGrp.npc_reit,
                            npc_spawn = npcGrp.npc_spawn,
                            npc_name = npcName.npc_name_ru,
                            npc_title = npcName.npc_title_ru,
                            npc_name_er = npcName.npc_name_en,
                            npc_title_er = npcName.npc_title_en
                        }).ToList();
            return (from npcGrp in northwind.npc_grp
                    join npcName in northwind.npc_name on npcGrp.npc_id equals npcName.npc_id into v1
                    from npcName in v1
                    where npcGrp.npc_id == npc_id
                    select new NpcDetails
                        {
                            npc_id = npcGrp.npc_id,
                            npc_class = npcGrp.npc_class,
                            npc_img = npcGrp.npc_img,
                            npc_level = npcGrp.npc_level,
                            npc_hp = npcGrp.npc_hp,
                            npc_exp = npcGrp.npc_exp,
                            npc_sp = npcGrp.npc_sp,
                            npc_reit = npcGrp.npc_reit,
                            npc_spawn = npcGrp.npc_spawn,
                            npc_name = npcName.npc_name_en,
                            npc_title = npcName.npc_title_en,
                            npc_name_er = npcName.npc_name_ru,
                            npc_title_er = npcName.npc_title_ru
                        }).ToList();
        }

        public IEnumerable<NpcDetailsSkils> GetNpcDetailsSkils(int npc_id, bool npcLocalization) // Получаем скилы конкретного npc
        {
            var northwind = DataBase.Db();
            List<NpcDetailsSkils> res;
            if (npcLocalization)
                res = (from npcSkils in northwind.npc_skilss
                       join skilGrp in northwind.skill_grp on npcSkils.npc_skil_id equals skilGrp.skil_id
                       join skilDesc in northwind.skils_desc_ru on npcSkils.npc_skil_id equals skilDesc.skil_id
                       where npcSkils.npc_id == npc_id && skilGrp.skil_lvl == npcSkils.npc_skil_lvl && skilDesc.skil_lvl == npcSkils.npc_skil_lvl
                       select new NpcDetailsSkils
                       {
                           skil_id = npcSkils.npc_skil_id,
                           skil_lvl = npcSkils.npc_skil_lvl,
                           skil_name = skilGrp.skil_name_ru,
                           skil_desc = skilDesc.skil_desc,
                           skil_img = skilGrp.skil_img,
                       }).Distinct().ToList();
            else
                res = (from npcSkils in northwind.npc_skilss
                       join skilGrp in northwind.skill_grp on npcSkils.npc_skil_id equals skilGrp.skil_id
                       join skilDesc in northwind.skils_desc_en on npcSkils.npc_skil_id equals skilDesc.skil_id
                       where npcSkils.npc_id == npc_id && skilGrp.skil_lvl == npcSkils.npc_skil_lvl && skilDesc.skil_lvl == npcSkils.npc_skil_lvl
                       select new NpcDetailsSkils
                       {
                           skil_id = npcSkils.npc_skil_id,
                           skil_lvl = npcSkils.npc_skil_lvl,
                           skil_name = skilGrp.skil_name_en,
                           skil_desc = skilDesc.skil_desc,
                           skil_img = skilGrp.skil_img,
                       }).Distinct().ToList();
            return res;
        }

        public List<string> GetNpcRbDesc(int npc_id, bool npcLocalization)// Получает описание рб
        {
            var northwind = DataBase.Db();
            var res = from npcDescRb in northwind.npc_desc_rb where npcDescRb.npc_id == npc_id select npcDescRb;
            if (npcLocalization)
                return res.Select(x => x.npc_raid_desc_ru).ToList();
            return res.Select(x => x.npc_raid_desc_en).ToList();
        }

        public List<NpcMinion> GetNpcMinion(int npc_id, bool npcLocalization)// Получает описание рб
        {
            var northwind = DataBase.Db();
            if (npcLocalization)
                return (from npcMinion in northwind.npc_rb_minion
                        join npcName in northwind.npc_name on npcMinion.npc_id_minion equals npcName.npc_id
                        where npcMinion.npc_id_rb == npc_id
                        select new NpcMinion
                        {
                            npc_minion_id = npcMinion.npc_id_minion,
                            npc_name = npcName.npc_name_ru,
                            npc_minion_count = npcMinion.npc_minion_count
                        }).ToList();
            return (from npcMinion in northwind.npc_rb_minion
                    join npcName in northwind.npc_name on npcMinion.npc_id_minion equals npcName.npc_id
                    where npcMinion.npc_id_rb == npc_id
                    select new NpcMinion
                        {
                            npc_minion_id = npcMinion.npc_id_minion,
                            npc_name = npcName.npc_name_en,
                            npc_minion_count = npcMinion.npc_minion_count
                        }).ToList();
        }

        public List<QuestItemNpc> GetNpcQuest(int npcId, bool npcLocalization)// Получает описание рб
        {
            var northwind = DataBase.Db();
            var firstOrDefault = northwind.quest_npc.Where(x => x.quest_npc_id == npcId).Select(x => x.quest_id).ToArray();
            if (npcLocalization)
            {
                return (from item in northwind.quest_name
                        join qGrp in northwind.quest_grp on item.quest_id equals qGrp.quest_id
                        where firstOrDefault.Contains(item.quest_id)
                        select new QuestItemNpc
                     {
                         quest_id = item.quest_id,
                         quest_names = item.quest_names,
                         quest_lvl_max = qGrp.quest_lvl_max,
                         quest_lvl_min = qGrp.quest_lvl_min
                     }).ToList();
            }
            return (from item in northwind.quest_name
                    join qGrp in northwind.quest_grp on item.quest_id equals qGrp.quest_id
                    where firstOrDefault.Contains(item.quest_id)
                    select new QuestItemNpc
                        {
                            quest_id = item.quest_id,
                            quest_names = item.quest_names_en,
                            quest_lvl_max = qGrp.quest_lvl_max,
                            quest_lvl_min = qGrp.quest_lvl_min
                        }).ToList();
        }
    }


}