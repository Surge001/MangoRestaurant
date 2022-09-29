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
        private readonly ICartRepository cartRepository;
        protected ResponseDto responseDto;
        public CartController(ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
            this.responseDto = new();
        }

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
    }
}
