using System;
using System.Collections.Generic;

namespace Agents.Models;

public partial class Type
{
    public int IdType { get; set; }

    public string? NameType { get; set; }

    public virtual ICollection<Agent> Agents { get; set; } = new List<Agent>();
}
