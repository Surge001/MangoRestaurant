using System.ComponentModel.DataAnnotations;
using System.IO.Pipelines;

namespace Mango.Services.ShoppingCartApi.Model.Dto
{
    public class CartHeaderDto
    {
        public int CartHeaderId { get; set; }

        public string UserId { get; set; }
        public string CouponCode { get; set; }
    }
}
