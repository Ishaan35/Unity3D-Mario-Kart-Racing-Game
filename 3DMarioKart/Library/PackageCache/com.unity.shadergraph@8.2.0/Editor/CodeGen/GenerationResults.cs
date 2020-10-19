using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    class GenerationResults
    {
        public string shader { get; set; }
        public List<PropertyCollector.TextureInfo> configuredTextures;
        public PreviewMode previewMode { get; set; }
        public Vector1ShaderProperty outputIdProperty { get; set; }
        public ShaderSourceMap sourceMap { get; set; }

        public GenerationResults()
        {
            configuredTextures = new List<PropertyCollector.TextureInfo>();
        }
    }
}
