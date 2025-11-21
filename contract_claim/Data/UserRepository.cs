using System.Text.Json;
using contract_claim.Models;

namespace contract_claim.Data
{
    public static class UserRepository
    {
        private static readonly string FilePath =
            Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "users.json");

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

        public static User? GetById(int id)
        {
            return GetAll().FirstOrDefault(u => u.Id == id);
        }

        public static bool Exists(string email)
        {
            return GetAll().Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public static void Update(User updated)
        {
            var users = GetAll();
            var existing = users.FirstOrDefault(u => u.Id == updated.Id);

            if (existing == null) return;

            existing.Username = updated.Username;
            existing.Email = updated.Email;
            existing.Role = updated.Role;
            existing.Password = updated.Password;  
            existing.HourlyRate = updated.HourlyRate;

            SaveAll(users);
        }

        public static User? Find(string email, string password)
        {
            var users = GetAll();
            return users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                && u.Password == password
            );
        }


        public static void Delete(int id)
        {
            var users = GetAll();
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                users.Remove(user);
                SaveAll(users);
            }
        }
    }
}