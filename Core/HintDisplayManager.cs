
using System;
using System.Linq;
using System.Threading.Tasks;
using Exiled.API.Features;
using CrazyHintFramework.API.Managers;
using CrazyHintFramework.API.Models;

namespace CrazyHintFramework.Core
{
    /// <summary>
    ///Hints display manager
    /// </summary>
    public class HintDisplayManager
    {
        private static HintDisplayManager _instance;
        public static HintDisplayManager Instance => _instance ??= new HintDisplayManager();

        private bool _isRunning;
        private readonly object _lock = new object();

        private HintDisplayManager()
        {
            HintDisplayManager.Instance.TriggerHintDisplay();
        }

        /// <summary>
        /// Start the Hints display system
        /// </summary>
        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning)
                    return;

                _isRunning = true;
                Task.Run(UpdateLoop);
                Log.Info("The Hints display system has been started");
            }
        }

        /// <summary>
        ///Stop the Hints display system
        /// </summary>
        public void Stop()
        {
            lock (_lock)
            {
                _isRunning = false;
                Log.Info("The Hints display system has been discontinued");
            }
        }

        /// <summary>
        /// Main update loop
        /// </summary>
        private async Task UpdateLoop()
        {
            while (_isRunning)
            {
                try
                {
                    // Update all Hints
                    HintManager.Instance.UpdateHints();

                    // With Harmony Patches, we don't need to display the Hints manually
                    // Patches will do this automatically when calling the original Hints display functions

                    //But we can run a periodic view to make sure the Hints are showing
                    TriggerHintDisplay();
                }
                catch (Exception ex)
                {
                    Log.Error($"خطأ في حلقة تحديث الـ Hints: {ex}");
                }

                // Wait before the next update
                await Task.Delay(500); // Reduce redundancy to improve performance

            }
        }

        /// <summary>
        /// Turn on Hints display for players with active Hints
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
                     
                        // Run an empty hint display to activate the patch
                        // The patch will replace this with the correct Hint
                        player.ShowHint("", 1);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error in playing the Hint display for the player {player?.Nickname}: {ex}");
                }
            }
        }

        /// <summary>
        /// Force a specific hint width to the player (for testing)
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="hint">The Hint</param>
        public void ForceDisplayHint(Player player, HintData hint)
        {
            if (player == null || hint == null)
                return;

            try
            {
                // Call the original Hint display function
                // The patch will overlap and display the Hint from our framework
                player.ShowHint(hint.Text, (ushort)hint.Duration);
            }
            catch (Exception ex)
            {
                Log.Error($"Error in forcing Hint display: {ex}");
            }
        }
    }
}

