namespace Informer.Areas.hf.Models
{
    public partial class questFull
    {
        public int quest_id { get; set; }
        public string quest_fulls { get; set; }
        public int quest_end { get; set; }
        public string quest_name_editor { get; set; }
    }

    public partial class questName
    {
        public int quest_id { get; set; }
        public string quest_names { get; set; }
        public string quest_names_er { get; set; }
        public string quest_short_desc { get; set; }
        public string quest_short_desc_er { get; set; }
        public int quest_end { get; set; }
        public string quest_name_editor { get; set; }
    }

    public partial class quest_info
    {
        public int quest_id { get; set; }
        public int quest_complete_id { get; set; }
        public string quest_names { get; set; }
        public string quest_restricions { get; set; }
        public int quest_lvl_min { get; set; }
        public int quest_lvl_max { get; set; }
        public int quest_type { get; set; }
        public int quest_clan_pet { get; set; }
        public string quest_complete { get; set; }
        public int quest_area_id { get; set; }
    }

    public partial class QuestItemInfo
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public string item_img { get; set; }
        public int quest_item_count { get; set; }
    }

    /// <summary>
    /// Показывает квесты где встречаеться итем или нпц
    /// </summary>
    public partial class QuestItemNpc
    {
        public int quest_id { get; set; }
        public string quest_names { get; set; }
        public int quest_lvl_min { get; set; }
        public int quest_lvl_max { get; set; }
    }
}