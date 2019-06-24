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
        [SerializeField] private Transform _privateRoomsListContainer;

        [SerializeField] private Transform _addMoneyContainer;
        [SerializeField] private Transform _withdrawMoneyContainer;
        [SerializeField] private Transform _rulesContainer;

        [Header("Link to animated footer marker")]
        [SerializeField] private TabMarkerAnimation _marker;

        private static int _lastPage = 0;

        private Transform[] _containers;

        private void Awake()
        {
            var containersList = new List<Transform>();

            containersList.Add(_homeContainer);
            containersList.Add(_createRoomContainer);
            containersList.Add(_openRoomsListContainer);
            containersList.Add(_privateRoomsListContainer);
            containersList.Add(_addMoneyContainer);
            containersList.Add(_withdrawMoneyContainer);
            containersList.Add(_rulesContainer);

            _containers = containersList.ToArray();

            OpenPageByNumber(_lastPage);
        }

        public void OpenPageByNumber(int pageN)
        {
            OpenPage(_containers[pageN]);
        }
        /*
        public void OpenPageByName(string pageName)
        {
            _marker.HideBackButton();
            OpenPage(pageName);
        }

        public void OpenPageWithBackButton(string pageName)
        {
            _marker.ShowBackButton();
            OpenPage(pageName);
        }
        */
        private void OpenPage(string pageName)
        {
            if (pageName != "")
            {
                var container = _containers.Single(c => c.name == pageName);
                OpenPage(container);
            }
            else
            {
                OpenPage(_containers[0]);
            }
        }
        private void OpenPage(Transform pageToOpen)
        {
            for (int i = 0; i < _containers.Length; i++)
            {
                if (_containers[i] == null) continue;

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
            _marker.HideBackButton();
        }

        public void OnOpenRoomsListClick()
        {
            OpenPage(_openRoomsListContainer);
            _marker.HideBackButton();
        }

        public void OnHomeClick()
        {
            OpenPage(_homeContainer);
            _marker.HideBackButton();
        }

        public void OnAddMoneyClick()
        {
            OpenPage(_addMoneyContainer);
            _marker.ShowBackButton();
        }

        public void OnWithdrawMoneyClick()
        {
            OpenPage(_withdrawMoneyContainer);
            _marker.ShowBackButton();
        }

        public void OnRulesClick()
        {
            OpenPage(_rulesContainer);
            _marker.ShowBackButton();
        }

        #endregion


    }
}
