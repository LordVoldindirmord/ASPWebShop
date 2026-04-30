using ASPWebShop.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShop.Domain.ViewModel.OrderModel;
using WebShop.Service.Interfaces;

namespace ASPWebShop.Controllers
{
    /// <summary>
    /// Контроллер для работы с заказами
    /// </summary>
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = this.GetUserId();
            var cartResponse = await _cartService.GetCartAsync(userId);

            if (!cartResponse.IsSuccess || cartResponse.Data == null || !cartResponse.Data.Items.Any())
            {
                TempData["Error"] = "Корзина пуста";
                return RedirectToAction("Cart", "Cart");
            }

            ViewBag.Cart = cartResponse.Data;
            return View(new CreateOrderViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CreateOrderViewModel model)
        {
            var userId = this.GetUserId();

            if (!ModelState.IsValid)
            {
                var cartResponseErr = await _cartService.GetCartAsync(userId);
                if (cartResponseErr.IsSuccess && cartResponseErr.Data != null)
                {
                    ViewBag.Cart = cartResponseErr.Data;
                }
                return View(model);
            }

            try
            {
                var orderResponse = await _orderService.CreateOrderAsync(userId, model);

                if (!orderResponse.IsSuccess)
                {
                    TempData["Error"] = orderResponse.Description ?? "Ошибка при создании заказа";
                    return RedirectToAction("Cart", "Cart");
                }

                return RedirectToAction("Payment", new { orderId = orderResponse.Data });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка: {ex.Message}";
                return RedirectToAction("Cart", "Cart");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int orderId)
        {
            var orderResponse = await _orderService.GetOrderDetailsAsync(orderId);

            if (!orderResponse.IsSuccess || orderResponse.Data == null)
            {
                TempData["Error"] = "Заказ не найден";
                return RedirectToAction("Cart", "Cart");
            }

            return View(orderResponse.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int orderId, string cardNumber, string cardHolder, string expiryDate, string cvv)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Replace(" ", "").Length != 16)
            {
                TempData["Error"] = "Некорректный номер карты (должен быть 16 цифр)";
                return RedirectToAction("Payment", new { orderId });
            }

            if (string.IsNullOrWhiteSpace(cardHolder) || cardHolder.Length < 3)
            {
                TempData["Error"] = "Некорректное имя держателя карты";
                return RedirectToAction("Payment", new { orderId });
            }

            if (string.IsNullOrWhiteSpace(cvv) || cvv.Length != 3)
            {
                TempData["Error"] = "Некорректный CVV код";
                return RedirectToAction("Payment", new { orderId });
            }

            var userId = this.GetUserId();
            await _orderService.UpdateOrderStatusAsync(orderId, WebShop.Domain.Enum.OrderStatus.Processing, userId);

            TempData["Success"] = $"Заказ №{orderId} успешно оплачен!";
            return RedirectToAction("Success", new { orderId });
        }

        [HttpGet]
        public async Task<IActionResult> Success(int orderId)
        {
            var orderResponse = await _orderService.GetOrderDetailsAsync(orderId);

            if (!orderResponse.IsSuccess || orderResponse.Data == null)
            {
                TempData["Error"] = "Заказ не найден";
                return RedirectToAction("MainMenu", "Product");
            }

            return View(orderResponse.Data);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = this.GetUserId();
            var response = await _orderService.GetUserOrdersAsync(userId);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Description ?? "Ошибка при загрузке заказов";
                return View(new List<OrderViewModel>());
            }

            return View(response.Data);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var response = await _orderService.GetOrderDetailsAsync(orderId);

            if (!response.IsSuccess || response.Data == null)
            {
                TempData["Error"] = "Заказ не найден";
                return RedirectToAction("MyOrders");
            }

            return View(response.Data);
        }
    }
}