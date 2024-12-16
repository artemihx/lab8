using System;
using System.Collections.Generic;

namespace cafeapp1.Models;

public partial class Food
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Price { get; set; }

    public virtual ICollection<Foodonorder> Foodonorders { get; set; } = new List<Foodonorder>();
}
