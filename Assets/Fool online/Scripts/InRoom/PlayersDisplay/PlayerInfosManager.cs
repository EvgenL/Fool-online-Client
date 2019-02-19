using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.InRoom.CardsScripts;
using Fool_online.Scripts.Manager;
using UnityEngine;

namespace Fool_online.Scripts.InRoom.PlayersDisplay
{

    /// <summary>
    /// Calss handling top part of the screen with enemy names and cards
    /// </summary>
    public class PlayerInfosManager : MonoBehaviourFoolObserver
    {
        public GameObject EnemySlotPrefab;
        public Transform SlotsContainer;

        public GameObject[] SlotsGo;
        /// <summary>
        /// Array of abstract class PlayerInfo which contains my player info and enemt player info
        /// </summary>
        public PlayerInfo[] SlotsScripts;

        private const float DelayBeforeNextTurn = 0.5f;

        /// <summary>
        /// Draw room on start
        /// </summary>
        private void Start()
        {
            InitRoomData();
        }

        /// <summary>
        /// Draw everything when local player enters room
        /// </summary>
        private void InitRoomData()
        {
            if (!FoolNetwork.IsConnected) return;
            //Clear slots is there were
            Util.DestroyAllChildren(SlotsContainer);

            print("EnemiesDisplay: InitRoomData");

            //There's gonna be amount of slots available
            SlotsScripts = new PlayerInfo[StaticRoomData.MaxPlayers];
            var MySlot = FindObjectOfType<MyPlayerInfo>();
            SlotsScripts[StaticRoomData.MySlotNumber] = MySlot;

            SlotsGo = new GameObject[StaticRoomData.MaxPlayers];

            DrawSlots();

            foreach (var slot in SlotsScripts)
            {
                if (slot != null)
                {
                    slot.ClearHand();
                }
            }
        }

        /// <summary>
        /// Draws slots
        /// //TODO if max slots changed during preparation (?)
        /// </summary>
        private void DrawSlots()
        {
            //Instanciate empty slots
            for (int slotI = 0; slotI < StaticRoomData.MaxPlayers; slotI++)
            {
                //If this is data about us then skip
                if (slotI == StaticRoomData.MySlotNumber) continue;

                AddEmptySlot(slotI);
            }

            SortSlotDisplays();

            //set occupied slots (if we aren't first to enter this room)
            for (int slotI = 0; slotI < StaticRoomData.MaxPlayers; slotI++)
            {
                //If this is data about us then skip
                if (slotI == StaticRoomData.MySlotNumber) continue;

                //if occupied by some player
                if (StaticRoomData.Players[slotI] != null)
                {
                    SetSlotAsOccupied(slotI);
                }
            }
        }

        private void AddEmptySlot(int slotI)
        {
            //spawn
            var slotGo = Instantiate(EnemySlotPrefab, SlotsContainer);
            var slotScript = slotGo.GetComponent<EnemyInfo>();

            //add to list
            SlotsGo[slotI] = slotGo;
            SlotsScripts[slotI] = slotScript;
        }

        /// <summary>
        /// sorts slots game objects to be in order:
        /// mySlotN+1 is first slot on top of the screen.
        /// mySlotN-1 is last slot on top of the screen.
        /// </summary>
        private void SortSlotDisplays()
        {
            if (SlotsGo.Length == 0) return;

            int mySlotN = StaticRoomData.MySlotNumber;

            int leftSlotN = (mySlotN + 1) % SlotsGo.Length;

            while (leftSlotN != mySlotN)
            {
                SlotsGo[leftSlotN].transform.SetAsLastSibling();

                leftSlotN = (leftSlotN + 1) % SlotsGo.Length;
            } 
        }

        /// <summary>
        /// Replace slot with prefab of Occupied slot
        /// </summary>
        private void SetSlotAsOccupied(int slotN)
        {
            SlotsScripts[slotN].DrawPlayer(StaticRoomData.Players[slotN]);
        }

        /// <summary>
        /// Hides redy checkmarks
        /// </summary>
        public override void OnStartGame()
        {
            //Hide checkmarks
            for (int i = 0; i < SlotsScripts.Length; i++)
            {
                if (i != StaticRoomData.MySlotNumber)
                {
                    SlotsScripts[i].SetReadyCheckmark(false);
                }
            }
        }

        /// <summary>
        /// Observer method: On Other Player Joined Room
        /// </summary>
        public override void OnOtherPlayerJoinedRoom(long joinedPlayerId, int slotN, string joinedPlayerNickname)
        {
            SetSlotAsOccupied(slotN);
        }

        /// <summary>
        /// Observer method: On Other Player leaves Room
        /// </summary>
        public override void OnOtherPlayerLeftRoom(long leftPlayerId, int slotN)
        {
            // if game is playing then mark slot as player-left
            if (StaticRoomData.IsPlaying)
            {
                SlotsScripts[slotN].DrawLeft();
            }
            else // if game is not playing then mark slot as empty
            {
                SlotsScripts[slotN].DrawEmpty();

                //Uncheck everybody's checkmarks
                foreach (var slot in SlotsScripts)
                {
                    slot.SetReadyCheckmark(false);
                }

                HideTextClouds();
                HideStatusIcons();
            }

        }

