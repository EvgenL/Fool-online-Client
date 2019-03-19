using System.Xml.Linq;
using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using UnityEngine;

namespace Fool_online.Scripts.FoolNetworkScripts.AccountsServer.Packets
{
    /// <summary>
    /// Static calss for sending packets to server
    /// </summary>
    static class SendAccountData
    {
        /// <summary>
        /// Try register by email
        /// </summary>
        public static void SendEmailRegistration(string nickname, string email, string password)
        {
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

    }
}
