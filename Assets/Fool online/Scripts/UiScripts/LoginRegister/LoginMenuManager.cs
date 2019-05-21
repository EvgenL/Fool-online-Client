using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject ContainerSelectLoginMethod;

    [SerializeField] private GameObject ContainerAnonLogin;

    [SerializeField] private GameObject ContainerEmailLogin;

    [SerializeField] private GameObject ContainerEmailRegister;

    private GameObject[] _containers;

    [Header("Show page by number on awake. Do nothing if = -1")]
    [SerializeField] private int _showOnAwake = -1;


    private void Awake()
    {
        _containers = new GameObject[] { ContainerSelectLoginMethod, ContainerAnonLogin, ContainerEmailLogin, ContainerEmailRegister };
        
        if (_showOnAwake >= 0)
        {
            Show(_showOnAwake);
        }
        else
        {
            // todo remember last login method
            ShowSelectLoginMethod();
        }
    }

    public void ShowSelectLoginMethod()
    {
        Show(0);
    }
    public void ShowAnonLogin()
    {
        Show(1);
    }
    public void ShowEmailLogin()
    {
        Show(2);
    }
    public void ShowEmailRegister()
    {
        Show(3);
    }

    /// <summary>
    /// Shows page and hides other pages
    /// todo animation
    /// </summary>
    private void Show(int pageNumber)
    {
        for (int i = 0; i < _containers.Length; i++)
        {
            _containers[i].SetActive(i == pageNumber);
        }
    }
}
