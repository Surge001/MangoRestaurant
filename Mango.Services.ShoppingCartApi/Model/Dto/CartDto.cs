
using System.Collections.Generic;

namespace Mango.Services.ShoppingCartApi.Model.Dto
{
    public class CartDto
    {
        public CartHeaderDto CartHeader { get; set; }

        public IEnumerable<CartDetailsDto> CartDetails { get; set; }


    }
}
