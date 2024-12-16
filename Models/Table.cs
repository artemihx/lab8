using System;
using System.Collections.Generic;

namespace cafeapp1.Models;

public partial class Table
{
    public int Id { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Waiterontable> Waiterontables { get; set; } = new List<Waiterontable>();
}
