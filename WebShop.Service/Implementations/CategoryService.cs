using System;
using System.Collections.Generic;
using System.Text;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.CategoryModel;
using WebShop.Service.Interfaces;

namespace WebShop.Service.Implementations
{
    /// <summary>
    /// Сервис для управления категориями товаров (администратор)
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<BaseResponse<int>> CreateCategoryAsync(CreateCategoryViewModel model)
        {
            try
            {
                // Проверяем уникальность названия
                var nameTaken = await _categoryRepository.IsCategoryNameTakenAsync(model.Name);
                if (nameTaken)
                    return CreatorResponse.Conflict<int>("Категория с таким названием уже существует");

                var category = new Category
                {
                    Name = model.Name,
                    Description = model.Description
                };

                await _categoryRepository.CreateAsync(category);
                return CreatorResponse.Created(category.Id, "Категория успешно создана");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<int>($"Ошибка при создании категории: {ex.Message}");
            }
        }

        public async Task<BaseResponse<IEnumerable<CategoryViewModel>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllWithProductCountAsync();
                var viewModels = categories.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.Products?.Count(p => p.Isactive == true) ?? 0
                });

                return CreatorResponse.Ok(viewModels);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<IEnumerable<CategoryViewModel>>($"Ошибка при получении категорий: {ex.Message}");
            }
        }

        public async Task<BaseResponse<CategoryViewModel>> GetCategoryWithProductsAsync(int categoryId)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryWithProductsAsync(categoryId);
                if (category == null)
                    return CreatorResponse.NotFound<CategoryViewModel>("Категория не найдена");

                var viewModel = new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ProductCount = category.Products?.Count ?? 0
                };

                return CreatorResponse.Ok(viewModel);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<CategoryViewModel>($"Ошибка при получении категории: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> UpdateCategoryAsync(int categoryId, CreateCategoryViewModel model)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                    return CreatorResponse.NotFound<bool>("Категория не найдена");

                // Проверяем уникальность, если название изменилось
                if (!string.Equals(category.Name, model.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var nameTaken = await _categoryRepository.IsCategoryNameTakenAsync(model.Name, categoryId);
                    if (nameTaken)
                        return CreatorResponse.Conflict<bool>("Категория с таким названием уже существует");
                }

                category.Name = model.Name;
                category.Description = model.Description;
                await _categoryRepository.UpdateAsync(category);

                return CreatorResponse.Ok(true, "Категория успешно обновлена");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при обновлении категории: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryWithProductsAsync(categoryId);
                if (category == null)
                    return CreatorResponse.NotFound<bool>("Категория не найдена");

                if (category.Products != null && category.Products.Any())
                    return CreatorResponse.Conflict<bool>("Нельзя удалить категорию, в которой есть товары");

                await _categoryRepository.DeleteAsync(category);
                return CreatorResponse.Ok(true, "Категория успешно удалена");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при удалении категории: {ex.Message}");
            }
        }
    }
}
