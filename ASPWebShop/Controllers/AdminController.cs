using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebShop.Domain.Enum;
using WebShop.Domain.ViewModel.CategoryModel;
using WebShop.Domain.ViewModel.SellerModel;
using WebShop.Domain.ViewModel.UserModel;
using WebShop.Service.Implementations;
using WebShop.Service.Interfaces;

namespace ASPWebShop.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly ISellerService _sellerService;

        public AdminController(IUserService userService, ICategoryService categoryService, ISellerService sellerService)
        {
            _userService = userService;
            _categoryService = categoryService;
            _sellerService = sellerService;
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var allUsers = new List<UserViewModel>();

            var customers = await _userService.GetUsersByRoleAsync(UserRole.Customer);
            if (customers.IsSuccess && customers.Data != null)
                allUsers.AddRange(customers.Data);

            var sellers = await _userService.GetUsersByRoleAsync(UserRole.Seller);
            if (sellers.IsSuccess && sellers.Data != null)
                allUsers.AddRange(sellers.Data);

            var admins = await _userService.GetUsersByRoleAsync(UserRole.Administrator);
            if (admins.IsSuccess && admins.Data != null)
                allUsers.AddRange(admins.Data);

            return View(allUsers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(int userId, string newRole)
        {
            if (!Enum.TryParse<UserRole>(newRole, out var role))
            {
                TempData["Error"] = "Неверная роль";
                return RedirectToAction("Users");
            }

            var userResponse = await _userService.GetByIdAsync(userId);
            if (!userResponse.IsSuccess || userResponse.Data == null)
            {
                TempData["Error"] = "Пользователь не найден";
                return RedirectToAction("Users");
            }

            var user = userResponse.Data;
            var oldRole = user.Role;

            // Если меняем на Seller — сначала создаём профиль
            if (role == UserRole.Seller && oldRole != UserRole.Seller)
            {
                var sellerCheck = await _sellerService.GetSellerByUserIdAsync(userId);

                if (!sellerCheck.IsSuccess || sellerCheck.Data == null)
                {
                    // Создаём профиль ДО смены роли
                    await _sellerService.BecomeSellerAsync(userId, new CreateSellerViewModel
                    {
                        StoreName = $"{user.FirstName} {user.LastName}",
                        Description = "Продавец товаров"
                    });
                }
            }

            // Теперь меняем роль
            var response = await _userService.ChangeRoleAsync(userId, role);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Description ?? "Ошибка при смене роли";
                return RedirectToAction("Users");
            }

            TempData["Success"] = "Роль изменена";
            return RedirectToAction("Users");
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var response = await _categoryService.GetAllCategoriesAsync();
            return View(response.IsSuccess ? response.Data : new List<CategoryViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(string name, string? description)
        {
            var model = new CreateCategoryViewModel { Name = name, Description = description };
            var response = await _categoryService.CreateCategoryAsync(model);

            TempData[response.IsSuccess ? "Success" : "Error"] = response.Description;
            return RedirectToAction("Categories");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, string name, string? description)
        {
            var model = new CreateCategoryViewModel { Name = name, Description = description };
            var response = await _categoryService.UpdateCategoryAsync(id, model);

            TempData[response.IsSuccess ? "Success" : "Error"] = response.Description;
            return RedirectToAction("Categories");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);
            TempData[response.IsSuccess ? "Success" : "Error"] = response.Description;
            return RedirectToAction("Categories");
        }
    }
}
