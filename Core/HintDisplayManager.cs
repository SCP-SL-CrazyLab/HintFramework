
using System;
using System.Linq;
using System.Threading.Tasks;
using Exiled.API.Features;
using CrazyHintFramework.API.Managers;
using CrazyHintFramework.API.Models;

namespace CrazyHintFramework.Core
{
    /// <summary>
    /// مدير عرض الـ Hints
    /// </summary>
    public class HintDisplayManager
    {
        private static HintDisplayManager _instance;
        public static HintDisplayManager Instance => _instance ??= new HintDisplayManager();

        private bool _isRunning;
        private readonly object _lock = new object();

        private HintDisplayManager()
        {
        }

        /// <summary>
        /// بدء نظام عرض الـ Hints
        /// </summary>
        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning)
                    return;

                _isRunning = true;
                Task.Run(UpdateLoop);
                Log.Info("تم بدء نظام عرض الـ Hints");
            }
        }

        /// <summary>
        /// إيقاف نظام عرض الـ Hints
        /// </summary>
        public void Stop()
        {
            lock (_lock)
            {
                _isRunning = false;
                Log.Info("تم إيقاف نظام عرض الـ Hints");
            }
        }

        /// <summary>
        /// حلقة التحديث الرئيسية
        /// </summary>
        private async Task UpdateLoop()
        {
            while (_isRunning)
            {
                try
                {
                    // تحديث جميع الـ Hints
                    HintManager.Instance.UpdateHints();

                    // مع Harmony Patches، لا نحتاج إلى عرض الـ Hints يدويًا
                    // الـ Patches ستتولى ذلك تلقائيًا عند استدعاء دوال عرض الـ Hints الأصلية

                    // ولكن يمكننا تشغيل عرض دوري للتأكد من أن الـ Hints تظهر
                    TriggerHintDisplay();
                }
                catch (Exception ex)
                {
                    Log.Error($"خطأ في حلقة تحديث الـ Hints: {ex}");
                }

                // انتظار قبل التحديث التالي
                await Task.Delay(500); // تقليل التكرار لتحسين الأداء
            }
        }

        /// <summary>
        /// تشغيل عرض الـ Hints للاعبين الذين لديهم hints نشطة
        /// </summary>
        private void TriggerHintDisplay()
        {
            foreach (var player in Player.List)
            {
                try
                {
                    if (player == null || !player.IsAlive)
                        continue;

                    var activeHints = HintManager.Instance.GetActiveHints(player);
                    if (activeHints.Count > 0)
                    {
                        // تشغيل عرض hint فارغ لتفعيل الـ Patch
                        // الـ Patch سيستبدل هذا بالـ Hint الصحيح
                        player.ShowHint("", 1);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"خطأ في تشغيل عرض الـ Hint للاعب {player?.Nickname}: {ex}");
                }
            }
        }

        /// <summary>
        /// فرض عرض hint محدد للاعب (للاختبار)
        /// </summary>
        /// <param name="player">اللاعب</param>
        /// <param name="hint">الـ Hint</param>
        public void ForceDisplayHint(Player player, HintData hint)
        {
            if (player == null || hint == null)
                return;

            try
            {
                // استدعاء دالة عرض الـ Hint الأصلية
                // الـ Patch سيتداخل ويعرض الـ Hint من إطار العمل الخاص بنا
                player.ShowHint(hint.Text, (ushort)hint.Duration);
            }
            catch (Exception ex)
            {
                Log.Error($"خطأ في فرض عرض الـ Hint: {ex}");
            }
        }
    }
}

