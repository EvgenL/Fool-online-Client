using System.Collections.Generic;
using UnityEngine;

namespace Fool_online.Ui.Mainmenu
{
    /// <summary>
    /// Class with logic of main menu and translations
    /// between windows
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("Window containers")]
        [SerializeField] private Transform _homeContainer;
        [SerializeField] private Transform _createRoomContainer;
        [SerializeField] private Transform _openRoomsListContainer;

        
        private Transform[] _containers;

        private void Awake()
        {
            var containersList = new List<Transform>();

            containersList.Add(_homeContainer);
            containersList.Add(_createRoomContainer);
            containersList.Add(_openRoomsListContainer);

            _containers = containersList.ToArray();
        }

        private void OpenPage(Transform pageToOpen)
        {
            foreach (var container in _containers)
            {
               container.gameObject.SetActive(false);
            }

            pageToOpen.gameObject.SetActive(true);
        }


        #region Buttons

        public void OnCreateRoomClick()
        {
            OpenPage(_createRoomContainer);
        }

        public void OnOpenRoomsListClick()
        {
            OpenPage(_openRoomsListContainer);
        }

        public void OnHomeClick()
        {
            OpenPage(_homeContainer);
        }

        public void OnDonateClick()
        {
            OpenPage(_homeContainer);
            // TODO _homeContainer.SetBackButton
        }

        #endregion


    }
}
