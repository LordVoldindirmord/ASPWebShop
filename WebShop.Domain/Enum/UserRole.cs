using System;
using System.Collections.Generic;
using System.Text;

namespace WebShop.Domain.Enum
{
    public enum UserRole
    {
        /// <summary>
        /// Покупатель (обычный пользователь)
        /// </summary>
        Customer,

        /// <summary>
        /// Продавец (может управлять своими товарами)
        /// </summary>
        Seller,

        /// <summary>
        /// Администратор (полный доступ ко всему)
        /// </summary>
        Administrator
    }
}
