using System;
using System.Collections.Generic;

namespace MediClinic.Models;

public partial class PhysicianAdvice
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }
    public int PhysicianAdviceId { get; set; }

    public int ScheduleId { get; set; }

    public string? Advice { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<PhysicianPrescrip> PhysicianPrescrips { get; set; } = new List<PhysicianPrescrip>();

    public virtual Schedule Schedule { get; set; } = null!;
    public Appointment? Appointment { get; set; }

}

