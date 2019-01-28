using System.Collections;
using System.Collections.Generic;
using Assets.Fool_online.Scripts.InRoom;
using Fool_online.Scripts.InRoom;
using UnityEngine;

public class AnimationTestButton : MonoBehaviour
{
    public Transform enemyHand;
    public Transform PlayerWhoGotReward;

    public void OnClick()
    {
        string card = "0.13";

       // CardAnimationsManager.Instance.YouGotCards(new[] { card }, e);
    }
    public void OnClickEnemy()
    {
        string card = "0.13";


        //var pi = FindObjectOfType<PlayerInfosManager>();
        CardAnimationsManager.Instance.EnemyGotCards(2, enemyHand);
    }

    public void PlayerGotReward()
    {
        MessageManager.Instance.PlayerGotReward(PlayerWhoGotReward, 250.0d);
    }
}
