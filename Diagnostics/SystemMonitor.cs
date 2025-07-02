using System;
using System.Threading.Tasks;
using Exiled.API.Features;
using CrazyHintFramework.API;

namespace CrazyHintFramework.Diagnostics
{
    public class SystemMonitor
    {
        private bool _isMonitoring;
        private readonly TimeSpan _monitoringInterval = TimeSpan.FromSeconds(30);

        public async Task StartMonitoring()
        {
            if (_isMonitoring) return;
            _isMonitoring = true;

            Log.Info("System monitoring has started...");

            while (_isMonitoring)
            {
                try
                {
                    await PerformHealthCheck();
                    await Task.Delay(_monitoringInterval);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error during system monitoring: {ex}");
                }
            }
        }

        public void StopMonitoring()
        {
            _isMonitoring = false;
            Log.Info("System monitoring has stopped.");
        }

        private async Task PerformHealthCheck()
        {
            var report = new HealthReport
            {
                FrameworkAvailable = HintAPI.IsAvailable(),
                ActivePlayers = Player.List.Count,
                TotalActiveHints = 0,
                Timestamp = DateTime.Now
            };

            foreach (var player in Player.List)
            {
                report.TotalActiveHints += HintAPI.GetActiveHints(player).Count;
            }

            long memoryBefore = GC.GetTotalMemory(false);
            GC.Collect();
            long memoryAfter = GC.GetTotalMemory(true);

            report.MemoryUsage = memoryAfter;
            report.MemoryFreed = memoryBefore - memoryAfter;

            if (report.HasIssues())
                report.PrintReport();
        }

        public class HealthReport
        {
            public bool FrameworkAvailable { get; set; }
            public int ActivePlayers { get; set; }
            public int TotalActiveHints { get; set; }
            public long MemoryUsage { get; set; }
            public long MemoryFreed { get; set; }
            public DateTime Timestamp { get; set; }

            public bool HasIssues()
            {
                return !FrameworkAvailable ||
                       TotalActiveHints > ActivePlayers * 20 || // More than 20 hints per player
                       MemoryUsage > 100 * 1024 * 1024;         // More than 100MB usage
            }

            public void PrintReport()
            {
                Log.Warn("=== System Health Report ===");
                Log.Info($"Timestamp: {Timestamp}");
                Log.Info($"Framework Available: {(FrameworkAvailable ? "✅" : "❌")}");
                Log.Info($"Active Players: {ActivePlayers}");
                Log.Info($"Total Active Hints: {TotalActiveHints}");
                Log.Info($"Memory Usage: {MemoryUsage / 1024 / 1024:F2} MB");

                if (MemoryFreed > 0)
                    Log.Info($"Memory Freed After GC: {MemoryFreed / 1024 / 1024:F2} MB");

                Log.Info("=============================");
            }
        }
    }
}
