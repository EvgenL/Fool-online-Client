using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fool_online.Scripts.InRoom;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DOTween = DG.Tweening.DOTween;

/// <summary>
/// Shows text message in game
/// Shows player status icon animations (who if fool and who won on this turn for example)
/// </summary>
public class MessageManager : MonoBehaviour
{

    public static MessageManager Instance;


    [Header("References to text")]
    [SerializeField] private TextMeshProUGUI _textMeshText;
    [SerializeField] private GameObject _textContainer;


    [Header("Center of the screen where status icons will be spawned")]
    [SerializeField] private RectTransform _playerStatusIconSpawner;

    [Header("Prefab of status icon")]
    [SerializeField] private GameObject _statusIconPrefab;
    [Header("Prefab of money icon and text")]
    [SerializeField] private GameObject _moneyPrefab;

    [Header("Sprites of status icons")]
    [SerializeField] private Sprite _wonSprite;
    [SerializeField] private Sprite _foolSprite;
    [SerializeField] private Sprite _attackerSprite;
    [SerializeField] private Sprite _defenderSprite;
    [SerializeField] private Sprite _defenderGaveUpSprite;

    [Header("Animation ease")]
    [SerializeField] private Ease _ease;

    private Queue<Sequence> _animationQueue = new Queue<Sequence>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Shows big text at full screen
    /// </summary>
    public void ShowFullScreenText(string message)
    {
        _textMeshText.text = message;
        _textContainer.SetActive(true);
        CancelInvoke(nameof(Hide));
        Invoke(nameof(Hide), 3f);
    }

    private void Hide()
    {
        _textContainer.SetActive(false);
    }


    public void PlayerGotReward(Transform playerRewardContainer, double amount)
    {
        var moneyTransform = SpawnIconAtScreenCentre(PlayerInfo.PlayerStatusIcon.Money);

        AnimateMoveIconToTransform(playerRewardContainer, moneyTransform);
    }

    /// <summary>
    /// Animates sword and shield icons to apperar 
    /// in centre of the screen and fly to players
    /// </summary>
    public void AnimateAttackerAndDefender(Transform attacketTarget, Transform defenderTarget)
    {
        //////////////////////Spawn icon//////////////////////

        //Spawn attacker and defender icons
        var iconTransforms = SpawnMultipleIconsAtCentre(
            PlayerInfo.PlayerStatusIcon.Attacker, PlayerInfo.PlayerStatusIcon.Defender);

        //////////////////////Aniamte//////////////////////
        
        var seq1 = AnimateMoveIconToTransform(attacketTarget, iconTransforms[0]);
        var seq2 = AnimateMoveIconToTransform(defenderTarget, iconTransforms[1]);

        seq1.Pause();
        seq2.Pause();

        //enqueue them as one
        Sequence combinedSequence = DOTween.Sequence();

        combinedSequence.Append(seq1);
        combinedSequence.Join(seq2);

        EnqueueNewSequence(combinedSequence);
    }

    /// <summary>
    /// Animates icon apperar in center of the screen
    /// then this icon translates to target Transform
    /// </summary>
    /// <param name="target">to where icon is going</param>
    /// <param name="statusIcon">what icon to spawn</param>
    public void AnimatePlayerStatusIcon(Transform target, PlayerInfo.PlayerStatusIcon statusIcon)
    {
        //////////////////////Spawn icon//////////////////////
        var iconTransform = SpawnIconAtScreenCentre(statusIcon);

        //////////////////////Aniamte and enqueue//////////////////////
        var seq = AnimateMoveIconToTransform(target, iconTransform);
        EnqueueNewSequence(seq);
    }
    
