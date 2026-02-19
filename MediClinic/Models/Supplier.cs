using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediClinic.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    [Required(ErrorMessage = "Supplier Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Supplier Name must be between 3 and 100 characters")]
    public string SupplierName { get; set; } = null!;


    [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
    public string? Address { get; set; }


    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
    public string? Phone { get; set; }


    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Enter a valid email address")]
    public string? Email { get; set; }


    public string? SupplierStatus { get; set; }

    public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; } = new List<PurchaseOrderHeader>();
}
