using WebShop.Domain.Entity;

namespace WebShop.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с категориями товаров
    /// </summary>
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        /// <summary>
        /// Получить категорию вместе со всеми активными товарами в ней
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Категория с коллекцией товаров</returns>
        Task<Category?> GetCategoryWithProductsAsync(int categoryId);

        /// <summary>
        /// Проверить, существует ли категория с таким названием
        /// Используется при создании и обновлении категории
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <param name="excludeCategoryId">ID категории для исключения (при обновлении)</param>
        /// <returns>true - название занято, false - свободно</returns>
        Task<bool> IsCategoryNameTakenAsync(string name, int? excludeCategoryId = null);

        Task<IEnumerable<Category>> GetAllWithProductCountAsync();
    }
}
