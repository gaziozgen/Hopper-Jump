using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public static class HapticManager
    {
        private static float delay = 0.5f;
        private static float latestHapticTime = -0.5f;

        public static void DoHaptic()
        {
            if (Time.time > latestHapticTime + delay)
            {
                //MMVibrationManager.Haptic(HapticTypes.LightImpact);
                latestHapticTime = Time.time;
            }
        }

    }
}