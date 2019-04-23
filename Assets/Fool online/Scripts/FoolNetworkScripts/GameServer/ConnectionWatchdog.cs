using Fool_online.Plugins;
using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;

namespace Assets.Fool_online.Scripts.FoolNetworkScripts.GameServer
{
    /// <summary>
    /// Class managing reconnection on connection lost
    /// </summary>
    class ConnectionWatchdog
    {
        /// <summary>
        /// try reconnect N times on connection lost
        /// </summary>
        private const int RECONNECT_TRIES = 5;

        /// <summary>
        /// How much tries left before ending this task
        /// </summary>
        private static int _reconnectTriesLeft = 0;

        /// <summary>
        /// State: is trying to estabilish connection
        /// </summary>
        public static bool IsTryingToReconnect { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static void OnOpen()
        {
        }

        public static void OnClose(WebSocketCloseCode closecode)
        {
            // if disconnected by error
            if (closecode == WebSocketCloseCode.Abnormal)
            {
                // if first time
                if (!IsTryingToReconnect)
                {
                    // init reconnection recursion
                    IsTryingToReconnect = true;
                    _reconnectTriesLeft = RECONNECT_TRIES;
                }

                // if in reconnection recursion
                if (_reconnectTriesLeft > 0)
                {
                    Reconnect();
                }
                // if no more tries left
                else
                {
                    Debug.Log("No more tries left. Stop reconnecting.");
                    IsTryingToReconnect = false;
                }

            }
            else // if connection is Ok
            {
                IsTryingToReconnect = false;
            }
        }

        /// <summary>
        /// Try connect with last saved server ip end point
        /// </summary>
        private static void Reconnect()
        {

            Debug.Log($"Trying to reconnect. Tries left: {_reconnectTriesLeft}/{RECONNECT_TRIES}");

            _reconnectTriesLeft--;

            FoolWebClient.ReconnectToGameServer();

        }
    }
}
