using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        #region Private Fields

        private readonly IProductService productService;
        private readonly ICartService cartService;

        #endregion

        #region Constructors

        public CartController(IProductService productService, ICartService cartService)
        {
            this.productService = productService;
            this.cartService = cartService;
        }

        #endregion

        public async Task<IActionResult> CartIndex()
        {
            return View(await this.LoadCartDtoForUser());
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            string userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            string accessToken = await this.HttpContext.GetTokenAsync("access_token");
            ResponseDto response = await this.cartService.RemoveFromCartAsync<ResponseDto>(cartDetailsId, accessToken);

            if(response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }


        #region Private Methods

        private async Task<CartDto> LoadCartDtoForUser()
        {
            string userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            string accessToken = await this.HttpContext.GetTokenAsync("access_token");
            ResponseDto response =  await this.cartService.GetByUserIdAsync<ResponseDto>(userId, accessToken);
            CartDto cartDto = new();
            if(response !=null && response.IsSuccess)
            {
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }
            if(cartDto.CartHeader != null)
            {
                foreach(var item in cartDto.CartDetails)
                {
                    cartDto.CartHeader.OrderTotal += item.Product.Price * item.Count;
                }
            }
            return cartDto;
        }
        #endregion
    }
}
