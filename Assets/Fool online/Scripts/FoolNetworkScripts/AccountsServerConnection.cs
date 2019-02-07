using System.Collections;
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

    public string LoginServerIp = "51.75.236.170";
    public int LoginServerPort = 5054;
    public bool useLocal = true;

    public static bool IsConnectingToAccountsServer = false;
    public static bool IsConnected = false;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(CheckVersion(LoginServerIp, LoginServerPort));
    }

    /// <summary>
    /// Login without registration if allowed by server
    /// </summary>
    /// <param name="nickname">Desired nickname</param>
    public void AnonymousLogin(string nickname)
    {
        StartCoroutine(AnonymousLoginCoroutine(nickname));
    }

    private IEnumerator AnonymousLoginCoroutine(string nickname)
    {
        //Create POST reauest
        var request = new UnityWebRequest(
            "http://" + (useLocal ? "192.168.0.22" : LoginServerIp) + ":" + LoginServerPort + "/",
            "POST");

        //Add headers
        request.SetRequestHeader("Client-version", Application.version);
        request.SetRequestHeader("Login", "anonymous");
        request.SetRequestHeader("Nickname", nickname);

        //Send
        //And wait for response
        yield return request.SendWebRequest();

        //When got response (or timeout)
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

            NetworkManager.Instance.ConnectToGameServer(gameServerIp, gameServerPort, authToken);
        }
        else //if not passed version check
        {

        }
    }


    private IEnumerator CheckVersion(string serverIp, int serverPort)
    {

        var request = new UnityWebRequest(
            "http://" + serverIp + ":" + serverPort + "/",
            "POST",
            new DownloadHandlerBuffer(),
            new UploadHandlerRaw(null));

        request.SetRequestHeader("Client-version", Application.version);

        //Send
        //And wait for response
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            //todo show error screen
            yield break;
        }

        Debug.Log("Version-check: " + request.GetResponseHeader("Version-check"));
        Debug.Log("Info: " + request.GetResponseHeader("Info"));
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
