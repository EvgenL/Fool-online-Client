using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.CardsScripts
{
    /// <summary>
    /// Respoisible fo card-mouse interactions
    /// </summary>
    public class InteractibleCard : MonoBehaviour
    {
        [Header("Animation: outline")]
        [SerializeField] private Color IdleOutlineColor;
        [SerializeField] private Color CanBeTargetedOutlineColor;
        [SerializeField] private Color TargetedOutlineColor;

        [Header("Animation: card")]
        [SerializeField] private Color IdleCardColor;
        [SerializeField] private Color CanBeTargetedCardColor;
        [SerializeField] private Color TargetedCardColor;


        public enum CardAnimationState
        {
            MovingToRoot,
            Dragged,
            Idle
        }


        public CardAnimationState AnimationState = CardAnimationState.Idle;

        private Vector3 targetPos;
        private Quaternion targetRot;
        private Vector3 targetScale;
        private CardRoot cardRoot;

        public float LerpSpeed = 15f;
        private const float SNAP_DISTANCE = 2f;
        private const float SNAP_ANGLE = 0.5f;
        public float CardMoveUpOnHover = 50f;

        public bool CanBeDragged = true;
        private static bool _mouseBusy = false;
        private bool _isDragged = false;
        private bool _zoom = false;
        private Image _cardImage;
        private Outline _outline;
        private static float _animationDuration = 0.25f;

        private void Awake()
        {
            cardRoot = transform.parent.GetComponent<CardRoot>();
            _cardImage = GetComponent<Image>();
            _outline = GetComponent<Outline>();

            targetPos = transform.position;
            targetRot = transform.rotation;
            targetScale = transform.localScale;
        }

        public void OnBeginDrag()
        {
            if (!_mouseBusy && CanBeDragged)
            {
                _isDragged = true;
                _mouseBusy = true;
                AnimationState = CardAnimationState.Dragged;
                ShowAboveUi();
            }
        }

        public void OnUpdateDrag()
        {
            InputManager.Instatnce.DraggedCardUpdate(Input.mousePosition, cardRoot);
        }

        public void OnEndDrag()
        {
            if (CanBeDragged)
            {
                _mouseBusy = false;
                _isDragged = false;
                AnimationState = CardAnimationState.MovingToRoot;
                UnZoom();
                ShowInUi();
                InputManager.Instatnce.DraggedCardDrop(Input.mousePosition, cardRoot);
            }
        }

        public void OnPointerEnter()
        {
            Zoom();
        }
        public void OnPointerExit()
        {
            UnZoom();
        }
        public void OnPointerDown()
        {
            Zoom();
        }
        public void OnPointerUp()
        {
            UnZoom();
        }

        private void Update()
        {
            if (_mouseBusy)
            {
                _zoom = false;
            }

            if (AnimationState == CardAnimationState.MovingToRoot)
            {
                targetPos = cardRoot.transform.position;
                targetRot = cardRoot.transform.rotation;
                targetScale = cardRoot.transform.localScale;
                /*
                //Lerp LOCAL transform values to root
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, LerpSpeed * Time.deltaTime);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, LerpSpeed / 2f * Time.deltaTime);
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, LerpSpeed * Time.deltaTime);
                */
                //Lerp world transform values to root
                transform.position = Vector3.Lerp(transform.position, targetPos, LerpSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, LerpSpeed / 2f * Time.deltaTime);
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, LerpSpeed * Time.deltaTime);

                //If close in distance
                if (CanSnapToRoot())
                {
                    //Snap
                    transform.position = targetPos;
                    transform.rotation = targetRot;
                    transform.localScale = targetScale;


                    AnimationState = CardAnimationState.Idle;
                }
            }
            else if (AnimationState == CardAnimationState.Dragged)
            {
                targetPos = (Input.mousePosition);
                targetRot = Quaternion.identity;
                targetScale = Vector3.one;
                
                //Lerp WORLD transform values to mouse
                transform.position = targetPos;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime);
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, LerpSpeed * Time.deltaTime);

            }
            else if (AnimationState == CardAnimationState.Idle)
            {
                if (CanBeDragged)
                {
                    //Zoom unzoom
                    if (_zoom)
                    {
                        targetPos = transform.parent.position + transform.parent.up * CardMoveUpOnHover;
                    }
                    else
                    {
                        targetPos = transform.parent.position;
                    }
                    transform.position = Vector3.Lerp(transform.position, targetPos, LerpSpeed * Time.deltaTime);
                }
            }

        }

        private bool CanSnapToRoot()
        {
            bool canSnapPosition = Vector3.Distance(transform.position, targetPos) < SNAP_DISTANCE;
            bool canSnapRotation = Quaternion.Angle(transform.rotation, targetRot) < SNAP_ANGLE;
            bool canSnapScale = Vector3.Distance(transform.localScale, targetScale) < 0.2f;

            return (canSnapScale && canSnapRotation && canSnapPosition);
        }

        private void ShowAboveUi()
        {
            transform.SetParent(transform.root, true); //Sets parent of this object to Canvas to show above up everything
            transform.SetAsLastSibling();
        }

        private void ShowInUi()
        {
            transform.SetParent(cardRoot.transform, true); //Sets parent of this object to Canvas to show above up everything
        }

        public void AnimateMoveFromToRoot(Vector3 startPosition)
        {
            AnimateMoveFromToRoot(startPosition, Quaternion.identity, Vector3.one);
        }

        public void AnimateMoveFromToRoot(Vector3 startPosition, Quaternion startRotation)
        {
            AnimateMoveFromToRoot(startPosition, startRotation, Vector3.one);
        }

        public void AnimateMoveFromToRoot(Vector3 startPosition, Quaternion startRotation, Vector3 startScale)
        {
            //Set start
            transform.position = startPosition;
            transform.rotation = startRotation;
            transform.localScale = startScale;

            //Set target
            targetPos = cardRoot.transform.position;
            targetRot = cardRoot.transform.rotation;
            targetScale = cardRoot.transform.localScale;

            AnimationState = CardAnimationState.MovingToRoot;
        }

        public void AnimateToMyHand(Vector3 startPosition, Quaternion startRotation, Vector3 startScale)
        {
            //Set start
            transform.position = startPosition;
            transform.rotation = startRotation;
            transform.localScale = startScale;

            //Set target
            targetPos = cardRoot.transform.position;
            targetRot = cardRoot.transform.rotation;
            targetScale = cardRoot.transform.localScale;

            AnimationState = CardAnimationState.MovingToRoot;
        }

        private void Zoom()
        {
            if (!_mouseBusy)
                _zoom = true;

        }
        private void UnZoom()
        {
            if (!_mouseBusy)
                _zoom = false;
        }

        public void AnimateIdle()
        {
            _cardImage.DOColor(IdleCardColor, _animationDuration);
            _outline.DOColor(IdleOutlineColor, _animationDuration);
        }

        public void AnimateCanBeTargeted()
        {
            _cardImage.DOColor(CanBeTargetedCardColor, _animationDuration);
            _outline.DOColor(CanBeTargetedOutlineColor, _animationDuration);
        }

        public void AnimateTargeted()
        {
            _cardImage.DOColor(TargetedCardColor, _animationDuration);
            _outline.DOColor(TargetedOutlineColor, _animationDuration);
        }
    }
}
