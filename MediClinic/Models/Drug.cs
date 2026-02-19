using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediClinic.Models;

public partial class Drug
{
    public int DrugId { get; set; }
    [Required]
    public string DrugTitle { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? Expiry { get; set; }

    public string? Dosage { get; set; }

    public string? DrugStatus { get; set; }

    public virtual ICollection<PhysicianPrescrip> PhysicianPrescrips { get; set; } = new List<PhysicianPrescrip>();

    public virtual ICollection<PurchaseProductLine> PurchaseProductLines { get; set; } = new List<PurchaseProductLine>();
}