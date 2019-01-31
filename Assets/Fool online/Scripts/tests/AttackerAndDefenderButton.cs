using Fool_online.Scripts.Manager;
using UnityEngine;

namespace Fool_online.Scripts.tests
{
    public class AttackerAndDefenderButton : MonoBehaviour
    {
        [SerializeField] private Transform player1container;
        [SerializeField] private Transform player2container;

        public void OnClick()
        {
            MessageManager.Instance.AnimateAttackerAndDefender(player1container, player2container);
        }
    }
}
