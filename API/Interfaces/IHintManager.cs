
using System;
using System.Collections.Generic;
using Exiled.API.Features;
using CrazyHintFramework.API.Models;

namespace CrazyHintFramework.API.Interfaces
{
  
    /// <summary>
    /// Hints management interface
    /// </summary>
    public interface IHintManager
    {
        /// <summary>
        /// Add a new Hint for the player
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="hint">Hint data</param>
        /// <returns>true if the Hint was added successfully</returns>
        bool AddHint(Player player, HintData hint);

        /// <summary>
        /// Remove a player specific Hint
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="hintId">Hint ID</param>
        /// <returns>true if the Hint was removed successfully</returns>
        bool RemoveHint(Player player, string hintId);

        /// <summary>
        /// Removes all player Hints
        /// </summary>
        /// <param name="player">Player</param>
        /// <returns>The number of Hints removed</returns>
        int ClearHints(Player player);

        /// <summary>
        /// Removes all Hints from a specific player's report
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="sourcePlugin">Name of the source plugin</param>
        /// <returns>The number of Hints removed</returns>
        int ClearHintsFromPlugin(Player player, string sourcePlugin);

        /// <summary>
        /// Get all the player's active Hints
        /// </summary>
        /// <param name="player">Player</param>
        /// <returns>List of active Hints</returns>
        List<HintData> GetActiveHints(Player player);

        /// <summary>
        /// Gets the player's specific Hint
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="hintId">Hint ID</param>
        /// <returns>Hint if found, null if not</returns>
        HintData GetHint(Player player, string hintId);

        /// <summary>
        /// Update all Hints and remove expired ones
        /// </summary>
        void UpdateHints();

        /// <summary>
        /// An event that is fired when a new Hint is added
        /// </summary>
        event Action<Player, HintData> HintAdded;

      
        /// <summary>
        /// An event that is fired when Hint is removed
        /// </summary>
        event Action<Player, HintData> HintRemoved;

        /// <summary>
        /// An event that is fired when Hint expires
        /// </summary>
        event Action<Player, HintData> HintExpired;
    }
}

