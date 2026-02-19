using System;
using System.Collections.Generic;

namespace MediClinic.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int PhysicianId { get; set; }

    public int AppointmentId { get; set; }

    public DateOnly? ScheduleDate { get; set; }

    public TimeOnly? ScheduleTime { get; set; }

    public string? ScheduleStatus { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Physician Physician { get; set; } = null!;

    public virtual ICollection<PhysicianAdvice> PhysicianAdvices { get; set; } = new List<PhysicianAdvice>();
}
