using System;
using System.Collections.Generic;

namespace WebShop.Domain.Entity;

public partial class Productimage
{
    public int Id { get; set; }

    public int Productid { get; set; }

    public string Imageurl { get; set; } = null!;

    public int? Sortorder { get; set; }

    public virtual Product Product { get; set; } = null!;
}
