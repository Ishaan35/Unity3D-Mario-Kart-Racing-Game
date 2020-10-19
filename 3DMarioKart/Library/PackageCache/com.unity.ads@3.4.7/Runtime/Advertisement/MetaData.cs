using System.Collections.Generic;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Advertisements
{
    /// <summary>
    /// A class for sending various metadata to UnityAds.
    /// </summary>
    public sealed class MetaData
    {
        private readonly IDictionary<string, object> m_MetaData = new Dictionary<string, object>();

        /// <summary>
        /// Metadata category.
        /// </summary>
        public string category { get; private set; }

        /// <summary>
        /// Constructs a metadata instance that can be passed to the <c>Advertisement</c> class.
        /// </summary>
        public MetaData(string category)
        {
            this.category = category;
        }

        /// <summary>
        /// Sets new metadata fields.
        /// </summary>
        /// <param name="key">Metadata key.</param>
        /// <param name="value">Metadata value (must be JSON serializable).</param>
        public void Set(string key, object value)
        {
            m_MetaData[key] = value;
        }

        /// <summary>
        /// Returns the stored metadata key.
        /// </summary>
        public object Get(string key)
        {
            return m_MetaData[key];
        }

        /// <summary>
        /// Returns the stored metadata.
        /// </summary>
        public IDictionary<string, object> Values()
        {
            return m_MetaData;
        }

        internal string ToJSON()
        {
            return Json.Serialize(m_MetaData);
        }
    }
}
