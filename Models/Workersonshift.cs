using System;
using System.Collections.Generic;

namespace cafeapp1.Models;

public partial class Workersonshift
{
    public int Id { get; set; }

    public int? Shiftid { get; set; }

    public int? Workerid { get; set; }

    public virtual Shift? Shift { get; set; }

    public virtual User? Worker { get; set; }
}
