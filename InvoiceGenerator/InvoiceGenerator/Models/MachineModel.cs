namespace InvoiceGenerator.Models
{
    public class MachineModel
    {
        public int Id { get; set; }
        public string MachineName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } // Working / Breakdown / Ideal
    }
}
