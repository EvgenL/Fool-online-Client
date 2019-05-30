using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAvatarButton : MonoBehaviour
{
    public void OnClick()
    {
        DialogueManager.Instance?.ShowMyProffile();
    }
}
