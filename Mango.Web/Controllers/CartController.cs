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
        private readonly ICouponService couponService;

        #endregion

        #region Constructors

        public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
        {
            this.productService = productService;
            this.cartService = cartService;
            this.couponService = couponService;
        }

        #endregion

        public async Task<IActionResult> CartIndex()
        {
            return View(await this.LoadCartDtoForUser());
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            string accessToken = await this.HttpContext.GetTokenAsync("access_token");
            ResponseDto result = await this.cartService.ApplyCouponAsync<ResponseDto>(cartDto, accessToken);

            if (result != null && result.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            string accessToken = await this.HttpContext.GetTokenAsync("access_token");
            //string userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            ResponseDto result = await this.cartService.RemoveCouponAsync<ResponseDto>(cartDto.CartHeader.UserId, accessToken);

            if (result != null && result.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            string userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            string accessToken = await this.HttpContext.GetTokenAsync("access_token");
            ResponseDto response = await this.cartService.RemoveFromCartAsync<ResponseDto>(cartDetailsId, accessToken);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoForUser());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {   
                string accessToken = await this.HttpContext.GetTokenAsync("access_token");
                ResponseDto response = await this.cartService.CartCheckout<ResponseDto>(cartDto.CartHeader, accessToken);

                if (!response.IsSuccess)
                {
                    TempData["Error"] = response.DisplayMessage;
                    return RedirectToAction(nameof(Checkout));
                }
                return RedirectToAction(nameof(Confirmation));
            }
            catch (Exception error)
            {
                return View(cartDto);
            }
        }

        public async Task<IActionResult> Confirmation()
        {
            return View(LoadCartDtoForUser());
        }

        #region Private Methods

        private async Task<CartDto> LoadCartDtoForUser()
        {
            string userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            string accessToken = await this.HttpContext.GetTokenAsync("access_token");
            ResponseDto response = await this.cartService.GetByUserIdAsync<ResponseDto>(userId, accessToken);
            CartDto cartDto = new();
            if (response != null && response.IsSuccess)
            {
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }
            if (cartDto.CartHeader != null)
            {
                CouponDto coupon = null;
                if (!string.IsNullOrWhiteSpace(cartDto.CartHeader.CouponCode))
                {
                    ResponseDto couponResponse = await this.couponService.GetCoupon<ResponseDto>(cartDto?.CartHeader?.CouponCode, accessToken);
                    if (couponResponse != null && couponResponse.IsSuccess)
                    {
                        coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(couponResponse.Result));

                        if (coupon != null && coupon.DiscountAmount > 0)
                        {
                            cartDto.CartHeader.DiscountTotal = coupon.DiscountAmount;
                        }
                    }
                }
                foreach (var item in cartDto.CartDetails)
                {
                    cartDto.CartHeader.OrderTotal += item.Product.Price * item.Count;
                }

                if (coupon != null && coupon.DiscountAmount > 0)
                {
                    cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.OrderTotal > coupon.DiscountAmount ? coupon.DiscountAmount : cartDto.CartHeader.OrderTotal;
                }
            }
            return cartDto;
        }
        #endregion
    }
}
