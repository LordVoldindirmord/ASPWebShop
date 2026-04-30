using Microsoft.EntityFrameworkCore;
using WebShop.DAL.Context;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;
using WebShop.Domain.Enum;

namespace WebShop.DAL.Repositories
{
    /// <summary>
    /// Репозиторий для работы с заказами
    /// </summary>
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(WebShopDbContext context) : base(context) { }

        /// <summary>
        /// Получить все заказы пользователя (сначала новые)
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.Userid == userId)
                .Include(o => o.Orderitems)  // Состав заказа
                    .ThenInclude(oi => oi.Product)  // Товар в позиции заказа
                .OrderByDescending(o => o.Orderdate)  // Новые сверху
                .ToListAsync();
        }

        /// <summary>
        /// Получить заказ с полным составом и информацией о покупателе
        /// </summary>
        public async Task<Order?> GetOrderWithItemsAsync(int orderId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Orderitems)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        /// <summary>
        /// Получить заказы по статусу (для админ-панели)
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.Status == status)
                .Include(o => o.Orderitems)
                .Include(o => o.User)  // Кто заказал
                .OrderBy(o => o.Orderdate)  // Сначала старые (ожидают обработки дольше)
                .ToListAsync();
        }

        /// <summary>
        /// Получить заказы, содержащие товары конкретного продавца
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersBySellerAsync(int sellerId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.Orderitems.Any(oi =>
                    oi.Product != null && oi.Product.Sellerid == sellerId))
                .Include(o => o.Orderitems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)  // Информация о покупателе
                .OrderByDescending(o => o.Orderdate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Orderitems)
                .Include(o => o.User)
                .OrderByDescending(o => o.Orderdate)
                .ToListAsync();
        }
    }
}