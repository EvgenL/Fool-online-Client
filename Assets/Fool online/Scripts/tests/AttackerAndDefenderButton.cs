using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerAndDefenderButton : MonoBehaviour
{
    [SerializeField] private Transform player1container;
    [SerializeField] private Transform player2container;

    public void OnClick()
    {
        MessageManager.Instance.AnimateAttackerAndDefender(player1container, player2container);
    }
}
