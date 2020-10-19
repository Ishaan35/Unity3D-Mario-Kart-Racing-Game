using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    interface IMayRequireMeshUV
    {
        bool RequiresMeshUV(UVChannel channel, ShaderStageCapability stageCapability = ShaderStageCapability.All);
    }

    static class MayRequireMeshUVExtensions
    {
        public static bool RequiresMeshUV(this ISlot slot, UVChannel channel)
        {
            var mayRequireMeshUV = slot as IMayRequireMeshUV;
            return mayRequireMeshUV != null && mayRequireMeshUV.RequiresMeshUV(channel);
        }
    }
}
