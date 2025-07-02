using System;
using System.Linq;
using HarmonyLib;
using Exiled.API.Features;
using CrazyHintFramework.API.Managers;

namespace CrazyHintFramework.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.ShowHint), new[] { typeof(string), typeof(float) })]
    public class HintNetworkPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Player __instance, ref string message, ref float duration)
        {
            try
            {
                // 1. Verify that HintManager exists
                if (HintManager.Instance == null)
                {
                    Log.Error("HintManager.Instance is null!");
                    return true;//Continue with the original function

                }

                // 2. Fetch the active Hints and check their presence
                var activeHints = HintManager.Instance.GetActiveHints(__instance);
                if (activeHints == null || !activeHints.Any())
                {
                    Log.Debug("There are no active player messages.");
                    return true;
                }

                // 3. تطبيق الرسالة ذات الأولوية الأعلى
                var topHint = activeHints.OrderByDescending(h => h.Priority).First();
                message = topHint.Text;
                duration = (float)topHint.Duration;

                Log.Debug($"The message has been modified to: {message} (Duration: {duration} second)");
            }
            catch (Exception ex)
            {
                Log.Error($"Error in HintPatch: {ex}");
            }

            return true; //Continue the original function with the modified values

        }
    }
}