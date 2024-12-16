using System;
using System.Collections.Generic;

namespace cafeapp1.Models;

public partial class User
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Fname { get; set; } = null!;

    public string Lname { get; set; } = null!;

    public string Mname { get; set; } = null!;

    public string? Photo { get; set; }

    public string? Doc { get; set; }

    public int? Roleid { get; set; }

    public bool Status { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<Waiterontable> Waiterontables { get; set; } = new List<Waiterontable>();

    public virtual ICollection<Workersonshift> Workersonshifts { get; set; } = new List<Workersonshift>();
}
