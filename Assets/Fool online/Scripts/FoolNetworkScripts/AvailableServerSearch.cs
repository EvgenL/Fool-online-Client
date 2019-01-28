using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Fool_online.Scripts.Network;

namespace Assets.Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// Searches for available servers listed in file 'CONFIG_FILE_NAME'
    /// </summary>
    public static class AvailableServerSearch
    {
        private static ConcurrentStack<AvailableServer> workingServers;

        private const string CONFIG_FILE_NAME = "clientconfig.xml";

        /// <summary>
        /// Reads config file 'client.config'
        /// </summary>
        /// <returns></returns>
        public static AvailableServer[] GetAvailableServers()
        {
            return null;

            FileStream clientconfig = null;
            try
            {
                string xmlstring = "";

                //read file
                try
                {
                    clientconfig = File.Open(CONFIG_FILE_NAME, FileMode.Open);

                    StreamReader sr = new StreamReader(clientconfig);
                    xmlstring = sr.ReadToEnd();
                    clientconfig.Close();
                }
                catch (Exception e)
                {
                    clientconfig.Close();
                    FoolNetwork.Disconnect("Файлы игры повреждены");
                    throw;
                }

                //parse as xml
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlstring);

                XmlNodeList serversNodes = doc.DocumentElement.SelectNodes("/Connection/Servers/Gameserver");

                //if file is null
                if (serversNodes == null || serversNodes.Count == 0)
                {
                    return null;
                }

                AvailableServer[] readServers = new AvailableServer[serversNodes.Count];

                for (int i = 0; i < serversNodes.Count; i++)
                {
                    /* Example of xml:
                      <Gameserver name = "local">
                      <ip>127.0.0.1</ip>
                      <port>5055</port>
                      </Gameserver>              
                    */
                    XmlNode node = serversNodes[i];

                    AvailableServer server = new AvailableServer();
                    server.Name = node.Attributes["name"].Value;
                    server.Ip = node.ChildNodes[0].InnerText;
                    server.Port = int.Parse(node.ChildNodes[1].InnerText);

                    readServers[i] = server;
                }

                workingServers = new ConcurrentStack<AvailableServer>();

                foreach (var server in readServers)
                {
                }
            }
            catch (Exception e)
            {
                clientconfig.Close();
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                workingServers.Clear();
            }
        }

        private static void ThreadFetchServer()
        {
            
        }
    }
}
