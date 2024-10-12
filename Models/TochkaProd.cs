using System;
using System.Collections.Generic;

namespace Agents.Models;

public partial class TochkaProd
{
    public int IdTochka { get; set; }

    public string? NameTochka { get; set; }

    public virtual ICollection<Agent> IdAgents { get; set; } = new List<Agent>();
}
