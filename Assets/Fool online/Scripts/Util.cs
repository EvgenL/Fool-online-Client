
using UnityEngine;

namespace Fool_online.Scripts
{
    /// <summary>
    /// Static class storing methods for expanding workflow with uinty objects
    /// </summary>
    public static class Util
    {
        public static void DestroyAllChildren(Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
