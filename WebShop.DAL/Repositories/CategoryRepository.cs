using Microsoft.EntityFrameworkCore;
using WebShop.DAL.Context;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;

namespace WebShop.DAL.Repositories
{
    /// <summary>
    /// Репозиторий для работы с категориями товаров
    /// </summary>
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(WebShopDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetAllWithProductCountAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Products)
                .ToListAsync();
        }

        /// <summary>
        /// Получить категорию с активными товарами в ней
        /// </summary>
        public async Task<Category?> GetCategoryWithProductsAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Products.Where(p => p.Isactive == true))  // Только активные товары
                    .ThenInclude(p => p.Productimages)  // Изображения товаров
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        /// <summary>
        /// Проверить, существует ли категория с таким названием
        /// </summary>
        public async Task<bool> IsCategoryNameTakenAsync(string name, int? excludeCategoryId = null)
        {
            // При обновлении исключаем текущую категорию из проверки
            if (excludeCategoryId.HasValue)
            {
                return await _dbSet
                    .AnyAsync(c => c.Name == name && c.Id != excludeCategoryId.Value);
            }

            // При создании проверяем все категории
            return await _dbSet.AnyAsync(c => c.Name == name);
        }
    }
}
