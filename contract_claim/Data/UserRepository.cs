using System.Text.Json;
using contract_claim.Models;

namespace contract_claim.Data
{
    public static class UserRepository
    {
        private static readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "users.json");

        static UserRepository()
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (!File.Exists(FilePath)) File.WriteAllText(FilePath, "[]");
        }

        public static List<User> GetAll()
        {
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        public static void SaveAll(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static void Add(User user)
        {
            var users = GetAll();
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            SaveAll(users);
        }

        public static User? Find(string email, string password)
        {
            var users = GetAll();
            return users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.Password == password);
        }

        public static bool Exists(string email)
        {
            var users = GetAll();
            return users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
    }
}
