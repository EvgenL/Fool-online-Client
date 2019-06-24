using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DOTween.Modules;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.UI;
using DOTween = DG.Tweening.DOTween;

public class TimerRadial : MonoBehaviourFoolObserver
{
    [Header("Colors")]
    [SerializeField] private bool _setBaseColorOnAwake = true;
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _attackColor;
    [SerializeField] private Color _defendColor;
    [SerializeField] private float _fadeAnimLen = 0.3f;

    [Header("LocalScales multiplied")]
    [SerializeField] private bool _setBaseScaleOnAwake = true;
    [SerializeField] private float _baseScale = 1f;
    [SerializeField] private float _attackScale = 1.2f;
    [SerializeField] private float _scaleAnimLen = 0.3f;

    [Header("Punch animation")]
    [SerializeField] private float _punchAnimLen = 0.79f;
    [SerializeField] private float _punchScale = 0.83f;
    [SerializeField] private int _vibrato = 1;
    [SerializeField] private float _elastic = 0.1f;


    private Image _timerImage;

    private float _turnDuration = 10f;
    private float _timeLeft;

    private void Awake()
    {
        _timerImage = GetComponent<Image>();

        if (_setBaseColorOnAwake) _baseColor = _timerImage.color;
        if (_setBaseScaleOnAwake) _baseScale = _timerImage.transform.localScale.x;

        _timerImage.fillAmount = 1f;
        _timerImage.color = _baseColor;
        _timerImage.transform.localScale = Vector3.one * _baseScale;
    }

    

    private void AttackStart()
    {
        Punch();
        transform.DOScale(_attackScale, _scaleAnimLen).SetDelay(_punchAnimLen);
        _timerImage.DOColor(_attackColor, _fadeAnimLen);
        StartCoroutine("TurnTimer");
    }

    private void DefenceStart()
    {
        Punch();
        transform.DOScale(_attackScale, _scaleAnimLen).SetDelay(_punchAnimLen);
        _timerImage.DOColor(_defendColor, _fadeAnimLen);
        StartCoroutine("TurnTimer");
    }

    private void Punch()
    {
        transform.DOPunchScale(_punchScale * Vector3.one, _punchAnimLen, _vibrato, _elastic);
    }

    private void DefaultState()
    {
        StopAllCoroutines();
        DG.Tweening.DOTween.Kill(_timerImage);
        transform.DOScale(_baseScale, _scaleAnimLen);
        _timerImage.DOColor(_baseColor, _fadeAnimLen);
        _timerImage.DOFillAmount(1f, _fadeAnimLen);
    }

    private IEnumerator TurnTimer()
    {
        _timeLeft = _turnDuration;

        _timerImage.fillAmount = 0f;
        _timerImage.DOFillAmount(1f, _punchAnimLen);

        yield return new WaitForSeconds(_punchAnimLen + _scaleAnimLen);
        _timeLeft -= (_punchAnimLen + _scaleAnimLen);

        _timerImage.DOFillAmount(0f, _timeLeft).SetEase(Ease.Linear);

        // wait till half of time is gone and show animation
        yield return new WaitForSeconds(_timeLeft /2f);
        Punch();

        // wait till 1/4 of time is gone and show animation
        yield return new WaitForSeconds(_timeLeft / 4f);
        Punch();

        yield return new WaitForSeconds(_timeLeft / 4f);
        DefaultState();
    }
}
