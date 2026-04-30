using WebShop.Domain.Entity;

namespace WebShop.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с корзиной пользователя
    /// </summary>
    public interface ICartItemRepository : IBaseRepository<Cartitem>
    {
        /// <summary>
        /// Получить все товары в корзине пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Коллекция товаров в корзине с информацией о продуктах</returns>
        Task<IEnumerable<Cartitem>> GetCartItemsByUserAsync(int userId);

        /// <summary>
        /// Получить позицию корзины с информацией о товаре
        /// </summary>
        /// <param name="cartItemId">ID позиции в корзине</param>
        /// <returns>Позиция корзины с загруженным товаром</returns>
        Task<Cartitem?> GetCartItemWithProductAsync(int cartItemId);

        /// <summary>
        /// Полностью очистить корзину пользователя
        /// Используется после оформления заказа
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        Task ClearCartAsync(int userId);

        /// <summary>
        /// Проверить, есть ли конкретный товар в корзине пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="productId">ID товара</param>
        /// <returns>true - товар в корзине, false - нет</returns>
        Task<bool> IsProductInCartAsync(int userId, int productId);
    }
}
