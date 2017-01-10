using System;

namespace Assets.Scripts
{
    public static class Log
    {
        private static bool _isEnabled = true;

        public static void Info(string obj, string location, string msg)
        {
            if (!_isEnabled) return;
            LogMessage(LogType.Info, obj, location, msg);
        }

        public static void Warn(string obj, string location, string msg)
        {
            if(!_isEnabled) return;
            LogMessage(LogType.Warning, obj, location, msg);
        }

        public static void Error(string obj, string location, string msg)
        {
            if (!_isEnabled) return;
            LogMessage(LogType.Error, obj, location, msg);
        }

        private static void LogMessage(LogType type, string obj, string location, string msg)
        {
            var dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logStr = string.Format("{0} [{1}][{2}->{3}] # {4}", dt, type, obj, location, msg);
            switch (type)
            {
                case LogType.Info:
                    UnityEngine.Debug.Log(logStr);
                    break;

                case LogType.Warning:
                    UnityEngine.Debug.LogWarning(logStr);
                    break;

                case LogType.Error:
                    UnityEngine.Debug.LogError(logStr);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        private enum LogType
        {
            Info,
            Warning,
            Error
        }
    }
}

