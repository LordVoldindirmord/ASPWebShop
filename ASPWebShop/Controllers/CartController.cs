using ASPWebShop.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShop.Domain.ViewModel.CartModel;
using WebShop.Service.Interfaces;

namespace ASPWebShop.Controllers
{
    /// <summary>
    /// Контроллер для управления корзиной пользователя
    /// </summary>
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Просмотр корзины пользователя
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var userId = this.GetUserId();
            var response = await _cartService.GetCartAsync(userId);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Description ?? "Ошибка при загрузке корзины";
                return View(new CartViewModel());
            }

            return View(response.Data);
        }

        /// <summary>
        /// Добавление товара в корзину
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = this.GetUserId();

            var model = new AddToCartViewModel
            {
                ProductId = productId,
                Quantity = quantity
            };

            var response = await _cartService.AddToCartAsync(userId, model);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Description ?? "Ошибка при добавлении в корзину";
            }
            else
            {
                TempData["Success"] = "Товар добавлен в корзину";
            }

            // Возвращаем туда, откуда пришли
            return Redirect(Request.Headers["Referer"].ToString());
        }

        /// <summary>
        /// Изменение количества товара в корзине
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var userId = this.GetUserId();
            var response = await _cartService.UpdateCartItemQuantityAsync(cartItemId, quantity, userId);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Description ?? "Ошибка при обновлении количества";
            }

            return RedirectToAction("Cart");
        }

        /// <summary>
        /// Удаление товара из корзины
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = this.GetUserId();
            var response = await _cartService.RemoveFromCartAsync(cartItemId, userId);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Description ?? "Ошибка при удалении из корзины";
            }
            else
            {
                TempData["Success"] = "Товар удалён из корзины";
            }

            return RedirectToAction("Cart");
        }
    }
}