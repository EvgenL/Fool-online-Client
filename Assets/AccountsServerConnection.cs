using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp.Net;
using NetworkManager = Fool_online.Scripts.Manager.NetworkManager;

public class AccountsServerConnection : MonoBehaviour
{


    #region Singleton
    public static AccountsServerConnection Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public string LoginServerIp = "127.0.0.1";
    public int LoginServerPort = 5054;

    public static bool IsConnectingToAccountsServer = false;
    public static bool IsConnected = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckVersion(LoginServerIp, LoginServerPort));
    }

    private IEnumerator CheckVersion(string serverIp, int serverPort)
    {

        var request = new UnityWebRequest(
            "http://" + serverIp + ":" + serverPort + "/",
            "POST",
            new DownloadHandlerBuffer(),
            new UploadHandlerRaw(null));

        request.SetRequestHeader("Client-version", Application.version);
        request.SetRequestHeader("Login", "anonymous");
        request.SetRequestHeader("Username", "test");

        //Send
        //And wait for response
        var op = request.SendWebRequest();

        while (!op.isDone)
        {
            yield return null;
        }

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            //todo show error screen
            yield break;
        }

        Debug.Log(request.GetResponseHeader("Version-check"));
        Debug.Log(request.GetResponseHeader("Info"));

        //if request returned OK then connect to geme server
        if (request.responseCode == (int)HttpStatusCode.OK)
        {
            string gameServerIp = request.GetResponseHeader("Game-server-ip");
            int gameServerPort = int.Parse(request.GetResponseHeader("Game-server-port"));
            string authToken = request.GetResponseHeader("Auth-token");

            Debug.Log("Anon login OK. Connecting to game server: " + gameServerIp + ":" + gameServerPort);

            NetworkManager.Instance.Connect(gameServerIp, gameServerPort);
        }
        else //if not passed version check
        {

        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
