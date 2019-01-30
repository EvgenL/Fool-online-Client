using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Class manages group of ui buttons where only one can be selected at a time.
/// </summary>
public class ButtonSwitchGroup : MonoBehaviour
{

    [SerializeField] private UnityEvent OnOptionSelected;
    [SerializeField] private Button Buttons;

    public void OnClick(int buttonN)
    {

    }
}
