using System.Collections.Generic;
using System.Linq;
using Fool_online.Scripts.CardsScripts;
using Fool_online.Scripts.Network;
using Fool_online.Scripts.Network.NetworksObserver;
using UnityEngine;

namespace Fool_online.Scripts.InRoom
{
    /// <summary>
    /// Calss responsive for fool game rules
    /// </summary>
    public class GameManager : MonoBehaviourFoolNetworkObserver
    {
        public enum GameState
        {
            WaitingForPlayersToConnect,
            PlayersGettingReady,
            Playing,
            Paused
        }

        public GameState State = GameState.WaitingForPlayersToConnect;

        public static GameManager Instance;

        public PlayerInfosManager PlayerInfosManager;

        public TableRenderer TableDisplay;

        public MyPlayerInfo MyPlayerInfoDisplay;

        public TalonRenderer TalonDisplay;
        public DiscardPile Discard;

        public RectTransform TableContainerTransform;

        public List<CardRoot> cardsOnTable = new List<CardRoot>();
        public List<CardRoot> cardsOnTableCovering = new List<CardRoot>();

        private int _cardsAddedByMe;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            PlayerInfosManager.InitRoomData();

            CheckIfAllPlayersJoined();
            
            Util.DestroyAllChildren(TableContainerTransform);
        }
    
        /// <summary>
        /// Number of current turn
        /// </summary>
        private int _turnN; 

        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnOtherPlayerJoinedRoom(long joinedPlayerId, int slotN)
        {
            CheckIfAllPlayersJoined();
        }

        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnOtherPlayerLeftRoom(long leftPlayerId, int slotN)
        {
            if (State == GameState.PlayersGettingReady || State == GameState.WaitingForPlayersToConnect)
            {
                CheckIfAllPlayersJoined();
            }
            else if (State == GameState.Playing)
            {
                //TODO not end game but wait
                EndGame();
            }
        }

        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnOtherPlayerDropsCardOnTable(long playerId, int slotN, string cardCode)
        {
            print(("OnOtherPlayerDropsCardOnTable"));
            var enemy = PlayerInfosManager.SlotsScripts[slotN];
            var cardRoot = enemy.DropCardOnTable(TableContainerTransform, cardCode);

            //Add to list
            cardsOnTable.Add(cardRoot);

            TableUpdated();
        }


        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnOtherPlayerPassed(long passedPlayerId, int slotN)
        {
            if (AllPassed())
            {
                //Waiting for server to send us next turn info
                State = GameState.Paused;
            }
        }

        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnOtherPlayerPickUpCards(long passedPlayerId, int slotN)
        {
            MyPlayerInfoDisplay.ShowPassbutton();

            if (AllPassed())
            {
                //Waiting for server to send us next turn info
                State = GameState.Paused;
            }
        }

        private bool AllPassed()
        {
            return StaticRoomData.Players.All(x => x.Pass);
        }

