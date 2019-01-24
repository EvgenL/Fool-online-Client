using System.Collections.Generic;
using DG.Tweening;
using Fool_online.Scripts.CardsScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.InRoom
{
    public abstract class PlayerInfo : MonoBehaviour
    {
        private const float ICON_FADE_TIME = 0.5f;

        [SerializeField] private RectTransform _turnStatusIconContainer;

        public enum PlayerStatusIcon
        {
            Won,
            Fool,
            Attacker,
            Defender,
            DefenderGaveUp
        }

        private PlayerStatusIcon CurrentStatusIcon;
        private Image CurrentStatusIconDisplay;

        [SerializeField] protected Text NicknameText;
        [SerializeField] protected Image Userpic;
        [SerializeField] protected Transform HandContainer;

        [SerializeField] private GameObject TextCloud;
        [SerializeField] private Text TextCloudText;


        protected int cardsN;

        public List<CardRoot> CardsInHand = new List<CardRoot>();


        public void ShowTextCloud(string message)
        {
            TextCloud.SetActive(true);
            TextCloudText.text = message;
        }
        public void HideTextCloud()
        {
            TextCloud.SetActive(false);
        }

        /// <summary>
        /// For calling it from debug button
        /// </summary>
        public void AnimateSpawnStatusIcon(int iconN)
        {
            AnimateSpawnStatusIcon((PlayerStatusIcon)iconN);
        }

        /// <summary>
        /// Animates apperar of player status icon like attack, defend, won, fool, etc
        /// in center of the screen and thel fly to player's info
        /// </summary>
        public void AnimateSpawnStatusIcon(PlayerStatusIcon icon)
        {
            AnimateHideCurrentStatusIcon();

            //init icon animation
            MessageManager.Instance.AnimatePlayerStatusIcon(_turnStatusIconContainer, icon);

            CurrentStatusIcon = icon;
        }


        /// <summary>
        /// Fade out current icon
        /// </summary>
        private void AnimateHideCurrentStatusIcon()
        {
            if (CurrentStatusIconDisplay != null)
            {
                var fadeTweener = CurrentStatusIconDisplay.DOFade(0, 0.5f);
            }
        }


        public abstract void PickCardsFromTable(List<CardRoot> cardsOnTable, List<CardRoot> cardsOnTableCovering);
    }
}
