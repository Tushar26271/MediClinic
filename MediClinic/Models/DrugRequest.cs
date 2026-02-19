
using System;
using System.Collections.Generic;

namespace MediClinic.Models;

public partial class DrugRequest
{
    public int DrugRequestId { get; set; }

    public int PhysicianId { get; set; }

    public string? DrugsInfoText { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? RequestStatus { get; set; }

    public virtual Physician Physician { get; set; } = null!;
}
