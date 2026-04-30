using System;
using System.Collections.Generic;
using WebShop.Domain.Enum;

namespace WebShop.Domain.Entity;

public partial class User
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string? Phonenumber { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual ICollection<Cartitem> Cartitems { get; set; } = new List<Cartitem>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Seller? Seller { get; set; }

    public UserRole Role { get; set; }
}
