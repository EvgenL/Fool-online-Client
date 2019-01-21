using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.Network;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GiveUpButton : MonoBehaviour
{
    public void OnClick()
    {
        FoolNetwork.GiveUp();
        SceneManager.LoadScene("Main menu");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //TODO give up confirmation
        }
    }
}
