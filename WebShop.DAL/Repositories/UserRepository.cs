using Microsoft.EntityFrameworkCore;
using WebShop.DAL.Context;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;
using WebShop.Domain.Enum;

namespace WebShop.DAL.Repositories
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(WebShopDbContext context) : base(context) { }

        /// <summary>
        /// Найти пользователя по Email
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Проверить, занят ли Email другим пользователем
        /// </summary>
        public async Task<bool> IsEmailTakenAsync(string email, int? excludeUserId = null)
        {
            // Если передан ID для исключения (при обновлении профиля)
            if (excludeUserId.HasValue)
            {
                return await _dbSet
                    .AnyAsync(u => u.Email == email && u.Id != excludeUserId.Value);
            }

            // Просто проверяем существование
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Получить всех пользователей с определённой ролью
        /// </summary>
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        /// <summary>
        /// Получить пользователя с его заказами
        /// </summary>
        public async Task<User?> GetUserWithOrdersAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Orders)
                    .ThenInclude(o => o.Orderitems)  // Состав заказов
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Получить пользователя с корзиной и товарами в ней
        /// </summary>
        public async Task<User?> GetUserWithCartAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Cartitems)
                    .ThenInclude(c => c.Product)  // Товары в корзине
                        .ThenInclude(p => p.Category)  // Категории товаров
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
