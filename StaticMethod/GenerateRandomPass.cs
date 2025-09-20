using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.StaticMethod
{
    public static class GenerateRandomPass
    {
        #region old code
        //// Define valid characters for the password
        //static string validCharsInvestorPassword = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%^&*";
        //static string validCharsMasterPassword = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ@#$%^&*";

        //// Replace with your desired password length
        //static int passwordLength = 10; // Adjust this to your desired length

        //public static string GenerateMasterPassword()
        //{
        //    Random random = new Random();
        //    StringBuilder password = new StringBuilder(passwordLength);

        //    // Ensure at least one uppercase letter (first character), one lowercase letter, one digit, and one special character
        //    password.Append(validCharsMasterPassword[random.Next(26)]); // Add a random uppercase letter (first character)
        //    password.Append(validCharsMasterPassword[random.Next(26, 52)]); // Add a random lowercase letter
        //    password.Append(validCharsMasterPassword[random.Next(62, validCharsMasterPassword.Length)]); // Add a random special character
        //    password.Append(validCharsMasterPassword[random.Next(52, 62)]); // Add a random digit

        //    // Fill the remaining length with random characters
        //    for (int i = 4; i < passwordLength; i++)
        //    {
        //        int index = random.Next(validCharsMasterPassword.Length);
        //        password.Append(validCharsMasterPassword[index]);
        //    }

        //    // Shuffle the characters in the password to make it more random
        //    return Shuffle(password.ToString());
        //}

        //public static string GenerateInvestorPassword()
        //{
        //    Random random = new Random();
        //    StringBuilder password = new StringBuilder(passwordLength);

        //    // Ensure at least one uppercase letter (first character), one lowercase letter, one digit, and one special character
        //    password.Append(validCharsInvestorPassword[random.Next(26)]); // Add a random uppercase letter (first character)
        //    password.Append(validCharsInvestorPassword[random.Next(26, 52)]); // Add a random lowercase letter
        //    password.Append(validCharsInvestorPassword[random.Next(62, validCharsInvestorPassword.Length)]); // Add a random special character
        //    password.Append(validCharsInvestorPassword[random.Next(52, 62)]); // Add a random digit

        //    // Fill the remaining length with random characters
        //    for (int i = 4; i < passwordLength; i++)
        //    {
        //        int index = random.Next(validCharsInvestorPassword.Length);
        //        password.Append(validCharsInvestorPassword[index]);
        //    }

        //    // Shuffle the characters in the password to make it more random
        //    return Shuffle(password.ToString());
        //}

        //private static string Shuffle(string input)
        //{
        //    char[] characters = input.ToCharArray();
        //    Random random = new Random();

        //    for (int i = characters.Length - 1; i > 0; i--)
        //    {
        //        int j = random.Next(0, i + 1);
        //        char temp = characters[i];
        //        characters[i] = characters[j];
        //        characters[j] = temp;
        //    }

        //    return new string(characters);
        //}
        #endregion

        private static readonly string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string DigitChars = "0123456789";
        private static readonly string SpecialChars = "+-@!*";

        public static string GenerateMasterPassword(int minLength)
        {
            return GeneratePassword(minLength, LowercaseChars + UppercaseChars + DigitChars + SpecialChars);
        }

        public static string GenerateInvestorPassword(int minLength)
        {
            return GeneratePassword(minLength, LowercaseChars + UppercaseChars + DigitChars + SpecialChars);
        }

        private static string GeneratePassword(int minLength, string validChars)
        {
            if (minLength < 8)
                throw new ArgumentException("Minimum length must be at least 8 characters.");

            // Ensure at least two of three types of characters
            StringBuilder password = new StringBuilder();

            password.Append(GetRandomChar(LowercaseChars));
            password.Append(GetRandomChar(UppercaseChars));
            password.Append(GetRandomChar(DigitChars));
            password.Append(GetRandomChar(SpecialChars));

            for (int i = 4; i < minLength; i++)
            {
                password.Append(GetRandomChar(validChars));
            }

            return Shuffle(password.ToString());
        }

        private static char GetRandomChar(string validChars)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[1];
                rng.GetBytes(randomBytes);
                int randomNumber = Convert.ToInt32(randomBytes[0]);

                return validChars[randomNumber % validChars.Length];
            }
        }

        private static string Shuffle(string input)
        {
            char[] characters = input.ToCharArray();
            Random random = new Random();

            for (int i = characters.Length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                char temp = characters[i];
                characters[i] = characters[j];
                characters[j] = temp;
            }

            return new string(characters);
        }
    }
}
