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
                // 1. التحقق من أن HintManager موجود
                if (HintManager.Instance == null)
                {
                    Log.Error("HintManager.Instance is null!");
                    return true; // الاستمرار بالدالة الأصلية
                }

                // 2. جلب الـ Hints النشطة وتحقق من وجودها
                var activeHints = HintManager.Instance.GetActiveHints(__instance);
                if (activeHints == null || !activeHints.Any())
                {
                    Log.Debug("لا توجد رسائل نشطة للاعب.");
                    return true;
                }

                // 3. تطبيق الرسالة ذات الأولوية الأعلى
                var topHint = activeHints.OrderByDescending(h => h.Priority).First();
                message = topHint.Text;
                duration = (float)topHint.Duration;

                Log.Debug($"تم تعديل الرسالة إلى: {message} (المدة: {duration} ثانية)");
            }
            catch (Exception ex)
            {
                Log.Error($"خطأ في HintPatch: {ex}");
            }

            return true; // الاستمرار بالدالة الأصلية مع القيم المعدلة
        }
    }
}