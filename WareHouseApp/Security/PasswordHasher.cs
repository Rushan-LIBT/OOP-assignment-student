using System;
using System.Security.Cryptography;
using System.Text;

namespace WareHouseApp.Security
{
    /// <summary>
    /// Hashes passwords with SHA-256 so plain text passwords are never stored.
    /// </summary>
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            if (password == null)
            {
                password = string.Empty;
            }

            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static bool Verify(string password, string storedHash)
        {
            return string.Equals(Hash(password), storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
