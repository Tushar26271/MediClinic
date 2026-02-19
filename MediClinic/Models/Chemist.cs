using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediClinic.Models;

public partial class Chemist
{
    public int ChemistId { get; set; }

    [Required(ErrorMessage = "Chemist Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Chemist Name must be between 3 and 100 characters")]
    public string ChemistName { get; set; } = null!;


    [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
    public string? Address { get; set; }


    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
    public string? Phone { get; set; }


    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Enter a valid email address")]
    public string? Email { get; set; }


    [StringLength(500, ErrorMessage = "Summary cannot exceed 500 characters")]
    public string? Summary { get; set; }


    public string? ChemistStatus { get; set; }
}
