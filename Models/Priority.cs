using System;
using System.Collections.Generic;

namespace Agents.Models;

public partial class Priority
{
    public int IdPriority { get; set; }

    public string? NamePr { get; set; }

    public virtual ICollection<AgPriorChange> AgPriorChanges { get; set; } = new List<AgPriorChange>();

    public virtual ICollection<Agent> Agents { get; set; } = new List<Agent>();
}
