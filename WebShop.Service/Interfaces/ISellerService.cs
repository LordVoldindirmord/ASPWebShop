using System;
using System.Collections.Generic;
using System.Text;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.SellerModel;

namespace WebShop.Service.Interfaces
{
    /// <summary>
    /// Сервис для управления профилем продавца
    /// </summary>
    public interface ISellerService
    {
        /// <summary>
        /// Стать продавцом: создаёт профиль и меняет роль на Seller
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="model">Название магазина и описание</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> BecomeSellerAsync(int userId, CreateSellerViewModel model);

        /// <summary>
        /// Получить информацию о продавце по ID пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Профиль продавца или ошибка NotFound</returns>
        Task<BaseResponse<SellerViewModel>> GetSellerByUserIdAsync(int userId);

        /// <summary>
        /// Получить продавца с его активными товарами
        /// </summary>
        /// <param name="sellerId">ID продавца</param>
        /// <returns>Продавец со списком товаров</returns>
        Task<BaseResponse<SellerViewModel>> GetSellerWithProductsAsync(int sellerId);

        /// <summary>
        /// Обновить название магазина и описание
        /// </summary>
        /// <param name="sellerId">ID продавца</param>
        /// <param name="model">Новые данные</param>
        /// <param name="userId">ID пользователя (владелец или админ)</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> UpdateSellerProfileAsync(int sellerId, CreateSellerViewModel model, int userId);

        /// <summary>
        /// Проверить, является ли пользователь продавцом
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>true - является, false - нет</returns>
        Task<BaseResponse<bool>> IsUserSellerAsync(int userId);
    }
}
