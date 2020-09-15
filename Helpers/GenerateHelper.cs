using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBilling.Helpers
{
    public static class GenerateHelper
    {
        private static readonly Random _random = new Random();

        private const string _bChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _lChars = "abcdefghijklmnopqrstuvwxyz";
        private const string _nums = "0123456789";
        private const string _syms = "!@#$%^&*(){}:\"\\/<>?'";

        public static string GetString(int length, bool lLetters = true, bool bLetters = false, bool numbers = false, bool symbols = false)
        {
            string chars = string.Empty;
            StringBuilder sb = new StringBuilder(length);

            if (lLetters) chars += _lChars;
            if (bLetters) chars += _bChars;
            if (numbers) chars += _nums;
            if (symbols) chars += _syms;

            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[_random.Next(0, chars.Count())]);
            }

            return sb.ToString();
        }

        //public static string GetPassword()
        //{

        //}

        public static async Task<string> GetStringAsync(int length, bool lLetters = true, bool bLetters = false, bool numbers = false, bool symbols = false)
            => await Task.Run(() => GetString(length, lLetters, bLetters, numbers, symbols));
    }
}
