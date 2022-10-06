using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Informer.Models
{
    public partial class news_model
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_avatar { get; set; }
        public int news_id { get; set; }
        public string news_title { get; set; }
        public int news_rating { get; set; }
        public Nullable<System.DateTime> news_data { get; set; }
        public string news_preview { get; set; }
        public string news_content { get; set; }
        public int news_autor_id { get; set; }
        public string category_name { get; set; }
        public int category_id { get; set; }
        public int roles_id { get; set; }
        public int adm_edit { get; set; }
    }

    public partial class ImageInfoModel
    {
        public string Name { get; set; }
        public string CkEditor { get; set; }
        public string CkEditorFuncNum { get; set; }
        public string PatchImg { get; set; }
        public int ImgLength { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    /// <summary>
    /// Личные сообшения отправленные
    /// </summary>
    public partial class MsgLsFrom
    {
        public int Id{ get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string SubJ { get; set; }
        public string Title { get; set; }
        public DateTime? MsgData { get; set; }
        public int Status { get; set; }
    }

    /// <summary>
    /// Личные сообшения полученные статус(прочитанные или нет)
    /// </summary>
    public partial class MsgLsTo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string SubJ { get; set; }
        public DateTime? MsgData { get; set; }
        public int Status { get; set; }     
        public string Title { get; set; }

    }

    [MetadataType(typeof(ls))]
    public partial class lsMessages
    {
    }

    public class ls
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string msg_id { get; set; }
    }

    /// <summary>
    /// Игнор пользователей
    /// </summary>
    public partial class UserIgnore
    {
        public string UserName { get; set; }
        public int UserId { get; set; }
    }
}