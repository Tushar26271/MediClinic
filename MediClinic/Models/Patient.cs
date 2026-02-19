using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediClinic.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Patient Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string PatientName { get; set; } = null!;

    [Required(ErrorMessage = "Date of Birth is required")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateOnly? Dob { get; set; }

    [Required(ErrorMessage = "Please select gender")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [StringLength(200)]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone must be 10 digits")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
    ErrorMessage = "Enter a valid email address (example: user@gmail.com)")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }

    [StringLength(500, ErrorMessage = "Summary cannot exceed 500 characters")]
    public string? Summary { get; set; }

    public string? PatientStatus { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
