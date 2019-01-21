using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fool_online.Scripts.CardsScripts;
using Fool_online.Scripts.Network.NetworksObserver;
using UnityEngine;
using UnityEngine.UI;

public class TalonRenderer : MonoBehaviourFoolNetworkObserver
{
    [Header("Talon")]
    public Text CardsLeftText;
    public Image TalonDisplay;

    [Header("Trump card")]
    public Image TrumpCard;
    public Image TrumpSuitIcon;

    [Header("")]
    public Sprite SpadesSprite;
    public Sprite HeartsSprite;
    public Sprite DiamondsSprite;
    public Sprite ClubsSprite;

    private Sprite[] _suits;

    public Vector3 hidPosition;
    private Vector3 showPosition;

    private int cardsInTalon;

    private void Awake()
    {
        showPosition = transform.position;

        transform.position = hidPosition;

        _suits = new Sprite[] {SpadesSprite, HeartsSprite, DiamondsSprite, ClubsSprite};
    }


    public void HideTalon()
    {
        transform.DOMove(hidPosition, 1f);
    }

    private void ShowTalon(int cards, string talonCardCode = null)
    {
        transform.DOMove(showPosition, 1f);
        cardsInTalon = cards;
        CardsLeftText.text = cards.ToString();
        TalonDisplay.enabled = true;
        TrumpCard.enabled = true;
        TrumpSuitIcon.enabled = false;

        if (cards == 1)
        {
            TalonDisplay.enabled = false;
        }

        if (cards == 0)
        {
            TrumpCard.enabled = false;
            TrumpSuitIcon.enabled = true;
        }

        if (talonCardCode != null)
        {
            TrumpCard.sprite = CardUtil.GetSprite(talonCardCode);

            int suitNumber = CardUtil.Suit(talonCardCode);
            TrumpSuitIcon.sprite = _suits[suitNumber];
        }
    }

    //observed callback
    public override void OnTalonData(int talonSize, string trumpCardCode)
    {
        ShowTalon(talonSize, trumpCardCode);
    }

    public override void OnYouGotCards(string[] cards)
    {
        cardsInTalon -= cards.Length;
        ShowTalon(cardsInTalon);
    }

    public override void OnEnemyGotCardsFromTalon(long playerId, int slotN, int cardsN)
    {
        cardsInTalon -= cardsN;
        ShowTalon(cardsInTalon);
    }
}
