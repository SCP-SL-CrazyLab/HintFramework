
using System;
using System.Collections.Generic;
using Exiled.API.Features;
using CrazyHintFramework.API.Managers;
using CrazyHintFramework.API.Models;

namespace CrazyHintFramework.API
{
    /// <Summary>
    /// The public interface of the Hints framework
    /// </summary>
    public static class HintAPI
    {
        /// <summary>
        /// Hint display to the player
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="text">Text</param>
        /// <param name="duration">Duration in seconds</param>
        /// <param name="priority">Priority (default: 0)</param>
        /// <param name="sourcePlugin">Source plugin name (default: "Unknown")</param>
        /// <returns>The identifier of the Hint if it was created successfully, null if it failed</returns>
        public static string ShowHint(Player player, string text, float duration, int priority = 0, string sourcePlugin = "Unknown")
        {
            if (player == null || string.IsNullOrEmpty(text) || duration <= 0)
                return null;

            var hint = new HintData(text, duration, priority, sourcePlugin);
            bool success = HintManager.Instance.AddHint(player, hint);
            return success ? hint.Id : null;
        }

        /// <summary>
        /// Hint display for all players
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="duration">Duration in seconds</param>
        /// <param name="priority">Priority (default: 0)</param>
        /// <param name="sourcePlugin">Source plugin name (default: "Unknown")</param>
        /// <returns>List of generated Hints</returns>
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
        ///Hide player specific Hint
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="hintId">Hint ID</param>
        /// <returns>true if the Hint was successfully hidden</returns>
        public static bool HideHint(Player player, string hintId)
        {
            return HintManager.Instance.RemoveHint(player, hintId);
        }

    
        /// <summary>
        /// Hide all Hints for the player
        /// </summary>
        /// <param name="player">Player</param>
        /// <returns>The number of hidden Hints</returns>
        public static int HideAllHints(Player player)
        {
            return HintManager.Instance.ClearHints(player);
        }

        /// <summary>
        /// Hide all Hints from a specific player's login
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="sourcePlugin">Name of the source plugin</param>
        /// <returns>The number of hidden Hints</returns>
        public static int HideHintsFromPlugin(Player player, string sourcePlugin)
        {
            return HintManager.Instance.ClearHintsFromPlugin(player, sourcePlugin);
        }

       
        /// <summary>
        /// Get all the player's active Hints
        /// </summary>
        /// <param name="player">Player</param>
        /// <returns>List of active Hints</returns>
        public static List<HintData> GetActiveHints(Player player)
        {
            return HintManager.Instance.GetActiveHints(player);
        }

        /// <summary>
        /// Gets the player's specific Hint
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="hintId">Hint ID</param>
        /// <returns>Hint if found, null if not</returns>
        public static HintData GetHint(Player player, string hintId)
        {
            return HintManager.Instance.GetHint(player, hintId);
        }

        /// <summary>
        /// Check if the Hints framework is available
        /// </summary>
        /// <returns>true if available</returns>
        public static bool IsAvailable()
        {
            return HintManager.Instance != null;
        }

        /// <summary>
        /// Get the Hints framework version
        /// </summary>
        /// <returns>Version number</returns>
        public static string GetVersion()
        {
            return "1.2.0"; // This can be updated dynamically from Assembly

        }
    }
}

