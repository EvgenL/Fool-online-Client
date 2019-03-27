using System.Collections;
using System.Collections.Generic;
using Assets.Fool_online.Scripts.Gameplay;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displayes rounded value of slider
/// </summary>
public class BetSlider : MonoBehaviour
{
    public float FloorValue;

    private Slider _slider;

    [SerializeField] private Text Text;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        FloorValue = BetConverter.FloorToAllowedBet(_slider.value);
        Text.text = FloorValue.ToString();
    }

    /// <summary>
    /// Called by slider
    /// </summary>
    public void OnValueChanged(float value)
    {
        FloorValue = BetConverter.MapToAllowedBet(value);
        Text.text = BetConverter.ToString(FloorValue);
    }
}
