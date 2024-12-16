using System;
using System.Collections.Generic;

namespace cafeapp1.Models;

public partial class Waiterontable
{
    public int Id { get; set; }

    public int? Idwaiter { get; set; }

    public int? Idtable { get; set; }

    public virtual Table? IdtableNavigation { get; set; }

    public virtual User? IdwaiterNavigation { get; set; }
}
