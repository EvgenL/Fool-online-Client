using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Fool_online.Scripts.Gameplay
{
    /// <summary>
    /// Convert float value to closest allowed bet 
    /// Like 5678 = 5K
    /// </summary>
    public static class BetConverter
    {
        /*private static readonly float[] allowedBets = {
            100f,
            250f,
            500f,
            1000f,
            2500f,
            5000f,
            10000f,
            25000f,
            50000f,
            100000f,
            250000f,
            500000f,
            1000000f,
            2500000f,
            5000000f,
            10000000f,
        };*/

        private static readonly float[] allowedBets = {
            10f,
            25f,
            50f,
            100f,
            200f,
            300f,
            400f,
            500f,
            650f,
            800f,
            1000f,
            1500f,
            2000f,
            2500f,
            3000f,
            3500f,
            4000f,
            4500f,
            5000f,
            6000f,
            7000f,
            8000f,
            9000f,
            10000f
        };

        public static float MinBet => allowedBets[0];

        public static float MaxBet => allowedBets[allowedBets.Length - 1];

        /// <summary>
        /// Floor rough value like 5678f to 5000f
        /// </summary>
        public static float FloorToAllowedBet(float roughValue)
        {
            for (int i = allowedBets.Length - 1; i >= 1; i--)
            {
                if (allowedBets[i] == roughValue) return allowedBets[i];

                float diff = allowedBets[i] - roughValue;
                float betDiff = allowedBets[i] - allowedBets[i - 1];

                if (diff < betDiff) return allowedBets[i - 1];

            }

            return allowedBets[0];

        }

        /// <summary>
        /// Snap float [0.0f, 1.0f] to some value from allowedBet
        /// </summary>
        public static float MapToAllowedBet(float value)
        {
            float capValue = Mathf.Clamp(value, 0.0f, 1.0f);
            float step = 1f / (float)allowedBets.Length;
            float div = capValue / step;
            int index = Mathf.CeilToInt(div) - 1;
            if (index < 0) index = 0;
            return allowedBets[index];
        }

        /// <summary>
        /// Convert 1000.0f to "1K"
        /// </summary>
        public static string ToString(float value)
        {
            float floorValue = FloorToAllowedBet(value);

            // < 1K
            if (floorValue <= 500) return floorValue.ToString("N0");

            float div = 0;
            string letter;
            // > 1M
            if (floorValue >= 1000000)
            {
                div = floorValue / 1000000f;
                letter = "M";
            }
            else // > 1K
            {
                div = floorValue / 1000f;
                letter = "K";
            }

            if (div == 2.5f)
            {
                return div.ToString("N1") + letter;
            }
            else
            {
                return div.ToString("N0") + letter;
            }


        }
    }
}
