

//DEFINES
#define TEST_MODE_LOCALHOST // if defined, will connect to localhost





using System.Text;
using System.Xml.Linq;
using HybridWebSocket;
using UnityEngine;
using NetworkManager = Fool_online.Scripts.Manager.NetworkManager;


/// <summary>
/// Connects to accounts server. Asks for game server ip and auth token
/// </summary>
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

    public static bool IsConnectingToAccountsServer = false;
    public static bool IsConnected = false;

    /// <summary>
    /// My client socket for sending data to server
    /// </summary>
    private WebSocket mySocket;

    /// <summary>
    /// Data to send
    /// </summary>
    private XElement bufferedBody;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(todo CheckVersion(LoginServerIp, LoginServerPort));
    }




    /// <summary>
    /// Login without registration if allowed by server
    /// </summary>
    /// <param name="nickname">Desired nickname</param>
    public void AnonymousLogin(string nickname)
    {
#if TEST_MODE_LOCALHOST
        string ip = "127.0.0.1";
#else
        string ip = LoginServerIp;
#endif
        ConnectAndAnonymousLogin(ip, LoginServerPort, nickname);
    }

    /// <summary>
    /// Sends to server data for anonymous login
    /// </summary>
    private void ConnectAndAnonymousLogin(string serverIp, int serverPort, string nickname)
    {
        //Create request body
        XElement body = new XElement(
            new XElement("Request",
                //add version
                new XElement("VersionCheck", Application.version
                    ),
                //add login data
                new XElement("Connection",
                    new XElement("LoginMethod", "Anonymous"),
                    new XElement("Nickname", nickname)
                    )
                )
            );

        ConnectAndSend(serverIp, serverPort, body);
    }

    /// <summary>
    /// Connects to server then dends XML data
    /// </summary>
    private void ConnectAndSend(string serverIp, int serverPort, XElement body)
    {
        //Create and set up a new socket
        mySocket = WebSocketFactory.CreateInstance("ws://" + serverIp + ":" + serverPort);

        //Init callbacks
        mySocket.OnOpen += SendBufferedBody;
        mySocket.OnMessage += OnMessage;
        mySocket.OnError += OnError;
        mySocket.OnClose += OnClose;
        bufferedBody = body;

        //Connect
        mySocket.Connect();
    }

    /// <summary>
    /// Triggered on open to send request immdiatelly
    /// </summary>
    private void SendBufferedBody()
    {
        byte[] data = Encoding.Unicode.GetBytes(bufferedBody.ToString());

        mySocket.Send(data);
    }


    private void OnMessage(byte[] data)
    {
        Debug.Log("OnMessage from server\n" + Encoding.Unicode.GetString(data));

        //parse response data
        string bodyString = Encoding.Unicode.GetString(data);
        XElement body = XElement.Parse(bodyString);

        //check for errors
        XElement error = GetChildElement(body, "Error");
        if (error != null)
        {
            //todo proper error handling
            Debug.LogError("Recieved error!\n" + GetChildElement(error, "Info").Value);
            return;
        }

        //check for version check data
        XElement versionCheck = GetChildElement(body, "VersionCheck");
        if (versionCheck != null && versionCheck.Value == "OK")
        {
            Debug.Log("Recieved versionCheck OK\n" + versionCheck.ToString());
        }

        //check for login data
        XElement loginData = GetChildElement(body, "LoginData");
        if (loginData != null)
        {
            Debug.Log("Recieved loginData\n" + loginData.ToString());

            //read server data
            string gameServerIp = GetChildElement(loginData, "GameServerIp").Value;
            int gameServerPort = int.Parse(GetChildElement(loginData, "GameServerPort").Value);
            string token = GetChildElement(loginData, "Token").Value;

            Debug.Log("Anon login OK. Connecting to game server: " + gameServerIp + ":" + gameServerPort);
            Debug.Log("token: " + token);

            NetworkManager.Instance.ConnectToGameServer(gameServerIp, gameServerPort, token);
            return;
        }

        /*
*/
    }

    private void OnError(string errormsg)
    {
        Debug.Log("Accounts server connection error:\n" + errormsg, this);
        //todo show error msg
        //throw new Exception(errormsg);
    }

    private void OnClose(WebSocketCloseCode closecode)
    {
        Debug.Log("Accounts server connection closed:\n" + closecode, this);
        mySocket = null;
    }

    /// <summary>
    /// Finds element nested in XML XElement by local name
    /// </summary>
    /// <param name="body">XElement which to look</param>
    /// <param name="elementLocalName">Target name</param>
    /// <returns>Found xelement. Null if none</returns>
    private static XElement GetChildElement(XElement body, string elementLocalName)
    {
        foreach (var element in body.Elements())
        {
            if (element.Name.LocalName == elementLocalName)
            {
                return element;
            }
        }

        return null;
    }
}
