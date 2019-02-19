using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.InRoom.CardsScripts;
using UnityEngine;

namespace Fool_online.Scripts.Manager
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instatnce;


        public RectTransform TableDropZone;

        private void Awake()
        {
            Instatnce = this;
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
    }
}
