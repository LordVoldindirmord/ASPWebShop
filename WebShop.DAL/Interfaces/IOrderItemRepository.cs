using WebShop.Domain.Entity;

namespace WebShop.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с позициями заказа (OrderItems)
    /// </summary>
    public interface IOrderItemRepository : IBaseRepository<Orderitem>
    {
        /// <summary>
        /// Получить все позиции конкретного заказа
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <returns>Коллекция позиций заказа с информацией о товарах</returns>
        Task<IEnumerable<Orderitem>> GetItemsByOrderAsync(int orderId);

        /// <summary>
        /// Получить позицию заказа с информацией о товаре
        /// </summary>
        /// <param name="orderItemId">ID позиции заказа</param>
        /// <returns>Позиция заказа с загруженным товаром (если товар ещё существует)</returns>
        Task<Orderitem?> GetOrderItemWithProductAsync(int orderItemId);
    }
}
