using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.DAL.Interfaces;
using WebShop.Domain.Entity;
using WebShop.Domain.Enum;
using WebShop.Domain.Response;
using WebShop.Domain.ViewModel.ProductModel;
using WebShop.Service.Interfaces;

namespace WebShop.Service.Implementations
{
    /// <summary>
    /// Сервис для работы с товарами: создание, редактирование, поиск, отображение
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ImageService _imageService;

        public ProductService(
            IProductRepository productRepository,
            IProductImageRepository productImageRepository,
            ISellerRepository sellerRepository,
            IUserRepository userRepository,
            ImageService imageService)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _sellerRepository = sellerRepository;
            _userRepository = userRepository;
            _imageService = imageService;
        }

        public async Task<BaseResponse<int>> CreateProductAsync(CreateProductViewModel model, int sellerId)
        {
            try
            {
                var seller = await _sellerRepository.GetByIdAsync(sellerId);
                if (seller == null)
                    return CreatorResponse.NotFound<int>("Продавец не найден");

                string? mainImageUrl = null;
                if (model.MainImage != null && model.MainImage.Length > 0)
                {
                    if (string.IsNullOrWhiteSpace(model.MainImageFileName))
                        return CreatorResponse.BadRequest<int>("Не указано имя файла главного изображения");

                    mainImageUrl = await _imageService.SaveImageAsync(model.MainImage, model.MainImageFileName);
                }

                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Stockquantity = model.StockQuantity,
                    Mainimageurl = mainImageUrl,
                    Categoryid = model.CategoryId,
                    Sellerid = sellerId,
                    Isactive = true
                };
                await _productRepository.CreateAsync(product);

                if (model.AdditionalImages != null && model.AdditionalImages.Any())
                {
                    foreach (var imageData in model.AdditionalImages)
                    {
                        if (imageData.ImageBytes != null && imageData.ImageBytes.Length > 0)
                        {
                            var imageUrl = await _imageService.SaveImageAsync(imageData.ImageBytes, imageData.FileName);
                            var productImage = new Productimage
                            {
                                Productid = product.Id,
                                Imageurl = imageUrl,
                                Sortorder = 0
                            };
                            await _productImageRepository.CreateAsync(productImage);
                        }
                    }
                }

                return CreatorResponse.Created(product.Id, "Товар успешно создан");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<int>($"Ошибка при создании товара: {ex.Message}");
            }
        }

        public async Task<BaseResponse<ProductDetailViewModel>> GetProductDetailsAsync(int productId)
        {
            try
            {
                var product = await _productRepository.GetProductWithDetailsAsync(productId);
                if (product == null)
                    return CreatorResponse.NotFound<ProductDetailViewModel>("Товар не найден");

                var viewModel = MapToDetailViewModel(product);
                return CreatorResponse.Ok(viewModel);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<ProductDetailViewModel>($"Ошибка при получении товара: {ex.Message}");
            }
        }

        public async Task<BaseResponse<IEnumerable<ProductViewModel>>> GetActiveProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetActiveProductsAsync();
                var viewModels = products.Select(MapToViewModel).ToList();
                return CreatorResponse.Ok((IEnumerable<ProductViewModel>)viewModels);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<IEnumerable<ProductViewModel>>($"Ошибка при получении товаров: {ex.Message}");
            }
        }

        public async Task<BaseResponse<IEnumerable<ProductViewModel>>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
                var viewModels = products.Select(MapToViewModel).ToList();
                return CreatorResponse.Ok((IEnumerable<ProductViewModel>)viewModels);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<IEnumerable<ProductViewModel>>($"Ошибка при получении товаров категории: {ex.Message}");
            }
        }

        public async Task<BaseResponse<IEnumerable<ProductViewModel>>> GetProductsBySellerAsync(int sellerId)
        {
            try
            {
                var products = await _productRepository.GetProductsBySellerAsync(sellerId);
                var viewModels = products.Select(MapToViewModel).ToList();
                return CreatorResponse.Ok((IEnumerable<ProductViewModel>)viewModels);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<IEnumerable<ProductViewModel>>($"Ошибка при получении товаров продавца: {ex.Message}");
            }
        }

        public async Task<BaseResponse<IEnumerable<ProductViewModel>>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return CreatorResponse.BadRequest<IEnumerable<ProductViewModel>>("Введите поисковый запрос");

                var products = await _productRepository.SearchProductsByNameAsync(searchTerm);
                var viewModels = products.Select(MapToViewModel).ToList();
                return CreatorResponse.Ok((IEnumerable<ProductViewModel>)viewModels);
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<IEnumerable<ProductViewModel>>($"Ошибка при поиске: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> UpdateProductAsync(int productId, EditProductViewModel model, int userId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                    return CreatorResponse.NotFound<bool>("Товар не найден");

                var hasRight = await CheckProductAccessAsync(product, userId);
                if (!hasRight)
                    return CreatorResponse.Forbidden<bool>("Недостаточно прав для редактирования товара");

                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Stockquantity = model.StockQuantity;
                product.Categoryid = model.CategoryId;

                if (model.NewMainImage != null && model.NewMainImage.Length > 0)
                {
                    if (!string.IsNullOrEmpty(product.Mainimageurl))
                        _imageService.DeleteImage(product.Mainimageurl);

                    var newUrl = await _imageService.SaveImageAsync(model.NewMainImage, model.NewMainImageFileName ?? "image.jpg");
                    product.Mainimageurl = newUrl;
                }

                if (model.ImagesToDelete != null && model.ImagesToDelete.Any())
                {
                    foreach (var imageId in model.ImagesToDelete)
                    {
                        var image = await _productImageRepository.GetByIdAsync(imageId);
                        if (image != null && image.Productid == productId)
                        {
                            _imageService.DeleteImage(image.Imageurl);
                            await _productImageRepository.DeleteAsync(image);
                        }
                    }
                }

                if (model.NewAdditionalImages != null && model.NewAdditionalImages.Any())
                {
                    foreach (var imageData in model.NewAdditionalImages)
                    {
                        if (imageData.ImageBytes != null && imageData.ImageBytes.Length > 0)
                        {
                            var imageUrl = await _imageService.SaveImageAsync(imageData.ImageBytes, imageData.FileName);
                            var productImage = new Productimage
                            {
                                Productid = productId,
                                Imageurl = imageUrl,
                                Sortorder = 0
                            };
                            await _productImageRepository.CreateAsync(productImage);
                        }
                    }
                }

                await _productRepository.UpdateAsync(product);
                return CreatorResponse.Ok(true, "Товар успешно обновлён");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при обновлении товара: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> DeactivateProductAsync(int productId, int userId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                    return CreatorResponse.NotFound<bool>("Товар не найден");

                var hasRight = await CheckProductAccessAsync(product, userId);
                if (!hasRight)
                    return CreatorResponse.Forbidden<bool>("Недостаточно прав");

                product.Isactive = false;
                await _productRepository.UpdateAsync(product);

                return CreatorResponse.Ok(true, "Товар деактивирован");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при деактивации товара: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> ActivateProductAsync(int productId, int userId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                    return CreatorResponse.NotFound<bool>("Товар не найден");

                var hasRight = await CheckProductAccessAsync(product, userId);
                if (!hasRight)
                    return CreatorResponse.Forbidden<bool>("Недостаточно прав");

                product.Isactive = true;
                await _productRepository.UpdateAsync(product);

                return CreatorResponse.Ok(true, "Товар активирован");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при активации товара: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> UpdateStockAsync(int productId, int quantity, int userId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                    return CreatorResponse.NotFound<bool>("Товар не найден");

                var hasRight = await CheckProductAccessAsync(product, userId);
                if (!hasRight)
                    return CreatorResponse.Forbidden<bool>("Недостаточно прав");

                if (quantity < 0)
                    return CreatorResponse.BadRequest<bool>("Количество не может быть отрицательным");

                product.Stockquantity = quantity;
                await _productRepository.UpdateAsync(product);

                return CreatorResponse.Ok(true, "Количество обновлено");
            }
            catch (Exception ex)
            {
                return CreatorResponse.InternalError<bool>($"Ошибка при обновлении остатка: {ex.Message}");
            }
        }

        private async Task<bool> CheckProductAccessAsync(Product product, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (user.Role == UserRole.Administrator) return true;

            var seller = await _sellerRepository.GetByUserIdAsync(userId);
            return seller != null && product.Sellerid == seller.Id;
        }

        private ProductViewModel MapToViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.Stockquantity,
                MainImageUrl = product.Mainimageurl,
                CategoryName = product.Category?.Name ?? "",
                SellerName = product.Seller?.Storename ?? "",
                IsActive = product.Isactive ?? false,
                CreatedAt = product.Createdat,
                Images = product.Productimages?.Select(img => img.Imageurl).ToList() ?? new List<string>()
            };
        }

        private ProductDetailViewModel MapToDetailViewModel(Product product)
        {
            return new ProductDetailViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.Stockquantity,
                MainImageUrl = product.Mainimageurl,
                CategoryName = product.Category?.Name ?? "",
                SellerName = product.Seller?.Storename ?? "",
                IsActive = product.Isactive ?? false,
                CreatedAt = product.Createdat,
                Images = product.Productimages?.Select(img => img.Imageurl).ToList() ?? new List<string>(),
                CategoryId = product.Categoryid,
                SellerId = product.Sellerid,
                SellerDescription = product.Seller?.Description
            };
        }
    }
}

public class ImageService
{
    private readonly string _webRootPath;
    private const string PRODUCT_IMAGES_FOLDER = "uploads/products";

    public ImageService(string webRootPath)
    {
        _webRootPath = webRootPath;
    }

    public async Task<string> SaveImageAsync(byte[] imageBytes, string fileName)
    {
        if (imageBytes == null || imageBytes.Length == 0)
            throw new ArgumentException("Изображение не может быть пустым");

        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid()}{extension}";

        var uploadPath = Path.Combine(_webRootPath, PRODUCT_IMAGES_FOLDER);
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, uniqueName);
        await File.WriteAllBytesAsync(filePath, imageBytes);

        return $"/{PRODUCT_IMAGES_FOLDER}/{uniqueName}";
    }

    public void DeleteImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return;

        var filePath = Path.Combine(_webRootPath, imageUrl.TrimStart('/'));
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}