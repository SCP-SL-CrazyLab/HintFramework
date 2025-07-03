# HintFramework :
### Revolutionize your SCP:SL server experience with advanced hint management. Built with performance, reliability, and developer experience in mind.
## Intelligent Priority Management:
### Advanced priority-based hint system that ensures important messages are always displayed first, with dynamic priority adjustment based on server conditions.
## Seamless Plugin Integration:
### Easy integration with existing plugins through a comprehensive API that requires minimal code changes while providing maximum functionality.
## Performance Optimized:
### Built with performance in mind, featuring object pooling, thread-safe operations, and memory optimization for high-load server environments.
## Developer Friendly:
### Comprehensive documentation, extensive examples, and powerful debugging tools make development and troubleshooting straightforward.

# Quick Example :
yml,,,
// Show a simple hint
string hintId = HintAPI.ShowHint(
    player: player,
    text: "Welcome to the server!",
    duration: 5.0f,
    priority: 5,
    sourcePlugin: "MyPlugin"
);

// Show hint to all players
var hintIds = HintAPI.ShowHintToAll(
    text: "Round started!",
    duration: 3.0f,
    priority: 10,
    sourcePlugin: "MyPlugin"
);

// Hide a specific hint
HintAPI.HideHint(player, hintId);

// Get active hints
var activeHints = HintAPI.GetActiveHints(player);
,,,

# API Reference :
## HintAPI Class :
### The main entry point for hint operations.
## ShowHint :
### string ShowHint(Player player, string text, float duration, int priority = 0, string sourcePlugin = "Unknown") /// Displays a hint to a specific player.
## Parameters:
### • player - Target player
### • text - Hint message content
### • duration - Display duration in seconds
### • priority - Priority level (0-10, higher = more important)
### • sourcePlugin - Identifying name of the source plugin
## Returns:
### Unique hint ID string

# ShowHintToAll :
### List<string> ShowHintToAll(string text, float duration, int priority = 0, string sourcePlugin = "Unknown")
Displays a hint to all connected players.
## Returns:  List of hint IDs for each player
# HideHint:
### bool HideHint(Player player, string hintId)
Hides a specific hint by its ID.
## Returns: 
### True if hint was found and hidden
# GetActiveHints:
### List<HintData> GetActiveHints(Player player)
Retrieves all active hints for a player.
## Returns:
List of active HintData objects

# HintData Class :
### Represents a hint instance with all its properties.

## Properties :
### string Id - Unique identifier
### string Text - Hint content
### float Duration - Display duration
### int Priority - Priority level
### string SourcePlugin - Source plugin name
### DateTime CreatedAt - Creation timestamp
### DateTime ExpiresAt - Expiration timestamp
### bool IsActive - Active status
## HintFramework:
# Your First Hint
### Let's create your first hint using CrazyHintFramework.
## Basic Hint Display :
using CrazyHintFramework.API;
-----------------------------------------------------------
// In your plugin's OnPlayerJoined event
public void OnPlayerJoined(JoinedEventArgs ev)
{
    // Show a welcome message
    string hintId = HintAPI.ShowHint(
        player: ev.Player,
        text: $"Welcome {ev.Player.Nickname}!",
        duration: 5.0f,
        priority: 5,
        sourcePlugin: this.Name
    );
    
    Log.Info($"Displayed welcome hint with ID: {hintId}");
}
---------------------------------------------------------------
## Priority-Based Messaging :
---------------------------------------------------------------
// High priority emergency message
HintAPI.ShowHint(
    player: player,
    text: "⚠️ EMERGENCY: Evacuate immediately!",
    duration: 10.0f,
    priority: 10, // High priority
    sourcePlugin: "EmergencySystem"
);

// Low priority informational message
HintAPI.ShowHint(
    player: player,
    text: "Tip: Use the keycard to open doors",
    duration: 3.0f,
    priority: 1, // Low priority
    sourcePlugin: "TipSystem"
);
-----------------------------------------------------------------
## Global Announcements :
-----------------------------------------------------------------
// Send message to all players
var hintIds = HintAPI.ShowHintToAll(
    text: "🎮 New round starting in 30 seconds!",
    duration: 5.0f,
    priority: 8,
    sourcePlugin: "RoundManager"
);

Log.Info($"Sent announcement to {hintIds.Count} players");
-----------------------------------------------------------------
# Creating Your First Plugin :
-----------------------------------------------------------------
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;
using CrazyHintFramework.API;
using System;

namespace MyFirstHintPlugin
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public string WelcomeMessage { get; set; } = "Welcome to our server!";
        public float WelcomeMessageDuration { get; set; } = 5.0f;
    }

    public class Plugin : Plugin<Config>
    {
        public override string Name => "MyFirstHintPlugin";
        public override string Author => "YourName";
        public override Version Version => new Version(1, 0, 0);

        public override void OnEnabled()
        {
            // Subscribe to player events
            Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            // Unsubscribe from events
            Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
            base.OnDisabled();
        }

        private void OnPlayerVerified(VerifiedEventArgs ev)
        {
            // Show welcome message using HintFramework
            string hintId = HintAPI.ShowHint(
                player: ev.Player,
                text: Config.WelcomeMessage,
                duration: Config.WelcomeMessageDuration,
                priority: 5,
                sourcePlugin: Name
            );

            if (Config.Debug)
            {
                Log.Info($"Showed welcome hint to {ev.Player.Nickname} with ID: {hintId}");
            }
        }
    }
}

-----------------------------------------------------------------------------------------------------
## Role-Based Messages : [add Hint for Role]
-----------------------------------------------------------------------------------------------------
private void OnPlayerChangingRole(ChangingRoleEventArgs ev)
{
    // Don't show hints for spectators
    if (ev.NewRole == RoleTypeId.Spectator) return;

    string roleMessage = ev.NewRole switch
    {
        RoleTypeId.ClassD => "You are a Class-D Personnel. Find a way to escape!",
        RoleTypeId.Scientist => "You are a Scientist. Escape to the surface!",
        RoleTypeId.FacilityGuard => "You are a Guard. Protect the facility!",
        RoleTypeId.NtfSpecialist => "You are MTF. Secure, Contain, Protect!",
        _ => $"You are now {ev.NewRole}. Good luck!"
    };

    HintAPI.ShowHint(
        player: ev.Player,
        text: roleMessage,
        duration: 4.0f,
        priority: 7, // Higher priority for role changes
        sourcePlugin: Name
    );
}
---------------------------------------------------------------------------------------------------
### Enjoy all if you need Support just open issus or go in discord server 














