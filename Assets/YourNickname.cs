using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays player name
/// onto Text component
/// </summary>
public class YourNickname : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var text = GetComponent<Text>();
        text.text = "Ваш ник: "+FoolNetwork.LocalPlayer.Nickname;
    }
}
