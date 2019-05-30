using System.Collections.Generic;
using System.Linq;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.InRoom;
using Fool_online.Scripts.InRoom.CardsScripts;
using Fool_online.Scripts.Manager;
using UnityEngine;

namespace Assets.Fool_online.Scripts.Manager
{
    /// <summary>
    /// Derived in RoomActions
    /// Container for fields that describe current state of room
    /// </summary>
    public class RoomStateFields : MonoBehaviourFoolObserver //todo remove inheritance from MonoBehaviourFoolObserver
    {
        /// <summary>
        /// Init all fields on constructor
        /// </summary>
        public RoomStateFields()
        {
            if (!FoolNetwork.IsConnected) return;
        }

        #region Constants and enums

        public enum RoomState
        {
            WaitingForPlayersToConnect,
            PlayersGettingReady,
            Playing,
            Paused
        }

        #endregion



        #region Fields

        private RoomState _state;
        /// <summary>
        /// Current game state
        /// </summary>
        protected RoomState State
        {
            get { return _state;}
            set
            {
                Debug.Log("State = " + value);
                _state = value;
            }
        }

        /// <summary>
        /// Options set on room creation
        /// </summary>
        public RoomRules Rules;

        /// <summary>
        /// Connected players
        /// </summary>
        public PlayerInRoom[] Players;

        /// <summary>
        /// Player who defends this turn
        /// </summary>
        public PlayerInRoom Defender;

        /// <summary>
        /// Player who lead attack this turn
        /// </summary>
        public PlayerInRoom Attacker;

        /// <summary>
        /// Set to true when all players are connected
        /// </summary>
        public bool RoomIsFull; //todo

        /// <summary>
        /// cards currently on table
        /// </summary>
        public List<CardRoot> cardsOnTable = new List<CardRoot>();
        /// <summary>
        /// cards currently covering other cards
        /// </summary>
        public List<CardRoot> cardsOnTableCovering = new List<CardRoot>();

        /// <summary>
        /// did attacker passed their turn
        /// </summary>
        public bool AttackerPassedPriority = false;

        /// <summary>
        /// did defender passed their turn
        /// and that means that he gave up a defense
        /// </summary>
        public bool DefenderPassedPriority = false;

        /// <summary>
        /// Number of current turn
        /// </summary>
        public int TurnN;

        #endregion



        #region Properties

        /// <summary>
        /// If you are leader of an attack
        /// </summary>
        protected bool MeLeadAttack => Attacker == StaticRoomData.MyPlayer;

        /// <summary>
        /// If you are defending from attack leader and players who can also add cards
        /// </summary>
        protected bool MeDefending => Defender == StaticRoomData.MyPlayer;

        /// <summary>
        /// If you are not leader of an attack but can add cards
        /// </summary>
        protected bool IcanAddCards => (AttackerPassedPriority || MeLeadAttack) && !MeDefending;

        /// <summary>
        /// Number of cards not covered by other card
        /// </summary>
        protected int NotCoveredCardsN => cardsOnTable.Count - cardsOnTableCovering.Count;

        /// <summary>
        /// total nomber of cards both on table and covering
        /// </summary>
        protected int CardsOnTableNumber => cardsOnTable.Count + cardsOnTableCovering.Count;

        /// <summary>
        /// Checks if everybody is ready
        /// </summary>
        protected bool EverybodyReady => 
            Players.All(player => player.IsReady);

        /// <summary>
        /// Checks game was just startred
        /// </summary>
        public bool TurnNumberIsFirst => TurnN == 1;

        /// <summary>
        /// Did all player clicked pass?
        /// Player who won counts as passed.
        /// </summary>
        protected bool AllPassed => 
            Players.All(player => player.Pass || player.Won);


        /// <summary>
        /// Did all player but defender clicked pass?
        /// Player who won counts as passed.
        /// </summary>
        protected bool AllButDefenderPassed =>
            Players.All(player => player.Pass || player.Won
                                              || (player == Defender && !player.Pass));


        /// <summary>
        /// Are all cards on table covered by other cards?
        /// </summary>
        public bool AllCardsCovered =>
            cardsOnTable.All(card => card.IsCoveredByACard);

        /// <summary>
        /// When i try add car check:
        /// Do defender have at least one card to defend with?
        /// </summary>
        protected bool DefenderHasCardsToDefend => Defender.CardsNumber - NotCoveredCardsN > 0;

        /// <summary>
        /// Do table have a slot for card?
        /// Table can contain 5 cards on first turn
        /// and 6 cards on any other turn
        /// </summary>
        protected bool TableIsFull => cardsOnTable.Count >= 6 || (cardsOnTable.Count >= 5 && TurnNumberIsFirst);

        protected bool TableIsEmpty => cardsOnTable.Count == 0;

        #endregion

    }
}
