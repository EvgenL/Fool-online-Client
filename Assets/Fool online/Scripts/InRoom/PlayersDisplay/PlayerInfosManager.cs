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
        public PlayerInfo[] SlotsScripts2; //todo

        /// <summary>
        /// Draw everything when local player enters room
        /// </summary>
        public void InitRoomData()
        {
            //Clear slots is there were
            Util.DestroyAllChildren(SlotsContainer);

            print("EnemiesDisplay: InitRoomData");

            //There's gonna be amount of slots available
            SlotsScripts = new EnemyInfo[StaticRoomData.MaxPalyers];
            SlotsGo = new GameObject[StaticRoomData.MaxPalyers];

            StartCoroutine(WaitForRoomDataAndDrawSlots());

            foreach (var slot in SlotsScripts)
            {
                if (slot != null)
                {
                    slot.ClearHand();
                }
            }
        }

        /// <summary>
        /// Waits 'till server sends data about room then draws it.
        /// This needed for first connect where we call this at scene awake and
        /// possibly we dont have room data yet
        /// TODO 1) If server's not sending us data for some period of time thend ask it
        /// TODO 2) timeout discoinnect
        /// </summary>
        private IEnumerator WaitForRoomDataAndDrawSlots()
        {
            //Waiting room data from server
            while (StaticRoomData.DataReady == false)
            {
                yield return null;
                //TODO timeout
            }

            StaticRoomData.DataReady = false;

            //Instanciate empty slots
            for (int slotI = 0; slotI < StaticRoomData.MaxPalyers; slotI++)
            {
                //If this is data about us then skip
                if (slotI == StaticRoomData.MySlotNumber) continue;

                //spawn
                var slotGo = Instantiate(EnemySlotPrefab, SlotsContainer);
                var slotScript = slotGo.GetComponent<EnemyInfo>();

                //add to list
                SlotsGo[slotI] = slotGo;
                SlotsScripts[slotI] = slotScript;
            }

            //set occupied slots (if we aren't first to enter this room)
            for (int slotI = 0; slotI < StaticRoomData.MaxPalyers; slotI++)
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

        /// <summary>
        /// Replace slot with prefab of empty slot
        /// </summary>
        private void SetSlotAsEmpty(int slotN)
        {
            SlotsScripts[slotN].DrawEmpty();
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
            SetSlotAsEmpty(slotN);

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
            if (StaticRoomData.DefenderPassed())
            {
                SlotsScripts[slotN].ShowTextCloud("Пас");
            }
            else
            {
                SlotsScripts[slotN].ShowTextCloud("Бито");
            }
        }
        public override void OnOtherPlayerPickUpCards(long passedPlayerId, int slotN)
        {
            SlotsScripts[slotN].ShowTextCloud("Беру");
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
