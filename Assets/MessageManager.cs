using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Shows text message in game
/// </summary>
public class MessageManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject TextContainer;

    private static MessageManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    public static void Show(string message)
    {
        _instance.text.text = message;
        _instance.TextContainer.SetActive(true);
        _instance.Invoke(nameof(Hide), 3f);
    }

    public void Hide()
    {
        TextContainer.SetActive(false);
    }

}
