using System;
using System.Xml.Linq;
using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using UnityEngine;

namespace Fool_online.Scripts.FoolNetworkScripts.AccountsServer.Packets
{
    /// <summary>
    /// Static calss for sending packets to server
    /// </summary>
    static class AccountPackets
    {
        private struct AccountData
        {
            public enum LoginMethod
            {
                Anonymous = 1,
                Email
            };

            public LoginMethod Method;

            public string Nickname;
            public string Email;
            public string Password;


        }

        /// <summary>
        /// Buffered login data for reconnection
        /// </summary>
        private static AccountData _lastLoginData;

        /// <summary>
        /// Try register by email
        /// </summary>
        public static void SendEmailRegistration(string nickname, string email, string password)
        {
            BufferLastLogin(nickname, email, password, AccountData.LoginMethod.Email);

            XElement body = new XElement(
                new XElement("Request",
                    //add login data
                    new XElement("Connection",
                        new XElement("ClientVersion", Application.version),
                        new XElement("LoginMethod", "EmailRegistration"),
                        new XElement("Nickname", nickname),
                        new XElement("Email", email),
                        new XElement("Password", password)
                    )
                )
            );

            AccountsTransport.Send(body);
        }

        /// <summary>
        /// try login via email and password
        /// </summary>
        public static void SendEmailLogin(string email, string password)
        {
            BufferLastLogin("", email, password, AccountData.LoginMethod.Email);

            XElement body = new XElement(
                new XElement("Request",
                    //add login data
                    new XElement("Connection",
                        new XElement("ClientVersion", Application.version),
                        new XElement("LoginMethod", "EmailLogin"),
                        new XElement("Email", email),
                        new XElement("Password", password)
                    )
                )
            );

            AccountsTransport.Send(body);
        }

        /// <summary>
        /// Try login withou registration
        /// </summary>
        public static void SendAnonLogin(string nickname)
        {
            BufferLastLogin(nickname, "", "", AccountData.LoginMethod.Email);

            XElement body = new XElement(
                new XElement("Request",
                    //add login data
                    new XElement("Connection",
                        new XElement("ClientVersion", Application.version),
                        new XElement("LoginMethod", "Anonymous"),
                        new XElement("Nickname", nickname)
                    )
                )
            );

            AccountsTransport.Send(body);
        }

        /// <summary>
        /// On login account data is buffered to send it again on reconnection
        /// (NEVER USED)
        /// </summary>
        private static void BufferLastLogin(string nickname, string email, string password, AccountData.LoginMethod method)
        {
            _lastLoginData = new AccountData()
            {
                Nickname = nickname,
                Email = email,
                Password = password,
                Method = method,
            };
        }

    }
}
