using WebShop.Domain.Entity;

namespace WebShop.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с изображениями товаров
    /// </summary>
    public interface IProductImageRepository : IBaseRepository<Productimage>
    {
        /// <summary>
        /// Получить все изображения конкретного товара
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <returns>Коллекция изображений, отсортированная по SortOrder</returns>
        Task<IEnumerable<Productimage>> GetImagesByProductAsync(int productId);

        /// <summary>
        /// Установить порядок изображений для товара
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <param name="imageIdsWithOrder">Словарь (ID изображения -> новый порядок)</param>
        Task UpdateImagesOrderAsync(int productId, Dictionary<int, int> imageIdsWithOrder);

        /// <summary>
        /// Получить количество изображений товара
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <returns>Количество изображений</returns>
        Task<int> GetImageCountByProductAsync(int productId);
    }
}
