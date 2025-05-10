using System;
using System.Security.Cryptography;
using System.Text;

namespace HealthMonitoring.SharedKernel.Helpers
{
    /// <summary>
    /// Helper class for cryptography operations
    /// </summary>
    public static class CryptographyHelper
    {
        /// <summary>
        /// Hashes a password using PBKDF2
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <param name="salt">The salt (generated if null)</param>
        /// <returns>A tuple containing the hashed password and the salt</returns>
        public static (string hashedPassword, string salt) HashPassword(string password, string salt = null)
        {
            // Generate a salt if one is not provided
            if (string.IsNullOrEmpty(salt))
            {
                salt = GenerateRandomSalt();
            }

            using var deriveBytes = new Rfc2898DeriveBytes(
                password,
                Encoding.UTF8.GetBytes(salt),
                10000,
                HashAlgorithmName.SHA256);

            var hashedBytes = deriveBytes.GetBytes(32);
            var hashedPassword = Convert.ToBase64String(hashedBytes);

            return (hashedPassword, salt);
        }

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        /// <param name="password">The password to verify</param>
        /// <param name="hashedPassword">The hashed password</param>
        /// <param name="salt">The salt used to hash the password</param>
        /// <returns>True if the password is correct, false otherwise</returns>
        public static bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            var (computedHash, _) = HashPassword(password, salt);
            return computedHash == hashedPassword;
        }

        /// <summary>
        /// Generates a random salt for password hashing
        /// </summary>
        /// <returns>A random salt</returns>
        public static string GenerateRandomSalt()
        {
            var saltBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Computes the SHA256 hash of a string
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The SHA256 hash as a hex string</returns>
        public static string ComputeSha256(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha256.ComputeHash(bytes);
            
            var builder = new StringBuilder();
            foreach (var b in hashBytes)
            {
                builder.Append(b.ToString("x2"));
            }
            
            return builder.ToString();
        }
    }
}