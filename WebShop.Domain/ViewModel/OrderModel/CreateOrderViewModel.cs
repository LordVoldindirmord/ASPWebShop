using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebShop.Domain.ViewModel.OrderModel
{
    /// <summary>
    /// Модель для оформления заказа
    /// </summary>
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Адрес доставки обязателен")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Адрес должен быть от 10 до 500 символов")]
        [Display(Name = "Адрес доставки")]
        [DataType(DataType.MultilineText)]
        public string ShippingAddress { get; set; } = null!;
    }
}
