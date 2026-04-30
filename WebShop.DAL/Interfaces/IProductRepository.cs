using WebShop.Domain.Entity;

namespace WebShop.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с товарами
    /// </summary>
    public interface IProductRepository : IBaseRepository<Product>
    {
        /// <summary>
        /// Получить все товары из определённой категории
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Коллекция товаров в категории</returns>
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);

        /// <summary>
        /// Получить все товары конкретного продавца
        /// </summary>
        /// <param name="sellerId">ID продавца</param>
        /// <returns>Коллекция товаров продавца</returns>
        Task<IEnumerable<Product>> GetProductsBySellerAsync(int sellerId);

        /// <summary>
        /// Получить только активные товары (отображаются в каталоге)
        /// </summary>
        /// <returns>Коллекция активных товаров с категориями</returns>
        Task<IEnumerable<Product>> GetActiveProductsAsync();

        /// <summary>
        /// Получить товар с полной информацией:
        /// категория, продавец, все изображения
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <returns>Товар со всеми связанными данными</returns>
        Task<Product?> GetProductWithDetailsAsync(int productId);

        /// <summary>
        /// Поиск товаров по названию (без учёта регистра)
        /// </summary>
        /// <param name="searchTerm">Поисковый запрос</param>
        /// <returns>Коллекция найденных товаров</returns>
        Task<IEnumerable<Product>> SearchProductsByNameAsync(string searchTerm);
    }
}
