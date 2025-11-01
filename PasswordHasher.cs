using System;
using System.Security.Cryptography;

namespace EduSync
{
    internal class PasswordHasher
    {
        // Hash a password using PBKDF2 + salt
        public static string HashPassword(string password)
        {
            // Generate a random 16-byte salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Derive a 256-bit subkey (32 bytes) using PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            // Combine salt and hash
            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            // Return Base64 string of salt + hash
            return Convert.ToBase64String(hashBytes);
        }

        // Verify entered password against stored hash
        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extract salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Extract original hash
            byte[] storedHashBytes = new byte[32];
            Array.Copy(hashBytes, 16, storedHashBytes, 0, 32);

            // Hash the entered password with the extracted salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] newHash = pbkdf2.GetBytes(32);

            // Compare byte-by-byte
            for (int i = 0; i < 32; i++)
            {
                if (newHash[i] != storedHashBytes[i])
                    return false;
            }

            return true;
        }
    }
}
