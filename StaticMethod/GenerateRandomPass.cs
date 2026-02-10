using System;
using System.Security.Cryptography;
using System.Text;

namespace MT5ConnectionService.StaticMethod
{
    public static class GenerateRandomPass
    {
        private static readonly string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string DigitChars = "0123456789";
        private static readonly string SpecialChars = "+-@!*";
        private static readonly string AllChars = LowercaseChars + UppercaseChars + DigitChars + SpecialChars;

        public static string GenerateMasterPassword(int minLength)
        {
            return GeneratePassword(minLength);
        }

        public static string GenerateInvestorPassword(int minLength)
        {
            return GeneratePassword(minLength);
        }

        private static string GeneratePassword(int minLength)
        {
            if (minLength < 8)
                throw new ArgumentException("Minimum length must be at least 8 characters.");

            StringBuilder password = new StringBuilder();

            password.Append(GetRandomChar(LowercaseChars));
            password.Append(GetRandomChar(UppercaseChars));
            password.Append(GetRandomChar(DigitChars));
            password.Append(GetRandomChar(SpecialChars));

            for (int i = 4; i < minLength; i++)
            {
                password.Append(GetRandomChar(AllChars));
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
