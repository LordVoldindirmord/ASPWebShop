using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;
using WebShop.Domain.Enum;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.OrderModel;
using WebShop.Service.Interfaces;

namespace WebShop.Service.Implementations
{
    /// <summary>
    /// Сервис для работы с заказами: создание, просмотр, изменение статуса
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly IUserRepository _userRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICartItemRepository cartItemRepository,
            IProductRepository productRepository,
            ISellerRepository sellerRepository,
            IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _sellerRepository = sellerRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Создать заказ из товаров в корзине пользователя
        /// </summary>
        public async Task<BaseResponse<int>> CreateOrderAsync(int userId, CreateOrderViewModel model)
        {
            try
            {
                // Получаем корзину (AsNoTracking - только для чтения)
                var cartItems = await _cartItemRepository.GetCartItemsByUserAsync(userId);
                if (cartItems == null || !cartItems.Any())
                    return CreatorResponse.BadRequest<int>("Корзина пуста");

                var orderItems = new List<Orderitem>();
                decimal totalAmount = 0;

                foreach (var cartItem in cartItems)
                {
                    // Получаем товар ОТДЕЛЬНО для обновления (с отслеживанием)
                    var product = await _productRepository.GetByIdAsync(cartItem.Productid);

                    if (product == null || product.Isactive != true)
                        return CreatorResponse.BadRequest<int>($"Товар '{cartItem.Product?.Name ?? "неизвестный"}' недоступен");

                    if (product.Stockquantity < cartItem.Quantity)
                        return CreatorResponse.BadRequest<int>($"Недостаточно товара '{product.Name}' на складе. Доступно: {product.Stockquantity}");

                    var orderItem = new Orderitem
                    {
                        Productid = product.Id,
                        Productname = product.Name,
                        Price = product.Price,
                        Quantity = cartItem.Quantity
                    };
                    orderItems.Add(orderItem);
                    totalAmount += product.Price * cartItem.Quantity;

                    // Уменьшаем остаток
                    product.Stockquantity -= cartItem.Quantity;
                    await _productRepository.UpdateAsync(product);
                }

                var order = new Order
                {
                    Userid = userId,
                    Totalamount = totalAmount,
                    Shippingaddress = model.ShippingAddress,
                    Status = OrderStatus.Pending,
                    Orderdate = DateTime.Now
                };
                await _orderRepository.CreateAsync(order);

                foreach (var item in orderItems)
                {
                    item.Orderid = order.Id;
                    await _orderItemRepository.CreateAsync(item);
                }

                await _cartItemRepository.ClearCartAsync(userId);

                return CreatorResponse.Created(order.Id, "Заказ успешно создан");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<int>($"Ошибка при создании заказа: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить все заказы пользователя
        /// </summary>
        public async Task<BaseResponse<IEnumerable<OrderViewModel>>> GetUserOrdersAsync(int userId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByUserAsync(userId);
                var viewModels = orders.Select(MapToOrderViewModel).ToList();
                return CreatorResponse.Ok((IEnumerable<OrderViewModel>)viewModels);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<IEnumerable<OrderViewModel>>($"Ошибка при получении заказов: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить заказ с деталями
        /// </summary>
        public async Task<BaseResponse<OrderViewModel>> GetOrderDetailsAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderWithItemsAsync(orderId);
                if (order == null)
                    return CreatorResponse.NotFound<OrderViewModel>("Заказ не найден");

                return CreatorResponse.Ok(MapToOrderViewModel(order));
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<OrderViewModel>($"Ошибка при получении заказа: {ex.Message}");
            }
        }

        /// <summary>
        /// Изменить статус заказа (продавец своих товаров или администратор)
        /// </summary>
        public async Task<BaseResponse<bool>> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, int userId)
        {
            try
            {
                var order = await _orderRepository.GetOrderWithItemsAsync(orderId);
                if (order == null)
                    return CreatorResponse.NotFound<bool>("Заказ не найден");

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return CreatorResponse.Unauthorized<bool>("Пользователь не найден");

                bool isAdmin = user.Role == UserRole.Administrator;
                if (!isAdmin)
                {
                    var seller = await _sellerRepository.GetByUserIdAsync(userId);
                    if (seller == null)
                        return CreatorResponse.Forbidden<bool>("Недостаточно прав");

                    bool ownsProduct = order.Orderitems.Any(oi => oi.Product?.Sellerid == seller.Id);
                    if (!ownsProduct)
                        return CreatorResponse.Forbidden<bool>("Вы не можете изменять этот заказ");
                }

                order.Status = newStatus;
                await _orderRepository.UpdateAsync(order);

                return CreatorResponse.Ok(true, $"Статус заказа изменён на {newStatus}");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при изменении статуса: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить заказы, в которых есть товары конкретного продавца
        /// </summary>
        public async Task<BaseResponse<IEnumerable<OrderViewModel>>> GetSellerOrdersAsync(int sellerId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersBySellerAsync(sellerId);
                var viewModels = orders.Select(MapToOrderViewModel).ToList();
                return CreatorResponse.Ok((IEnumerable<OrderViewModel>)viewModels);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<IEnumerable<OrderViewModel>>($"Ошибка при получении заказов продавца: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить все заказы (для администратора)
        /// </summary>
        public async Task<BaseResponse<IEnumerable<OrderViewModel>>> GetAllOrdersAsync()
        {
            try
            {
                // Используем специальный метод репозитория, подгружающий Orderitems и User
                var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();
                var viewModels = orders.Select(MapToOrderViewModel).ToList();
                return CreatorResponse.Ok((IEnumerable<OrderViewModel>)viewModels);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<IEnumerable<OrderViewModel>>($"Ошибка при получении всех заказов: {ex.Message}");
            }
        }

        /// <summary>
        /// Маппинг Order -> OrderViewModel
        /// </summary>
        private OrderViewModel MapToOrderViewModel(Order order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                OrderDate = order.Orderdate,
                TotalAmount = order.Totalamount,  // ← должно быть
                Status = order.Status.ToString(),
                ShippingAddress = order.Shippingaddress,
                CustomerName = order.User != null ? $"{order.User.Firstname} {order.User.Lastname}" : "Неизвестный",
                CustomerEmail = order.User?.Email ?? "",
                Items = order.Orderitems?.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.Productid,
                    ProductName = oi.Productname,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList() ?? new List<OrderItemViewModel>()
            };
        }
    }
}