        /// <summary>
        /// Observer method: On Other Player click ready
        /// </summary>
        public override void OnOtherPlayerGotReady(long playerId, int slotN)
        {
            //Display enemy's checkmark
            SlotsScripts[slotN].SetReadyCheckmark(true);
        }

        /// <summary>
        /// Observer method: On Other Player click not ready
        /// </summary>
        public override void OnOtherPlayerGotNotReady(long playerId, int slotN)
        {
            //Display enemy's checkmark
            SlotsScripts[slotN].SetReadyCheckmark(false);
        }

        public override void OnEnemyGotCardsFromTalon(long playerId, int slotN, int cardsN)
        {
            print("OnEnemyGotCardsFromTalon " + playerId);
            (SlotsScripts[slotN] as EnemyInfo).TakeCardsFromTalon(cardsN);
        }

        public override void OnMePassed()
        {
            OnOtherPlayerPassed(StaticRoomData.MyPlayer.ConnectionId, StaticRoomData.MySlotNumber);
        }

        public override void OnOtherPlayerPassed(long passedPlayerId, int slotN)
        {
            //Set text clouds
            if (StaticRoomData.WhoseDefend == passedPlayerId)
            {
                SlotsScripts[slotN].ShowTextCloud("Беру");
                SlotsScripts[slotN].SetStatusIconNoAnimation(PlayerInfo.PlayerStatusIcon.DefenderGaveUp);
            }
            else if (GameManager.Instance.AllCardsCovered())
            {
                // if player has no more cards left then dont show text cloud
                if (StaticRoomData.Players[slotN].CardsNumber > 0)
                {
                    SlotsScripts[slotN].ShowTextCloud("Бито");
                }
            }
            else
            {
                // if player has no more cards left then dont show text cloud
                if (StaticRoomData.Players[slotN].CardsNumber > 0)
                {
                    SlotsScripts[slotN].ShowTextCloud("Пас");
                }
            }

            //Set status icons
            if (StaticRoomData.WhoseAttack == passedPlayerId)
            {
                //Animate icons
                var defenderSlot = SlotsScripts[StaticRoomData.Denfender.SlotN];

                foreach (var slot in SlotsScripts)
                {
                    if (slot != defenderSlot)
                    {
                        slot.SetStatusIconNoAnimation(PlayerInfo.PlayerStatusIcon.Attacker);
                    }
                }
            }
        }




        /// <summary>
        /// observer event
        /// </summary>
        public override void OnNextTurn(long whoseTurnPlayerId, int slotN, long defendingPlayerId, int defSlotN, int turnN)
        {
            HideTextClouds();
            HideStatusIcons();

            //Animate icons
            PlayerInfo attacker = SlotsScripts[slotN];
            PlayerInfo defender = SlotsScripts[defSlotN];

            if (GameManager.Instance.TurnN == 0)
            {
                MessageManager.Instance.DelayNextAnimation(DelayBeforeNextTurn * 2);
            }
            else
            {
                MessageManager.Instance.DelayNextAnimation(DelayBeforeNextTurn);
            }
            attacker.AnimateSpawnStatusIcon(PlayerInfo.PlayerStatusIcon.Attacker);
            defender.AnimateSpawnStatusIcon(PlayerInfo.PlayerStatusIcon.Defender);
            

            //MessageManager.Instance.AnimateAttackerAndDefender(attacker, defender);
        }

        public override void OnEndGame(long foolConnectionId, Dictionary<long, double> rewards)
        {
            //todo animate rewards, wait, mark all slots of players who left as empty

            // mark all slots of players who left as empty
            for (int i = 0; i < SlotsScripts.Length; i++)
            {
                if (StaticRoomData.Players[i].Left)
                {
                    SlotsScripts[i].DrawEmpty();
                }
            }

            HideTextClouds();
            HideStatusIcons();
        }

        private void HideStatusIcons()
        {
            //hide old icons if were
            foreach (var slot in SlotsScripts)
            {
                slot.AnimateHideCurrentStatusIcon();
            }
        }

        public void HideTextClouds()
        {
            foreach (var slot in SlotsScripts)
            {
                 slot.HideTextCloud();
            }
        }

        public void HideTextCloudsNoDefender()
        {
            var defenderSlot = SlotsScripts[StaticRoomData.Denfender.SlotN];
            foreach (var slot in SlotsScripts)
            {
                if (slot != defenderSlot)
                    slot.HideTextCloud();
            }
        }

        public void PickCardsFromTable(int slotN, List<CardRoot> cardsOnTable, List<CardRoot> cardsOnTableCovering)
        {
            HideTextCloudsNoDefender();

            SlotsScripts[slotN].PickCardsFromTable(cardsOnTable, cardsOnTableCovering);
        }
    }
}
