using AutoMapper;
using Mango.Services.ShoppingCartApi.DbContexts;
using Mango.Services.ShoppingCartApi.Model;
using Mango.Services.ShoppingCartApi.Model.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartApi.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public CartRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await this.dbContext.CartHeaders.FirstOrDefaultAsync(i => i.UserId == userId);
            if (cartHeaderFromDb == null)
            {
                this.dbContext.CartDetails
                    .RemoveRange(this.dbContext.CartDetails
                        .Where(i => i.CartHeaderId == cartHeaderFromDb.CartHeaderId));
                this.dbContext.CartHeaders.Remove(cartHeaderFromDb);
                await this.dbContext.SaveChangesAsync();
                return true;
            }else
            {
                return false;
            }
        }

        public async Task<CartDto> CreateUpdate(CartDto cartDto)
        {
            Cart cart = this.mapper.Map<Cart>(cartDto);
            Product product = await this.dbContext.Products.FirstOrDefaultAsync(i => i.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);
            if (product == null)
            {
                this.dbContext.Products.Add(cart.CartDetails.FirstOrDefault()?.Product);
                await this.dbContext.SaveChangesAsync();
            }

            var headerFromDb = await this.dbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(i => i.UserId == cartDto.CartHeader.UserId);
            if (headerFromDb == null)
            {
                this.dbContext.CartHeaders.Add(cart.CartHeader);
                await this.dbContext.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                this.dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await this.dbContext.SaveChangesAsync();
            }
            else
            {
                var cartDetailsFromDb = await this.dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(i => i.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                i.CartHeaderId == headerFromDb.CartHeaderId);

                if (cartDetailsFromDb == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = headerFromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    this.dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await this.dbContext.SaveChangesAsync();
                }
                else
                {
                    // Update the count:
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDb.Count;
                    this.dbContext.Update(cart.CartDetails.FirstOrDefault());
                    await this.dbContext.SaveChangesAsync();
                }
            }
            return this.mapper.Map<CartDto>(cart);
            // if product exists in database. If not, create it.
            // 
            // check if is null
            // create header and details

            // if header is not null
            // check if details has the same product
            // if it has then update the count
            // else create details

        }

        public async Task<CartDto> GetByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await dbContext.CartHeaders.FirstOrDefaultAsync(i => i.UserId == userId)
            };
            cart.CartDetails = dbContext.CartDetails.Where(i => i.CartHeaderId == cart.CartHeader.CartHeaderId).Include(u => u.Product);
            return this.mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                CartDetails details = await this.dbContext.CartDetails
                    .FirstOrDefaultAsync(i => i.CartDetailsId == cartDetailsId);
                int totalCount = this.dbContext.CartDetails.Where(i => i.CartHeaderId == details.CartHeaderId).Count();
                this.dbContext.CartDetails.Remove(details);

                if (totalCount == 1)
                {
                    // last item in the cart removed, delete header:
                    var cartHeaderRemoved = await this.dbContext.CartHeaders
                        .FirstOrDefaultAsync(i => i.CartHeaderId == details.CartHeaderId);
                    this.dbContext.CartHeaders.Remove(cartHeaderRemoved);
                }
                await this.dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception error)
            {
                // Log the error;
                return false;
            }
        }
    }
}
