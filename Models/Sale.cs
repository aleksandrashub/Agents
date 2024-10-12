using System;
using System.Collections.Generic;

namespace Agents.Models;

public partial class Sale
{
    public int IdSale { get; set; }

    public int? IdAgent { get; set; }

    public int? Amount { get; set; }

    public DateOnly? Date { get; set; }

    public int? IdProduct { get; set; }

    public virtual Agent? IdAgentNavigation { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}
