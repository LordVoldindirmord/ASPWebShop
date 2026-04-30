using Microsoft.EntityFrameworkCore;
using WebShop.DAL.Context;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;

namespace WebShop.DAL.Repositories
{
    /// <summary>
    /// Репозиторий для работы с продавцами
    /// </summary>
    public class SellerRepository : BaseRepository<Seller>, ISellerRepository
    {
        public SellerRepository(WebShopDbContext context) : base(context) { }

        /// <summary>
        /// Получить продавца по ID пользователя (связь один-к-одному)
        /// </summary>
        public async Task<Seller?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Userid == userId);
        }

        /// <summary>
        /// Получить продавца со всеми его товарами
        /// </summary>
        public async Task<Seller?> GetSellerWithProductsAsync(int sellerId)
        {
            return await _dbSet
                .Include(s => s.User)  // Базовая информация (имя, email)
                .Include(s => s.Products.Where(p => p.Isactive == true))  // Только активные товары
                    .ThenInclude(p => p.Category)  // Категория товара
                .Include(s => s.Products)
                    .ThenInclude(p => p.Productimages)  // Изображения товаров
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == sellerId);
        }

        /// <summary>
        /// Проверить, является ли пользователь продавцом
        /// </summary>
        public async Task<bool> IsUserSellerAsync(int userId)
        {
            return await _dbSet.AnyAsync(s => s.Userid == userId);
        }
    }
}
