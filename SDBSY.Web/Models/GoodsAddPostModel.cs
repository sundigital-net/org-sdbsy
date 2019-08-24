using System.ComponentModel.DataAnnotations;

namespace SDBSY.Web.Models
{
    public class GoodsAddPostModel
    {
        public long Id { get; set; }
        public long GoodsTypeId { get; set; }

        [Required(ErrorMessage = "名称必填")]
        public string Name { get; set; }
        [Required(ErrorMessage ="计量单位必填")]
        public string Unit { get; set; }
        [Required(ErrorMessage ="规格必填")]
        public string Format { get; set; }
        public string Seller { get; set; }
        public string Maker { get; set; }
        public string ImgUrl { get; set; }

    }
}