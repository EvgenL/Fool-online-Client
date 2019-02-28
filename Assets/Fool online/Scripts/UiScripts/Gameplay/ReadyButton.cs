using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.UiScripts.Gameplay
{
    public class ReadyButton : MonoBehaviourFoolObserver
    {
        [Header("The 'ready' button")]
        [SerializeField] private GameObject GetReadyButton;

        /// <summary>
        /// Show 'ready' button if me joined last
        /// </summary>
        private void Start()
        {
            CheckIfAllPlayersJoined();
        }

        #region Observer callbacks

        public override void OnOtherPlayerJoinedRoom(long joinedPlayerId, int slotN, string joinedPlayerNickname)
        {
            CheckIfAllPlayersJoined();
        }

        public override void OnOtherPlayerLeftRoom(long leftPlayerId, int slotN)
        {
            CheckIfAllPlayersJoined();
        }

        public override void OnEndGame(long foolConnectionId, Dictionary<long, double> rewards)
        {
            CheckIfAllPlayersJoined();
        }

        public override void OnStartGame()
        {
            HideButton();
        }

        #endregion

        /// <summary>
        /// Shows 'ready' button if all players are joined
        /// </summary>
        private void CheckIfAllPlayersJoined()
        {
            if (StaticRoomData.ConnectedPlayersCount == StaticRoomData.MaxPlayers)
            {
                ShowButton();
            }
            else
            {
                HideButton();
            }
        }

        private void ShowButton()
        {
            //todo animation
            GetReadyButton.SetActive(true);
        }

        private void HideButton()
        {
            GetReadyButton.SetActive(false);
            GetReadyButton.GetComponentInChildren<Toggle>().isOn = false;
        }

        public void OnClick(bool value)
        {
            InputManager.Instance.OnGetReadyClick(value);
        }

    }
}
