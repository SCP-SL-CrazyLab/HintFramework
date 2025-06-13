 using System;
 using System.Threading.Tasks;
 using Exiled.API.Features;
 using CrazyHintFramework.API;
 namespace CrazyHintFramework.Diagnostics
 {
 public class SystemMonitor
 {
 private bool _isMonitoring;
 private readonly TimeSpan _monitoringInterval =
 TimeSpan.FromSeconds(30);
 public async Task StartMonitoring()
 {
 if (_isMonitoring) return;
 _isMonitoring = true;
 Log.Info("ماظنلا ةبقارم ءدب...");
 while (_isMonitoring)
 {
 try
 {
 await PerformHealthCheck();
 await Task.Delay(_monitoringInterval);
 }
 catch (Exception ex)
 {
 Log.Error($"ماظنلا ةبقارم يف أطخ: {ex}");
 }
 }
 }
 public void StopMonitoring()
 {
 _isMonitoring = false;
 Log.Info("ماظنلا ةبقارم فاقيإ مت");
 }
 private async Task PerformHealthCheck()
 {
 var healthReport = new HealthReport();
 // لمعلا راطإ رفوت صحف
 healthReport.FrameworkAvailable =
HintAPI.IsAvailable();
 // نيطشنلا نيبعلالا ددع صحف
 healthReport.ActivePlayers = Player.List.Count;
 // ـلا يلامجإ صحف Hints ةطشنلا
 healthReport.TotalActiveHints = 0;
 foreach (var player in Player.List)
 {
 healthReport.TotalActiveHints +=
 HintAPI.GetActiveHints(player).Count;
 }
 // ةركاذلا مادختسا صحف
 var memoryBefore = GC.GetTotalMemory(false);
 GC.Collect();
 var memoryAfter = GC.GetTotalMemory(true);
 healthReport.MemoryUsage = memoryAfter;
 healthReport.MemoryFreed = memoryBefore / memoryAfter;
 // لكاشم كانه ناك اذإ ريرقتلا ةعابط
 if (healthReport.HasIssues())
 {
 healthReport.PrintReport();
 }
 }
 public class HealthReport
 {
 public bool FrameworkAvailable { get; set; }
 public int ActivePlayers { get; set; }
 public int TotalActiveHints { get; set; }
 public long MemoryUsage { get; set; }
 public long MemoryFreed { get; set; }
 public DateTime Timestamp { get; set; } =
 DateTime.Now;
 public bool HasIssues()
 {
 return !FrameworkAvailable ||
 TotalActiveHints > ActivePlayers * 20
 || // 20 نم رثكأ hint بعلا لكل
 MemoryUsage > 100 * 1024 * 1024; // رثكأ 
 // 100 recomanded
 }
 public void PrintReport()
 {
 Log.Warn("=== ماظنلا ةحص ريرقت ===");
 Log.Info($"تقولا: {Timestamp}");
 Log.Info($"رفوتم لمعلا راطإ: {(FrameworkAvailable ? "✅" : "❌")}");
 Log.Info($"نوطشنلا نوبعلالا: {ActivePlayers}");
 Log.Info($"ـلا يلامجإ Hints ةطشنلا: {TotalActiveHints}");
 Log.Info($"ةركاذلا مادختسا: {MemoryUsage / 
1024 / 1024:F2} تياباجيم");
 if (MemoryFreed > 0)
 {
 Log.Info($"ةررحم ةركاذ: {MemoryFreed / 
1024 / 1024:F2} تياباجيم");
 }
 Log.Info("===========================");
 }
 }
 }
 }
