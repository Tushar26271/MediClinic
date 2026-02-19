using MediClinic.Models;
using System.Collections.Generic;

namespace MediClinic.ViewModels
{
    public class PatientDashboardViewModel
    {
        public Patient Patient { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<PhysicianAdvice> PhysicianAdvices { get; set; }
        public List<PhysicianPrescrip> PhysicianPrescrip { get; set; }
    }
}
