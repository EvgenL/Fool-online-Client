using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject ContainerSelectLoginMethod;

    [SerializeField] private GameObject ContainerAnonLogin;

    [SerializeField] private GameObject ContainerEmailLogin;

    [SerializeField] private GameObject ContainerEmailRegister;

    private void Awake()
    {
        // todo remember last login method
        ShowSelectLoginMethod();
    }

    public void ShowSelectLoginMethod()
    {
        Show(1);
    }
    public void ShowAnonLogin()
    {
        Show(2);
    }
    public void ShowEmailLogin()
    {
        Show(3);
    }
    public void ShowEmailRegister()
    {
        Show(4);
    }

    /// <summary>
    /// Shows page and hides other pages
    /// todo animation
    /// </summary>
    private void Show(int pageNumber)
    {
        switch (pageNumber)
        {
            case 1:
                ContainerSelectLoginMethod.SetActive(true);
                ContainerAnonLogin.SetActive(false);
                ContainerEmailLogin.SetActive(false);
                ContainerEmailRegister.SetActive(false);
                break;
            case 2:
                ContainerSelectLoginMethod.SetActive(false);
                ContainerAnonLogin.SetActive(true);
                ContainerEmailLogin.SetActive(false);
                ContainerEmailRegister.SetActive(false);
                break;
            case 3:
                ContainerSelectLoginMethod.SetActive(false);
                ContainerAnonLogin.SetActive(false);
                ContainerEmailLogin.SetActive(true);
                ContainerEmailRegister.SetActive(false);
                break;
            case 4:
                ContainerSelectLoginMethod.SetActive(false);
                ContainerAnonLogin.SetActive(false);
                ContainerEmailLogin.SetActive(false);
                ContainerEmailRegister.SetActive(true);
                break;
        }
    }
}
