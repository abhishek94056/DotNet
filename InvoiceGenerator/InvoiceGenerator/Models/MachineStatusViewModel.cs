namespace InvoiceGenerator.Models
{
    public class MachineStatusViewModel
    {
        public int Id { get; set; }
        public string MachineName { get; set; }
        public string Status { get; set; } // Idle / In-use / Breakdown / Shutdown / Preventive Maintenance
        public string Activity { get; set; } // e.g. "In-use", "Idle"
        public string WorkType { get; set; } // e.g. "Work", "No Work"
        public string ItemName { get; set; } // Part / item name
        public string AssignedTo { get; set; } // Operator name
        public int Counter1 { get; set; }
        public int Counter2 { get; set; }
        public int Counter3 { get; set; }
    }
}
