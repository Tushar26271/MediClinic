//using Microsoft.AspNetCore.Mvc.Rendering;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MediClinic.Models.ChemistViewModel
{
    //    //public class PurchaseOrderVM
    //    //{
    //    //    public int Poid { get; set; }

    //    //    [Display(Name = "PO Number")]
    //    //    public string PONo { get; set; } = string.Empty;

    //    //    [Display(Name = "PO Date")]
    //    //    public DateTime PODate { get; set; }

    //    //    [Display(Name = "Supplier")]
    //    //    [Required(ErrorMessage = "Please select a supplier")]
    //    //    public int SupplierId { get; set; }

    //    //    // List of suppliers for dropdown
    //    //    public SelectList? Suppliers { get; set; }

    //    //    // Product lines
    //    //    [Required]
    //    //    public List<PurchaseLineVM> ProductLines { get; set; } = new List<PurchaseLineVM>();
    //    //}
    //    public class PurchaseOrderVM
    //    {
    //        public string PONo { get; set; }
    //        public DateTime PODate { get; set; }
    //        public int SupplierId { get; set; }

    //        public SelectList Suppliers { get; set; }  // <--- SelectList type

    //        public List<PurchaseLineVM> ProductLines { get; set; }
    //    }

    //    public class PurchaseLineVM
    //    {
    //        public int SlNo { get; set; }

    //        [Display(Name = "Drug")]
    //        [Required(ErrorMessage = "Please select a drug")]
    //        public int DrugId { get; set; }

    //        [Display(Name = "Quantity")]
    //        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    //        public int Qty { get; set; }

    //        public string? Note { get; set; }
    //    }
    //}

    public class PurchaseOrderVM
    {
        public string PONo { get; set; }
        public DateTime PODate { get; set; }
        public int SupplierId { get; set; }
        public List<PurchaseLineVM> ProductLines { get; set; } = new List<PurchaseLineVM>();
        public SelectList Suppliers { get; set; }
    }

    public class PurchaseLineVM
    {
        public int SlNo { get; set; }
        public int DrugId { get; set; }
        public int Qty { get; set; }
        public string Note { get; set; }
    }
}