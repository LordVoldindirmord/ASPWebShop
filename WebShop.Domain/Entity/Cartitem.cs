using System;
using System.Collections.Generic;

namespace WebShop.Domain.Entity;

public partial class Cartitem
{
    public int Id { get; set; }

    public int Userid { get; set; }

    public int Productid { get; set; }

    public int Quantity { get; set; }

    public DateTime? Addedat { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
