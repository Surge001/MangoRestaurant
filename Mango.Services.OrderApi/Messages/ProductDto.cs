namespace Mango.Services.OrderApi.Messages
{
    #region Using
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    #endregion

    /// <summary>
    /// Declares members of Product entity which correspond to Product table in the database model.
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Primary key of Product table
        /// </summary>
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
