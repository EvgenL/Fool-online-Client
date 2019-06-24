using System.Collections.Generic;
using DG.Tweening;
using DOTween.Modules;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.InRoom.CardsScripts;
using Fool_online.Scripts.Manager;
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
            Fool,
            Attacker,
            Defender,
            DefenderGaveUp,
            Money
        }

        private PlayerStatusIcon CurrentStatusIcon;

        [SerializeField] protected Text NicknameText;
        [SerializeField] protected AvatarDownloader Avatar;
        [SerializeField] protected Transform HandContainer;
        [SerializeField] protected Transform MoneyIconContainer;

        [SerializeField] private GameObject TextCloud;
        [SerializeField] private Text TextCloudText;

        public long connectionId;

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

            //Money icon contains text so uses different prefab
            if (icon == PlayerStatusIcon.Money)
            {
                //init icon animation
                return; //GameplayMessageManager.Instance.AnimatePlayerStatusIcon(MoneyIconContainer, icon);
            }
            else
            {
                //init icon animation
                GameplayMessageManager.Instance.AnimatePlayerStatusIcon(TurnStatusIconContainer, icon);
            }

            CurrentStatusIcon = icon;
        }

        public void AnimateMoneyReward(double amount)
        {
            AnimateHideCurrentStatusIcon();
            GameplayMessageManager.Instance.AnimateMoney(MoneyIconContainer, amount);
        }
        /// <summary>
        /// Spawns player status icon like attack, defend, won, fool, etc
        /// in player's info without flying from screen centre
        /// </summary>
        public void SetStatusIconNoAnimation(PlayerStatusIcon icon)
        {
            AnimateHideCurrentStatusIcon();

            //init icon animation
            var iconTransform = GameplayMessageManager.Instance.SpawnIconAtScreenCentre(icon);
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
            var currntIcon = TurnStatusIconContainer.GetComponentInChildren<Image>();

            if (currntIcon != null)
            {
                var fadeTweener = currntIcon.DOFade(0, 0.5f);
                fadeTweener.OnComplete(() => Destroy(currntIcon.gameObject));
            }
        }

        public void HideMoneyIcon()
        {
            var currntIcon = MoneyIconContainer.GetComponentInChildren<Image>();

            if (currntIcon != null)
            {
                var fadeTweener = currntIcon.DOFade(0, 0.5f);
                fadeTweener.OnComplete(() => Destroy(currntIcon.gameObject));

                var text = currntIcon.GetComponentInChildren<Text>();
                text.DOFade(0, 0.5f);
            }

            Util.DestroyAllChildren(MoneyIconContainer);
        }

        /// <summary>
        /// Demove all card displays from hand
        /// </summary>
        public virtual void ClearHand()
        {
            Util.DestroyAllChildren(HandContainer);
            CardsInHand.Clear();
        }

        public virtual void DrawPlayerslot(PlayerInRoom playerInRoom)
        {
            connectionId = playerInRoom.ConnectionId;
            NicknameText.text = playerInRoom.Nickname;

            Avatar.AvatarHolderConnectionId = playerInRoom.ConnectionId;

            // force callback for avatar update
            if (!string.IsNullOrEmpty(playerInRoom.AvatarFile))
            {
                Avatar.OnUpdateUserAvatar(playerInRoom.ConnectionId, playerInRoom.AvatarFile);
            }

            HideMoneyIcon();
            HideTextCloud();
        }

        public virtual void DrawEmptySlot()
        {
            NicknameText.text = "Ожидание противника";
            Avatar.ResetImage();

            HideTextCloud();
            HideMoneyIcon();
            AnimateHideCurrentStatusIcon();
            SetReadyCheckmark(false);
        }

        public virtual void DrawLeftSlot()
        {
            NicknameText.text += "(вышел)";

            HideTextCloud();
            AnimateHideCurrentStatusIcon();
            SetReadyCheckmark(false);
        }


        public virtual void SetReadyCheckmark(bool value)
        {
        }


        public abstract void PickCardsFromTable(List<CardRoot> cards);

        /// <summary>
        /// Moves all cards to discard at game end
        /// </summary>
        public virtual void AnimateRemoveCardsToDiscardPile(DiscardPile discard, float delay)
        {
            foreach (var cardInHand in CardsInHand)
            {
                discard.AnimateRemoveCardToDiscardPile(cardInHand, delay);
            }

            CardsInHand.Clear();
        }
    }
}
