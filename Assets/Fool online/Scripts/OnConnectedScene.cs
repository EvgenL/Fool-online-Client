using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts;
using Fool_online.Scripts.Network;
using Fool_online.Scripts.Network.NetworksObserver;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnConnectedScene : MonoBehaviour   
{
    public string NextScene = "";

    private void Start()
    {
        NetworkManager.Instance.Connect();
    }

    private void Update()
    {
        if (FoolNetwork.connectionState == FoolNetwork.ConnectionState.Connected)
        {
            SceneManager.LoadScene(NextScene);
            this.enabled = false;
        }
    }

    /*public override void OnConnectedToGameServer()
    {
        SceneManager.LoadScene(NextScene);
    }*/
}
