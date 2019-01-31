using UnityEngine;

namespace Fool_online.Scripts.tests
{
    public class PlayersGetRewardsButton : MonoBehaviour
    {
        [SerializeField] private Transform player1container;
        [SerializeField] private Transform player2container;

        public void OnClick()
        {
            int p1reward = 10000;
            int p2reward = -10000;

           // MessageManager.Instance.AnimatePlayersRewards

        }
    }
}
