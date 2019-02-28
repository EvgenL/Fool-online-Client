using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLoginMethod : MonoBehaviour
{
    [SerializeField] private LoginMenuManager MenuManger;

    public void OnClickAnonLogin()
    {
        MenuManger.ShowAnonLogin();
    }
    public void OnClickVkLogin()
    {
        OnClickAnonLogin();
    }
    public void OnClickOkLogin()
    {
        OnClickAnonLogin();
    }
    public void OnClickFbLogin()
    {
        OnClickAnonLogin();
    }
    public void OnClickEmailLogin()
    {
        OnClickAnonLogin();
    }
    public void OnClickEmailRegister()
    {
        MenuManger.ShowEmailRegister();
    }
}
