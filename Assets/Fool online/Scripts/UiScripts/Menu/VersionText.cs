using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.UiScripts.Menu
{
    /// <summary>
    /// Displays game version which set in player settings
    /// onto Text component
    /// </summary>
    public class VersionText : MonoBehaviour
    {
        void Start()
        {
            var text = GetComponent<Text>();
            text.text = Application.version;
        }
    }
}
