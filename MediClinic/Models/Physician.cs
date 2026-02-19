using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediClinic.Models;

public partial class Physician
{
    public int PhysicianId { get; set; }

    [Required(ErrorMessage = "Physician Name is required")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Name must be between 3 and 100 characters")]
    public string PhysicianName { get; set; } = null!;


    [Required(ErrorMessage = "Specialization is required")]
    [StringLength(100)]
    public string? Specialization { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
    public string? Address { get; set; }


    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[0-9]{10}$",
        ErrorMessage = "Phone number must be exactly 10 digits")]
    public string? Phone { get; set; }


    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "Enter a valid email address")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Summary is required")]
    [StringLength(500, ErrorMessage = "Summary cannot exceed 500 characters")]
    public string? Summary { get; set; }


    
    public string? PhysicianStatus { get; set; }

    public virtual ICollection<DrugRequest> DrugRequests { get; set; } = new List<DrugRequest>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
