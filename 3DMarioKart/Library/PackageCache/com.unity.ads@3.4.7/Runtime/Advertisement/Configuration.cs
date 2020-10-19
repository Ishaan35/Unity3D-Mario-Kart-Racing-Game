using System.Collections.Generic;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Advertisements
{
    sealed class Configuration
    {
        public bool enabled { get; }
        public string defaultPlacement { get; }
        public Dictionary<string, bool> placements { get; }

        public Configuration(string configurationResponse)
        {
            var configurationJson = (Dictionary<string, object>)Json.Deserialize(configurationResponse);
            enabled = (bool)configurationJson["enabled"];
            placements = new Dictionary<string, bool>();
            foreach (Dictionary<string, object> placement in (List<object>)configurationJson["placements"])
            {
                var id = (string)placement["id"];
                var allowSkip = (bool)placement["allowSkip"];
                if ((bool)placement["default"])
                {
                    defaultPlacement = id;
                }
                placements.Add(id, allowSkip);
            }
        }
    }
}
