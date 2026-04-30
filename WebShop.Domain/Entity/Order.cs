using System;
using System.Collections.Generic;
using WebShop.Domain.Enum;

namespace WebShop.Domain.Entity;

public partial class Order
{
    public int Id { get; set; }

    public int Userid { get; set; }

    public DateTime? Orderdate { get; set; }

    public decimal Totalamount { get; set; }

    public string Shippingaddress { get; set; } = null!;

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual User User { get; set; } = null!;

    public OrderStatus Status { get; set; }
}
