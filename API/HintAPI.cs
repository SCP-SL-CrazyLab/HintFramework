
using System;
using System.Collections.Generic;
using Exiled.API.Features;
using CrazyHintFramework.API.Managers;
using CrazyHintFramework.API.Models;

namespace CrazyHintFramework.API
{
    /// <summary>
    /// الواجهة العامة لإطار عمل الـ Hints
    /// </summary>
    public static class HintAPI
    {
        /// <summary>
        /// عرض Hint للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <param name="text">النص</param>
        /// <param name="duration">المدة بالثواني</param>
        /// <param name="priority">الأولوية (افتراضي: 0)</param>
        /// <param name="sourcePlugin">اسم البلغن المصدر (افتراضي: "Unknown")</param>
        /// <returns>معرف الـ Hint إذا تم إنشاؤه بنجاح، null إذا فشل</returns>
        public static string ShowHint(Player player, string text, float duration, int priority = 0, string sourcePlugin = "Unknown")
        {
            if (player == null || string.IsNullOrEmpty(text) || duration <= 0)
                return null;

            var hint = new HintData(text, duration, priority, sourcePlugin);
            bool success = HintManager.Instance.AddHint(player, hint);
            return success ? hint.Id : null;
        }

        /// <summary>
        /// عرض Hint لجميع اللاعبين
        /// </summary>
        /// <param name="text">النص</param>
        /// <param name="duration">المدة بالثواني</param>
        /// <param name="priority">الأولوية (افتراضي: 0)</param>
        /// <param name="sourcePlugin">اسم البلغن المصدر (افتراضي: "Unknown")</param>
        /// <returns>قائمة بمعرفات الـ Hints التي تم إنشاؤها</returns>
        public static List<string> ShowHintToAll(string text, float duration, int priority = 0, string sourcePlugin = "Unknown")
        {
            var hintIds = new List<string>();

            foreach (var player in Player.List)
            {
                var hintId = ShowHint(player, text, duration, priority, sourcePlugin);
                if (hintId != null)
                {
                    hintIds.Add(hintId);
                }
            }

            return hintIds;
        }

        /// <summary>
        /// إخفاء Hint محدد للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <param name="hintId">معرف الـ Hint</param>
        /// <returns>true إذا تم إخفاء الـ Hint بنجاح</returns>
        public static bool HideHint(Player player, string hintId)
        {
            return HintManager.Instance.RemoveHint(player, hintId);
        }

        /// <summary>
        /// إخفاء جميع الـ Hints للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <returns>عدد الـ Hints التي تم إخفاؤها</returns>
        public static int HideAllHints(Player player)
        {
            return HintManager.Instance.ClearHints(player);
        }

        /// <summary>
        /// إخفاء جميع الـ Hints من بلغن محدد للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <param name="sourcePlugin">اسم البلغن المصدر</param>
        /// <returns>عدد الـ Hints التي تم إخفاؤها</returns>
        public static int HideHintsFromPlugin(Player player, string sourcePlugin)
        {
            return HintManager.Instance.ClearHintsFromPlugin(player, sourcePlugin);
        }

        /// <summary>
        /// الحصول على جميع الـ Hints النشطة للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <returns>قائمة بالـ Hints النشطة</returns>
        public static List<HintData> GetActiveHints(Player player)
        {
            return HintManager.Instance.GetActiveHints(player);
        }

        /// <summary>
        /// الحصول على Hint محدد للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <param name="hintId">معرف الـ Hint</param>
        /// <returns>الـ Hint إذا وُجد، null إذا لم يوجد</returns>
        public static HintData GetHint(Player player, string hintId)
        {
            return HintManager.Instance.GetHint(player, hintId);
        }

        /// <summary>
        /// التحقق مما إذا كان إطار عمل الـ Hints متاحًا
        /// </summary>
        /// <returns>true إذا كان متاحًا</returns>
        public static bool IsAvailable()
        {
            return HintManager.Instance != null;
        }

        /// <summary>
        /// الحصول على إصدار إطار عمل الـ Hints
        /// </summary>
        /// <returns>رقم الإصدار</returns>
        public static string GetVersion()
        {
            return "1.0.0"; // يمكن تحديث هذا ديناميكيًا من Assembly
        }
    }
}

