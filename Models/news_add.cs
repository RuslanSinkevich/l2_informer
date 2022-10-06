using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Informer.Models // частичное представление класса  home_news для добавки атрибута
{
    [MetadataType(typeof(_news))]
    public partial class home_news
    {
    }

    public class _news
    {
        [Required]
        [StringLength(100, ErrorMessage = @"Значение ""{0}"" должно содержать не менее {2} символов.", MinimumLength = 6)]
        public string title { get; set; }
        [Required]
        [AllowHtml]
        public string content { get; set; }
        [Required]
        [AllowHtml]
        public string preview { get; set; }
    }
}