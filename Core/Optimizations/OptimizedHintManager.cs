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
 /// ـلل نسحم ريدم Hints ءادلأا تانيسحت عم
 /// </summary>
 public class OptimizedHintManager
 {
 private static OptimizedHintManager _instance;
 public static OptimizedHintManager Instance =>
 _instance ??= new OptimizedHintManager();
 // مادختسا ConcurrentDictionary نامضل Thread Safety
 private readonly ConcurrentDictionary<Player,
 ConcurrentBag<HintData>> _playerHints;
 // تانئاكلا مادختسا ةداعلإ ةعومجم
 private readonly ConcurrentQueue<HintData> _hintPool;
 // نيسحتلا تادادعإ
 private readonly int _maxHintsPerPlayer = 10;
 private readonly int _poolSize = 100;
// ءادلأا تايئاصحإ
 private long _totalHintsCreated;
 private long _totalHintsRemoved;
 private long _poolHits;
 private long _poolMisses;
 private OptimizedHintManager()
 {
 _playerHints = new ConcurrentDictionary<Player,
 ConcurrentBag<HintData>>();
 _hintPool = new ConcurrentQueue<HintData>();
 // ً اقبسم ةعومجملا ءلم
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
 // ىلع لوصحلا hint ديدج ءاشنإ وأ ةعومجملا نم
 var hint = GetHintFromPool();
 if (hint == null)
 {
 hint = new HintData(text, duration, priority,
 sourcePlugin);
 Interlocked.Increment(ref _poolMisses);
 }
 else
 {
 // ـلا ةئيهت ةداعإ hint داعتسملا
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
 // ـلا ةعومجم ىلع لوصحلا hints بعلال
 var playerHints = _playerHints.GetOrAdd(player, _
 => new ConcurrentBag<HintData>());
 // ـلل ىصقلأا دحلا نم ققحتلا hints بعلا لكل
 if (playerHints.Count >= _maxHintsPerPlayer)
 {
 // مدقأ ةلازإ hint
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
 // تانايبلا فيظنت
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
 // ـلا نودب ةديدج ةعومجم ءاشنإ hint مدقلأا
var newHints = hints.Where(h => h.Id !=
 oldestHint.Id).ToArray();
 // ةعومجملا لادبتسا
 _playerHints.TryUpdate(player, new
 ConcurrentBag<HintData>(newHints), playerHints);
 // ـلا عاجرإ hint ةعومجملا ىلإ
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
 // ـلا ةعومجم ثيدحت hints
 _playerHints.TryUpdate(player, new
 ConcurrentBag<HintData>(activeHints), hints);
 // ـلا عاجرإ hints ىلإ ةيحلاصلا ةيهتنملا 

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
 Log.Info("=== ءادلأا تايئاصحإ ===");
 Log.Info($"ـلا يلامجإ Hints ةأشنملا: {TotalHintsCreated:N0}");
 Log.Info($"ـلا يلامجإ Hints ةفوذحملا: {TotalHintsRemoved:N0}");
 Log.Info($"ةعومجملا تاحاجن: {PoolHits:N0}");
 Log.Info($"ةعومجملا تاقافخإ: {PoolMisses:N0}");
 Log.Info($"ةعومجملا ةءافك: {PoolEfficiency:F2}%");
 Log.Info($"نوطشنلا نوبعلالا: {ActivePlayers}");
 Log.Info($"ةعومجملا مجح: {PoolSize}");
 Log.Info("========================");
 }
 }
 }
 }
