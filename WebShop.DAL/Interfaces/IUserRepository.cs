using WebShop.Domain.Entity;
using WebShop.Domain.Enum;

namespace WebShop.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с пользователями
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Найти пользователя по Email
        /// </summary>
        /// <param name="email">Email пользователя</param>
        /// <returns>Пользователь или null</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Проверить, занят ли Email другим пользователем
        /// </summary>
        /// <param name="email">Email для проверки</param>
        /// <param name="excludeUserId">ID пользователя, которого нужно исключить из проверки (для обновления)</param>
        /// <returns>true - если Email занят, false - если свободен</returns>
        Task<bool> IsEmailTakenAsync(string email, int? excludeUserId = null);

        /// <summary>
        /// Получить всех пользователей с определённой ролью
        /// </summary>
        /// <param name="role">Роль для фильтрации</param>
        /// <returns>Коллекция пользователей с указанной ролью</returns>
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);

        /// <summary>
        /// Получить пользователя с заказами
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Пользователь с загруженными заказами</returns>
        Task<User?> GetUserWithOrdersAsync(int userId);

        /// <summary>
        /// Получить пользователя с корзиной
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Пользователь с загруженной корзиной</returns>
        Task<User?> GetUserWithCartAsync(int userId);
    }
}
