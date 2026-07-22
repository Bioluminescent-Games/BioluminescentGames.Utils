using System.Diagnostics;
using UnityEngine;
using Unity.Scripting.LifecycleManagement;
using Debug = UnityEngine.Debug;

namespace BioluminescentGames.Utils.StaticUtilities
{
    [AutoStaticsCleanup]
    public static partial class Log
    {
        public static void Info(object message) => Debug.Log(message);
        public static void Info(object message, Object context) => Debug.Log(message, context);
        
        public static void Error(object message) => Debug.LogError(message);
        public static void Error(object message, Object context) => Debug.LogError(message, context);
        
        public static void Warning(object message) => Debug.LogWarning(message);
        public static void Warning(object message, Object context) => Debug.LogWarning(message, context);
        
        public static void Exception(System.Exception exception) => Debug.LogException(exception);
        public static void Exception(System.Exception exception, Object context) => Debug.LogException(exception, context);
        
        public static void Assert(bool condition) => Debug.Assert(condition);
        public static void Assert(bool condition, Object context) => Debug.Assert(condition, context);
        public static void Assert(bool condition, object message) => Debug.Assert(condition, message);
        public static void Assert(bool condition, object message, Object context) => Debug.Assert(condition, message, context);
        
        [Conditional("DEBUG")]
        public static void Verbose(object message) => Debug.Log($"[VERBOSE] {message}");
        
        [Conditional("DEBUG")]
        public static void Verbose(object message, Object context) => Debug.Log($"[VERBOSE] {message}", context);
        
        [Conditional("BG_ENABLE_TRACE")]
        public static void Trace(object message) => Debug.Log($"[TRACE] {message}");
        
        [Conditional("BG_ENABLE_TRACE")]
        public static void Trace(object message, Object context) => Debug.Log($"[TRACE] {message}", context);
    }
}
