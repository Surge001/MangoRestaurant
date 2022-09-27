namespace Mango.Services.ShoppingCartApi.Model
{
    #region Using
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    #endregion

    /// <summary>
    /// Declares members of Product entity which correspond to Product table in the database model.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Primary key of Product table
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] // <= Key but not automatically generated.
        public int ProductId { get; set; }

        /// <summary>
        /// Name of the product
        /// </summary>
        [Required]
        public string Name { get; set; }

        [Range(1, 1000, ErrorMessage = "Price must be between 1 and 1000")]
        public double Price { get; set; }

        public string Description { get; set; }

        public string CategoryName { get; set; }

        public string ImageUrl { get; set; }
    }
}