        private bool AllButDefenderPassed()
        {
            foreach (var player in StaticRoomData.Players)
            {
                if (player == StaticRoomData.Denfender)
                {
                    if (player.Pass)
                    {
                        return false;
                    }
                }
                else
                {
                    if (player.Pass != true)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnOtherPlayerCoversCard(long coveredPlayerId, int slotN,
            string cardOnTableCode, string cardDroppedCode)
        {
            print("Other player covers card! " + cardOnTableCode + " by " + cardDroppedCode);

            var droppedCardRoot = PlayerInfosManager.SlotsScripts[slotN].SpawnCard(cardDroppedCode);

            CardRoot cardOnTable = cardsOnTable.Find(card => card.CardCode == cardOnTableCode);

            AnimateCoverCardBy(cardOnTable, droppedCardRoot);

            TableUpdated();
        }

        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnBeaten()
        {
            cardsOnTable.Clear();
            cardsOnTableCovering.Clear();

            //Wait for server to send us NextTurn
            State = GameState.Paused;
        }

        public override void OnDefenderPicksCards(long pickedPlayerId, int slotN)
        {
            if (slotN == StaticRoomData.MySlotNumber)
            {
                MyPlayerInfoDisplay.PickCardsFromTable(cardsOnTable, cardsOnTableCovering);
            }
            else
            {
                PlayerInfosManager.PickCardsFromTable(slotN, cardsOnTable, cardsOnTableCovering);
            }

            cardsOnTable.Clear();
            cardsOnTableCovering.Clear();

            //Wait for server to send us NextTurn
            State = GameState.Paused;
        }

        public override void OnEndGameFool(long foolPlayerId)
        {
            MessageManager.Show("Игрок " + foolPlayerId + " дурак");

            Invoke("EndGame", 4f);

            State = GameState.WaitingForPlayersToConnect;
        }


        /// <summary>
        /// Checks if everybody's in room and enables 'ready' button
        /// </summary>
        private void CheckIfAllPlayersJoined()
        {
            if (StaticRoomData.ConnectedPlayersCount == StaticRoomData.MaxPalyers)
            {
                State = GameState.PlayersGettingReady;
                MyPlayerInfoDisplay.ShowGetReadyButton();
            }
            else
            {
                if (State == GameState.Playing)
                {
                    State = GameState.WaitingForPlayersToConnect;
                }
                else
                {
                    State = GameState.PlayersGettingReady;
                }
                MyPlayerInfoDisplay.HideGetReadyButton();
            }
        }

        /// <summary>
        /// On i clicked get ready button
        /// </summary>
        public void OnGetReady(bool value)
        {
            if (State != GameState.PlayersGettingReady) return;

            if (value)
            {
                ClientSendPackets.Send_GetReady();
            }
            else
            {
                ClientSendPackets.Send_GetNotReady();
            }

            StaticRoomData.MyPlayer.IsReady = value;

            CheckEverybodyReady();
        }

        /// <summary>
        /// observer event
        /// </summary>
        public override void OnOtherPlayerGotReady(long leftPlayerId, int slotN)
        {
            CheckEverybodyReady();
        }

        public override void OnStartGame()
        {
            _turnN = 0;

        }

        /// <summary>
        /// observer event
        /// </summary>
        public override void OnNextTurn(long whoseTurn, int slotN, long defendingPlayerId, int defSlotN, int turnN)
        {
            print("OnNextTurn");

            this._turnN = turnN;
            _cardsAddedByMe = 0;
            State = GameState.Playing;
            MyPlayerInfoDisplay.HideTextCloud();

            //if first turn
            if (this._turnN == 1)
            {

                MyPlayerInfoDisplay.HideAllButtons();

                if (ILedAttack())
                {
                    MessageManager.Show("Вы ходите первым");
                }
                else
                {
                    MessageManager.Show("Первым ходит " + StaticRoomData.Players[slotN].Nickname);
                }
                return;
            }

            //if not first turn
            if (ILedAttack())
            {
                MessageManager.Show("Ваш ход");
            }
            else
            {
                MessageManager.Show("Ход игрока " + StaticRoomData.Players[slotN].Nickname);
            }

        }

        /// <summary>
        /// observer event
        /// </summary>
        public override void OnDropCardOnTableErrorCantDropThisCard(string cardCode)
        {
            MessageManager.Show("Можно подкидывать только карты той же стоимости что и на столе");
            //TODO go back (cardCode)
        }

        /// <summary>
        /// observer event
        /// </summary>
        public override void OnDropCardOnTableErrorNotYourTurn(string cardCode)
        {
            MessageManager.Show("Не ваш ход");
        }

        /// <summary>
        /// observer event
        /// </summary>
        public override void OnDropCardOnTableErrorTableIsFull(string cardCode)
        {
            MessageManager.Show("Перебор");
        }

        public override void OnEndGameGiveUp(long foolConnectionId, Dictionary<long, int> rewards)
        {
            MessageManager.Show("Игрок " + foolConnectionId + " сдался.");
            EndGame();
        }

        /// <summary>
        /// Checks if everybody is ready and starts a game
        /// </summary>
        private void CheckEverybodyReady()
        {
            //if atleast one player not ready then return
            foreach (var player in StaticRoomData.Players) {
                if (player != null && !player.IsReady) return;
            }

            //else if everybody's ready start game
            MyPlayerInfoDisplay.HideGetReadyButton();
        }

        /// <summary>
        /// Called by InputManager whenever i drop card on table
        /// </summary>
        public void CardDroppedOnTableByMe(CardRoot droppedCardRoot)
        {
            if (State != GameState.Playing) return;

            if (AllPassed() || 
                (AllButDefenderPassed() &&
                 AllCardsCovered())) return;

            //if i am attacking. Else if i am defending.
            if (ILedAttack() || IcanAddCards())
            {
                //If table is not empty and trying to drop wrong card
                if (cardsOnTable.Count >= 1 && !CanDropThisCard(droppedCardRoot))
                {
                    //'Cant add this card to table'
                    MessageManager.Show("Эту карту нельзя подкинуть");
                    return;
                }

                //if table is empty or not full
                if (cardsOnTable.Count < 6)
                {
                    //ADD CARD 
                    droppedCardRoot.SetOnTable(true);
                    cardsOnTable.Add(droppedCardRoot);
                    MyPlayerInfoDisplay.RemoveCardFromHand(droppedCardRoot);
                    _cardsAddedByMe++;
                    //init animation
                    droppedCardRoot.AnimateMoveToTransform(TableContainerTransform);
                    //StartCoroutine(AnimatePutMyCardOnTableOnNextFrame(droppedCardRoot, TableContainerTransform));
                    //TODO save in buffer for in case if server will say no
                    //Send to server
                    ClientSendPackets.Send_DropCardOnTable(droppedCardRoot.CardCode);

                    TableUpdated();
                }
                //if table IS full
                else
                {
                    //Too much cards on table
                    MessageManager.Show("Перебор");
                } 
            }
            //Else if i am defending. COVERING A CARD LOGIC
            else if (IamDefending())
            {
                //if table is empty
                if (cardsOnTable.Count == 0)
                {
                    //you are defending
                    MessageManager.Show("На вас ходят");
                    return;
                }

                //if i passed
                if (StaticRoomData.DefenderPassed())
                {
                    MessageManager.Show("Вы решили брать");
                    return;
                }

                //Choose cards that can be covered with grabbed card
                var cardsCanBeTargeted = GetCardsCanBeTargetedBy(droppedCardRoot);
                //if theres no cards that you cab beat then return this card to hand
                if (cardsCanBeTargeted.Count == 0)
                {
                    //you can not defent with this card
                    MessageManager.Show("Вы не можете побиться этой картой");
                    return;
                }
                //Chose closest beatable card
                var closestCard = GetClosestCardFrom(cardsCanBeTargeted, Input.mousePosition);

                //Beat it
                AnimateCoverCardBy(closestCard, droppedCardRoot);

                //Send to server
                ClientSendPackets.Send_CoverCardOnTable(closestCard.CardCode, droppedCardRoot.CardCode);

                TableUpdated();

            }
            //else if i not attacking nor defenfing and cant add cards
            else
            {
                MessageManager.Show("Не ваш ход");
            }
        }

        /// <summary>
        /// Do table contains card of same value with grabbed?
        /// </summary>
        private bool CanDropThisCard(CardRoot cardRoot)
        {
            foreach (var cardOnTable in cardsOnTable)
            {
                if (CardUtil.Value(cardRoot.CardCode) == CardUtil.Value(cardOnTable.CardCode))
                {
                    return true;
                }
            }
            foreach (var cardOnTableCovering in cardsOnTableCovering)
            {
                if (CardUtil.Value(cardRoot.CardCode) == CardUtil.Value(cardOnTableCovering.CardCode))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Covers card on table with dropped card. Adds to covered cards list
        /// </summary>
        private void AnimateCoverCardBy(CardRoot cardOnTable, CardRoot droppedCard)
        {
            //Disable interactions
            droppedCard.SetOnTable(true);
            //Set start position for animation
            Vector3 startPosition = droppedCard.interactibleCard.transform.position;
            Quaternion startRotation = droppedCard.interactibleCard.transform.rotation;
            //Set root pos to be on covered card
            droppedCard.transform.SetParent(cardOnTable.transform);
            droppedCard.transform.localPosition = cardOnTable.CoveredPosition;
            droppedCard.transform.localRotation = cardOnTable.CoveredRotation;
            //Animate
            droppedCard.AnimateFromToRoot(startPosition, startRotation);

            //Add to list
            cardsOnTableCovering.Add(droppedCard);
            //Set cards parameters
            cardOnTable.IsCoveredByACard = true;
            cardOnTable.CoveredByCard = droppedCard;

        }

        /// <summary>
        /// If you are not leader of an attack but can add cards
        /// </summary>
        private bool IcanAddCards()
        {
            return ILedAttack();
        }

        /// <summary>
        /// If you are leader of an attack
        /// </summary>
        private bool ILedAttack()
        {
            return StaticRoomData.WhoseTurn == FoolNetwork.LocalPlayer.ConnectionId;
        }

        /// <summary>
        /// if you are defending from attack leader and player who cal also add cards
        /// </summary>
        private bool IamDefending()
        {
            //TODO multiplayer
            return !ILedAttack();
        }

        /// <summary>
        /// called when someone drops card on table
        /// </summary>
        private void TableUpdated()
        {
            //If there is at least one card
            if (cardsOnTable.Count >= 1 && cardsOnTable.Count <= 6)
            {
                if (IamDefending())
                {
                    //if i've succesfully defended
                    if (AllCardsCovered())
                    {
                        MyPlayerInfoDisplay.HideAllButtons();
                    }
                    else
                    {
                        //i can give up an attack and take all cards from table
                        MyPlayerInfoDisplay.ShowPickUpCardsButton();
                    }
                }
                //if not my turn but i can add cards to table
                else if (!ILedAttack() && IcanAddCards() && StaticRoomData.DefenderPassed())
                {
                    if (_cardsAddedByMe > 0)
                    {
                        MyPlayerInfoDisplay.ShowBeatenbutton();
                    }
                    else
                    {
                        MyPlayerInfoDisplay.ShowPassbutton();
                    }
                }
                else if (ILedAttack() && AllCardsCovered())
                {
                    MyPlayerInfoDisplay.ShowBeatenbutton();
                }
            }
            else
            {
                MyPlayerInfoDisplay.HideAllButtons();
            }

            //Hide 'i pass' texts
            MyPlayerInfoDisplay.HideTextCloud();
            PlayerInfosManager.HideTextClouds();

            //if defender didn't gave up an attack
            if (!StaticRoomData.DefenderPassed())
            {
                //set every player to no-pass
                foreach (var player in StaticRoomData.Players)
                {
                    if (player != null)
                        player.Pass = false;
                }
            }

            StopTableCardAnimations();
        }

        private void EndGame()
        {
            _turnN = 0;

            RemoveAllCardsToDiscardPileAnimation();
            TableUpdated();

            //Set state accordingly to how much players are in here
            CheckIfAllPlayersJoined();
        }

        /// <summary>
        /// Removes every card from table and players hands and moves to discard pile
        /// </summary>
        public void RemoveAllCardsToDiscardPileAnimation()
        {
            TalonDisplay.HideTalon();

            TableDisplay.RemoveCardsFromTableToDiscardPile();

            //cards on table
            cardsOnTable.Clear();
            cardsOnTableCovering.Clear();

            //my cards
            foreach (var cardInHand in MyPlayerInfoDisplay.CardsInHand)
            {
                Discard.AnimateRemoveCardToDiscardPile(cardInHand);

            }
            MyPlayerInfoDisplay.CardsInHand.Clear();

            //enemies
            foreach (var slot in PlayerInfosManager.SlotsScripts)
            {
                if (slot == null) continue;

                //enemies cards
                foreach (var cardInHand in slot.CardsInHand)
                {
                    Discard.AnimateRemoveCardToDiscardPile(cardInHand);
                }
                slot.CardsInHand.Clear();
            }
        }

        public void OnEndTurnButtonClick()
        {
            MyPlayerInfoDisplay.HideAllButtons();

            if (State != GameState.Playing) return;

            if (ILedAttack() && AllCardsCovered())
            {
                //pass
                ClientSendPackets.Send_Pass();
                MyPlayerInfoDisplay.ShowTextCloud("Бито");
                StaticRoomData.MyPlayer.Pass = true;
            }
            //if i am defending 
            else if (IamDefending())
            {
                //pick up cards
                ClientSendPackets.Send_PickUpCards();
                MyPlayerInfoDisplay.ShowTextCloud("Беру");
                StaticRoomData.MyPlayer.Pass = true;
            }
            //if my turn and i am attacking
            else if ((!ILedAttack() || IcanAddCards()) && _cardsAddedByMe > 0)
            {
                //pass
                ClientSendPackets.Send_Pass();
                MyPlayerInfoDisplay.ShowTextCloud("Пас");
                StaticRoomData.MyPlayer.Pass = true;
            }
        }

        /// <summary>
        /// Called on each frame by InputManger when i drag card
        /// Animates cards that can be covered when you are defending
        /// </summary>
        public void DraggedCardUpdate(Vector2 mousePos, CardRoot draggedCardRoot, bool inTableZone)
        {
            //Am i defending or attacking?
            if (IamDefending())
            {
                //if dragged above a table
                if (inTableZone)
                {
                    //Choose cards that can be covered with grabbed card
                    var cardsCanBeTargeted = GetCardsCanBeTargetedBy(draggedCardRoot);
                    if (cardsCanBeTargeted.Count == 0) return;

                    //Chose closest beatable card
                    var closestCard = GetClosestCardFrom(cardsCanBeTargeted, mousePos);

                    //Animate them
                    foreach (var cardCanBeTargeted in cardsCanBeTargeted)
                    {
                        if (cardCanBeTargeted == closestCard)
                        {
                            cardCanBeTargeted.interactibleCard.AnimateTargeted();
                        }
                        else
                        {
                            cardCanBeTargeted.interactibleCard.AnimateCanBeTargeted();
                        }
                    }
                }
                else
                {
                    //if not in table: idle animation
                    StopTableCardAnimations();
                }
            }
            else if (ILedAttack() || IcanAddCards())
            {

            }

        }

        /// <summary>
        /// Stops glowing on cards which you could beat on table
        /// </summary>
        private void StopTableCardAnimations()
        {
            foreach (var cardOnTable in cardsOnTable)
            {
                cardOnTable.interactibleCard.AnimateIdle();
            }
        }

        /// <summary>
        /// find cards which can be targeted (has lower value and same suit)
        /// </summary>
        private List<CardRoot> GetCardsCanBeTargetedBy(CardRoot draggedCardRoot)
        {

            bool draggingTrump = draggedCardRoot.IsTrump();
            List<CardRoot> cardsCanBeTargeted = new List<CardRoot>();
            foreach (var cardOnTable in cardsOnTable)
            {
                //Not counting cards that already covered
                if (cardOnTable.IsCoveredByACard) continue;
                //if holding a trump card...
                if (draggingTrump)
                {
                    //..and card on table is not trump
                    if (!cardOnTable.IsTrump())
                    {
                        cardsCanBeTargeted.Add(cardOnTable);
                    }
                    else //..and card on table IS trump
                    {
                        //count only value
                        if (cardOnTable.Value < draggedCardRoot.Value)
                        {
                            cardsCanBeTargeted.Add(cardOnTable);
                        }
                    }
                }
                else //if holding not a trump card...
                {
                    //..and card on table is not trump
                    if (!cardOnTable.IsTrump())
                    {
                        //count only value
                        if (cardOnTable.Value < draggedCardRoot.Value && cardOnTable.Suit == draggedCardRoot.Suit)
                        {
                            cardsCanBeTargeted.Add(cardOnTable);
                        }
                    }
                    else //..and card on table IS trump
                    {
                        //No card can beat trump if its not trump
                    }
                }
            }

            return cardsCanBeTargeted;
        }

        /// <summary>
        /// returns closest card to mousePos
        /// </summary>
        private CardRoot GetClosestCardFrom(List<CardRoot> cardsOnTable, Vector2 mousePos)
        {
            CardRoot closest = null;
            float minDistance = float.MaxValue;

            foreach (var cardOnTable in cardsOnTable)
            {
                float dist = Vector2.Distance(cardOnTable.transform.position, mousePos);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = cardOnTable;
                }
            }

            return closest;
        }


        public void TakeAllCardsFromTable()
        {
            foreach (var cardOnTable in cardsOnTable)
            {
                MyPlayerInfoDisplay.PickUpCard(cardOnTable);
            }
            foreach (var cardOnTable in cardsOnTableCovering)
            {
                MyPlayerInfoDisplay.PickUpCard(cardOnTable);
            }
            cardsOnTable.Clear();
            cardsOnTableCovering.Clear();
        }

        private bool AllCardsCovered()
        {
            return cardsOnTable.All(card => card.IsCoveredByACard);
        }


    }
}
