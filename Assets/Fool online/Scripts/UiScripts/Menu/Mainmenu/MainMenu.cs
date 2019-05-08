using System.Collections.Generic;
using System.Linq;
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


        private static int _lastPage = 0;

        private Transform[] _containers;

        private void Awake()
        {
            var containersList = new List<Transform>();

            containersList.Add(_homeContainer);
            containersList.Add(_createRoomContainer);
            containersList.Add(_openRoomsListContainer);

            _containers = containersList.ToArray();

            OpenPage(_lastPage);
        }

        private void OpenPage(int pageNumber)
        {
            OpenPage(_containers[pageNumber]);
        }

        private void OpenPage(Transform pageToOpen)
        {
            for (int i = 0; i < _containers.Length; i++)
            {
                if (_containers[i] == pageToOpen)
                {
                    _containers[i].gameObject.SetActive(true);
                    _lastPage = i;
                }
                else
                {
                    _containers[i].gameObject.SetActive(false);
                }
            }
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
