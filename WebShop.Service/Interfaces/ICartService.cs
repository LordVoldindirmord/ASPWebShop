using System;
using System.Collections.Generic;
using System.Text;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.CartModel;

namespace WebShop.Service.Interfaces
{
    /// <summary>
    /// Сервис для управления корзиной пользователя
    /// </summary>
    public interface ICartService
    {
        /// <summary>
        /// Получить полную корзину пользователя с товарами и суммой
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Модель корзины</returns>
        Task<BaseResponse<CartViewModel>> GetCartAsync(int userId);

        /// <summary>
        /// Добавить товар в корзину. Если уже есть — увеличивает количество
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="model">ID товара и количество</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> AddToCartAsync(int userId, AddToCartViewModel model);

        /// <summary>
        /// Изменить количество товара в корзине
        /// </summary>
        /// <param name="cartItemId">ID позиции в корзине</param>
        /// <param name="quantity">Новое количество</param>
        /// <param name="userId">ID владельца корзины</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> UpdateCartItemQuantityAsync(int cartItemId, int quantity, int userId);

        /// <summary>
        /// Удалить один товар из корзины
        /// </summary>
        /// <param name="cartItemId">ID позиции в корзине</param>
        /// <param name="userId">ID владельца корзины</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> RemoveFromCartAsync(int cartItemId, int userId);

        /// <summary>
        /// Полностью очистить корзину пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> ClearCartAsync(int userId);
    }
}
