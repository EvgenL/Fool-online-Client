using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.CardsScripts;
using Fool_online.Scripts.Network.NetworksObserver;
using UnityEngine;

namespace Fool_online.Scripts.InRoom
{

    /// <summary>
    /// Calss handling top part of the screen with enemy names and cards
    /// </summary>
    public class PlayerInfosManager : MonoBehaviourFoolNetworkObserver
    {
        public GameObject EnemySlotPrefab;
        public Transform SlotsContainer;

        public GameObject[] SlotsGo;
        public EnemyInfo[] SlotsScripts;

        /// <summary>
        /// Draw everything when local player enters room
        /// </summary>
        public void InitRoomData()
        {
            //Clear slots is there were
            Util.DestroyAllChildren(SlotsContainer);

            print("EnemiesDisplay: InitRoomData");

            //There's gonna be amount of slots available
            SlotsScripts = new EnemyInfo[StaticRoomData.MaxPlayers];
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
            foreach (var slot in SlotsScripts)
            {
                if (slot != null)
                {
                    slot.SetReadyCheckmark(false);
                }
            }
        }

        /// <summary>
        /// Observer method: On Other Player Joined Room
        /// </summary>
        public override void OnOtherPlayerJoinedRoom(long joinedPlayerId, int slotN)
        {
            SetSlotAsOccupied(slotN);
        }

        /// <summary>
        /// Observer method: On Other Player leaves Room
        /// </summary>
        public override void OnOtherPlayerLeftRoom(long leftPlayerId, int slotN)
        {
            SlotsScripts[slotN].DrawEmpty();

            //Uncheck everybody's checkmarks
            foreach (var slot in SlotsScripts)
            {
                if (slot != null)
                    slot.SetReadyCheckmark(false);
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
            SlotsScripts[slotN].TakeCardsFromTalon(cardsN);
        }

        public override void OnOtherPlayerPassed(long passedPlayerId, int slotN)
        {
            if (StaticRoomData.WhoseDefend == passedPlayerId)
            {
                SlotsScripts[slotN].ShowTextCloud("Беру");
            }
            else if (StaticRoomData.WhoseAttack == passedPlayerId)
            {
                SlotsScripts[slotN].ShowTextCloud("Бито");
            }
            else
            {
                SlotsScripts[slotN].ShowTextCloud("Бито");
            }
        }



        /// <summary>
        /// observer event
        /// </summary>
        public override void OnNextTurn(long whoseTurn, int slotN, long defendingPlayerId, int defSlotN, int turnN)
        {
            HideTextClouds();
        }

        public void HideTextClouds()
        {
            foreach (var slot in SlotsScripts)
            {
                if (slot != null)
                {
                    slot.HideTextCloud();
                }
            }
        }

        public void PickCardsFromTable(int slotN, List<CardRoot> cardsOnTable, List<CardRoot> cardsOnTableCovering)
        {
            HideTextClouds();

            SlotsScripts[slotN].PickCardsFromTable(cardsOnTable, cardsOnTableCovering);
        }
    }
}
