using System.ComponentModel.DataAnnotations;
using System.IO.Pipelines;

namespace Mango.Services.ShoppingCartApi.Model
{
    public class CartHeader
    {
        [Key]
        public int CartHeaderId { get; set; }

        public string UserId { get; set; }
        public string CouponCode { get; set; }
    }
}
