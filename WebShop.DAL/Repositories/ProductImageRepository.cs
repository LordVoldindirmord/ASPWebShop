using Microsoft.EntityFrameworkCore;
using WebShop.DAL.Context;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;

namespace WebShop.DAL.Repositories
{
    /// <summary>
    /// Репозиторий для работы с изображениями товаров
    /// </summary>
    public class ProductImageRepository : BaseRepository<Productimage>, IProductImageRepository
    {
        public ProductImageRepository(WebShopDbContext context) : base(context) { }

        /// <summary>
        /// Получить все изображения товара, отсортированные по порядку
        /// </summary>
        public async Task<IEnumerable<Productimage>> GetImagesByProductAsync(int productId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(img => img.Productid == productId)
                .OrderBy(img => img.Sortorder)  // По порядку сортировки
                .ToListAsync();
        }

        /// <summary>
        /// Обновить порядок изображений для товара
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <param name="imageIdsWithOrder">Словарь: ID изображения -> новый порядок</param>
        public async Task UpdateImagesOrderAsync(int productId, Dictionary<int, int> imageIdsWithOrder)
        {
            // Получаем все изображения товара
            var images = await _dbSet
                .Where(img => img.Productid == productId && imageIdsWithOrder.Keys.Contains(img.Id))
                .ToListAsync();

            // Обновляем порядок для каждого изображения
            foreach (var image in images)
            {
                if (imageIdsWithOrder.TryGetValue(image.Id, out int newOrder))
                {
                    image.Sortorder = newOrder;
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Получить количество изображений товара
        /// </summary>
        public async Task<int> GetImageCountByProductAsync(int productId)
        {
            return await _dbSet
                .CountAsync(img => img.Productid == productId);
        }
    }
}
