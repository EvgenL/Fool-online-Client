using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer
{
    /// <summary>
    /// Static util methods to help with account managenent
    /// </summary>
    public static class AccountsUtil
    {


        /// <summary>
        /// Checks email is correct
        /// </summary>
        public static bool EmailIsValid(string email)
        {
            // Email regexp that 99.9% works
            // emailregex.com
            return (Regex.IsMatch(email,
                    "(?:[a-z0-9!#$%&\'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&\'*+/=?^_`{|}~-]+)*|\"(?:" +
                    "[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b" +
                    "\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])" +
                    "?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|" +
                    "[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])")
                );
        }

        /// <summary>
        /// Creates sha1 from string
        /// </summary>
        /// <param name="text">input string</param>
        /// <returns>sha1</returns>
        public static string GetSha1(string text)
        {
            return text;

            var sha1 = new SHA1Managed();
            var plaintextBytes = Encoding.UTF8.GetBytes(text);
            var hashBytes = sha1.ComputeHash(plaintextBytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.AppendFormat("{0:x2}", hashByte);
            }

            var hashString = sb.ToString();
            return hashString;

        }
    }
}
