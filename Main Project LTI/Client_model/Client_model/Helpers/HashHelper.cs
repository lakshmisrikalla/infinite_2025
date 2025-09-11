using System;
using System.Security.Cryptography;
using System.Text;

namespace Client_model.Helpers
{
    public static class HashHelper
    {
        // Hash password using PBKDF2 (Rfc2898)
        public static string HashPassword(string password)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);

                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(32);

                byte[] hashBytes = new byte[49]; // 1 versi on + 16 salt + 32 hash
                hashBytes[0] = 0x01;
                Buffer.BlockCopy(salt, 0, hashBytes, 1, 16);
                Buffer.BlockCopy(hash, 0, hashBytes, 17, 32);

                return Convert.ToBase64String(hashBytes);
            }
        }

        // Verify password against stored hash
        public static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(storedHash);
                if (hashBytes.Length != 49 || hashBytes[0] != 0x01) return false;

                byte[] salt = new byte[16];
                Buffer.BlockCopy(hashBytes, 1, salt, 0, 16);

                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(32);

                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[17 + i] != hash[i]) return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Generate a temporary random password
        public static string GenerateTempPassword(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$!";
            var sb = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider())
            {
                var buffer = new byte[4];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    sb.Append(chars[(int)(num % (uint)chars.Length)]);
                }
            }
            return sb.ToString();
        }

        // Generate unique company code (letters + numbers, easy to type)
        public static string GenerateCompanyCode(int length = 8)
        {
            const string chars = "ABCDEFGHJKMNPQRSTUVWXYZ23456789"; // removed confusing chars
            var sb = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider())
            {
                var buffer = new byte[4];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    sb.Append(chars[(int)(num % (uint)chars.Length)]);
                }
            }
            return sb.ToString();
        }
    }
}
