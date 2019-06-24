using DG.Tweening;
using DOTween.Modules;
using Fool_online.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.InRoom.CardsScripts
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

        [Header("Animation: hover")]
        /*[SerializeField]*/ public const float CardMoveUpOnHover = 70f;
        [SerializeField] private Ease _hoverAnimationEase = Ease.InSine;
        [SerializeField] private const float _animationDuration = 0.15f;


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
        private CardRoot _cardRoot;

        public float LerpSpeed = 15f;
        private const float SNAP_DISTANCE = 2f;
        private const float SNAP_ANGLE = 0.5f;

        public bool CanBeDragged = true;
        private static bool _mouseBusy = false;
        public bool IsDragged { get; private set; }
        private bool _zoom = false;
        private Image _cardImage;
        private Outline _outline;

        private void Awake()
        {
            _cardRoot = transform.parent.GetComponent<CardRoot>();
            _cardImage = GetComponent<Image>();
            _outline = GetComponent<Outline>();

            targetPos = transform.position;
            targetRot = transform.rotation;
            targetScale = transform.localScale;
        }

        /// <summary>
        /// Called from CardHoverInputzone
        /// </summary>
        public void BeginDrag()
        {
            if (!_mouseBusy && CanBeDragged)
            {
                IsDragged = true;
                _mouseBusy = true;
                AnimationState = CardAnimationState.Dragged;
                ShowAboveUi();
            }
        }

        /// <summary>
        /// Called from CardHoverInputzone
        /// </summary>
        public void UpdateDrag()
        {
            InputManager.Instance.DraggedCardUpdate(Input.mousePosition, _cardRoot);
        }

        /// <summary>
        /// Called from CardHoverInputzone
        /// </summary>
        public void EndDrag()
        {
            if (CanBeDragged)
            {
                _mouseBusy = false;
                IsDragged = false;
                AnimationState = CardAnimationState.MovingToRoot;
                ShowInUi();
                InputManager.Instance.DraggedCardDrop(Input.mousePosition, _cardRoot);
            }
        }


        private void Update()
        {
            // If any card got grabbed by mouse then every other card gets un-zoomed
            // (the _mouseBusy is static)
            if (_mouseBusy)
            {
                _zoom = false;
            }

            if (AnimationState == CardAnimationState.MovingToRoot)
            {
                targetPos = _cardRoot.transform.position;
                targetRot = _cardRoot.transform.rotation;
                targetScale = _cardRoot.transform.localScale;
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
                //transform.position = targetPos;
                transform.position = Vector3.Lerp(transform.position, targetPos, LerpSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime);
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, LerpSpeed * Time.deltaTime);

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
            transform.SetParent(_cardRoot.transform, true); //Sets parent of this object to Canvas to show above up everything
            transform.SetAsFirstSibling();
        }

        public void AnimateMoveToRootFrom(Vector3 startPosition)
        {
            AnimateMoveToRootFrom(startPosition, Quaternion.identity, Vector3.one);
        }

        public void AnimateMoveToRootFrom(Vector3 startPosition, Quaternion startRotation)
        {
            AnimateMoveToRootFrom(startPosition, startRotation, Vector3.one);
        }

        public void AnimateMoveToRootFrom(Vector3 startPosition, Quaternion startRotation, Vector3 startScale)
        {
            //Set start
            transform.position = startPosition;
            transform.rotation = startRotation;
            transform.localScale = startScale;

            //Set target
            targetPos = _cardRoot.transform.position;
            targetRot = _cardRoot.transform.rotation;
            targetScale = _cardRoot.transform.localScale;

            AnimationState = CardAnimationState.MovingToRoot;
        }

        public void AnimateToMyHand(Vector3 startPosition, Quaternion startRotation, Vector3 startScale)
        {
            //Set start
            transform.position = startPosition;
            transform.rotation = startRotation;
            transform.localScale = startScale;

            //Set target
            targetPos = _cardRoot.transform.position;
            targetRot = _cardRoot.transform.rotation;
            targetScale = _cardRoot.transform.localScale;

            AnimationState = CardAnimationState.MovingToRoot;
        }

        public void Zoom()
        {
            if (!_mouseBusy && !_zoom)
            {
                Vector3 targetPos = transform.parent.position + transform.parent.up * CardMoveUpOnHover;
                transform.DOMove(targetPos, _animationDuration).SetEase(_hoverAnimationEase);
             //   transform.parent.position + transform.parent.up * CardMoveUpOnHover;
                _zoom = true;
            }

        }
        public void UnZoom()
        {
            if (!_mouseBusy && _zoom)
            {
                Vector3 targetPos = transform.parent.position;
                transform.DOMove(targetPos, _animationDuration).SetEase(_hoverAnimationEase);
                _zoom = false;
            }
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
