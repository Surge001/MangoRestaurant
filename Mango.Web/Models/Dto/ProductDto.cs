using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.Dto
{
    public class ProductDto
    {
        /// <summary>
        /// Primary key of Product table
        /// </summary>
        [Key]
        public int ProductId { get; set; }

        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public string CategoryName { get; set; }

        public string ImageUrl { get; set; }
    }
}
