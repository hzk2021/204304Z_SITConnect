using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace _204304Z_SITConnect.Validator
{
    public static class CustomRegex
    {
        public static bool IsValidPassword(string password)
        {
            if (!string.IsNullOrWhiteSpace(password))
            {
                Regex regexHasNumber = new Regex(@"[0-9]+");
                Regex regexHasUpperCase = new Regex(@"[A-Z]+");
                Regex regexHasLowerCase = new Regex(@"[a-z]+");
                Regex regexHasSpecialChar = new Regex(@"[^A-Za-z0-9]+");
                Regex regexhasMin12Letters = new Regex(@".{12,}");

                bool isValid = regexHasNumber.IsMatch(password) &&
                    regexHasUpperCase.IsMatch(password) &&
                    regexHasLowerCase.IsMatch(password) &&
                    regexHasSpecialChar.IsMatch(password) &&
                    regexhasMin12Letters.IsMatch(password);

                if (isValid)
                {
                    return true;
                }
                return false;
            }

            return false;

        }

        public static bool IsValidEmail(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                Regex regex = new Regex(@"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$");
                Match match = regex.Match(email);
                if (match.Success)
                    return true;
                else
                    return false;
            }

            return false;

        }
    }
}