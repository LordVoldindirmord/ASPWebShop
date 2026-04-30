using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebShop.DAL.Context;
using WebShop.DAL.Interfaces;

namespace WebShop.DAL.Repositories
{
    /// <summary>
    /// Базовый абстрактный класс репозитория с общей реализацией CRUD операций
    /// </summary>
    /// <typeparam name="T">Тип сущности (должен быть классом)</typeparam>
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly WebShopDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(WebShopDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Получить все записи
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Получить записи по условию
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Получить запись по ID
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Создать новую запись
        /// </summary>
        public virtual async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Обновить существующую запись
        /// </summary>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Удалить запись
        /// </summary>
        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Проверить существование по условию
        /// </summary>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Сохранить все изменения
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}