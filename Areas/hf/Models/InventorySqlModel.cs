using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Informer.Areas.hf.Models
{
    public partial class InventoryInfoSql
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public string item_img { get; set; }
        public int item_cry_count { get; set; }
        public int item_cry_tip { get; set; }
        public int item_crafr { get; set; }
        public int item_tradeable { get; set; }
        public int item_mdef { get; set; }
        public int item_pdef { get; set; }
        public int item_patt { get; set; }
        public int item_matt { get; set; }
        public int item_price { get; set; }
        public string item_add_name { get; set; }
    }
}