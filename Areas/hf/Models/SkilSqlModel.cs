namespace Informer.Areas.hf.Models
{
    public class SkilTree
    {
        public int skil_id { get; set; }
        public int skil_lvl { get; set; }
        public string skil_img { get; set; }
        public string skil_name { get; set; }
        public string skil_desc { get; set; }
        public int skil_cost { get; set; }
        public int skil_item_id { get; set; }
        public string skil_oper_type { get; set; }
    }

    public class SkilDetail
    {
        public int skil_id { get; set; }
        public int skil_lvl { get; set; }
        public string skil_img { get; set; }
        public string skil_name { get; set; }
        public string skil_desc { get; set; }
        public string skil_name_er { get; set; }
        public string skil_desc_add1 { get; set; }
        public string skil_desc_add2 { get; set; }
        public string skil_oper_type { get; set; }
    }
}