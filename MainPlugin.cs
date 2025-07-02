
using Exiled.API.Features;
using Exiled.API.Interfaces;
using HarmonyLib;
using System;
using CrazyHintFramework.API.Managers;
using CrazyHintFramework.Core;
using CrazyHintFramework.Diagnostics;
using CrazyHintFramework.Patches;

namespace CrazyHintFramework
{
    public class MainPlugin : Plugin<Config>
    {
        public override string Name => "CrazyHintFramework";
        public override string Author => "MONCEF50G";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(9, 6, 1); 

        public static MainPlugin Instance { get; private set; }
        private Harmony _harmony;
        private HintDisplayManager _hintDisplayManager;
        private SystemMonitor _systemMonitor;

        public override void OnEnabled()
        {
            Instance = this;

            // تهيئة Harmony
            _harmony = new Harmony("com.MONCEF50G.CrazyHintFramework");
            _harmony.PatchAll();

            // بدء مدير عرض الـ Hints
            _hintDisplayManager = HintDisplayManager.Instance;
            _hintDisplayManager.Start();

            // بدء مراقبة النظام إذا كانت مفعلة
            if (Config.Debug)
            {
                _systemMonitor = new SystemMonitor();
                _ = _systemMonitor.StartMonitoring(); // تشغيل المراقبة في خلفية
            }

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            // إيقاف مراقبة النظام
            _systemMonitor?.StopMonitoring();

            // إيقاف مدير عرض الـ Hints
            _hintDisplayManager?.Stop();

            // إلغاء Patch Harmony
            _harmony.UnpatchAll("com.MONCEF50G.CrazyHintFramework");

            // تنظيف بيانات الـ Hints
            HintManager.Instance.Cleanup();
            HintPatch.Cleanup();

            Instance = null;
            base.OnDisabled();
        }
    }
}

