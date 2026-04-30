using System;
using System.Collections.Generic;

namespace WebShop.Domain.Entity;

public partial class Seller
{
    public int Id { get; set; }

    public int Userid { get; set; }

    public string Storename { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual User User { get; set; } = null!;
}
