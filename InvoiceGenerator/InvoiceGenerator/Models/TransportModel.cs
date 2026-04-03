// Models/TransportMaster.cs
using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    public class TransportModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mode Name is required")]
        [StringLength(50, ErrorMessage = "Mode Name cannot exceed 50 characters")]
        public string ModeName { get; set; }
    }
}