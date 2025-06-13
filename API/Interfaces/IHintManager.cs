
using System;
using System.Collections.Generic;
using Exiled.API.Features;
using CrazyHintFramework.API.Models;

namespace CrazyHintFramework.API.Interfaces
{
    /// <summary>
    /// واجهة إدارة الـ Hints
    /// </summary>
    public interface IHintManager
    {
        /// <summary>
        /// إضافة Hint جديد للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <param name="hint">بيانات الـ Hint</param>
        /// <returns>true إذا تم إضافة الـ Hint بنجاح</returns>
        bool AddHint(Player player, HintData hint);

        /// <summary>
        /// إزالة Hint محدد للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <param name="hintId">معرف الـ Hint</param>
        /// <returns>true إذا تم إزالة الـ Hint بنجاح</returns>
        bool RemoveHint(Player player, string hintId);

        /// <summary>
        /// إزالة جميع الـ Hints للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <returns>عدد الـ Hints التي تم إزالتها</returns>
        int ClearHints(Player player);

        /// <summary>
        /// إزالة جميع الـ Hints من بلغن محدد للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <param name="sourcePlugin">اسم البلغن المصدر</param>
        /// <returns>عدد الـ Hints التي تم إزالتها</returns>
        int ClearHintsFromPlugin(Player player, string sourcePlugin);

        /// <summary>
        /// الحصول على جميع الـ Hints النشطة للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <returns>قائمة بالـ Hints النشطة</returns>
        List<HintData> GetActiveHints(Player player);

        /// <summary>
        /// الحصول على Hint محدد للاعب
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <param name="hintId">معرف الـ Hint</param>
        /// <returns>الـ Hint إذا وُجد، null إذا لم يوجد</returns>
        HintData GetHint(Player player, string hintId);

        /// <summary>
        /// تحديث جميع الـ Hints وإزالة المنتهية الصلاحية
        /// </summary>
        void UpdateHints();

        /// <summary>
        /// حدث يتم تشغيله عند إضافة Hint جديد
        /// </summary>
        event Action<Player, HintData> HintAdded;

        /// <summary>
        /// حدث يتم تشغيله عند إزالة Hint
        /// </summary>
        event Action<Player, HintData> HintRemoved;

        /// <summary>
        /// حدث يتم تشغيله عند انتهاء صلاحية Hint
        /// </summary>
        event Action<Player, HintData> HintExpired;
    }
}

