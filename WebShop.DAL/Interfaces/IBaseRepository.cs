using System.Linq.Expressions;

namespace WebShop.DAL.Interfaces
{
    /// <summary>
    /// Базовый интерфейс репозитория с общими методами для всех сущностей
    /// </summary>
    /// <typeparam name="T">Тип сущности (должен быть классом)</typeparam>
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// Получить все записи сущности
        /// </summary>
        /// <returns>Коллекция всех записей</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Получить записи по условию
        /// </summary>
        /// <param name="predicate">Условие для фильтрации (например: u => u.Role == UserRole.Customer)</param>
        /// <returns>Коллекция записей, удовлетворяющих условию</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Получить одну запись по ID
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns>Найденная запись или null</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Создать новую запись
        /// </summary>
        /// <param name="entity">Сущность для создания</param>
        /// <returns>Созданная сущность с заполненным ID</returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Обновить существующую запись
        /// </summary>
        /// <param name="entity">Сущность с обновлёнными данными</param>
        /// <returns>Обновлённая сущность</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Удалить запись
        /// </summary>
        /// <param name="entity">Сущность для удаления</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Проверить, существует ли запись по условию
        /// </summary>
        /// <param name="predicate">Условие для проверки</param>
        /// <returns>true - если запись существует, false - если нет</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Сохранить все изменения в базе данных
        /// </summary>
        Task SaveChangesAsync();
    }
}
