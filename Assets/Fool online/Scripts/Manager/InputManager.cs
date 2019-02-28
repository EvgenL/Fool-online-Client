using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.InRoom.CardsScripts;
using UnityEngine;

namespace Fool_online.Scripts.Manager
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;


        public RectTransform TableDropZone;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Called on each frame when i drag card
        /// </summary>
        public void DraggedCardUpdate(Vector2 mousePos, CardRoot cardRoot)
        {
            //if it was dragged insude table rect
            if (RectTransformUtility.RectangleContainsScreenPoint(TableDropZone, mousePos))
            {
                FoolObservable.OnDraggedCardUpdate(mousePos, cardRoot, true);
            }
            else
            {
                FoolObservable.OnDraggedCardUpdate(mousePos, cardRoot, false);
            }
        }

        public void DraggedCardDrop(Vector2 mousePos, CardRoot cardRoot)
        {
            //if it was dropped insude table rect
            if (RectTransformUtility.RectangleContainsScreenPoint(TableDropZone, mousePos))
            {
                FoolObservable.OnCardDroppedOnTableByMe(cardRoot);
            }
        }

        /// <summary>
        /// Called by 'pass' button
        /// </summary>
        public void OnMePass()
        {
            FoolObservable.OnMePassed();
        }

        /// <summary>
        /// Called by 'ready' button
        /// </summary>
        public void OnGetReadyClick(bool value)
        {
            FoolObservable.OnMeGotReady(value);
        }
    }
}
