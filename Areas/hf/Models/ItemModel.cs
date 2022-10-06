using System;
using System.Collections.Generic;
using System.Linq;
using Informer.Areas.hf.Class;
using Ninject;

namespace Informer.Areas.hf.Models
{
    public class ItemModel
    {
        public ItemModel(IDb dataBase)
        {
            DataBase = dataBase;
        }

        public ItemModel()
        {
            
        }

        [Inject]
        private IDb DataBase { get; set; }

        public IQueryable<string> GetItemNameStr(string arg) // При наборе в поле поиска item выводит имена
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
                return (from name in northwind.item_name
                        where name.item_name_en.Contains(arg)
                        select name.item_name_en).Take(100);
            }
            return (from name in northwind.item_name
                    where name.item_name_ru.Contains(arg)
                    select name.item_name_ru).Take(100);
        }

        public IQueryable GetItemTip(int item_tip, bool npcLocalization) // Получаем итемы по типу
        {
            if (item_tip == -100)
                return null;
            var northwind = DataBase.Db();
            if (npcLocalization)
                return from itemGrp in northwind.item_grp
                       join itemName in northwind.item_name on itemGrp.item_id equals itemName.item_id
                       where itemGrp.item_tip == item_tip
                       select new {
                           itemGrp.item_id,
                           item_name = itemName.item_name_ru, 
                           itemGrp.item_img };
            return from itemGrp in northwind.item_grp
                   join itemName in northwind.item_name on itemGrp.item_id equals itemName.item_id
                   where itemGrp.item_tip == item_tip
                   select new
                       {
                           itemGrp.item_id,
                           item_name = itemName.item_name_en, 
                           itemGrp.item_img
                       };
        }

        public IQueryable<ItemInfo> GetItemInform(string name, int item_tip, bool npcLocalization) // Получаем список item в грид ItemInform
        {
            if (item_tip == -100 && name == "pusto")
                return null;
            if (name.Trim() == "")
                return null;
            var northwind = DataBase.Db();

            if (name.Trim() == "") name = "";
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
            name.ToLower();
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
            int engCount = 0;
            int rusCount = 0;
            int namber;

            foreach (char c in name)
            {
                if ((c > 'а' && c < 'я') || (c > 'А' && c < 'Я'))
                    rusCount++;
                else if ((c > 'a' && c < 'z') || (c > 'A' && c < 'Z'))
                    engCount++;
            }
            if (name == "pusto")
            {
                if (npcLocalization)
                    return from itemGrp in northwind.item_grp
                           join itemName in northwind.item_name on itemGrp.item_id equals itemName.item_id
                           join itemDesc in northwind.item_desc on itemGrp.item_id equals itemDesc.item_id
                           where itemGrp.item_tip == item_tip
                           select new ItemInfo
                           {
                               item_id = itemGrp.item_id,
                               item_name = itemName.item_name_ru,
                               item_img = itemGrp.item_img,
                               item_weight = itemGrp.item_weight,
                               item_price = itemGrp.item_price,
                               item_add_name = itemDesc.item_add_name_ru
                           };
                return from itemGrp in northwind.item_grp
                       join itemName in northwind.item_name on itemGrp.item_id equals itemName.item_id
                       join itemDesc in northwind.item_desc on itemGrp.item_id equals itemDesc.item_id
                       where itemGrp.item_tip == item_tip
                       select new ItemInfo
                           {
                               item_id = itemGrp.item_id,
                               item_name = itemName.item_name_en,
                               item_img = itemGrp.item_img,
                               item_weight = itemGrp.item_weight,
                               item_price = itemGrp.item_price,
                               item_add_name = itemDesc.item_add_name_en
                           };
            }
            if (Int32.TryParse(name.Trim(), out namber))
            {
                int argInt = Convert.ToInt32(name);
                if (npcLocalization)
                    return from itemGrp in northwind.item_grp
                           join itemDesc in northwind.item_desc on itemGrp.item_id equals itemDesc.item_id
                           join itemName in northwind.item_name on itemGrp.item_id equals itemName.item_id
                           where itemGrp.item_id == argInt
                           select new ItemInfo
                               {
                                   item_id = itemGrp.item_id,
                                   item_name = itemName.item_name_ru,
                                   item_img = itemGrp.item_img,
                                   item_weight = itemGrp.item_weight,
                                   item_price = itemGrp.item_price,
                                   item_add_name = itemDesc.item_add_name_ru
                               };
                return from itemGrp in northwind.item_grp
                       join itemDesc in northwind.item_desc on itemGrp.item_id equals itemDesc.item_id
                       join itemName in northwind.item_name on itemGrp.item_id equals itemName.item_id
                       where itemGrp.item_id == argInt
                       select new ItemInfo
                           {
                               item_id = itemGrp.item_id,
                               item_name = itemName.item_name_en,
                               item_img = itemGrp.item_img,
                               item_weight = itemGrp.item_weight,
                               item_price = itemGrp.item_price,
                               item_add_name = itemDesc.item_add_name_en
                           };
            }
            if (engCount > rusCount)
            {
                return from itemGrp in northwind.item_grp
                       join itemDesc in northwind.item_desc on itemGrp.item_id equals itemDesc.item_id
                       join itemName in northwind.item_name on itemGrp.item_id equals itemName.item_id
                       where itemName.item_name_en.Contains(name)
                       select new ItemInfo
                           {
                               item_id = itemGrp.item_id,
                               item_name = itemName.item_name_en,
                               item_img = itemGrp.item_img,
                               item_weight = itemGrp.item_weight,
                               item_price = itemGrp.item_price,
                               item_add_name = itemDesc.item_add_name_en
                           };
            }
            return from itemGrp in northwind.item_grp
                   join itemDesc in northwind.item_desc on itemGrp.item_id equals itemDesc.item_id
                   join itemName in northwind.item_name on itemGrp.item_id equals itemName.item_id
                   where itemName.item_name_ru.Contains(name)
                   select new ItemInfo
                       {
                           item_id = itemGrp.item_id,
                           item_name = itemName.item_name_ru,
                           item_img = itemGrp.item_img,
                           item_weight = itemGrp.item_weight,
                           item_price = itemGrp.item_price,
                           item_add_name = itemDesc.item_add_name_ru
                       };
        }

        public IQueryable<ItemDropSpoil> GetItemDrop(int item_id, bool npcLocalization, int rate) // Получаем по итему с мобов  дроп и спойл
        {
            var northwind = DataBase.Db();
            IQueryable<ItemDropSpoil> res;
            if (npcLocalization)
                res = from npcDrop in northwind.npc_drop
                       join npcGrp in northwind.npc_grp on npcDrop.npc_id equals npcGrp.npc_id
                       join npcName in northwind.npc_name on npcDrop.npc_id equals npcName.npc_id
                       where npcDrop.npc_item_id == item_id && npcDrop.category == 1
                       select new ItemDropSpoil
                       {
                           npc_id = npcDrop.npc_id,
                           npc_level = npcGrp.npc_level,
                           npc_name = npcName.npc_name_ru,
                           npc_titl = npcName.npc_title_ru,
                           item_min = npcDrop.npc_min,
                           item_max = npcDrop.npc_max,
                           item_change = npcDrop.npc_chance * rate,
                           npc_category = npcDrop.category
                       };
            else
            res = from npcDrop in northwind.npc_drop
                   join npcGrp in northwind.npc_grp on npcDrop.npc_id equals npcGrp.npc_id
                   join npcName in northwind.npc_name on npcDrop.npc_id equals npcName.npc_id
                   where npcDrop.npc_item_id == item_id && npcDrop.category == 1
                   select new ItemDropSpoil
                       {
                           npc_id = npcDrop.npc_id,
                           npc_level = npcGrp.npc_level,
                           npc_name = npcName.npc_name_en,
                           npc_titl = npcName.npc_title_en,
                           item_min = npcDrop.npc_min,
                           item_max = npcDrop.npc_max,
                           item_change = npcDrop.npc_chance * rate,
                           npc_category = npcDrop.category
                       };
            return res;
        }

        public IQueryable<ItemDropSpoil> GetItemSpoil(int item_id, bool npcLocalization, int rate) // Получаем по итему с мобов  дроп и спойл
        {
            var northwind = DataBase.Db();
            IQueryable<ItemDropSpoil> res;
            if (npcLocalization)
                res = from npcDrop in northwind.npc_drop
                      join npcGrp in northwind.npc_grp on npcDrop.npc_id equals npcGrp.npc_id
                      join npcName in northwind.npc_name on npcDrop.npc_id equals npcName.npc_id
                      where npcDrop.npc_item_id == item_id && npcDrop.category == -1
                      select new ItemDropSpoil
                          {
                              npc_id = npcDrop.npc_id,
                              npc_level = npcGrp.npc_level,
                              npc_name = npcName.npc_name_ru,
                              npc_titl = npcName.npc_title_ru,
                              item_min = npcDrop.npc_min,
                              item_max = npcDrop.npc_max,
                              item_change = npcDrop.npc_chance * rate,
                              npc_category = npcDrop.category
                          };
            else
                res = from npcDrop in northwind.npc_drop
                      join npcGrp in northwind.npc_grp on npcDrop.npc_id equals npcGrp.npc_id
                      join npcName in northwind.npc_name on npcDrop.npc_id equals npcName.npc_id
                      where npcDrop.npc_item_id == item_id && npcDrop.category == -1
                      select new ItemDropSpoil
                          {
                              npc_id = npcDrop.npc_id,
                              npc_level = npcGrp.npc_level,
                              npc_name = npcName.npc_name_en,
                              npc_titl = npcName.npc_title_en,
                              item_min = npcDrop.npc_min,
                              item_max = npcDrop.npc_max,
                              item_change = npcDrop.npc_chance * rate,
                              npc_category = npcDrop.category
                          };
            return res;
        }

        public List<item_grp> GetItemDetailsGrp(int item_id) // Получаем детали item_grp о конкретном item
        {
            var northwind = DataBase.Db();
            return (from itemGrp in northwind.item_grp
                        where itemGrp.item_id == item_id
                        select itemGrp).ToList();
        }

        public List<ItemName> GetItemDetailsName(int item_id, bool npcLocalization) 
        {
            var northwind = DataBase.Db();
            if (npcLocalization)
            {
                return (from itemName in northwind.item_name
                        join itemDesc in northwind.item_desc on itemName.item_id equals itemDesc.item_id
                        where itemName.item_id == item_id
                        select new ItemName
                        {
                            item_name = itemName.item_name_ru,
                            item_desc = itemDesc.item_desc_ru,
                            item_name_er = itemName.item_name_en,
                            item_desc_er = itemDesc.item_desc_en,
                            item_add_name = itemDesc.item_add_name_ru
                        }).ToList();
            }
            return (from itemName in northwind.item_name
                    join itemDesc in northwind.item_desc on itemName.item_id equals itemDesc.item_id
                    where itemName.item_id == item_id
                    select new ItemName
                        {
                            item_name = itemName.item_name_en,
                            item_desc = itemDesc.item_desc_en,
                            item_name_er = itemName.item_name_ru,
                            item_desc_er = itemDesc.item_desc_ru,
                            item_add_name = itemDesc.item_add_name_ru
                        }).ToList();
        }

        public List<QuestItemNpc> GetItemQuest(int itemId, bool npcLocalization)
        {
            var northwind = DataBase.Db();
            var firstOrDefault = northwind.quest_items.Where(x => x.quest_items_id == itemId).Select(x => x.quest_id).ToArray();
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
                        }).OrderBy(x => x.quest_lvl_max).ToList();
            }
            return (from item in northwind.quest_name
                    join qGrp in northwind.quest_grp on item.quest_id equals qGrp.quest_id
                    where firstOrDefault.Contains(item.quest_id)
                    select new QuestItemNpc
                    {
                        quest_id = item.quest_id,
                        quest_names = item.quest_names,
                        quest_lvl_max = qGrp.quest_lvl_max,
                        quest_lvl_min = qGrp.quest_lvl_min
                    }).OrderBy(x => x.quest_lvl_max).ToList();
        }
    }
}