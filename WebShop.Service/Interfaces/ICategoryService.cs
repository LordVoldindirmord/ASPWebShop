using System;
using System.Collections.Generic;
using System.Text;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.CategoryModel;

namespace WebShop.Service.Interfaces
{
    /// <summary>
    /// Сервис для управления категориями товаров (администратор)
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Создать новую категорию
        /// </summary>
        /// <param name="model">Название и описание категории</param>
        /// <returns>ID созданной категории; ошибка при неудаче</returns>
        Task<BaseResponse<int>> CreateCategoryAsync(CreateCategoryViewModel model);

        /// <summary>
        /// Получить все категории (без товаров)
        /// </summary>
        /// <returns>Коллекция категорий</returns>
        Task<BaseResponse<IEnumerable<CategoryViewModel>>> GetAllCategoriesAsync();

        /// <summary>
        /// Получить категорию с количеством активных товаров в ней
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Категория с заполненным ProductCount или ошибка NotFound</returns>
        Task<BaseResponse<CategoryViewModel>> GetCategoryWithProductsAsync(int categoryId);

        /// <summary>
        /// Обновить название или описание категории
        /// </summary>
        /// <param name="categoryId">ID обновляемой категории</param>
        /// <param name="model">Новые данные</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> UpdateCategoryAsync(int categoryId, CreateCategoryViewModel model);

        /// <summary>
        /// Удалить категорию (только если нет привязанных товаров)
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> DeleteCategoryAsync(int categoryId);
    }
}
