using contract_claim.Models;

namespace contract_claim.Services
{
    public static class ClaimPolicyService
    {
        public class PolicyResult
        {
            public bool IsValid { get; set; }
            public bool AutoReject { get; set; }
            public List<string> Warnings { get; set; } = new();
        }

        public static PolicyResult ValidateClaim(Claim claim, List<Claim> allClaims)
        {
            var result = new PolicyResult { IsValid = true };

            // RULE 1 — Max hours
            if (claim.HoursWorked > 300)
            {
                result.IsValid = false;
                result.AutoReject = true;
                result.Warnings.Add("Hours worked exceeds the maximum allowed (300).");
            }

            // RULE 2 — Hourly Rate Range
            if (claim.HourlyRate < 100 || claim.HourlyRate > 1000)
            {
                result.IsValid = false;
                result.AutoReject = true;
                result.Warnings.Add("Hourly rate outside permitted range (R100 - R1000).");
            }

            // RULE 3 — Missing document
            if (string.IsNullOrEmpty(claim.FileName))
            {
                result.Warnings.Add("No supporting document uploaded.");
            }

            // RULE 4 — Large payout flag
            if (claim.TotalAmount > 15000)
            {
                result.Warnings.Add("Large payout detected (> R15,000). Requires careful review.");
            }

            bool duplicate = allClaims.Any(c =>
    c.Id != claim.Id &&
    c.LecturerName == claim.LecturerName &&
    c.SubmittedDate.ToString("yyyy-MM") == claim.SubmittedDate.ToString("yyyy-MM")
);

            if (duplicate)
            {
                result.IsValid = false;
                result.AutoReject = true;
                result.Warnings.Add("Duplicate claim detected for this month.");
            }

            return result;
        }
    }
}