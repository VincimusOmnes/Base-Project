using UnityEngine;
using System.Runtime.CompilerServices;

namespace Marmalade.Shared
{
    /// <summary>
    /// Provides a consistent log format of [ClassName:LineNumber] across all assemblies.
    /// Info and Warning are gated behind ENABLE_LOGGING and compile out entirely in release builds.
    /// Error and Exception always remain active regardless of build configuration.
    /// Use this instead of Debug.Log directly to ensure consistent formatting and build stripping.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Logs an informational message to the Unity console.
        /// Only active when ENABLE_LOGGING is defined.
        /// </summary>
        [HideInCallstack]
        public static void Info(
            object message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            #if ENABLE_LOGGING
                Debug.Log(Format(message, filePath, lineNumber));
            #endif
        }

        /// <summary>
        /// Logs a warning message to the Unity console.
        /// Only active when ENABLE_LOGGING is defined.
        /// </summary>
        [HideInCallstack]
        public static void Warning(
            object message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            #if ENABLE_LOGGING
                Debug.LogWarning(Format(message, filePath, lineNumber));
            #endif
        }

        /// <summary>
        /// Logs an error message to the Unity console.
        /// Always active regardless of build configuration.
        /// </summary>
        [HideInCallstack]
        public static void Error(
            object message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogError(Format(message, filePath, lineNumber));
        }

        /// <summary>
        /// Logs an exception with its message and stack trace to the Unity console.
        /// Always active regardless of build configuration.
        /// </summary>
        [HideInCallstack]
        public static void Exception(
            System.Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogError(Format($"{exception.Message}\n{exception.StackTrace}", filePath, lineNumber));
        }

        private static string Format(object message, string filePath, int lineNumber)
        {
            string className = System.IO.Path.GetFileNameWithoutExtension(filePath);
            return $"[{className}:{lineNumber}] {message}";
        }
    }
}