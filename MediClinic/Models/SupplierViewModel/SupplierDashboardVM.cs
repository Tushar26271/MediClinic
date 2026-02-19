using MediClinic.Models;

namespace MediClinic.ViewModels
{
    public class SupplierDashboardVM
    {
        public string SupplierName { get; set; }
        public int TotalAssignedPO { get; set; }
        public List<PurchaseOrderHeader> RecentPO { get; set; }
    }
}
