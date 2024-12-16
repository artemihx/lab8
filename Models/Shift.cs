using System;
using System.Collections.Generic;

namespace cafeapp1.Models;

public partial class Shift
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Workersonshift> Workersonshifts { get; set; } = new List<Workersonshift>();
}
