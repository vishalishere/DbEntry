﻿using System.Collections.Generic;

namespace Lephone.Util.Logging
{
    static class LogRecorderProvider
    {
        private static readonly Dictionary<string, ILogRecorder> Jar = new Dictionary<string, ILogRecorder>();

        public static ILogRecorder GetLogRecorder(string name)
        {
            lock (Jar)
            {
                if (Jar.ContainsKey(name))
                {
                    return Jar[name];
                }
                var ilc = (ILogRecorder)ClassHelper.CreateInstance(name);
                Jar[name] = ilc;
                return ilc;
            }
        }
    }
}