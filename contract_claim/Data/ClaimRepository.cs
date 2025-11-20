using System.Text.Json;
using contract_claim.Models;

namespace contract_claim.Data
{
    public static class ClaimRepository
    {
        // ALWAYS save claims.json inside the actual project folder
        private static readonly string FilePath =
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "App_Data", "claims.json");

        static ClaimRepository()
        {
            // Resolve absolute path
            FilePath = Path.GetFullPath(FilePath);

            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            // Ensure file exists
            if (!File.Exists(FilePath))
                File.WriteAllText(FilePath, "[]");
        }

        public static List<Claim> GetAll()
        {
            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Claim>>(json) ?? new List<Claim>();
        }

        public static void SaveAll(List<Claim> claims)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(claims, options);
            File.WriteAllText(FilePath, json);
        }

        public static void Add(Claim claim)
        {
            var claims = GetAll();
            claim.Id = claims.Any() ? claims.Max(c => c.Id) + 1 : 1;
            claims.Add(claim);
            SaveAll(claims);
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

            claims.RemoveAll(c =>
                c.LecturerName.Equals(lecturerName, StringComparison.OrdinalIgnoreCase));

            SaveAll(claims);
        }
        public static void DeleteClaim(int id, string lecturerName)
        {
            var claims = GetAll();

            var claim = claims.FirstOrDefault(c =>
                c.Id == id &&
                c.LecturerName.Equals(lecturerName, StringComparison.OrdinalIgnoreCase));

            if (claim != null)
            {
                claims.Remove(claim);
                SaveAll(claims);
            }
        }

    }
}

