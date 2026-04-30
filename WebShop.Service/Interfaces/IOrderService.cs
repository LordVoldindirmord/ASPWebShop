using System;
using System.Collections.Generic;
using System.Text;
using WebShop.Domain.Enum;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.OrderModel;

namespace WebShop.Service.Interfaces
{
    /// <summary>
    /// Сервис для работы с заказами: создание, просмотр, изменение статуса
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Создать заказ из товаров в корзине пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="model">Адрес доставки</param>
        /// <returns>ID созданного заказа; ошибка при неудаче</returns>
        Task<BaseResponse<int>> CreateOrderAsync(int userId, CreateOrderViewModel model);

        /// <summary>
        /// Получить все заказы текущего пользователя (от новых к старым)
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Коллекция заказов</returns>
        Task<BaseResponse<IEnumerable<OrderViewModel>>> GetUserOrdersAsync(int userId);

        /// <summary>
        /// Получить заказ с полным составом и информацией о покупателе
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <returns>Заказ с деталями или ошибка NotFound</returns>
        Task<BaseResponse<OrderViewModel>> GetOrderDetailsAsync(int orderId);

        /// <summary>
        /// Изменить статус заказа (продавец своих товаров или администратор)
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <param name="newStatus">Новый статус</param>
        /// <param name="userId">ID пользователя, меняющего статус</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, int userId);

        /// <summary>
        /// Получить заказы, в которых есть товары конкретного продавца
        /// </summary>
        /// <param name="sellerId">ID продавца</param>
        /// <returns>Коллекция заказов</returns>
        Task<BaseResponse<IEnumerable<OrderViewModel>>> GetSellerOrdersAsync(int sellerId);

        /// <summary>
        /// Получить все заказы (только для администратора)
        /// </summary>
        /// <returns>Коллекция всех заказов</returns>
        Task<BaseResponse<IEnumerable<OrderViewModel>>> GetAllOrdersAsync();
    }
}
