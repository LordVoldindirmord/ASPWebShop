using Microsoft.EntityFrameworkCore;
using WebShop.DAL.Context;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;

namespace WebShop.DAL.Repositories
{
    /// <summary>
    /// Репозиторий для работы с корзиной пользователя
    /// </summary>
    public class CartItemRepository : BaseRepository<Cartitem>, ICartItemRepository
    {
        public CartItemRepository(WebShopDbContext context) : base(context) { }

        /// <summary>
        /// Получить все товары в корзине пользователя с деталями
        /// </summary>
        public async Task<IEnumerable<Cartitem>> GetCartItemsByUserAsync(int userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.Userid == userId)
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Include(c => c.Product)
                .ThenInclude(p => p.Seller)
                .ToListAsync();
        }

        /// <summary>
        /// Получить позицию корзины с информацией о товаре
        /// </summary>
        public async Task<Cartitem?> GetCartItemWithProductAsync(int cartItemId)
        {
            return await _dbSet
                .Include(c => c.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == cartItemId);
        }

        /// <summary>
        /// Полностью очистить корзину пользователя (после оформления заказа)
        /// </summary>
        public async Task ClearCartAsync(int userId)
        {
            // Находим все позиции в корзине пользователя
            var userCartItems = await _dbSet
                .Where(c => c.Userid == userId)
                .ToListAsync();

            // Удаляем их
            _dbSet.RemoveRange(userCartItems);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Проверить, есть ли товар уже в корзине пользователя
        /// </summary>
        public async Task<bool> IsProductInCartAsync(int userId, int productId)
        {
            return await _dbSet
                .AnyAsync(c => c.Userid == userId && c.Productid == productId);
        }
    }
}
