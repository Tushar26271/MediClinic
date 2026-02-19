using System;
using System.Collections.Generic;

namespace MediClinic.Models;

public partial class PhysicianPrescrip
{
    public int PhysicianPrescripId { get; set; }

    public int PhysicianAdviceId { get; set; }

    public int DrugId { get; set; }

    public string? Prescription { get; set; }

    public string? Dosage { get; set; }

    public virtual Drug Drug { get; set; } = null!;

    public virtual PhysicianAdvice PhysicianAdvice { get; set; } = null!;
}