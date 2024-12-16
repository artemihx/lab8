using System;
using System.Collections.Generic;

namespace cafeapp1.Models;

public partial class Foodonorder
{
    public int Id { get; set; }

    public int? Idfood { get; set; }

    public int? Idorder { get; set; }

    public virtual Food? IdfoodNavigation { get; set; }

    public virtual Order? IdorderNavigation { get; set; }
}
