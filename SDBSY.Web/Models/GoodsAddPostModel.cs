using System.ComponentModel.DataAnnotations;

namespace SDBSY.Web.Models
{
    public class GoodsAddPostModel
    {
        [Required(ErrorMessage = "名称必填")]
        public string Name { get; set; }
        [Required(ErrorMessage ="计量单位必填")]
        public string Unit { get; set; }
        [Required(ErrorMessage ="规格必填")]
        public string Format { get; set; }
        [Required(ErrorMessage = "卖家必填")]
        public string Seller { get; set; }
        public string Maker { get; set; }
        public string ImgUrl { get; set; }

    }
}