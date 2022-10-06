using Mango.Web.Models.Dto;

namespace Mango.Web.Services.IServices
{
    public interface ICartService
    {
        Task<T> GetByUserIdAsync<T>(string userId, string token = null);
        Task<T> AddToCartAsync<T>(CartDto cartDto, string token = null);
        Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null);
        Task<T> RemoveFromCartAsync<T>(int cartDetailsId, string token = null);
        Task<T> ApplyCouponAsync<T>(CartDto cartDto, string token = null);
        Task<T> RemoveCouponAsync<T>(string userId, string token = null);

        /// <summary>
        /// Handles cart checkout sending information to Service Bus
        /// </summary>
        /// <typeparam name="T">Type of response</typeparam>
        /// <param name="cartHeader">Cart header data</param>
        /// <param name="token">security token</param>
        /// <returns>Expected response object</returns>
        Task<T> CartCheckout<T>(CartHeaderDto cartHeader, string token = null);
    }
}
