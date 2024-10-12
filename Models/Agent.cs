using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Agents.Models;

public partial class Agent
{
    public int IdAgent { get; set; }

    public string? Name { get; set; }

    public string? Logo { get; set; }

    public string? Address { get; set; }

    public long? Inn { get; set; }

    public long? Kpp { get; set; }

    public string? Director { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? IdType { get; set; }

    public int? IdPriority { get; set; }

    public int? IdDiscount { get; set; }

    public virtual Discount? IdDiscountNavigation { get; set; }

    public virtual Priority? IdPriorityNavigation { get; set; }

    public virtual Type? IdTypeNavigation { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual ICollection<TochkaProd> IdTochkaPrs { get; set; } = new List<TochkaProd>();
    public int allAmount => Sales.Sum(x => x.Amount).Value;
    public Bitmap? bitmap => Logo != null ? new Bitmap($@"Assets\\{Logo}") : null;
    public SolidColorBrush? Color => IdDiscountNavigation.NameDisc=="25"
        ? new SolidColorBrush(Colors.LightGreen)
        : new SolidColorBrush(Colors.White);

}
