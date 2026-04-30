using System;
using System.Collections.Generic;

namespace WebShop.Domain.Entity;

public partial class Orderitem
{
    public int Id { get; set; }

    public int Orderid { get; set; }

    public int? Productid { get; set; }

    public string Productname { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product? Product { get; set; }
}
