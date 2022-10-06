using System;
using System.Linq;
using System.Text.RegularExpressions;
using Informer.Areas.hf.Models;

namespace Informer.Filters
{
    public class ConvertTagHtml
    {
        public static string ConvertHf(string input)
        {

            var northwind = new high_five_4lUtkeEntities();

            if (Regex.IsMatch(input, @"\[npc\{"))
            {
                var npcMas = Regex.Matches(input, @"npc\{(.+?)\}").Cast<Match>().Distinct().ToArray();

                foreach (Match t in npcMas)
                {
                    string replace, nameNpc;
                    try
                    {
                        var masNpcIdReplace = t.Groups[1].Value.Split(',').ToArray();
                        var idNpc = Convert.ToInt32(masNpcIdReplace[0]);
                        var npcAll = northwind.npc_name.FirstOrDefault(x => x.npc_id == idNpc);
                        var map = "";
                        if (masNpcIdReplace.Length > 1)
                            map = string.Format(@"<img class=""mapNewsNpcId""  src=""/Images/map.png"" npcId=""{3}""  npcName=""{0} {2} ({1})"" />", npcAll.npc_name_ru, npcAll.npc_name_en, npcAll.npc_title_ru, idNpc);
                        nameNpc = "<a href='/hf/Npc/details/" + idNpc + "'><span>" + npcAll.npc_name_ru + " " + npcAll.npc_title_ru + " (" + npcAll.npc_name_en + ")</span></a> " + map;
                        replace = @"\[npc\{" + t.Groups[1].Value + @"\}]";
                    }
                    catch (Exception)
                    {
                        nameNpc = @"[npc{" + t.Groups[1].Value + @"}] - <span style=""color: red"">Ошибка тэга или отсутствует данный npc </span>";
                        replace = @"\[npc\{" + t.Groups[1].Value + @"\}]"; ;
                    }
                    input = Regex.Replace(input, replace, nameNpc);
                }
            }
            if (Regex.IsMatch(input, @"\[map_x\{"))
            {
                var mapMas = Regex.Matches(input, @"map_x\{(.+?)\}").Cast<Match>().Distinct().ToArray();
                foreach (Match t in mapMas)
                {
                    string replace, map;
                    try
                    {
                        var ints = t.Groups[1].Value.Split(Convert.ToChar(","));
                        foreach (var i in ints)
                        {
                            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                            Convert.ToInt32(i);
                            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
                        }
                         map = @"<img class='toolNpcQ mapNewsIndx' loc='" + t.Groups[0].Value + @"' src='/Images/map.png'>";
                         replace = @"\[map_x\{" + t.Groups[1].Value + @"\}\]";
                    }
                    catch (Exception)
                    {
                        map = @"[map_x{" + t.Groups[1].Value + @"}] - <span style=""color: red"">Ошибка тэга</span>";
                        replace = @"\[map_x\{" + t.Groups[1].Value + @"\}\]";
                    }
                    input = Regex.Replace(input, replace, map);
                }
            }
            if (Regex.IsMatch(input, @"\[map\{"))
            {
                var mapMas = Regex.Matches(input, @"map\{(.+?)\}").Cast<Match>().Distinct().ToArray();

                foreach (var t in mapMas)
                {
                    string replace, map;
                    try
                    {
                        var masNpc = t.Groups[1].Value.Split(',').ToArray();
                        var idNpc = Convert.ToInt32(masNpc.Length > 1 ? masNpc[0] : t.Groups[1].Value);
                        var npcAll = northwind.npc_name.FirstOrDefault(x => x.npc_id == idNpc);
                        map =
                            string.Format(
                                @"<img class=""mapNewsNpcId""  src=""/Images/map.png"" npcId=""{3}""  npcName=""{0} {2} ({1})"" />",
                                npcAll.npc_name_ru, npcAll.npc_name_en, npcAll.npc_title_ru, idNpc);
                        replace = @"\[map\{" + t.Groups[1].Value + @"\}\]";
                    }
                    catch (Exception)
                    {
                        map = "[map{" + t.Groups[1].Value + @"] - <span style=""color: red"">Ошибка тэга</span>"; ;
                        replace = @"\[map\{" + t.Groups[1].Value + @"\}\]";
                    }
                    input = Regex.Replace(input, replace, map);
                }
            }
            if (Regex.IsMatch(input, @"\[q_end\{"))
            {
                var mapMas = Regex.Matches(input, @"q_end\{(.+?)\}").Cast<Match>().ToArray();

                foreach (Match t in mapMas)
                {
                    string replace, res;
                    try
                    {
                        var questId = Convert.ToInt32(t.Groups[1].Value);

                        var questName = northwind.quest_name.FirstOrDefault(x => x.quest_id == questId);

                         res = @"<a href=""/hf/Quest/details/" + questId + @""">" + questName.quest_names + " (" +
                                  questName.quest_names_en + ")</a>";
                         replace = @"\[q_end\{" + t.Groups[1].Value + @"\}\]";
                    }
                    catch (Exception)
                    {
                        res = @"[q_end{" + t.Groups[1].Value + @"}] - <span style=""color: red"">Ошибка тэга или квест отсутствует</span>"; 
                        replace = @"\[q_end\{" + t.Groups[1].Value + @"\}\]";
                    }
                    input = Regex.Replace(input, replace, res);
                }
            }
            if (Regex.IsMatch(input, @"\[item\{"))
            {
                var item = Regex.Matches(input, @"item\{(.+?)\}").Cast<Match>().Distinct().ToArray();

                foreach (Match t in item)
                {
                    string replace, full;
                    try
                    {
                        var itemIdCount = t.Groups[1].Value.Split(',').ToArray();
                        var idItem = Convert.ToInt32(itemIdCount[0]);
                        var img = northwind.item_grp.FirstOrDefault(x => x.item_id == idItem).item_img;
                        var itemName = northwind.item_name.FirstOrDefault(x => x.item_id == idItem);
                        var itemDesc = northwind.item_desc.FirstOrDefault(x => x.item_id == idItem);
                        if (itemIdCount.Length > 1)
                            full = @"<img src=""/Images/icon/" + img +
                                    @".jpg"" onerror=""NpcdroploadImage(this)"" itemId=""" + idItem +
                                    @""" title="""" class=""News"">" +
                                    @"<a href=""/hf/Item/details/" + idItem + @"""><span>" + itemName.item_name_ru +
                                    " (" + itemName.item_name_en + @")</span></a> - <span>" + itemDesc.item_desc_ru +
                                    "</span>";
                        else
                            full = @"<img src=""/Images/icon/" + img +
                                    @".jpg"" onerror=""NpcdroploadImage(this)"" itemId=""" + idItem +
                                    @""" title="""" class=""News"">" +
                                    @"<a href=""/hf/Item/details/" + idItem + @"""><span>" + itemName.item_name_ru +
                                    " (" + itemName.item_name_en + @") </span></a>";

                         replace = @"\[item\{" + t.Groups[1].Value + @"\}\]";
                    }
                    catch (Exception)
                    {
                        full = @"[item{" + t.Groups[1].Value + @"}] - <span style=""color: red"">Ошибка тэга или итем отсутствует</span>";
                        replace = @"\[item\{" + t.Groups[1].Value + @"\}\]";
                    }
                    input = Regex.Replace(input, replace, full);
                }
            }
            if (Regex.IsMatch(input, @"\[skils\{"))
            {
                var item = Regex.Matches(input, @"skils\{(.+?)\}").Cast<Match>().Distinct().ToArray();

                foreach (Match t in item)
                {
                    string replace, full;
                    try
                    {
                        var skilIdCount = t.Groups[1].Value.Split(',').ToArray();
                        var idSkil = Convert.ToInt32(skilIdCount[0]);
                        var img = northwind.skill_grp.FirstOrDefault(x => x.skil_id == idSkil).skil_img;
                        var skilName = northwind.skill_grp.FirstOrDefault(x => x.skil_id == idSkil);
                        var skilDesc =
                            northwind.skils_desc_ru.FirstOrDefault(x => x.skil_id == idSkil && x.skil_lvl == 1);
                        if (skilIdCount.Length > 1)
                            full = @"<img src=""/Images/icon/" + img +
                                    @".jpg"" onerror=""NpcdroploadImage(this)""   title="""" class=""News"">" +
                                    @"<a href=""/hf/Skils/details/" + idSkil + @"""><span>" + skilName.skil_name_ru +
                                    " (" + skilName.skil_name_en + @")</span></a> - <span>" + skilDesc.skil_desc +
                                    "</span>";
                        else
                            full = @"<img src=""/Images/icon/" + img +
                                    @".jpg"" onerror=""NpcdroploadImage(this)""   title="""" class=""News"">" +
                                    @"<a href=""/hf/Skils/details/" + idSkil + @"""><span>" + skilName.skil_name_ru +
                                    " (" + skilName.skil_name_en + @") </span></a>";

                         replace = @"\[skils\{" + t.Groups[1].Value + @"\}\]";
                    }
                    catch (Exception)
                    {
                        full = @"[skils{" + t.Groups[1].Value + @"}] - <span style=""color: red"">Ошибка тэга или скил отсутствует</span>";
                        replace = @"\[skils\{" + t.Groups[1].Value + @"\}\]";

                    }
                    input = Regex.Replace(input, replace, full);
                }
            }
            return input;
        }
    }
}