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

        public RectTransform TurnStatusIconContainer;

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
            MessageManager.Instance.AnimatePlayerStatusIcon(TurnStatusIconContainer, icon);

            CurrentStatusIcon = icon;
        }
        /// <summary>
        /// Spawns player status icon like attack, defend, won, fool, etc
        /// in player's info without flying from screen centre
        /// </summary>
        public void SetStatusIconNoAnimation(PlayerStatusIcon icon)
        {
            AnimateHideCurrentStatusIcon();

            //init icon animation
            var iconTransform = MessageManager.Instance.SpawnIconAtScreenCentre(icon);
            iconTransform.SetParent(TurnStatusIconContainer, false);

            var iconDisplay = iconTransform.GetComponent<Image>();
            //make transparent
            iconDisplay.color = new Color(1, 1, 1, 0);
            iconDisplay.DOFade(1f, 0.5f);

            CurrentStatusIcon = icon;
        }


        /// <summary>
        /// Fade out current icon
        /// </summary>
        public void AnimateHideCurrentStatusIcon()
        {
            CurrentStatusIconDisplay = TurnStatusIconContainer.GetComponentInChildren<Image>();

            if (CurrentStatusIconDisplay != null)
            {
                var fadeTweener = CurrentStatusIconDisplay.DOFade(0, 0.5f);
                fadeTweener.OnComplete(() => Destroy(CurrentStatusIconDisplay.gameObject));
            }
        }

        /// <summary>
        /// Demove all card displays from hand
        /// </summary>
        public virtual void ClearHand()
        {
            Util.DestroyAllChildren(HandContainer);
            CardsInHand.Clear();
        }

        public virtual void DrawPlayer(PlayerInRoom playerInRoom)
        {
            NicknameText.text = playerInRoom.Nickname;
        }

        public virtual void DrawEmpty()
        {
            NicknameText.text = "Ожидание противника";

            HideTextCloud();
            AnimateHideCurrentStatusIcon();
            SetReadyCheckmark(false);
        }


        public virtual void SetReadyCheckmark(bool value)
        {
        }


        public abstract void PickCardsFromTable(List<CardRoot> cardsOnTable, List<CardRoot> cardsOnTableCovering);
    }
}
