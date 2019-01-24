using System.Collections;
using System.Collections.Generic;
using Assets.Fool_online.Scripts.InRoom;
using Fool_online.Scripts.InRoom;
using UnityEngine;

public class TakeCardFromTalonButton : MonoBehaviour
{
    public Transform enemyHand;

    public void OnClick()
    {
        string card = "0.13";

        CardAnimationsManager.Instance.YouGotCards(new[] { card });
    }
    public void OnClickEnemy()
    {
        string card = "0.13";


        //var pi = FindObjectOfType<PlayerInfosManager>();
        CardAnimationsManager.Instance.EnemyGotCards(2, enemyHand);
    }
}
