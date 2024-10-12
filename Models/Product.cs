using System;
using System.Collections.Generic;

namespace Agents.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string? NameProduct { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
