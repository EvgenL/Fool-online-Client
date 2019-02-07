using System.Collections.Generic;
using System.Linq;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.InRoom;
using Fool_online.Scripts.InRoom.CardsScripts;
using Fool_online.Scripts.InRoom.PlayersDisplay;
using UnityEngine;

namespace Fool_online.Scripts.Manager
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
        public DiscardPile Discard; //todo show discarded cardbacks

        public RectTransform TableContainerTransform;

        public List<CardRoot> cardsOnTable = new List<CardRoot>();
        public List<CardRoot> cardsOnTableCovering = new List<CardRoot>();

        public bool DefenderGaveUpDefence {private set; get; }
        private bool _attackerPassedPriority = false;

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
        public int TurnN {private set; get; }

        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnOtherPlayerJoinedRoom(long joinedPlayerId, int slotN, string joinedPlayerNickname)
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
                //EndGame();
            }
        }

        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnOtherPlayerDropsCardOnTable(long playerId, int slotN, string cardCode)
        {
            var enemy = PlayerInfosManager.SlotsScripts[slotN];
            var cardRoot = (enemy as EnemyInfo).DropCardOnTable(TableContainerTransform, cardCode);

            //Add to list
            cardsOnTable.Add(cardRoot);

            TableUpdated();
        }


        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnOtherPlayerPassed(long passedPlayerId, int slotN)
        {
                //if defender just passed then i can add card or just pass
                if (StaticRoomData.Denfender.ConnectionId == passedPlayerId)
                {
                    if (_attackerPassedPriority || ILedAttack())
                    {
                        MyPlayerInfoDisplay.ShowPassbutton();
                    }
                    else
                    {
                        // wait for attacker pass
                    }

                    DefenderGaveUpDefence = true;
                }
                //if attacker just passed then i can add card or just pass. If attack was defeated (all cards covered) then i can say 'beaten'
                else if (StaticRoomData.Attacker.ConnectionId == passedPlayerId)
                {
                    _attackerPassedPriority = true;

                    if (AllCardsCovered())
                    {
                        if (IamDefending())
                        {
                            if (AllButDefenderPassed())
                            {
                                //wait for next turn
                                State = GameState.Paused;
                            }
                        }
                        else
                        {
                            MyPlayerInfoDisplay.ShowBeatenbutton();
                        }
                    }
                    else
                    {
                        if (!IamDefending())
                        {
                            MyPlayerInfoDisplay.ShowPassbutton();
                        }
                    }
                }

            if (AllPassed())
            {
                //Waiting for server to send us next turn info
                State = GameState.Paused;
            }
        }

        private bool AllPassed()
        {
            return StaticRoomData.Players.All(x => x.Pass || x.Won);
        }

        private bool AllButDefenderPassed()
        {
            foreach (var player in StaticRoomData.Players)
            {
                if (player.Won) continue;

                if (player == StaticRoomData.Denfender)
                {
                    if (player.Pass)
                    {
                        return false;
                    }
                }
                else
                {
                    if (player.Pass == false)
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

            var enemy = PlayerInfosManager.SlotsScripts[slotN];
            var droppedCardRoot = (enemy as EnemyInfo).SpawnCard(cardDroppedCode);

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
            string foolNickname = StaticRoomData.GetPlayerNickname(foolPlayerId);
            MessageManager.Instance.ShowFullScreenText(foolNickname + " - дурак");

            Invoke("EndGame", 4f);

            State = GameState.WaitingForPlayersToConnect;
        }


        /// <summary>
        /// Checks if everybody's in room and enables 'ready' button
        /// </summary>
        private void CheckIfAllPlayersJoined()
        {
            if (StaticRoomData.ConnectedPlayersCount == StaticRoomData.MaxPlayers)
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
            TurnN = 0;
        }

        /// <summary>
        /// observer event
        /// </summary>
        public override void OnNextTurn(long whoseTurnPlayerId, int slotN, long defendingPlayerId, int defSlotN, int turnN)
        {
            DefenderGaveUpDefence = false;
            _attackerPassedPriority = false;

            this.TurnN = turnN;
            State = GameState.Playing;

            //if first turn
            if (this.TurnN == 1)
            {

                MyPlayerInfoDisplay.HideAllButtons();

                if (ILedAttack())
                {
                   // MessageManager.Instance.ShowFullScreenText("Вы ходите первым");
                }
                else
                {
                   // MessageManager.Instance.ShowFullScreenText("Первым ходит " + StaticRoomData.Players[slotN].Nickname);
                }
                return;
            }

            //if not first turn
            if (ILedAttack())
            {
                // MessageManager.Instance.ShowFullScreenText("Ваш ход");
            }
            else
            {
                // MessageManager.Instance.ShowFullScreenText("Ход игрока " + StaticRoomData.Players[slotN].Nickname);
            }
            
            

        }

        /// <summary>
        /// observer event
        /// </summary>
        public override void OnDropCardOnTableErrorCantDropThisCard(string cardCode)
        {
            MessageManager.Instance.ShowFullScreenText("Нельзя подкинуть эту карту");
            //TODO go back (cardCode)
        }

        /// <summary>
        /// observer event
        /// </summary>
        public override void OnDropCardOnTableErrorNotYourTurn(string cardCode)
        {
            MessageManager.Instance.ShowFullScreenText("Не ваш ход");
            //TODO go back (cardCode)
        }

        /// <summary>
        /// observer event
        /// </summary>
        public override void OnDropCardOnTableErrorTableIsFull(string cardCode)
        {
            MessageManager.Instance.ShowFullScreenText("Перебор");
            //TODO go back (cardCode)
        }

        public override void OnEndGameGiveUp(long foolConnectionId, Dictionary<long, double> rewards)
        {
            string nickname = StaticRoomData.GetPlayerNickname(foolConnectionId);
            MessageManager.Instance.ShowFullScreenText(nickname + " сдался.");
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

            //Wait for server to start game
            State = GameState.Paused;
        }

        /// <summary>
        /// Called by InputManager whenever i drop card on table
        /// </summary>
        public override void OnCardDroppedOnTableByMe(CardRoot droppedCardRoot)
        {
            StopTableCardAnimations();

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
                    MessageManager.Instance.ShowFullScreenText("Эту карту нельзя подкинуть");
                    return;
                }

                //if table is not full (5 cards on first turn, 6 on any other)
                if (cardsOnTable.Count < 6 || (TurnN == 1 && cardsOnTable.Count < 5))
                {
                    //ADD CARD 
                    droppedCardRoot.SetOnTable(true);
                    cardsOnTable.Add(droppedCardRoot);
                    MyPlayerInfoDisplay.RemoveCardFromHand(droppedCardRoot);
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
                    MessageManager.Instance.ShowFullScreenText("Перебор");
                } 
            }
            //Else if i am defending. COVERING A CARD LOGIC
            else if (IamDefending())
            {
                //if table is empty
                if (cardsOnTable.Count == 0)
                {
                    //you are defending
                    MessageManager.Instance.ShowFullScreenText("На вас ходят");
                    return;
                }

                //if i passed
                if (DefenderGaveUpDefence)
                {
                    MessageManager.Instance.ShowFullScreenText("Вы решили брать");
                    return;
                }

                //Choose cards that can be covered with grabbed card
                var cardsCanBeTargeted = GetCardsCanBeTargetedBy(droppedCardRoot);
                //if theres no cards that you cab beat then return this card to hand
                if (cardsCanBeTargeted.Count == 0)
                {
                    //you can not defent with this card
                    MessageManager.Instance.ShowFullScreenText("Вы не можете побиться этой картой");
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
                if (!_attackerPassedPriority)
                {
                    MessageManager.Instance.ShowFullScreenText("У атакующего приоритет");
                }
                else
                {
                    //ok
                }
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
            //Vector3 startPosition = droppedCard.interactibleCard.transform.position;
            //Quaternion startRotation = droppedCard.interactibleCard.transform.rotation;
            //Animate
            droppedCard.AnimateMoveToTransform(cardOnTable.CoveredCardContainer);
            cardOnTable.CoveredCardContainer.SetAsLastSibling();
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
            return _attackerPassedPriority || ILedAttack(); //&& todo am i neighbour 
        }

        /// <summary>
        /// If you are leader of an attack
        /// </summary>
        private bool ILedAttack()
        {
            return StaticRoomData.Attacker == StaticRoomData.MyPlayer;
        }

        /// <summary>
        /// if you are defending from attack leader and player who cal also add cards
        /// </summary>
        private bool IamDefending()
        {
            return StaticRoomData.Denfender == StaticRoomData.MyPlayer;
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
                    else if (!DefenderGaveUpDefence)
                    {
                        //i can give up an attack and take all cards from table
                        MyPlayerInfoDisplay.ShowPickUpCardsButton();
                    }
                    else //if (_defenderGaveUpDefence)
                    {
                        MyPlayerInfoDisplay.HideAllButtons();
                    }
                }
                //im not defending
                else
                {
                    MyPlayerInfoDisplay.HideAllButtons();

                    if (DefenderGaveUpDefence)
                    {
                        //attacker passed or i am attacker
                        if (IcanAddCards())
                        {
                            MyPlayerInfoDisplay.ShowPassbutton();
                        }
                    }
                    else
                    {
                        if (AllCardsCovered())
                        {
                            if (IcanAddCards())
                            {
                                MyPlayerInfoDisplay.ShowBeatenbutton();
                            }
                        }
                    }
                }
            }
            else
            {
                MyPlayerInfoDisplay.HideAllButtons();
            }

            if (DefenderGaveUpDefence)
            {
                //Hide 'i pass' texts
                PlayerInfosManager.HideTextCloudsNoDefender();
                //set every player to no-pass
                foreach (var player in StaticRoomData.Players)
                {
                    if (player != StaticRoomData.Denfender)
                    {
                        player.Pass = false;
                    }
                }
            }
            else
            {
                //set every player to no-pass
                foreach (var player in StaticRoomData.Players)
                {
                    //left player will be null if this was triggered on leave
                    if (player != null)
                    {
                        player.Pass = false;
                    }
                }

                //Hide 'i pass' texts
                PlayerInfosManager.HideTextClouds();
            }

            FoolNetworkObservableCallbacksWrapper.Instance.TableUpdated();
        }

        private void EndGame()
        {
            TurnN = 0;

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

        public void OnMePass()
        {
            MyPlayerInfoDisplay.HideAllButtons();

            if (State != GameState.Playing) return;

            if (AllCardsCovered())
            {
                //pass
                ClientSendPackets.Send_Pass();
               // MyPlayerInfoDisplay.ShowTextCloud("Бито");
                StaticRoomData.MyPlayer.Pass = true;
                FoolNetworkObservableCallbacksWrapper.Instance.MePassed();
            }
            //if i am defending 
            else if (IamDefending())
            {
                //pick up cards
                DefenderGaveUpDefence = true;

                ClientSendPackets.Send_Pass();
               // MyPlayerInfoDisplay.ShowTextCloud("Беру");
                StaticRoomData.MyPlayer.Pass = true;
                FoolNetworkObservableCallbacksWrapper.Instance.MePassed();
            }
            //if my turn and i am attacking
            else if (IcanAddCards())
            {
                //pass
                ClientSendPackets.Send_Pass();
               // MyPlayerInfoDisplay.ShowTextCloud("Пас");
                StaticRoomData.MyPlayer.Pass = true;
                FoolNetworkObservableCallbacksWrapper.Instance.MePassed();
            }

        }

        /// <summary>
        /// Called on each frame by InputManger when i drag card
        /// Animates cards that can be covered when you are defending
        /// </summary>
        public override void OnDraggedCardUpdate(Vector2 mousePos, CardRoot draggedCardRoot, bool inTableZone)
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

        }

        /// <summary>
        /// Stops glowing on cards which you could beat on table //todo migrate to table renderer class
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

        internal bool AllCardsCovered()
        {
            return cardsOnTable.All(card => card.IsCoveredByACard);
        }


    }
}
