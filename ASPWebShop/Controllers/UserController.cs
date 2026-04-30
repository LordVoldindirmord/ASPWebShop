using ASPWebShop.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShop.Domain.ViewModel.UserModel;
using WebShop.Service.Interfaces;

namespace ASPWebShop.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Авторизация
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var response = await _userService.LoginAsync(model);

            // Ошибочка словлена
            if (!response.IsSuccess || !response.Data)
            {
                ModelState.AddModelError(string.Empty, response.Description ?? "Ошибка при авторизации");
                return View(model);
            }

            // Логин успешен: получаем данные пользователя по Email
            var userResponse = await _userService.GetByEmailAsync(model.Email);

            if (!userResponse.IsSuccess || userResponse.Data == null)
            {
                ModelState.AddModelError(string.Empty, userResponse.Description ?? "Не удалось получить данные пользователя");
                return View(model);
            }

            var user = userResponse.Data;

            await this.SignInUserAsync(user.Id, $"{user.FirstName} {user.LastName}", user.Email, user.Role.ToString(), model.RememberMe);

            return RedirectToAction("MainMenu", "Product");
        }

        // Регистрация
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var response = await _userService.RegisterAsync(model);

            // Ошибочка словлена
            if (!response.IsSuccess || !response.Data)
            {
                ModelState.AddModelError(string.Empty, response.Description ?? "Ошибка при регистрации");
                return View(model);
            }

            // Все прошло хорошо
            return RedirectToAction("Login");
        }

        // Основной — через GET (просто ссылка "Выйти")
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await this.SignOutUserAsync();
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = this.GetUserId();

            var response = await _userService.GetByIdAsync(userId);

            if(!response.IsSuccess || response.Data == null)
            {
                TempData["Error"] = response.Description ?? "Не удалось загрузить профиль";
                return RedirectToAction("MainMenu", "Product");
            }

            return View(response.Data);
        }
    }
}