namespace Server
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BSKdbContext : DbContext
    {
        public BSKdbContext()
            : base("name=BSKdbContext")
        {
        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Producer> Producers { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SpecificArticle> SpecificArticles { get; set; }
        public virtual DbSet<TablePermission> TablePermissions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Article>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Article>()
                .HasMany(e => e.SpecificArticles)
                .WithRequired(e => e.Article)
                .HasForeignKey(e => e.article_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.surname)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Sales)
                .WithRequired(e => e.Customer)
                .HasForeignKey(e => e.customer_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Producer>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Producer>()
                .Property(e => e.address)
                .IsUnicode(false);

            modelBuilder.Entity<Producer>()
                .HasMany(e => e.Articles)
                .WithRequired(e => e.Producer)
                .HasForeignKey(e => e.producer_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Sale>()
                .Property(e => e.netPrice)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Sale>()
                .Property(e => e.grossPrice)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Sale>()
                .HasMany(e => e.SpecificArticles)
                .WithRequired(e => e.Sale)
                .HasForeignKey(e => e.sale_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SpecificArticle>()
                .Property(e => e.price)
                .HasPrecision(5, 2);

            modelBuilder.Entity<SpecificArticle>()
                .Property(e => e.article_id)
                .IsUnicode(false);

            modelBuilder.Entity<TablePermission>()
                .Property(e => e.nameOfTable)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.login)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.salt)
                .IsUnicode(false);
        }
    }
}
