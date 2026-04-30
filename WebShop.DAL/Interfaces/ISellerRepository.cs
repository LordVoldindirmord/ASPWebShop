using WebShop.Domain.Entity;

namespace WebShop.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с продавцами
    /// </summary>
    public interface ISellerRepository : IBaseRepository<Seller>
    {
        /// <summary>
        /// Получить продавца по ID пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Продавец или null, если пользователь не является продавцом</returns>
        Task<Seller?> GetByUserIdAsync(int userId);

        /// <summary>
        /// Получить продавца вместе с его товарами
        /// </summary>
        /// <param name="sellerId">ID продавца</param>
        /// <returns>Продавец с коллекцией активных товаров</returns>
        Task<Seller?> GetSellerWithProductsAsync(int sellerId);

        /// <summary>
        /// Проверить, является ли пользователь продавцом
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>true - пользователь продавец, false - нет</returns>
        Task<bool> IsUserSellerAsync(int userId);
    }
}
