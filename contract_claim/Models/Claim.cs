namespace contract_claim.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public string LecturerName { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public string Notes { get; set; }
        public string FileName { get; set; } 
        public string Status { get; set; } = "Pending"; 
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime SubmittedDate { get; set; } = DateTime.Now;




        public double TotalAmount => HoursWorked * HourlyRate;
    }

}

