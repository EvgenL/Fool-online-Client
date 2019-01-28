using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// Search for known servers returns this
    /// </summary>
    public struct AvailableServer
    {
        public string Name;
        public string Ip;
        public int Port;

        public int PlayersOnline;
        public int MaxPlayers;
    }
}
