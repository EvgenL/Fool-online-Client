using Fool_online.Scripts;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;

namespace Fool_online.Ui.Mainmenu
{
    public class OpenRoomsList : MonoBehaviourFoolObserver
    {
        [Header("Prefab of room display which will be spawned")]
        [SerializeField] private GameObject _roomDisplayPrefab;
        [Header("Container where room display will be spawned")]
        [SerializeField] private Transform _roomDisplaysContainer;

        //private RoomInstance _currentRooms;

        private new void OnEnable()
        {
            Refresh();

            base.OnEnable();
        }

        public void Refresh()
        {
            Util.DestroyAllChildren(_roomDisplaysContainer);

            ClientSendPackets.Send_RefreshRoomList();
            //todo animaton until OnRoomList
        }

        /// <summary>
        /// Observed after Refresh.
        /// </summary>
        public override void OnRoomList(RoomInstance[] rooms)
        {
            //todo filter, add/delete only different. Buffer currntly displayed rooms. 

            Util.DestroyAllChildren(_roomDisplaysContainer);

            foreach (var roomInstance in rooms)
            {
                //spawn
                var roomDisplayGo = Instantiate(_roomDisplayPrefab, _roomDisplaysContainer);
                var roomDisplayScript = roomDisplayGo.GetComponent<RoomDisplay>();
                //display
                roomDisplayScript.DrawRoom(roomInstance);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
