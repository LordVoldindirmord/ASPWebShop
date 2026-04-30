using System;
using System.Collections.Generic;
using System.Text;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;
using WebShop.Domain.Enum;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.UserModel;
using WebShop.Service.Interfaces;

namespace WebShop.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<BaseResponse<bool>> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                // Проверяем, не занят ли Email
                var emailTaken = await _userRepository.IsEmailTakenAsync(model.Email);

                if (emailTaken)
                {
                    return CreatorResponse.Conflict<bool>("Пользователь с таким Email уже существует");
                }

                // Хэшируем пароль
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Создаём сущность пользователя (роль по умолчанию — Customer)
                var user = new User
                {
                    Firstname = model.FirstName,
                    Lastname = model.LastName,
                    Email = model.Email,
                    Passwordhash = passwordHash,
                    Role = UserRole.Customer
                };

                await _userRepository.CreateAsync(user);

                return CreatorResponse.Ok(true, "Регистрация прошла успешно");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при регистрации: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> LoginAsync(LoginViewModel model)
        {
            try
            {
                // Ищем пользователя по Email
                var user = await _userRepository.GetByEmailAsync(model.Email);

                if (user == null)
                {
                    return CreatorResponse.NotFound<bool>("Пользователь с таким Email не найден");
                }

                // Проверяем пароль
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Passwordhash))
                {
                    return CreatorResponse.Unauthorized<bool>("Неверный пароль");
                }

                return CreatorResponse.Ok(true, "Вход выполнен успешно");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при входе: {ex.Message}");
            }
        }

        public async Task<BaseResponse<UserViewModel>> GetByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    return CreatorResponse.NotFound<UserViewModel>("Пользователь не найден");
                }

                var viewModel = MapToViewModel(user);

                return CreatorResponse.Ok(viewModel);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<UserViewModel>($"Ошибка при получении пользователя: {ex.Message}");
            }
        }

        public async Task<BaseResponse<UserViewModel>> GetByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);

                if (user == null)
                {
                    return CreatorResponse.NotFound<UserViewModel>("Пользователь не найден");
                }

                var viewModel = MapToViewModel(user);

                return CreatorResponse.Ok(viewModel);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<UserViewModel>($"Ошибка при поиске пользователя: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> UpdateProfileAsync(int userId, UserViewModel model)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return CreatorResponse.NotFound<bool>("Пользователь не найден");
                }

                // Проверяем Email (если изменился) на уникальность
                if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailTaken = await _userRepository.IsEmailTakenAsync(model.Email, userId);

                    if (emailTaken)
                    {
                        return CreatorResponse.Conflict<bool>("Этот Email уже используется другим пользователем");
                    }
                }

                // Обновляем только разрешённые поля
                user.Firstname = model.FirstName;
                user.Lastname = model.LastName;
                user.Phonenumber = model.PhoneNumber;
                user.Email = model.Email;

                await _userRepository.UpdateAsync(user);

                return CreatorResponse.Ok(true, "Профиль успешно обновлён");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при обновлении профиля: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> ChangeRoleAsync(int userId, UserRole newRole)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return CreatorResponse.NotFound<bool>("Пользователь не найден");
                }

                user.Role = newRole;

                await _userRepository.UpdateAsync(user);

                return CreatorResponse.Ok(true, $"Роль пользователя изменена на {newRole}");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при смене роли: {ex.Message}");
            }
        }

        public async Task<BaseResponse<IEnumerable<UserViewModel>>> GetUsersByRoleAsync(UserRole role)
        {
            try
            {
                var users = await _userRepository.GetUsersByRoleAsync(role);

                var viewModels = users.Select(MapToViewModel).ToList();

                return CreatorResponse.Ok((IEnumerable<UserViewModel>)viewModels);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<IEnumerable<UserViewModel>>($"Ошибка при получении списка пользователей: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> IsEmailTakenAsync(string email, int? excludeUserId = null)
        {
            try
            {
                var result = await _userRepository.IsEmailTakenAsync(email, excludeUserId);

                return CreatorResponse.Ok(result);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка проверки Email: {ex.Message}");
            }
        }

        private UserViewModel MapToViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.Email,
                PhoneNumber = user.Phonenumber,
                Role = user.Role,
                CreatedAt = user.Createdat
            };
        }
    }
}
