using ASPWebShop.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShop.Domain.ViewModel.ProductModel;
using WebShop.Domain.ViewModel.SellerModel;
using WebShop.Service.Interfaces;

namespace ASPWebShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISellerService _sellerService;

        public ProductController(IProductService productService, ICategoryService categoryService, ISellerService sellerService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _sellerService = sellerService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MainMenu()
        {
            ViewBag.UserName = this.GetUserName();
            ViewBag.UserRole = this.GetUserRole();

            var response = await _productService.GetActiveProductsAsync();

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Description ?? "Ошибка на стороне сервера при получении товаров";
                return View(new List<ProductViewModel>());
            }

            return View(response.Data);
        }

        [Authorize(Roles = "Seller,Administrator")]
        [HttpGet]
        public async Task<IActionResult> MyProducts()
        {
            var userId = this.GetUserId();
            var userRole = this.GetUserRole();

            if (userRole == "Administrator")
            {
                var allResponse = await _productService.GetActiveProductsAsync();
                ViewBag.IsAdmin = true;
                return View("MyProducts", allResponse.IsSuccess ? allResponse.Data : new List<ProductViewModel>());
            }

            var sellerResponse = await _sellerService.GetSellerByUserIdAsync(userId);
            if (!sellerResponse.IsSuccess || sellerResponse.Data == null)
            {
                TempData["Error"] = "Профиль продавца не найден";
                return RedirectToAction("MainMenu");
            }

            var productsResponse = await _productService.GetProductsBySellerAsync(sellerResponse.Data.Id);

            if (!productsResponse.IsSuccess)
            {
                TempData["Error"] = productsResponse.Description ?? "Ошибка при загрузке товаров";
                return View("MyProducts", new List<ProductViewModel>());
            }

            ViewBag.IsAdmin = false;
            ViewBag.SellerId = sellerResponse.Data.Id;
            return View("MyProducts", productsResponse.Data);
        }

        [Authorize(Roles = "Seller,Administrator")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categoriesResponse = await _categoryService.GetAllCategoriesAsync();
            if (categoriesResponse.IsSuccess && categoriesResponse.Data != null)
            {
                ViewBag.Categories = categoriesResponse.Data;
            }

            return View();
        }

        [Authorize(Roles = "Seller,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel model, IFormFile? mainImage, List<IFormFile>? additionalImages)
        {
            if (!ModelState.IsValid)
            {
                var categoriesResponse = await _categoryService.GetAllCategoriesAsync();
                if (categoriesResponse.IsSuccess && categoriesResponse.Data != null)
                {
                    ViewBag.Categories = categoriesResponse.Data;
                }
                return View(model);
            }

            var userId = this.GetUserId();
            int sellerId;
            var userRole = this.GetUserRole();

            if (userRole == "Administrator")
            {
                var sellerResponse = await _sellerService.GetSellerByUserIdAsync(userId);
                if (!sellerResponse.IsSuccess || sellerResponse.Data == null)
                {
                    var becomeResponse = await _sellerService.BecomeSellerAsync(userId, new CreateSellerViewModel
                    {
                        StoreName = "Администратор",
                        Description = "Товары от администрации"
                    });

                    if (!becomeResponse.IsSuccess)
                    {
                        TempData["Error"] = "Не удалось создать профиль продавца";
                        return View(model);
                    }

                    sellerId = (await _sellerService.GetSellerByUserIdAsync(userId)).Data!.Id;
                }
                else
                {
                    sellerId = sellerResponse.Data.Id;
                }
            }
            else
            {
                var sellerResponse = await _sellerService.GetSellerByUserIdAsync(userId);
                if (!sellerResponse.IsSuccess || sellerResponse.Data == null)
                {
                    TempData["Error"] = "Профиль продавца не найден";
                    return View(model);
                }
                sellerId = sellerResponse.Data.Id;
            }

            if (mainImage != null)
            {
                using (var ms = new MemoryStream())
                {
                    await mainImage.CopyToAsync(ms);
                    model.MainImage = ms.ToArray();
                }
                model.MainImageFileName = mainImage.FileName;
            }

            if (additionalImages != null && additionalImages.Any())
            {
                model.AdditionalImages = new List<ProductImageData>();
                foreach (var image in additionalImages)
                {
                    using (var ms = new MemoryStream())
                    {
                        await image.CopyToAsync(ms);
                        model.AdditionalImages.Add(new ProductImageData
                        {
                            ImageBytes = ms.ToArray(),
                            FileName = image.FileName
                        });
                    }
                }
            }

            var response = await _productService.CreateProductAsync(model, sellerId);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Description ?? "Ошибка при создании товара";
                return View(model);
            }

            TempData["Success"] = "Товар успешно создан!";
            return RedirectToAction("MyProducts");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _productService.GetProductDetailsAsync(id);

            if (!response.IsSuccess || response.Data == null)
            {
                TempData["Error"] = response.Description ?? "Товар не найден";
                return RedirectToAction("MainMenu");
            }

            ViewBag.UserRole = this.GetUserRole();
            return View(response.Data);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            ViewBag.SearchTerm = searchTerm;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("Search", new List<ProductViewModel>());
            }

            var response = await _productService.SearchProductsAsync(searchTerm);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Description ?? "Ошибка при поиске товаров";
                return View("Search", new List<ProductViewModel>());
            }

            return View("Search", response.Data);
        }

        [Authorize(Roles = "Seller,Administrator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _productService.GetProductDetailsAsync(id);

            if (!response.IsSuccess || response.Data == null)
            {
                TempData["Error"] = "Товар не найден";
                return RedirectToAction("MyProducts");
            }

            var product = response.Data;

            var categoriesResponse = await _categoryService.GetAllCategoriesAsync();
            if (categoriesResponse.IsSuccess && categoriesResponse.Data != null)
            {
                ViewBag.Categories = categoriesResponse.Data;
            }

            var model = new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                ExistingMainImageUrl = product.MainImageUrl,
                ExistingAdditionalImages = product.Images
            };

            return View(model);
        }

        [Authorize(Roles = "Seller,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProductViewModel model,
            IFormFile? newMainImage, List<IFormFile>? newAdditionalImages, List<int>? imagesToDelete)
        {
            var userId = this.GetUserId();

            if (!ModelState.IsValid)
            {
                var categoriesResponse = await _categoryService.GetAllCategoriesAsync();
                if (categoriesResponse.IsSuccess && categoriesResponse.Data != null)
                    ViewBag.Categories = categoriesResponse.Data;
                return View(model);
            }

            if (newMainImage != null && newMainImage.Length > 0)
            {
                using var ms = new MemoryStream();
                await newMainImage.CopyToAsync(ms);
                model.NewMainImage = ms.ToArray();
                model.NewMainImageFileName = newMainImage.FileName;
            }

            if (newAdditionalImages != null && newAdditionalImages.Any())
            {
                model.NewAdditionalImages = new List<ProductImageData>();
                foreach (var image in newAdditionalImages)
                {
                    if (image.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await image.CopyToAsync(ms);
                        model.NewAdditionalImages.Add(new ProductImageData
                        {
                            ImageBytes = ms.ToArray(),
                            FileName = image.FileName
                        });
                    }
                }
            }

            model.ImagesToDelete = imagesToDelete;

            var response = await _productService.UpdateProductAsync(id, model, userId);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Description ?? "Ошибка при обновлении товара";
                return RedirectToAction("Edit", new { id });
            }

            TempData["Success"] = "Товар успешно обновлён";
            return RedirectToAction("MyProducts");
        }

        /// <summary>
        /// Переключение статуса товара (скрыть/показать)
        /// </summary>
        [Authorize(Roles = "Seller,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleProductStatus(int productId)
        {
            var userId = this.GetUserId();

            var productResponse = await _productService.GetProductDetailsAsync(productId);

            if (!productResponse.IsSuccess || productResponse.Data == null)
            {
                TempData["Error"] = "Товар не найден";
                return RedirectToAction("MyProducts");
            }

            if (productResponse.Data.IsActive)
            {
                var response = await _productService.DeactivateProductAsync(productId, userId);
                TempData["Success"] = response.IsSuccess ? "Товар скрыт" : response.Description;
            }
            else
            {
                var response = await _productService.ActivateProductAsync(productId, userId);
                TempData["Success"] = response.IsSuccess ? "Товар снова активен" : response.Description;
            }

            return RedirectToAction("MyProducts");
        }
    }
}