
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using CrazyHintFramework.API.Interfaces;
using CrazyHintFramework.API.Models;

namespace CrazyHintFramework.API.Managers
{
    /// <summary>
    /// مدير الـ Hints الرئيسي
    /// </summary>
    public class HintManager : IHintManager
    {
        private static HintManager _instance;
        public static HintManager Instance => _instance ??= new HintManager();

        // قاموس يحتوي على الـ Hints لكل لاعب
        private readonly ConcurrentDictionary<Player, List<HintData>> _playerHints;

        // الأحداث
        public event Action<Player, HintData> HintAdded;
        public event Action<Player, HintData> HintRemoved;
        public event Action<Player, HintData> HintExpired;

        private HintManager()
        {
            _playerHints = new ConcurrentDictionary<Player, List<HintData>>();
        }

        public bool AddHint(Player player, HintData hint)
        {
            if (player == null || hint == null)
                return false;

            // الحصول على قائمة الـ Hints للاعب أو إنشاء قائمة جديدة
            var hints = _playerHints.GetOrAdd(player, _ => new List<HintData>());

            lock (hints)
            {
                // التحقق من عدم وجود Hint بنفس المعرف
                if (hints.Any(h => h.Id == hint.Id))
                    return false;

                hints.Add(hint);
                hints.Sort((h1, h2) => h2.Priority.CompareTo(h1.Priority)); // ترتيب حسب الأولوية
            }

            // تشغيل الحدث
            HintAdded?.Invoke(player, hint);
            return true;
        }

        public bool RemoveHint(Player player, string hintId)
        {
            if (player == null || string.IsNullOrEmpty(hintId))
                return false;

            if (!_playerHints.TryGetValue(player, out var hints))
                return false;

            HintData removedHint = null;
            lock (hints)
            {
                var hint = hints.FirstOrDefault(h => h.Id == hintId);
                if (hint != null)
                {
                    hints.Remove(hint);
                    removedHint = hint;
                }
            }

            if (removedHint != null)
            {
                HintRemoved?.Invoke(player, removedHint);
                return true;
            }

            return false;
        }

        public int ClearHints(Player player)
        {
            if (player == null)
                return 0;

            if (!_playerHints.TryGetValue(player, out var hints))
                return 0;

            int count;
            lock (hints)
            {
                count = hints.Count;
                foreach (var hint in hints.ToList())
                {
                    HintRemoved?.Invoke(player, hint);
                }
                hints.Clear();
            }

            return count;
        }

        public int ClearHintsFromPlugin(Player player, string sourcePlugin)
        {
            if (player == null || string.IsNullOrEmpty(sourcePlugin))
                return 0;

            if (!_playerHints.TryGetValue(player, out var hints))
                return 0;

            int count = 0;
            lock (hints)
            {
                var hintsToRemove = hints.Where(h => h.SourcePlugin == sourcePlugin).ToList();
                foreach (var hint in hintsToRemove)
                {
                    hints.Remove(hint);
                    HintRemoved?.Invoke(player, hint);
                    count++;
                }
            }

            return count;
        }

        public List<HintData> GetActiveHints(Player player)
        {
            if (player == null)
                return new List<HintData>();

            if (!_playerHints.TryGetValue(player, out var hints))
                return new List<HintData>();

            lock (hints)
            {
                return hints.Where(h => h.IsActive && !h.IsExpired()).ToList();
            }
        }

        public HintData GetHint(Player player, string hintId)
        {
            if (player == null || string.IsNullOrEmpty(hintId))
                return null;

            if (!_playerHints.TryGetValue(player, out var hints))
                return null;

            lock (hints)
            {
                return hints.FirstOrDefault(h => h.Id == hintId);
            }
        }

        public void UpdateHints()
        {
            var playersToRemove = new List<Player>();

            foreach (var kvp in _playerHints)
            {
                var player = kvp.Key;
                var hints = kvp.Value;

                // التحقق من صحة اللاعب
                if (player == null || !Player.List.Contains(player))
                {
                    playersToRemove.Add(player);
                    continue;
                }

                lock (hints)
                {
                    var expiredHints = new List<HintData>();

                    // تحديث حالة الـ Hints والعثور على المنتهية الصلاحية
                    foreach (var hint in hints.ToList())
                    {
                        hint.UpdateStatus();
                        if (hint.IsExpired())
                        {
                            expiredHints.Add(hint);
                        }
                    }

                    // إزالة الـ Hints المنتهية الصلاحية
                    foreach (var expiredHint in expiredHints)
                    {
                        hints.Remove(expiredHint);
                        HintExpired?.Invoke(player, expiredHint);
                    }
                }
            }

            // إزالة اللاعبين غير الصحيحين
            foreach (var player in playersToRemove)
            {
                _playerHints.TryRemove(player, out _);
            }
        }

        /// <summary>
        /// تنظيف جميع البيانات (يُستخدم عند إيقاف البلغن)
        /// </summary>
        public void Cleanup()
        {
            _playerHints.Clear();
        }
    }
}

