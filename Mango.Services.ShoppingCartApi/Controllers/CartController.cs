using Mango.MessageBus;
using Mango.Services.ShoppingCartApi.Messages;
using Mango.Services.ShoppingCartApi.Model.Dto;
using Mango.Services.ShoppingCartApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartApi.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : Controller
    {
        #region Private Fields

        private readonly ICartRepository cartRepository;
        private readonly IMessageBus messageBus;
        private readonly ICouponRepository couponRepository;
        protected ResponseDto responseDto;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CartController"/> class.
        /// </summary>
        /// <param name="cartRepository">Repository instance</param>
        /// <param name="messageBus">Message Bus instance</param>
        public CartController(ICartRepository cartRepository, IMessageBus messageBus, ICouponRepository couponRepository)
        {
            this.cartRepository = cartRepository;
            this.messageBus = messageBus;
            this.couponRepository = couponRepository;
            this.responseDto = new();
        }

        #endregion

        [HttpGet("Get/{userId}")]
        public async Task<object> Get(string userId)
        {
            try
            {
                CartDto result = await this.cartRepository.GetByUserId(userId);
                this.responseDto.Result = result;
            }
            catch (Exception error)
            {
                this.responseDto.IsSuccess = false;
                this.responseDto.ErrorMessages = new List<string>() { error.Message };

            }
            return this.responseDto;
        }

        [HttpPost("Add")]
        public async Task<object> Add(CartDto cartDto)
        {
            try
            {
                CartDto result = await this.cartRepository.CreateUpdate(cartDto);
                this.responseDto.Result = result;
            }
            catch (Exception error)
            {
                this.responseDto.IsSuccess = false;
                this.responseDto.ErrorMessages = new List<string>() { error.Message };

            }
            return this.responseDto;
        }

        [HttpPut("Update")]
        public async Task<object> Update(CartDto cartDto)
        {
            try
            {
                CartDto result = await this.cartRepository.CreateUpdate(cartDto);
                this.responseDto.Result = result;
            }
            catch (Exception error)
            {
                this.responseDto.IsSuccess = false;
                this.responseDto.ErrorMessages = new List<string>() { error.Message };

            }
            return this.responseDto;
        }



        [HttpPost("Remove")]
        public async Task<object> Remove([FromBody] int cartDetailsId)
        {
            try
            {
                bool result = await this.cartRepository.RemoveFromCart(cartDetailsId);
                this.responseDto.Result = result;
            }
            catch (Exception error)
            {
                this.responseDto.IsSuccess = false;
                this.responseDto.ErrorMessages = new List<string>() { error.Message };

            }
            return this.responseDto;
        }


        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                bool result = await this.cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                this.responseDto.Result = result;
            }
            catch (Exception error)
            {
                this.responseDto.IsSuccess = false;
                this.responseDto.ErrorMessages = new List<string>() { error.Message };

            }
            return this.responseDto;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                bool result = await this.cartRepository.RemoveCoupon(userId);
                this.responseDto.Result = result;
            }
            catch (Exception error)
            {
                this.responseDto.IsSuccess = false;
                this.responseDto.ErrorMessages = new List<string>() { error.Message };

            }
            return this.responseDto;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeaderDto)
        {
            try
            {
                CartDto cartDto = await this.cartRepository.GetByUserId(checkoutHeaderDto.UserId);

                if(cartDto == null)
                {
                    return BadRequest();
                }

                if (!string.IsNullOrWhiteSpace(checkoutHeaderDto.CouponCode))
                {
                    CouponDto coupon = await this.couponRepository.Get(checkoutHeaderDto.CouponCode);
                    if(checkoutHeaderDto.DiscountTotal != coupon.DiscountAmount)
                    {
                        responseDto.IsSuccess = false;
                        responseDto.ErrorMessages = new List<string>() { "Coupon amount changed." };
                        responseDto.DisplayMessage = "Coupon amount changed.";
                        return responseDto;
                    }
                }

                checkoutHeaderDto.CartDetails = cartDto.CartDetails;

                // Logic to add message for Service Bus to process the order.
                await this.messageBus.PublishMessage(checkoutHeaderDto, "checkoutmessagetopic");
                //await this.cartRepository.ClearCart(checkoutHeaderDto.UserId);
            }
            catch (Exception error)
            {
                this.responseDto.IsSuccess = false;
                this.responseDto.ErrorMessages = new List<string>() { error.Message };

            }
            return this.responseDto;
        }
    }
}
