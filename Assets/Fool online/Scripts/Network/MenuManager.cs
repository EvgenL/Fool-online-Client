using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.Network
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;

        private Menu _currentMenu;
        public enum Menu
        {
            Loading,
            Home,
            Register,
            Disconnected
        }

        [Header("Menu game object")]
        public GameObject LoadingMenu;
        public GameObject HomeMenu;
        public GameObject RegisterMenu;
        public GameObject DisconnectedMenu;
        public Text DisconnectedText;

        void Awake()
        {
            Instance = this;

            //When we connected to server we set this window to be home
            //TODO FoolTcpClient.Instance.OnConnectedEvent += SetMenuHome;
            //When we disconnected from server we set this window to be disconnect screen
            //FoolTcpClient.Instance.OnDisconnectedEvent += SetMenuDisconnected;
        }

        void Start()
        {
            SetMenu(0);
        }

        void Update()
        {
            switch (_currentMenu)
            {
                case Menu.Loading:
                    LoadingMenu.SetActive(true);
                    HomeMenu.SetActive(false);
                    RegisterMenu.SetActive(false);
                    DisconnectedMenu.SetActive(false);
                    break;
                case Menu.Home:
                    LoadingMenu.SetActive(false);
                    HomeMenu.SetActive(true);
                    RegisterMenu.SetActive(false);
                    DisconnectedMenu.SetActive(false);
                    break;
                case Menu.Register:
                    LoadingMenu.SetActive(false);
                    HomeMenu.SetActive(false);
                    RegisterMenu.SetActive(true);
                    DisconnectedMenu.SetActive(false);
                    break;
                case Menu.Disconnected:
                    LoadingMenu.SetActive(false);
                    HomeMenu.SetActive(false);
                    RegisterMenu.SetActive(false);
                    DisconnectedMenu.SetActive(true);
                    break;
            }
        }

        public void SetMenu(int i)
        {
            _currentMenu = (Menu)i;
        }

        private void SetMenuHome()
        {
            print("SetMenuHome");
            SetMenu((int)Menu.Home);
            //FoolTcpClient.Instance.OnConnectedEvent -= SetMenuHome;
        }

        private void SetMenuDisconnected(string text)
        {
            print("SetMenuDisconnected: " + text);
            SetMenu((int)Menu.Disconnected);
            //FoolTcpClient.Instance.OnDisconnectedEvent -= SetMenuDisconnected;
        }
    }
}
