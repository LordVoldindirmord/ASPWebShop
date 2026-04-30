using Microsoft.EntityFrameworkCore;
using WebShop.Domain.Entity;
using WebShop.Domain.Enum;

namespace WebShop.DAL.Context;

public partial class WebShopDbContext : DbContext
{
    public WebShopDbContext(DbContextOptions<WebShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cartitem> Cartitems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderitem> Orderitems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Productimage> Productimages { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum<OrderStatus>("order_status")
            .HasPostgresEnum<UserRole>("user_role");

        modelBuilder.Entity<Cartitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cartitems_pkey");

            entity.ToTable("cartitems");

            entity.HasIndex(e => e.Userid, "ix_cartitems_userid");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Addedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("addedat");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Product).WithMany(p => p.Cartitems)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("fk_cartitems_products");

            entity.HasOne(d => d.User).WithMany(p => p.Cartitems)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("fk_cartitems_users");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "categories_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.HasIndex(e => e.Userid, "ix_orders_userid");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Orderdate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("orderdate");
            entity.Property(e => e.Shippingaddress).HasColumnName("shippingaddress");
            entity.Property(e => e.Totalamount)
                .HasPrecision(18, 2)
                .HasColumnName("totalamount");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("fk_orders_users");
            
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("order_status")
                .HasDefaultValue(OrderStatus.Pending)
                .HasConversion<string>();
        });

        modelBuilder.Entity<Orderitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orderitems_pkey");

            entity.ToTable("orderitems");

            entity.HasIndex(e => e.Orderid, "ix_orderitems_orderid");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Price)
                .HasPrecision(18, 2)
                .HasColumnName("price");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Productname)
                .HasMaxLength(200)
                .HasColumnName("productname");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.Orderitems)
                .HasForeignKey(d => d.Orderid)
                .HasConstraintName("fk_orderitems_orders");

            entity.HasOne(d => d.Product).WithMany(p => p.Orderitems)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_orderitems_products");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products");

            entity.HasIndex(e => e.Categoryid, "ix_products_categoryid");

            entity.HasIndex(e => e.Sellerid, "ix_products_sellerid");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Mainimageurl)
                .HasMaxLength(500)
                .HasColumnName("mainimageurl");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(18, 2)
                .HasColumnName("price");
            entity.Property(e => e.Sellerid).HasColumnName("sellerid");
            entity.Property(e => e.Stockquantity).HasColumnName("stockquantity");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_products_categories");

            entity.HasOne(d => d.Seller).WithMany(p => p.Products)
                .HasForeignKey(d => d.Sellerid)
                .HasConstraintName("fk_products_sellers");
        });

        modelBuilder.Entity<Productimage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productimages_pkey");

            entity.ToTable("productimages");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Imageurl)
                .HasMaxLength(500)
                .HasColumnName("imageurl");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Sortorder)
                .HasDefaultValue(0)
                .HasColumnName("sortorder");

            entity.HasOne(d => d.Product).WithMany(p => p.Productimages)
                .HasForeignKey(d => d.Productid)
                .HasConstraintName("fk_productimages_products");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sellers_pkey");

            entity.ToTable("sellers");

            entity.HasIndex(e => e.Userid, "sellers_userid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Storename)
                .HasMaxLength(200)
                .HasColumnName("storename");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithOne(p => p.Seller)
                .HasForeignKey<Seller>(d => d.Userid)
                .HasConstraintName("fk_sellers_users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(255)
                .HasColumnName("passwordhash");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(20)
                .HasColumnName("phonenumber");

            entity.Property(e => e.Role)
                .HasColumnName("role")
                .HasDefaultValue(UserRole.Customer)
                .HasConversion<string>();
        });
    }
}
