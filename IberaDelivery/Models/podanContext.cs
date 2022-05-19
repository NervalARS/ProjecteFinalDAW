using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IberaDelivery.Models
{
    public partial class podanContext : DbContext
    {
        public podanContext()
        {
        }

        public podanContext(DbContextOptions<podanContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<CreditCard> CreditCards { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<LnOrder> LnOrders { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Shipment> Shipments { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Valoration> Valorations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name")
                    .UseCollation("Modern_Spanish_CI_AS");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Contens)
                    .HasColumnType("text")
                    .HasColumnName("contens")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("comments_ProductFK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("comments_User_FK");
            });

            modelBuilder.Entity<CreditCard>(entity =>
            {
                entity.ToTable("credit_cards");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CardNumber)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("card_number")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Cardholder)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("cardholder")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CreditCards)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("credit_cards_FK");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("image");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Image1)
                    .HasColumnType("image")
                    .HasColumnName("image");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("image_Product_FK");
            });

            modelBuilder.Entity<LnOrder>(entity =>
            {
                entity.HasKey(e => new { e.NumOrder, e.NumLine })
                    .HasName("ln_order_PK");

                entity.ToTable("ln_order");

                entity.Property(e => e.NumOrder).HasColumnName("num_order");

                entity.Property(e => e.NumLine)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("num_line");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.RefProduct).HasColumnName("ref_product");

                entity.Property(e => e.TotalImport)
                    .HasColumnType("decimal(38, 2)")
                    .HasColumnName("total_import");

                entity.HasOne(d => d.NumOrderNavigation)
                    .WithMany(p => p.LnOrders)
                    .HasForeignKey(d => d.NumOrder)
                    .HasConstraintName("ln_order_Order_FK");

                entity.HasOne(d => d.RefProductNavigation)
                    .WithMany(p => p.LnOrders)
                    .HasForeignKey(d => d.RefProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ln_order_Product_FK");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("order");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreditCardId).HasColumnName("credit_card_id");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.Import)
                    .HasColumnType("decimal(38, 2)")
                    .HasColumnName("import");

                entity.Property(e => e.ShipmentId).HasColumnName("shipment_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.CreditCard)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CreditCardId)
                    .HasConstraintName("order_credit_card_FK");

                entity.HasOne(d => d.Shipment)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ShipmentId)
                    .HasConstraintName("order_FK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("order_FK_1");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Iva)
                    .HasColumnType("decimal(38, 2)")
                    .HasColumnName("iva");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(38, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.ProviderId).HasColumnName("provider_id");

                entity.Property(e => e.Stock).HasColumnName("stock");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("product_Category_FK");

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProviderId)
                    .HasConstraintName("product_user_FK");
            });

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.ToTable("shipment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("address")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.City)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("city")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Country)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("country")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("postal_code")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Shipments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("adress_FK");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Activate).HasColumnName("activate");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("email")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("first_name")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("last_name")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("password")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Rol).HasColumnName("rol");

                entity.Property(e => e.Token)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("token")
                    .UseCollation("Modern_Spanish_CI_AS");
            });

            modelBuilder.Entity<Valoration>(entity =>
            {
                entity.ToTable("valoration");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Valorations)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("valoration_product_FK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Valorations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("valoration_user_FK");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