    /// <summary>
    /// Spawns player status icon at screen centre
    /// it will be a child of a PlayerStatusIconSpawner
    /// </summary>
    public Transform SpawnIconAtScreenCentre(PlayerInfo.PlayerStatusIcon statusIcon)
    {
        if (statusIcon == PlayerInfo.PlayerStatusIcon.Money)
        {
            return Instantiate(_moneyPrefab, _playerStatusIconSpawner).transform;
        }

        //Spawn icon to centre of the PlayerStatusIconSpawner
        GameObject iconGo = Instantiate(_statusIconPrefab, _playerStatusIconSpawner);

        iconGo.transform.position = _playerStatusIconSpawner.position;

        //Get image
        var iconImage = iconGo.GetComponent<Image>();

        //Choose what image to draw
        switch (statusIcon)
        {
            case PlayerInfo.PlayerStatusIcon.Fool:
                iconImage.sprite = _foolSprite;
                break;
            case PlayerInfo.PlayerStatusIcon.Attacker:
                iconImage.sprite = _attackerSprite;
                break;
            case PlayerInfo.PlayerStatusIcon.Defender:
                iconImage.sprite = _defenderSprite;
                break;
            case PlayerInfo.PlayerStatusIcon.DefenderGaveUp:
                iconImage.sprite = _defenderGaveUpSprite;
                break;
        }


        return iconGo.transform;
    }

    /// <summary>
    /// Moves status icon to players container transrorm
    /// </summary>
    /// <param name="target">players container transrorm</param>
    /// <param name="iconGo">game object of this icon</param>
    /// <param name="queue">should be queued?</param>
    private Sequence AnimateMoveIconToTransform(Transform target, Transform iconTransform)
    {
        iconTransform.SetParent(target, true);

        Image iconImage = iconTransform.GetComponent<Image>();

        //Set color to transparent (animation start)
        iconImage.color = new Color(1, 1, 1, 0);

        float fadeInDuration = 0.4f;
        float fadeInMoveDistance = 70f;
        float delayAfterFadeIn = 0.7f;
        float moveDuration = 0.5f;


        var sequence = DG.Tweening.DOTween.Sequence();

        //append this icon to sequence
        sequence.Append(iconTransform.DOMoveY(transform.position.y + fadeInMoveDistance, fadeInDuration));
        sequence.Join(iconImage.DOFade(1, fadeInDuration));

        var scaleUp = iconTransform.DOScale(Vector3.one * 1.3f, fadeInDuration);
        sequence.Join(scaleUp);


        sequence.AppendInterval(delayAfterFadeIn);
        sequence.Append(iconTransform.DOMove(target.position, moveDuration));
        sequence.Join(iconTransform.DOScale(Vector3.one, moveDuration));

        //Experimentally found quint to be prettiest
        sequence.SetEase(_ease);

        return sequence;
    }


    /// <summary>
    /// Spawns multiple icons with position managed by grid layout
    /// </summary>
    private Transform[] SpawnMultipleIconsAtCentre(params PlayerInfo.PlayerStatusIcon[] statusIcons)
    {
        Transform[] result = new Transform[statusIcons.Length];

        for (int i = 0; i < statusIcons.Length; i++)
        {
            result[i] = SpawnIconAtScreenCentre(statusIcons[i]);
        }

        //Rebuild layout at this frame
        LayoutRebuilder.ForceRebuildLayoutImmediate(_playerStatusIconSpawner);

        return result;
    }

    public void DelayNextAnimation(float seconds)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(seconds);
        EnqueueNewSequence(seq);
    }

    /// <summary>
    /// and adds sequence to queue
    /// </summary>
    private void EnqueueNewSequence(Sequence sequence)
    {
        //pause sequence
        sequence.Pause();

        //append sequence to queue
        _animationQueue.Enqueue(sequence);

        //Play first sequence in queue in case wasn't playng
        _animationQueue.Peek().Play();

        //add callback
        sequence.OnComplete(OnSequenceComplete);
    }

    //Callback
    private void OnSequenceComplete()
    {
        //remove animation that was completed
        _animationQueue.Dequeue();

        //if there's animations in queue left
        if (_animationQueue.Count > 0)
        {
            //play next
            _animationQueue.Peek().Play();
        }
    }

}
