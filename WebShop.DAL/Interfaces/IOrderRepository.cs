using WebShop.Domain.Entity;
using WebShop.Domain.Enum;

namespace WebShop.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с заказами
    /// </summary>
    public interface IOrderRepository : IBaseRepository<Order>
    {
        /// <summary>
        /// Получить все заказы конкретного пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Коллекция заказов пользователя (отсортированы по дате, новые первыми)</returns>
        Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);

        /// <summary>
        /// Получить заказ с полным составом товаров
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <returns>Заказ со списком товаров и информацией о покупателе</returns>
        Task<Order?> GetOrderWithItemsAsync(int orderId);

        /// <summary>
        /// Получить все заказы с определённым статусом
        /// </summary>
        /// <param name="status">Статус заказа (Pending, Processing, Shipped, Delivered, Cancelled)</param>
        /// <returns>Коллекция заказов с указанным статусом</returns>
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);

        /// <summary>
        /// Получить заказы, в которых есть товары конкретного продавца
        /// Полезно для продавца - видеть кто заказал его товары
        /// </summary>
        /// <param name="sellerId">ID продавца</param>
        /// <returns>Коллекция заказов с товарами продавца</returns>
        Task<IEnumerable<Order>> GetOrdersBySellerAsync(int sellerId);

        /// <summary>
        /// Получить все заказы с подгруженными позициями и пользователями (для администратора)
        /// </summary>
        Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
    }
}
