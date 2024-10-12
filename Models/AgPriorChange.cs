using System;
using System.Collections.Generic;

namespace Agents.Models;

public partial class AgPriorChange
{
    public int IdPriorChange { get; set; }

    public int? IdAgent { get; set; }

    public int? IdNewPrior { get; set; }

    public virtual Priority? IdNewPriorNavigation { get; set; }
}
