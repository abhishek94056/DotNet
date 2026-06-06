using System.ComponentModel.DataAnnotations;

namespace InvoiceGenerator.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
