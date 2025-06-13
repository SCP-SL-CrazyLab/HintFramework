using HarmonyLib;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CrazyHintFramework.API.Managers;
using CrazyHintFramework.API.Managers;
using CrazyHintFramework.API.Models;

namespace CrazyHintFramework.Patches
{
    /// <summary>
    /// Harmony Patch لتعديل طريقة عرض Hints في اللعبة.
    /// </summary>
    [HarmonyPatch]
    public class HintPatch
    {
        // تخزين آخر رسالة ظهرت لكل لاعب لتجنب التكرار
        private static readonly Dictionary<Player, string> _lastDisplayedHints = new();

        /// <summary>
        /// تحديد الطريقة المستهدفة (ShowHint) باستخدام الانعكاس (Reflection).
        /// </summary>
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            var playerType = typeof(Player);
            var showHintMethod = playerType.GetMethod("ShowHint", new[] { typeof(string), typeof(ushort) });

            if (showHintMethod != null)
                return showHintMethod;

            var gameAssembly = Assembly.GetAssembly(typeof(Player));

            foreach (var type in gameAssembly.GetTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                      BindingFlags.Instance | BindingFlags.Static))
                {
                    if (method.Name.Contains("Hint") && method.GetParameters().Length >= 2)
                    {
                        var parameters = method.GetParameters();
                        if (parameters[0].ParameterType == typeof(string) &&
                            (parameters[1].ParameterType == typeof(float) || parameters[1].ParameterType == typeof(ushort)))
                        {
                            Log.Info($"Found Hints method: {type.Name}.{method.Name}");
                            return method;
                        }
                    }
                }
            }

            Log.Warn("لم يتم العثور على طريقة ShowHint في اللعبة.");
            return null;
        }

        /// <summary>
        /// Prefix patch لتغيير محتوى الرسالة قبل عرضها.
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix(object __instance, object[] __args, ref object __result)
        {
            try
            {
                Player player = GetPlayerFromContext(__instance, __args);
                if (player == null)
                    return true;

                var activeHints = HintManager.Instance.GetActiveHints(player);
                if (activeHints.Count == 0)
                    return true;

                var topHint = activeHints.OrderByDescending(h => h.Priority)
                                         .ThenBy(h => h.CreatedAt)
                                         .FirstOrDefault();

                if (_lastDisplayedHints.TryGetValue(player, out string lastHintId) && lastHintId == topHint.Id)
                    return false;

                _lastDisplayedHints[player] = topHint.Id;

                if (__args.Length >= 2)
                {
                    __args[0] = topHint.Text;

                    if (__args[1] is ushort)
                        __args[1] = (ushort)Math.Max(1, topHint.Duration);
                    else if (__args[1] is float)
                        __args[1] = topHint.Duration;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"خطأ في HintPatch.Prefix: {ex}");
                return true;
            }
        }

        /// <summary>
        /// Postfix patch (يمكن استخدامه لاحقًا لأغراض أخرى).
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(object __instance, object[] __args, object __result)
        {
            try
            {
                // يمكن استخدام هذه الطريقة لاحقًا إذا احتجت لتنفيذ شيء بعد عرض الرسالة
            }
            catch (Exception ex)
            {
                Log.Error($"خطأ في HintPatch.Postfix: {ex}");
            }
        }

        /// <summary>
        /// استخراج كائن اللاعب من سياق المثيل أو المعطيات.
        /// </summary>
        private static Player GetPlayerFromContext(object instance, object[] args)
        {
            try
            {
                if (instance is Player playerInstance)
                    return playerInstance;

                foreach (var arg in args)
                {
                    if (arg is Player playerArg)
                        return playerArg;
                }

                Type instanceType = instance.GetType();
                MemberInfo playerMember = instanceType.GetProperty("Player") ??
                                          (MemberInfo)instanceType.GetField("Player");

                if (playerMember != null)
                {
                    object playerValue = playerMember is PropertyInfo prop
                        ? prop.GetValue(instance)
                        : ((FieldInfo)playerMember).GetValue(instance);

                    if (playerValue is Player playerProp)
                        return playerProp;
                }

                foreach (var arg in args)
                {
                    if (arg is int playerId)
                        return Player.Get(playerId);
                    else if (arg is string playerName)
                        return Player.Get(playerName);
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"خطأ في GetPlayerFromContext: {ex}");
                return null;
            }
        }

        /// <summary>
        /// تنظيف القاموس عند إعادة تحميل الإضافة أو إيقافها.
        /// </summary>
        public static void Cleanup()
        {
            _lastDisplayedHints.Clear();
        }
    }
}