using System;
using System.Collections.Generic;

namespace Agents.Models;

public partial class Discount
{
    public int IdDiscount { get; set; }

    public string? NameDisc { get; set; }

    public virtual ICollection<Agent> Agents { get; set; } = new List<Agent>();
}
