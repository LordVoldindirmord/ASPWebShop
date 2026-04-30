using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPWebShop.Extension
{
    /// <summary>
    /// Расширения для Controller — работа с аутентификацией
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Войти в систему (создать зашифрованную куку)
        /// </summary>
        public static async Task SignInUserAsync(this Controller controller,
            int userId, string userName, string email, string role, bool rememberMe)
        {
            // Сбор данных (ключ - значение)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
            };

            // Это паспорт нашего куки (что-то из мира шифрования похоже, почитать еще)
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Это я не очень понял, как именно работает, но это срок жизни нашего кука (долговечность)
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe, 
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
            };

            // Это как бы и есть сама кука, которая будет в браузере
            await controller.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, // Тип входа
                new ClaimsPrincipal(claimsIdentity), // Владелец паспорта
                authProperties); // Жизнь куки
        }

        /// <summary>
        /// Выйти из системы (удалить куку)
        /// </summary>
        public static async Task SignOutUserAsync(this Controller controller)
        {
            await controller.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Получить ID текущего пользователя
        /// </summary>
        public static int GetUserId(this Controller controller)
        {
            return int.Parse(controller.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        /// <summary>
        /// Получить имя текущего пользователя
        /// </summary>
        public static string GetUserName(this Controller controller)
        {
            return controller.User.FindFirstValue(ClaimTypes.Name) ?? "Гость";
        }

        /// <summary>
        /// Получить Email текущего пользователя
        /// </summary>
        public static string GetUserEmail(this Controller controller)
        {
            return controller.User.FindFirstValue(ClaimTypes.Email) ?? "";
        }

        /// <summary>
        /// Получить роль текущего пользователя
        /// </summary>
        public static string GetUserRole(this Controller controller)
        {
            return controller.User.FindFirstValue(ClaimTypes.Role) ?? "";
        }

        /// <summary>
        /// Проверить, авторизован ли пользователь
        /// </summary>
        public static bool IsAuthenticated(this Controller controller)
        {
            return controller.User.Identity?.IsAuthenticated ?? false;
        }
    }
}
