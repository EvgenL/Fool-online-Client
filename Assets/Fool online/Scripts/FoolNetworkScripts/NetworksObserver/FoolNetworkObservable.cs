using System;
using System.Collections.Generic;
using Fool_online.Scripts.CardsScripts;
using UnityEngine;

namespace Fool_online.Scripts.Network.NetworksObserver
{

    /// <summary>
    /// Implemented by fool networking classes to call server events on observer objects
    /// Uses subscriber pattern with observers subscribing on their constructor and unsubscribing
    /// via unity callback OnDestroy()
    /// </summary>
    public class FoolNetworkObservable
    {
        //Handling of observers list
        #region Observers list

        
        /// Subscribed observers
        
        private static HashSet<MonoBehaviourFoolNetworkObserver> _observers = new HashSet<MonoBehaviourFoolNetworkObserver>();

        
        /// Called from observer constructor. Attaches it to _observers list.
        
        public static void Attach(MonoBehaviourFoolNetworkObserver observer)
        {
            _observers.Add(observer);
        }

        
        /// Called from observer constructor. Detaches it from _observers list.
        
        public static void Detach(MonoBehaviourFoolNetworkObserver observer)
        {
            _observers.Remove(observer);
        }

        #endregion

        //Methods that being sent to every observer
        #region Events

        protected void OnTryingConnectToGameServer()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnTryingConnectToGameServer();
                }
            });
        }
        
        protected void OnConnectedToGameServer()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnConnectedToGameServer();
                }
            });
        }

        protected void OnDisconnectedFromGameServer()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnDisconnectedFromGameServer();
                }
            });
        }

        protected void OnJoinRoom()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnJoinRoom();
                }
            });
        }

        protected void OnLeaveRoom()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnLeaveRoom();
                }
            });
        }

        protected void OnFailedToJoinFullRoom()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnFailedToJoinFullRoom();
                }
            });
        }
        
        protected void OnYouAreAlreadyInRoom()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnYouAreAlreadyInRoom();
                }
            });
        }
        
        protected void OnRoomData()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnRoomData();
                }
            });
        }
        
        protected void OnOtherPlayerJoinedRoom(long joinedPlayerId, int slotN)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnOtherPlayerJoinedRoom(joinedPlayerId, slotN);
                }
            });
        }

        protected void OnOtherPlayerLeftRoom(long leftPlayerId, int slotN)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnOtherPlayerLeftRoom(leftPlayerId, slotN);
                }
            });
        }

        protected void OnOtherPlayerGotReady(long playerId, int slotN)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnOtherPlayerGotReady(playerId, slotN);
                }
            });
        }

        protected void OnOtherPlayerGotNotReady(long playerId, int slotN)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnOtherPlayerGotNotReady(playerId, slotN);
                }
            });
        }

        protected void OnYouGotCardsFromTalon(string[] cards)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnYouGotCards(cards);
                }
            });
        }
        protected void OnEnemyGotCardsFromTalon(long playerId, int slotN, int cardsN)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnEnemyGotCardsFromTalon(playerId, slotN, cardsN);
                }
            });
        }
        protected void OnStartGame()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnStartGame();
                }
            });
        }
        protected void OnNextTurn(long whoseTurnPlayerId, int slotN, long defendingPlayerId, int defSlotN, int turnN)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnNextTurn(whoseTurnPlayerId, slotN, defendingPlayerId, defSlotN, turnN);
                }
            });
        }
        protected void OnTalonData(int talonSize, string trumpCardCode)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnTalonData(talonSize, trumpCardCode);
                }
            });
        }
        protected void OnDropCardOnTableOk(string cardCode)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnDropCardOnTableOk(cardCode);
                }
            });
        }
        protected void OnDropCardOnTableErrorNotYourTurn(string cardCode)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnDropCardOnTableErrorNotYourTurn(cardCode);
                }
            });
        }
        protected void OnDropCardOnTableErrorTableIsFull(string cardCode)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnDropCardOnTableErrorTableIsFull(cardCode);
                }
            });
        }
        protected void OnDropCardOnTableErrorCantDropThisCard(string cardCode)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnDropCardOnTableErrorCantDropThisCard(cardCode);
                }
            });
        }
        protected void OnOtherPlayerDropsCardOnTable(long playerId, int slotN, string cardCode)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnOtherPlayerDropsCardOnTable(playerId, slotN, cardCode);
                }
            });
        }
        protected void OnEndGame(long foolConnectionId, Dictionary<long, double> rewards)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnEndGame(foolConnectionId, rewards);
                }
            });
        }
        protected void OnEndGameGiveUp(long foolConnectionId, Dictionary<long, double> rewards)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnEndGameGiveUp(foolConnectionId, rewards);
                }
            });
        }
        protected void OnOtherPlayerPassed(long passedPlayerId, int slotN)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnOtherPlayerPassed(passedPlayerId, slotN);
                }
            });
        }
        protected void OnOtherPlayerCoversCard(long coveredPlayerId, int slotN,
            string cardOnTableCode, string cardDroppedCode)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnOtherPlayerCoversCard(coveredPlayerId, slotN, cardOnTableCode, cardDroppedCode);
                }
            });
        }
        protected void OnBeaten()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnBeaten();
                }
            });
        }
        protected void OnDefenderPicksCards(long pickedPlayerId, int slotN)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnDefenderPicksCards(pickedPlayerId, slotN);
                }
            });
        }
        protected void OnEndGameFool(long foolPlayerId)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnEndGameFool(foolPlayerId);
                }
            });
        }
        protected void OnMePassed()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnMePassed();
                }
            });
        }
        protected void OnDraggedCardUpdate(Vector2 mousePos, CardRoot draggedCardRoot, bool inTableZone)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnDraggedCardUpdate(mousePos, draggedCardRoot, inTableZone);
                }
            });
        }
        protected void OnCardDroppedOnTableByMe(CardRoot cardRoot)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnCardDroppedOnTableByMe(cardRoot);
                }
            });
        }
        protected void OnTableUpdated()
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnTableUpdated();
                }
            });
        }
        protected void OnPlayerWon(long wonPlayerId, double winnerReward)
        {
            //This is needed for access to unity's api
            DoInMainUnityThread(delegate {
                CheckDestroyedObservers();
                foreach (var obs in _observers)
                {
                    obs.OnPlayerWon(wonPlayerId, winnerReward);
                }
            });
        }



        #endregion

        //Methods that holp to work tith FoolNetworkObservable
        #region Util methods

        private void CheckDestroyedObservers()
        {
            HashSet<MonoBehaviourFoolNetworkObserver> newObservers = new HashSet<MonoBehaviourFoolNetworkObserver>();
            foreach (var obs in _observers)
            {
                if (obs != null)
                {
                    newObservers.Add(obs);
                }
            }

            _observers = newObservers;
        }

        //This is needed for access to unity's api
        private static void DoInMainUnityThread(Action action)
        {
            UnityMainThreadDispatcher.Enqueue(action);
        }

        #endregion

    }
}

