using Mango.Services.ShoppingCartApi.Model.Dto;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartApi.Repository
{
    public interface ICartRepository
    {
        Task<CartDto> GetByUserId(string userId);
        Task<CartDto> CreateUpdate(CartDto cart);
        Task<bool> RemoveFromCart(int cartDetailsId);
        Task<bool> ClearCart(string userId);
    }
}
