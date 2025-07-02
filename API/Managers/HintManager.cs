
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
    /// The main Hints manager
    /// </summary>
    public class HintManager : IHintManager
    {
        private static HintManager _instance;
        public static HintManager Instance => _instance ??= new HintManager();

        // A dictionary containing the Hints for each player
        private readonly ConcurrentDictionary<Player, List<HintData>> _playerHints;

        // events
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

            // Get the player's Hints list or create a new list
            var hints = _playerHints.GetOrAdd(player, _ => new List<HintData>());

            lock (hints)
            {
                // Verify that there is no Hint with the same ID
                if (hints.Any(h => h.Id == hint.Id))
                    return false;

                hints.Add(hint);
                hints.Sort((h1, h2) => h2.Priority.CompareTo(h1.Priority)); // Arrange by priority

            }

            // Trigger event
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

                // Validate the player
                if (player == null || !Player.List.Contains(player))
                {
                    playersToRemove.Add(player);
                    continue;
                }

                lock (hints)
                {
                    var expiredHints = new List<HintData>();

                    // Update the status of Hints and find expired ones
                    foreach (var hint in hints.ToList())
                    {
                        hint.UpdateStatus();
                        if (hint.IsExpired())
                        {
                            expiredHints.Add(hint);
                        }
                    }

                    // Remove expired Hints
                    foreach (var expiredHint in expiredHints)
                    {
                        hints.Remove(expiredHint);
                        HintExpired?.Invoke(player, expiredHint);
                    }
                }
            }

            //Remove invalid players
            foreach (var player in playersToRemove)
            {
                _playerHints.TryRemove(player, out _);
            }
        }

        /// <summary>
        /// Clean all data (used when reporting is stopped)
        /// </summary>
        public void Cleanup()
        {
            _playerHints.Clear();
        }
    }
}

