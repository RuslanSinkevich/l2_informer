using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Informer.Areas.hf.Models
{
    public partial class CraftSql
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public string item_img { get; set; }
        public int item_cry_tip { get; set; }
        public int item_price { get; set; }
    }
}