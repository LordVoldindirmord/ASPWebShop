using System;
using System.Collections.Generic;
using System.Text;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;
using WebShop.Domain.Enum;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.SellerModel;
using WebShop.Service.Interfaces;

namespace WebShop.Service.Implementations
{
    /// <summary>
    /// Сервис для управления профилем продавца
    /// </summary>
    public class SellerService : ISellerService
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly IUserRepository _userRepository;

        public SellerService(ISellerRepository sellerRepository, IUserRepository userRepository)
        {
            _sellerRepository = sellerRepository;
            _userRepository = userRepository;
        }

        public async Task<BaseResponse<bool>> BecomeSellerAsync(int userId, CreateSellerViewModel model)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return CreatorResponse.NotFound<bool>("Пользователь не найден");

                // Проверяем, не является ли уже продавцом или админом
                if (user.Role == UserRole.Seller || user.Role == UserRole.Administrator)
                    return CreatorResponse.Conflict<bool>(
                        user.Role == UserRole.Seller ? "Вы уже зарегистрированы как продавец" : "Администратор не может стать продавцом");

                // Проверяем, нет ли уже профиля продавца (на всякий случай)
                var existingSeller = await _sellerRepository.GetByUserIdAsync(userId);
                if (existingSeller != null)
                    return CreatorResponse.Conflict<bool>("Профиль продавца уже существует");

                // Создаём профиль продавца
                var seller = new Seller
                {
                    Userid = userId,
                    Storename = model.StoreName,
                    Description = model.Description
                };
                await _sellerRepository.CreateAsync(seller);

                // Меняем роль пользователя
                user.Role = UserRole.Seller;
                await _userRepository.UpdateAsync(user);

                return CreatorResponse.Ok(true, "Вы успешно стали продавцом");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при регистрации продавца: {ex.Message}");
            }
        }

        public async Task<BaseResponse<SellerViewModel>> GetSellerByUserIdAsync(int userId)
        {
            try
            {
                var seller = await _sellerRepository.GetByUserIdAsync(userId);
                if (seller == null)
                    return CreatorResponse.NotFound<SellerViewModel>("Продавец не найден");

                // Подгружаем User вручную, если репозиторий не загрузил
                if (seller.User == null)
                {
                    seller.User = await _userRepository.GetByIdAsync(userId);
                }

                var viewModel = MapToViewModel(seller);
                return CreatorResponse.Ok(viewModel);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<SellerViewModel>($"Ошибка при получении продавца: {ex.Message}");
            }
        }

        public async Task<BaseResponse<SellerViewModel>> GetSellerWithProductsAsync(int sellerId)
        {
            try
            {
                var seller = await _sellerRepository.GetSellerWithProductsAsync(sellerId);
                if (seller == null)
                    return CreatorResponse.NotFound<SellerViewModel>("Продавец не найден");

                var viewModel = MapToViewModel(seller);
                viewModel.ProductCount = seller.Products?.Count ?? 0;
                return CreatorResponse.Ok(viewModel);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<SellerViewModel>($"Ошибка при получении продавца с товарами: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> UpdateSellerProfileAsync(int sellerId, CreateSellerViewModel model, int userId)
        {
            try
            {
                var seller = await _sellerRepository.GetByIdAsync(sellerId);
                if (seller == null)
                    return CreatorResponse.NotFound<bool>("Продавец не найден");

                // Проверяем права
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return CreatorResponse.Unauthorized<bool>("Пользователь не найден");

                bool isAdmin = user.Role == UserRole.Administrator;
                bool isOwner = seller.Userid == userId;

                if (!isAdmin && !isOwner)
                    return CreatorResponse.Forbidden<bool>("Вы не можете редактировать этот профиль");

                // Обновляем поля
                seller.Storename = model.StoreName;
                seller.Description = model.Description;
                await _sellerRepository.UpdateAsync(seller);

                return CreatorResponse.Ok(true, "Профиль продавца обновлён");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при обновлении профиля: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> IsUserSellerAsync(int userId)
        {
            try
            {
                var result = await _sellerRepository.IsUserSellerAsync(userId);
                return CreatorResponse.Ok(result);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка проверки продавца: {ex.Message}");
            }
        }

        private SellerViewModel MapToViewModel(Seller seller)
        {
            return new SellerViewModel
            {
                Id = seller.Id,
                UserId = seller.Userid,
                StoreName = seller.Storename,
                Description = seller.Description,
                SellerFirstName = seller.User?.Firstname ?? "",
                SellerLastName = seller.User?.Lastname ?? "",
                SellerEmail = seller.User?.Email ?? "",
                ProductCount = seller.Products?.Count ?? 0
            };
        }
    }
}
