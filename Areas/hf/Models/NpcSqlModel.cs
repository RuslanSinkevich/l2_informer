namespace Informer.Areas.hf.Models
{
    using System;
    using System.Collections.Generic;

    public partial class NpcDropSpoil
    {
        public int item_id { get; set; }
        public int category { get; set; }
        public int npc_id { get; set; }
        public int item_cry_tip { get; set; }
        public string item_img { get; set; }
        public string item_name { get; set; }
        public int npc_min { get; set; }
        public int npc_max { get; set; }
        public float npc_chance { get; set; }
    }

    public partial class NpcNmGpSk
    {
        public int npc_id { get; set; }
        public int npc_level { get; set; }
        public string npc_name { get; set; }
        public string npc_title { get; set; }
        public int npc_spawn { get; set; }
        public string npc_img { get; set; }
    }

    public partial class NpcDetails 
    {
        public int npc_id { get; set; }
        public int npc_class { get; set; }
        public string npc_img { get; set; }
        public int npc_level { get; set; }
        public int npc_hp { get; set; }
        public int npc_exp { get; set; }
        public int npc_sp { get; set; }
        public int npc_reit { get; set; }
        public int npc_spawn { get; set; }
        public string npc_name { get; set; }
        public string npc_name_er { get; set; }
        public string npc_title { get; set; }
        public string npc_title_er { get; set; }
        public string npc_desc { get; set; }
    }

    public partial class NpcDetailsSkils
    {
        public int skil_id { get; set; }
        public int skil_lvl { get; set; }
        public string skil_name { get; set; }
        public string skil_desc { get; set; }
        public string skil_img { get; set; }
    }

    public partial class NpcMinion
    {
        public int npc_minion_id { get; set; }
        public string npc_name { get; set; }
        public int npc_minion_count { get; set; }
    }
}