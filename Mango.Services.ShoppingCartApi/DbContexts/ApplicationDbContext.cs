﻿using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartApi.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">Options</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #endregion

        ///// <summary>
        ///// Gets or sets Products
        ///// </summary>
        //public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}