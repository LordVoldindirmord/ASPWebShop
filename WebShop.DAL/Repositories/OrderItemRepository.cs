using Microsoft.EntityFrameworkCore;
using WebShop.DAL.Context;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;

namespace WebShop.DAL.Repositories
{
    /// <summary>
    /// Репозиторий для работы с позициями заказа
    /// </summary>
    public class OrderItemRepository : BaseRepository<Orderitem>, IOrderItemRepository
    {
        public OrderItemRepository(WebShopDbContext context) : base(context) { }

        /// <summary>
        /// Получить все позиции заказа с информацией о товарах
        /// </summary>
        public async Task<IEnumerable<Orderitem>> GetItemsByOrderAsync(int orderId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(oi => oi.Orderid == orderId)
                .Include(oi => oi.Product)  // Текущая информация о товаре
                    .ThenInclude(p => p.Seller)  // Продавец товара
                .ToListAsync();
        }

        /// <summary>
        /// Получить позицию заказа с товаром (если товар ещё существует)
        /// </summary>
        public async Task<Orderitem?> GetOrderItemWithProductAsync(int orderItemId)
        {
            return await _dbSet
                .Include(oi => oi.Product)  // Может быть null если товар удалён
                .AsNoTracking()
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId);
        }
    }
}
