using System;
using System.Collections.Generic;
using System.Text;
using WebShop.Domain.Enum;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.UserModel;

namespace WebShop.Service.Interfaces
{
    /// <summary>
    /// Сервис для работы с пользователями: регистрация, вход, управление профилем
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Зарегистрировать нового пользователя
        /// </summary>
        /// <param name="model">Модель регистрации (имя, фамилия, email, пароль)</param>
        /// <returns>true при успехе; ошибка с описанием при неудаче</returns>
        Task<BaseResponse<bool>> RegisterAsync(RegisterViewModel model);

        /// <summary>
        /// Войти в систему (проверить email и пароль)
        /// </summary>
        /// <param name="model">Модель входа (email, пароль)</param>
        /// <returns>true при успехе; ошибка с описанием при неудаче</returns>
        Task<BaseResponse<bool>> LoginAsync(LoginViewModel model);

        /// <summary>
        /// Получить пользователя по идентификатору
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <returns>Найденный пользователь или ошибка NotFound</returns>
        Task<BaseResponse<UserViewModel>> GetByIdAsync(int id);

        /// <summary>
        /// Получить пользователя по Email
        /// </summary>
        /// <param name="email">Email пользователя</param>
        /// <returns>Найденный пользователь или ошибка NotFound</returns>
        Task<BaseResponse<UserViewModel>> GetByEmailAsync(string email);

        /// <summary>
        /// Обновить профиль текущего пользователя (имя, фамилия, телефон)
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="model">Обновлённые данные</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> UpdateProfileAsync(int userId, UserViewModel model);

        /// <summary>
        /// Изменить роль пользователя (только для администратора)
        /// </summary>
        /// <param name="userId">ID пользователя, которому меняют роль</param>
        /// <param name="newRole">Новая роль</param>
        /// <returns>true при успехе; ошибка при неудаче</returns>
        Task<BaseResponse<bool>> ChangeRoleAsync(int userId, UserRole newRole);

        /// <summary>
        /// Получить всех пользователей с определённой ролью
        /// </summary>
        /// <param name="role">Роль для фильтрации</param>
        /// <returns>Коллекция пользователей или ошибка</returns>
        Task<BaseResponse<IEnumerable<UserViewModel>>> GetUsersByRoleAsync(UserRole role);

        /// <summary>
        /// Проверить, занят ли Email другим пользователем
        /// </summary>
        /// <param name="email">Проверяемый Email</param>
        /// <param name="excludeUserId">ID пользователя, которого исключаем из проверки (для обновления)</param>
        /// <returns>true - занят, false - свободен</returns>
        Task<BaseResponse<bool>> IsEmailTakenAsync(string email, int? excludeUserId = null);
    }
}
