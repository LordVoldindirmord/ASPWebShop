using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebShop.DAL.Context;
using WebShop.DAL.Interfaces;
using WebShop.DAL.Repositories;
using WebShop.Service.Implementations;
using WebShop.Service.Interfaces;

namespace ASPWebShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region cookie

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/User/Login";                  // Куда редиректить, если не авторизован
                options.LogoutPath = "/User/Logout";                // Путь для выхода
                options.AccessDeniedPath = "/User/Login";    // Доступ запрещён
                options.Cookie.Name = "WebShopAuth";                // Имя куки
                options.ExpireTimeSpan = TimeSpan.FromDays(7);      // Время жизни
                options.SlidingExpiration = true;                   // Продлевать при активности
            });

            #endregion

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            #region Регистрация DAL

            // Строка подключения из appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Регистрируем DbContext с PostgreSQL
            builder.Services.AddDbContext<WebShopDbContext>(options => options.UseNpgsql(connectionString));

            // Регистрируем репозитории
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
            builder.Services.AddScoped<ISellerRepository, SellerRepository>();
            builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();

            #endregion

            #region Регистрация Service

            // Регистрация Service

            // Регистрируем сервисы
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ISellerService, SellerService>();

            // Регистрируем ImageService (нужен доступ к wwwroot для сохранения картинок)
            builder.Services.AddScoped<ImageService>(provider =>
            {
                var env = provider.GetRequiredService<IWebHostEnvironment>();
                return new ImageService(env.WebRootPath);
            });

            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseMiddleware<UserRoleMiddleware>();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}