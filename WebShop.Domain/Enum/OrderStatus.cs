using System;
using System.Collections.Generic;
using System.Text;

namespace WebShop.Domain.Enum
{
    public enum OrderStatus
    {
        /// <summary>
        /// Ожидает обработки (новый заказ)
        /// </summary>
        Pending,

        /// <summary>
        /// В обработке (продавец начал сборку)
        /// </summary>
        Processing,

        /// <summary>
        /// Отправлен (передан в службу доставки)
        /// </summary>
        Shipped,

        /// <summary>
        /// Доставлен (покупатель получил заказ)
        /// </summary>
        Delivered,

        /// <summary>
        /// Отменён (покупателем или продавцом)
        /// </summary>
        Cancelled
    }
}
