using System;


namespace TempAlert
{
    internal class ConfigSet
    {
        public bool FileExists { get; private set; }
        public bool Syncs { get; private set; }
        public string ServerUrl { get; private set; }
        public TimeSpan UpdateInterval { get; private set; }
        public float UpperThreshold { get; private set; }
        public float CriticalThreshold { get; private set; }

        public ConfigSet()
        {
            FileExists = false;
            Syncs = false;
            UpdateInterval = TimeSpan.FromMinutes(1);
            UpperThreshold = 50;
            CriticalThreshold = 85;
        }

        public bool Reload()
        {
            return false;
        }
    }
}
