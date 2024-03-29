using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    public static class Logging
    {
        public static string LogFilePath
        {
            get
            {
                var assemblyDir = SixAxisPlugin.AssemblyDirectory;
                var path = Path.Combine(assemblyDir, "log.txt");
                return path;
            }
        }

        public static void Initialize()
        {
            File.Delete(LogFilePath);
        }

        public static void Log(string message)
        {
            Log(new Dictionary<string, string>(), message);
        }

        public static void Log(IDictionary<string, string> values, string message)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("DateTime={0} ", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            foreach (var key in values.Keys)
            {
                sb.AppendFormat("{0}={1} ", key, values[key]);
            }
            sb.Append("\n");
            sb.Append(message);

            UnityEngine.Debug.Log("[SixAxis]: " + sb.ToString().Replace("\n", "\n\t"));

            sb.Append("\n\n");
            File.AppendAllText(LogFilePath, sb.ToString());
        }
    }
}