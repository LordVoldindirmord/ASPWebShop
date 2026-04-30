using Microsoft.EntityFrameworkCore;
using WebShop.DAL.Context;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;

namespace WebShop.DAL.Repositories
{
    /// <summary>
    /// Репозиторий для работы с товарами
    /// </summary>
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(WebShopDbContext context) : base(context) { }

        /// <summary>
        /// Получить товары по категории
        /// </summary>
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.Categoryid == categoryId && p.Isactive == true)
                .Include(p => p.Seller)
                    .ThenInclude(s => s.User)  // Информация о продавце
                .ToListAsync();
        }

        /// <summary>
        /// Получить товары конкретного продавца
        /// </summary>
        public async Task<IEnumerable<Product>> GetProductsBySellerAsync(int sellerId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.Sellerid == sellerId)
                .Include(p => p.Category)  // Категория товара
                .Include(p => p.Productimages)  // Изображения товара
                .ToListAsync();
        }

        /// <summary>
        /// Получить только активные товары (для каталога)
        /// </summary>
        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.Isactive == true)
                .Include(p => p.Category)  // Название категории
                .Include(p => p.Seller)
                    .ThenInclude(s => s.User)  // Имя продавца
                .ToListAsync();
        }

        /// <summary>
        /// Получить товар с полной информацией для страницы "Подробнее"
        /// </summary>
        public async Task<Product?> GetProductWithDetailsAsync(int productId)
        {
            return await _dbSet
                .Include(p => p.Category)  // Категория
                .Include(p => p.Seller)  // Продавец
                    .ThenInclude(s => s.User)  // Данные пользователя-продавца
                .Include(p => p.Productimages.OrderBy(img => img.Sortorder))  // Все изображения по порядку
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        /// <summary>
        /// Поиск товаров по названию (регистронезависимый)
        /// </summary>
        public async Task<IEnumerable<Product>> SearchProductsByNameAsync(string searchTerm)
        {
            // EF Core преобразует ToLower() и Contains() в SQL ILIKE или LOWER()
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(searchTerm.ToLower())
                         && p.Isactive == true)  // Ищем только активные
                .Include(p => p.Category)
                .ToListAsync();
        }
    }
}
