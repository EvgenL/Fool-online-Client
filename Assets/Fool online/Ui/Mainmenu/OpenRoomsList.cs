using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts;
using UnityEditor.PackageManager;
using UnityEngine;

public class OpenRoomsList : MonoBehaviour
{


    public void Refresh()
    {
        ClientSendPackets.Send_RefreshRoomList();
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
