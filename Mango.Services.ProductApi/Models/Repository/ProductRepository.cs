using AutoMapper;
using Mango.Services.ProductApi.DbContext;
using Mango.Services.ProductApi.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductApi.Models.Repository
{
    public class ProductRepository : IProductRepository
    {
        #region Private Fields

        private readonly ApplicationDbContext dbContext;

        private readonly IMapper mapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="dbContext">Db Context instance</param>
        /// <param name="mapper">Type mapper</param>
        public ProductRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        #endregion

        public async Task<ProductDto> CreateUpdateProducct(ProductDto product)
        {
            Action<ProductDto, Product> assignFields = (dto, proxy) =>
            {

                proxy.Description = dto.Description;
                proxy.Price = dto.Price;
                proxy.Name = dto.Name;
                proxy.CategoryName = dto.CategoryName;
                proxy.ImageUrl = dto.ImageUrl;
            };
            try
            {
                Product dbProduct = await this.dbContext.Products.FirstOrDefaultAsync(item => item.ProductId == product.ProductId);
                if(dbProduct == null)
                {
                    dbProduct = new Product();
                    assignFields(product, dbProduct);
                    await this.dbContext.Products.AddAsync(dbProduct);
                }
                else
                {
                    assignFields(product, dbProduct);
                    this.dbContext.Products.Update(dbProduct);
                }
                this.dbContext.SaveChanges();
                return this.mapper.Map<ProductDto>(dbProduct);
            }
            catch
            {
                return new ProductDto();
            }
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                Product product = await this.dbContext.Products.FirstOrDefaultAsync(item => item.ProductId == productId);
                if (product != null)
                {
                    this.dbContext.Products.Remove(product);
                    await this.dbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public async Task<ProductDto> GetProductById(int productId)
        {
            Product product = await this.dbContext.Products.FirstOrDefaultAsync(item => item.ProductId == productId);
            return this.mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            List<Product> list = await this.dbContext.Products.ToListAsync();
            return this.mapper.Map<IEnumerable<ProductDto>>(list);
        }
    }
}
