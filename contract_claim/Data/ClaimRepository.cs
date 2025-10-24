using System.Text.Json;
using contract_claim.Models;

namespace contract_claim.Data
{
    public static class ClaimRepository
    {
        private static readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "claims.json");

        static ClaimRepository()
        {
            // Ensure directory exists
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            // Create empty file if it doesn't exist
            if (!File.Exists(FilePath))
                File.WriteAllText(FilePath, "[]");
        }

        public static List<Claim> GetAll()
        {
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Claim>>(json) ?? new List<Claim>();
        }

        public static void SaveAll(List<Claim> claims)
        {
            var json = JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static void Update(Claim updatedClaim)
        {
            var claims = GetAll();
            var index = claims.FindIndex(c => c.Id == updatedClaim.Id);
            if (index >= 0)
            {
                claims[index] = updatedClaim;
                SaveAll(claims);
            }
        }
        public static void ClearClaimsForLecturer(string lecturerName)
        {
            var claims = GetAll();
            claims.RemoveAll(c => c.LecturerName.Equals(lecturerName, StringComparison.OrdinalIgnoreCase));
            SaveAll(claims);
        }

        public static void DeleteClaim(int id, string lecturerName)
        {
            var claims = GetAll();
            var claim = claims.FirstOrDefault(c => c.Id == id &&
                         c.LecturerName.Equals(lecturerName, StringComparison.OrdinalIgnoreCase));

            if (claim != null)
            {
                claims.Remove(claim);
                SaveAll(claims);
            }
        }

        public static void Add(Claim claim)
        {
            var claims = GetAll();
            claim.Id = claims.Any() ? claims.Max(c => c.Id) + 1 : 1;
            claims.Add(claim);
            SaveAll(claims);
        }
    }
}
