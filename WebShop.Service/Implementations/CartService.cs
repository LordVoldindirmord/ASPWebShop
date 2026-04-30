using System;
using System.Collections.Generic;
using System.Text;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.CartModel;
using WebShop.Service.Interfaces;

namespace WebShop.Service.Implementations
{
    /// <summary>
    /// Сервис для управления корзиной пользователя
    /// </summary>
    public class CartService : ICartService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartItemRepository cartItemRepository, IProductRepository productRepository)
        {
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
        }

        public async Task<BaseResponse<CartViewModel>> GetCartAsync(int userId)
        {
            try
            {
                var cartItems = await _cartItemRepository.GetCartItemsByUserAsync(userId);
                var viewModel = new CartViewModel();

                if (cartItems != null && cartItems.Any())
                {
                    var items = new List<CartItemViewModel>();
                    foreach (var cartItem in cartItems)
                    {
                        if (cartItem.Product != null)
                        {
                            items.Add(MapToViewModel(cartItem));
                        }
                    }

                    viewModel.Items = items;
                    viewModel.TotalAmount = items.Sum(i => i.TotalPrice);  // ← сумма
                    viewModel.ItemsCount = items.Sum(i => i.Quantity);
                }

                return CreatorResponse.Ok(viewModel);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<CartViewModel>($"Ошибка при получении корзины: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> AddToCartAsync(int userId, AddToCartViewModel model)
        {
            try
            {
                // Валидация товара
                var product = await _productRepository.GetByIdAsync(model.ProductId);
                if (product == null)
                    return CreatorResponse.NotFound<bool>("Товар не найден");

                if (product.Isactive != true)
                    return CreatorResponse.BadRequest<bool>("Товар недоступен для заказа");

                // Проверяем остаток
                if (product.Stockquantity < model.Quantity)
                    return CreatorResponse.BadRequest<bool>($"Недостаточно товара на складе. Доступно: {product.Stockquantity}");

                // Проверяем, есть ли уже такой товар в корзине
                var existingItems = await _cartItemRepository.FindAsync(
                    c => c.Userid == userId && c.Productid == model.ProductId);

                var existing = existingItems.FirstOrDefault();

                if (existing != null)
                {
                    // Увеличиваем количество
                    var newQuantity = existing.Quantity + model.Quantity;
                    if (newQuantity > product.Stockquantity)
                        return CreatorResponse.BadRequest<bool>($"Общее количество превышает доступное на складе ({product.Stockquantity})");

                    existing.Quantity = newQuantity;
                    await _cartItemRepository.UpdateAsync(existing);
                }
                else
                {
                    // Создаём новую запись в корзине
                    var cartItem = new Cartitem
                    {
                        Userid = userId,
                        Productid = model.ProductId,
                        Quantity = model.Quantity
                    };
                    await _cartItemRepository.CreateAsync(cartItem);
                }

                return CreatorResponse.Ok(true, "Товар добавлен в корзину");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при добавлении в корзину: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> UpdateCartItemQuantityAsync(int cartItemId, int quantity, int userId)
        {
            try
            {
                if (quantity <= 0)
                    return CreatorResponse.BadRequest<bool>("Количество должно быть больше нуля");

                var cartItem = await _cartItemRepository.GetCartItemWithProductAsync(cartItemId);
                if (cartItem == null)
                    return CreatorResponse.NotFound<bool>("Позиция в корзине не найдена");

                // Проверка владельца
                if (cartItem.Userid != userId)
                    return CreatorResponse.Forbidden<bool>("Нельзя изменять чужую корзину");

                // Проверяем остаток товара
                if (cartItem.Product == null)
                    return CreatorResponse.BadRequest<bool>("Товар в позиции корзины более не существует");

                if (cartItem.Product.Stockquantity < quantity)
                    return CreatorResponse.BadRequest<bool>($"Недостаточно товара на складе. Доступно: {cartItem.Product.Stockquantity}");

                cartItem.Quantity = quantity;
                await _cartItemRepository.UpdateAsync(cartItem);

                return CreatorResponse.Ok(true, "Количество обновлено");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при обновлении корзины: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> RemoveFromCartAsync(int cartItemId, int userId)
        {
            try
            {
                var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);
                if (cartItem == null)
                    return CreatorResponse.NotFound<bool>("Позиция в корзине не найдена");

                if (cartItem.Userid != userId)
                    return CreatorResponse.Forbidden<bool>("Нельзя удалять из чужой корзины");

                await _cartItemRepository.DeleteAsync(cartItem);

                return CreatorResponse.Ok(true, "Товар удалён из корзины");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при удалении из корзины: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> ClearCartAsync(int userId)
        {
            try
            {
                await _cartItemRepository.ClearCartAsync(userId);
                return CreatorResponse.Ok(true, "Корзина очищена");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при очистке корзины: {ex.Message}");
            }
        }

        private CartItemViewModel MapToViewModel(Cartitem cartItem)
        {
            var product = cartItem.Product;

            return new CartItemViewModel
            {
                Id = cartItem.Id,
                ProductId = cartItem.Productid,
                ProductName = product?.Name ?? "Товар удалён",
                Price = product?.Price ?? 0,
                Quantity = cartItem.Quantity,
                MainImageUrl = product?.Mainimageurl,
                StockQuantity = product?.Stockquantity ?? 0,
                CategoryName = product?.Category?.Name ?? "Без категории"
            };
        }
    }
}
