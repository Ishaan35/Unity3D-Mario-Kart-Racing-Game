#if UNITY_EDITOR
using System.Collections.Generic;

namespace UnityEngine.Monetization
{
    sealed class Configuration
    {
        public bool enabled { get; private set; }
        public string defaultPlacement { get; private set; }
        public Dictionary<string, PlacementContent> placementContents { get; private set; }

        public Configuration(string configurationResponse)
        {
            var configurationJson = (Dictionary<string, object>)MiniJSON.Json.Deserialize(configurationResponse);
            enabled = (bool)configurationJson["enabled"];
            placementContents = new Dictionary<string, PlacementContent>();
            foreach (Dictionary<string, object> placement in (List<object>)configurationJson["placements"])
            {
                var id = (string)placement["id"];
                var allowSkip = (bool)placement["allowSkip"];
                if ((bool)placement["default"])
                {
                    defaultPlacement = id;
                }
                foreach (object type in (List<object>)placement["adTypes"])
                {
                    if ((string)type == "IAP")
                    {
                        EditorPromoAdOperations operations = new EditorPromoAdOperations();
                        operations.allowSkip = allowSkip;
                        operations.placementId = id;
                        PromoAdPlacementContent placementContent = new PromoAdPlacementContent(id, operations);
                        placementContents.Add(id, placementContent);
                        placementContent.extras = new Dictionary<string, object> {};
                        break;
                    }
                    if ((string)type == "VIDEO")
                    {
                        EditorShowAdOperations operations = new EditorShowAdOperations();
                        operations.allowSkip = allowSkip;
                        operations.placementId = id;
                        ShowAdPlacementContent placementContent = new ShowAdPlacementContent(id, operations);
                        placementContents.Add(id, placementContent);
                        placementContent.extras = new Dictionary<string, object> {};
                        break;
                    }
                }
            }
        }
    }
}
#endif
