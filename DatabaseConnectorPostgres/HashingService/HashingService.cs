using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DatabaseConnectorPostgres.Hashing
{
    public class HashingService
    {
        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">{assword</param>
        /// <returns>Password hash</returns>
        public virtual string CreatePasswordHash(string password)
        {

            // Generate a random salt
            var csprng = new RNGCryptoServiceProvider();
            var generatedSalt = new byte[SALT_BYTE_SIZE];
            csprng.GetBytes(generatedSalt);

            // Hash the password and encode the parameters
            var hash = PBKDF2(password, generatedSalt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);
            return PBKDF2_ITERATIONS + ":" + Convert.ToBase64String(generatedSalt) + ":" + Convert.ToBase64String(hash);

        }

        /// <summary>
        /// Validates a password hash
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <param name="correctHash">Saved password hash</param>
        /// <returns>Boolean</returns>
        public virtual bool ValidatePasswordHash(string password, string correctHash)
        {
            return ValidatePBKDF2Password(password, correctHash);

        }

        /// <summary>
        /// Generate salt
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt</returns>
        public virtual string GenerateSalt(int size)
        {
            // Generate a cryptographic random number
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        #region PBKDF2

        // The following constants may be changed without breaking existing hashes.
        public const int SALT_BYTE_SIZE = 24;
        public const int HASH_BYTE_SIZE = 24;
        public const int PBKDF2_ITERATIONS = 64000;

        public const int ITERATION_INDEX = 0;
        public const int SALT_INDEX = 1;
        public const int PBKDF2_INDEX = 2;

        /// <summary>
        /// Validates a password given a hash of the correct one.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <param name="correctHash">A hash of the correct password.</param>
        /// <returns>True if the password is correct. False otherwise.</returns>
        private bool ValidatePBKDF2Password(string password, string correctHash)
        {
            try
            {
                // Extract the parameters from the hash
                char[] delimiter = { ':' };
                var split = correctHash.Split(delimiter);
                var iterations = int.Parse(split[ITERATION_INDEX]);
                var salt = Convert.FromBase64String(split[SALT_INDEX]);
                var hash = Convert.FromBase64String(split[PBKDF2_INDEX]);

                var testHash = PBKDF2(password, salt, iterations, hash.Length);
                return SlowEquals(hash, testHash);

            }
            catch
            {
                // Something is wrong and perhaps it isn't a PBKDF2 Hash.
                return false;
            }

        }

        /// <summary>
        /// Compares two byte arrays in length-constant time. This comparison
        /// method is used so that password hashes cannot be extracted from
        /// on-line systems using a timing attack and then attacked off-line.
        /// </summary>
        /// <param name="a">The first byte array.</param>
        /// <param name="b">The second byte array.</param>
        /// <returns>True if both byte arrays are equal. False otherwise.</returns>
        private bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (var i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

        /// <summary>
        /// Computes the PBKDF2-SHA1 hash of a password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The PBKDF2 iteration count.</param>
        /// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
        /// <returns>A hash of the password.</returns>
        private byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }
        }

        #endregion
    }
}
