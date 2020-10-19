using System.Collections.Generic;

namespace UnityEngine.Monetization
{
    /// <summary>
    /// Class for sending various metadata to UnityAds.
    /// </summary>
    public sealed class MetaData : Dictionary<string, object>
    {
        /// <summary>
        /// Metadata category.
        /// </summary>
        public string category { get; }

        /// <summary>
        /// Constructs an metadata instance that can be passed to the Advertisement class.
        /// </summary>
        public MetaData(string category)
        {
            this.category = category;
        }

        /// <summary>
        /// Sets new metadata fields.
        /// </summary>
        /// <param name="key">Metadata key.</param>
        /// <param name="value">Metadata value. Must be JSON serializable.</param>
        public void Set(string key, object value)
        {
            this[key] = value;
        }

        /// <summary>
        /// Returns the stored metadata key.
        /// </summary>
        public object Get(string key)
        {
            return this[key];
        }

        internal string ToJSON()
        {
            return MiniJSON.Json.Serialize(this);
        }
    }
}
