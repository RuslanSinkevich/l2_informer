using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Informer.Areas.hf.Models
{
    public partial class ItemDropSpoil
    {
        public int npc_id { get; set; }
        public int npc_level { get; set; }
        public string npc_name { get; set; }
        public string npc_titl { get; set; }
        public int item_min { get; set; }
        public int item_max { get; set; }
        public double item_change { get; set; }
        public int npc_category { get; set; }
    }

    public partial class ItemInfo
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public string item_img { get; set; }
        public int item_weight { get; set; }
        public int item_price { get; set; }
        public string item_add_name { get; set; }
    }

    public partial class ItemCraft
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public string item_img { get; set; }
    }

    public partial class ItemName
    {
        public string item_name { get; set; }
        public string item_desc { get; set; }
        public string item_name_er { get; set; }
        public string item_desc_er { get; set; }
        public string item_add_name { get; set; }
    }

}