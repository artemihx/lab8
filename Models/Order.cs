using System;
using System.Collections.Generic;

namespace cafeapp1.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int? Status { get; set; }

    public int? Tableid { get; set; }

    public int? Shiftid { get; set; }

    public virtual ICollection<Foodonorder> Foodonorders { get; set; } = new List<Foodonorder>();

    public virtual Shift? Shift { get; set; }

    public virtual Orderstatus? StatusNavigation { get; set; }

    public virtual Table? Table { get; set; }
}
