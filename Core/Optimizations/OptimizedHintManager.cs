 using System;
 using System.Collections.Concurrent;
 using System.Collections.Generic;
 using System.Linq;
 using System.Threading;
 using System.Threading.Tasks;
 using CrazyHintFramework.API.Models;
 using Exiled.API.Features;
 namespace CrazyHintFramework.Core.Optimizations
 {
 /// <summary>
 /// make it easy. Hints. Start again
 /// </summary>
 public class OptimizedHintManager
 {
 private static OptimizedHintManager _instance;
 public static OptimizedHintManager Instance =>
 _instance ??= new OptimizedHintManager();
// Use ConcurrentDictionary for Thread Safety
 private readonly ConcurrentDictionary<Player,
 ConcurrentBag<HintData>> _playerHints;
// Don't talk to anyone about it
 private readonly ConcurrentQueue<HintData> _hintPool;
//Necessary settings
 private readonly int _maxHintsPerPlayer = 10;
 private readonly int _poolSize = 100;
// Get started correctly
 private long _totalHintsCreated;
 private long _totalHintsRemoved;
 private long _poolHits;
 private long _poolMisses;
 private OptimizedHintManager()
 {
 _playerHints = new ConcurrentDictionary<Player,
 ConcurrentBag<HintData>>();
 _hintPool = new ConcurrentQueue<HintData>();
// Divide the word into a suitable sentence
 PreFillPool();
 }
 private void PreFillPool()
 {
 for (int i = 0; i < _poolSize; i++)
 {
 _hintPool.Enqueue(new HintData("", 0, 0, ""));
 }
 }
 public bool AddHint(Player player, string text, float
 duration, int priority = 0, string sourcePlugin = "Unknown")
 {
 if (player == null || string.IsNullOrEmpty(text) ||
 duration <= 0)
 return false;
// On the right hint hint about what happened or what happened in general
 var hint = GetHintFromPool();
 if (hint == null)
 {
 hint = new HintData(text, duration, priority,
 sourcePlugin);
 Interlocked.Increment(ref _poolMisses);
 }
 else
 {
  // Example Created hint for Devlopers
 hint.Text = text;
 hint.Duration = duration;
 hint.Priority = priority;
 hint.SourcePlugin = sourcePlugin;
 hint.Id = Guid.NewGuid().ToString();
 hint.CreatedAt = DateTime.Now;
 hint.ExpiresAt =
 hint.CreatedAt.AddSeconds(duration);
 hint.IsActive = true;
Interlocked.Increment(ref _poolHits);
 }
 var playerHints = _playerHints.GetOrAdd(player, _
 => new ConcurrentBag<HintData>());
 if (playerHints.Count >= _maxHintsPerPlayer)
 {
 RemoveOldestHint(player, playerHints);
 }
 playerHints.Add(hint);
 Interlocked.Increment(ref _totalHintsCreated);
 return true;
 }
 private HintData GetHintFromPool()
 {
 return _hintPool.TryDequeue(out var hint) ? hint :
 null;
 }
 private void ReturnHintToPool(HintData hint)
 {
 if (_hintPool.Count < _poolSize)
 {
 // Hint settings 
 hint.Text = "";
 hint.Duration = 0;
 hint.Priority = 0;
 hint.SourcePlugin = "";
 hint.IsActive = false;
 _hintPool.Enqueue(hint);
 }
 }
 private void RemoveOldestHint(Player player,
 ConcurrentBag<HintData> playerHints)
 {
 var hints = playerHints.ToArray();
 if (hints.Length == 0) return;
 var oldestHint = hints.OrderBy(h =>
 h.CreatedAt).First();
var newHints = hints.Where(h => h.Id !=
 oldestHint.Id).ToArray();
 _playerHints.TryUpdate(player, new
 ConcurrentBag<HintData>(newHints), playerHints);
 ReturnHintToPool(oldestHint);
 Interlocked.Increment(ref _totalHintsRemoved);
 }
 public List<HintData> GetActiveHints(Player player)
 {
 if (!_playerHints.TryGetValue(player, out var
 playerHints))
 return new List<HintData>();
 var now = DateTime.Now;
 return playerHints
 .Where(h => h.IsActive && h.ExpiresAt > now)
 .OrderByDescending(h => h.Priority)
 .ThenBy(h => h.CreatedAt)
 .ToList();
 }
 public void CleanupExpiredHints()
 {
 var now = DateTime.Now;
 var playersToUpdate = new List<Player>();
 foreach (var kvp in _playerHints)
 {
 var player = kvp.Key;
 var hints = kvp.Value;
 var activeHints = hints.Where(h => h.ExpiresAt
 > now).ToArray();
 var expiredHints = hints.Where(h => h.ExpiresAt
 <= now).ToArray();
 if (expiredHints.Length > 0)
 {
 _playerHints.TryUpdate(player, new
 ConcurrentBag<HintData>(activeHints), hints);

 foreach (var expiredHint in expiredHints)
 {
 ReturnHintToPool(expiredHint);
Interlocked.Increment(ref
 _totalHintsRemoved);
 }
 }
 }
 }
 public PerformanceStats GetPerformanceStats()
 {
 return new PerformanceStats
 {
 TotalHintsCreated = _totalHintsCreated,
 TotalHintsRemoved = _totalHintsRemoved,
 PoolHits = _poolHits,
 PoolMisses = _poolMisses,
 PoolEfficiency = _poolHits + _poolMisses > 0 ?
 (double)_poolHits / (_poolHits + _poolMisses) * 100 : 0,
 ActivePlayers = _playerHints.Count,
 PoolSize = _hintPool.Count
 };
 }
 public class PerformanceStats
 {
 public long TotalHintsCreated { get; set; }
 public long TotalHintsRemoved { get; set; }
 public long PoolHits { get; set; }
 public long PoolMisses { get; set; }
 public double PoolEfficiency { get; set; }
 public int ActivePlayers { get; set; }
 public int PoolSize { get; set; }
 public void PrintStats()
 {
 Log.Info("=== Fast performens and Optimazed ===");
 Log.Info($"TotalHintsCreated: {TotalHintsCreated:N0}");
 Log.Info($"TotalHintsRemoved: {TotalHintsRemoved:N0}");
 Log.Info($"PoolHits: {PoolHits:N0}");
 Log.Info($"ةPoolMisses: {PoolMisses:N0}");
 Log.Info($"PoolEfficiency: {PoolEfficiency:F2}%");
 Log.Info($"Players Activated: {ActivePlayers}");
 Log.Info($"Hint Size: {PoolSize}");
 Log.Info("========================");
 }
 }
 }
 }
