using System;
using System.Collections.Generic;
using System.Text;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.ProductModel;

namespace WebShop.Service.Interfaces
{
    /// <summary>
    /// Сервис для работы с товарами: создание, просмотр, редактирование, поиск
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Активировать товар (показать в каталоге)
        /// </summary>
        Task<BaseResponse<bool>> ActivateProductAsync(int productId, int userId);

        /// <summary>
        /// Создать новый товар (доступно продавцу)
        /// </summary>
        /// <param name="model">Модель с данными товара и изображениями</param>
        /// <param name="sellerId">ID продавца, создающего товар</param>
        /// <returns>ID созданного товара; ошибка при неудаче</returns>
        Task<BaseResponse<int>> CreateProductAsync(CreateProductViewModel model, int sellerId);

        /// <summary>
        /// Получить товар с полной информацией для страницы "Подробнее"
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <returns>Детальная модель товара или ошибка NotFound</returns>
        Task<BaseResponse<ProductDetailViewModel>> GetProductDetailsAsync(int productId);

        /// <summary>
        /// Получить все активные товары для отображения в каталоге
        /// </summary>
        /// <returns>Коллекция товаров</returns>
        Task<BaseResponse<IEnumerable<ProductViewModel>>> GetActiveProductsAsync();

        /// <summary>
        /// Получить активные товары из определённой категории
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Коллекция товаров категории или ошибка</returns>
        Task<BaseResponse<IEnumerable<ProductViewModel>>> GetProductsByCategoryAsync(int categoryId);

        /// <summary>
        /// Получить все товары конкретного продавца
        /// </summary>
        /// <param name="sellerId">ID продавца</param>
        /// <returns>Коллекция товаров продавца или ошибка</returns>
        Task<BaseResponse<IEnumerable<ProductViewModel>>> GetProductsBySellerAsync(int sellerId);

        /// <summary>
        /// Поиск товаров по названию (регистронезависимый)
        /// </summary>
        /// <param name="searchTerm">Поисковая строка</param>
        /// <returns>Найденные товары или пустая коллекция</returns>
        Task<BaseResponse<IEnumerable<ProductViewModel>>> SearchProductsAsync(string searchTerm);

        /// <summary>
        /// Редактировать товар (владелец или администратор)
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <param name="model">Новые данные товара</param>
        /// <param name="userId">ID пользователя, выполняющего редактирование</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> UpdateProductAsync(int productId, EditProductViewModel model, int userId);

        /// <summary>
        /// Деактивировать товар (мягкое удаление)
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <param name="userId">ID пользователя (для проверки прав)</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> DeactivateProductAsync(int productId, int userId);

        /// <summary>
        /// Обновить остаток товара на складе
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <param name="quantity">Новое количество</param>
        /// <param name="userId">ID пользователя (владелец или админ)</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> UpdateStockAsync(int productId, int quantity, int userId);
    }
}
