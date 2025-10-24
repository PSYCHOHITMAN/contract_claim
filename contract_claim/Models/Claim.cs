﻿namespace contract_claim.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public string LecturerName { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public string Notes { get; set; }
        public string FileName { get; set; } // uploaded document
        public string Status { get; set; } = "Pending"; // Pending / Approved / Rejected

        public double TotalAmount => HoursWorked * HourlyRate;
    }

}

