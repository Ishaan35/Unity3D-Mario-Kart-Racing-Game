using System;
using System.Collections.Generic;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class ShaderSourceMap
    {
        // Indicates where a new node begins
        List<int> m_LineStarts;
        List<AbstractMaterialNode> m_Nodes;
        int m_LineCount;

        internal ShaderSourceMap(string source, List<ShaderStringMapping> mappings)
        {
            m_LineStarts = new List<int>();
            m_Nodes = new List<AbstractMaterialNode>();

            // File line numbers are 1-based
            var line = 1;
            var currentIndex = 0;
            foreach (var mapping in mappings)
            {
                var stopIndex = mapping.startIndex + mapping.count;
                if (currentIndex >= stopIndex)
                    continue;

                m_LineStarts.Add(line);
                m_Nodes.Add(mapping.node);

                while (currentIndex < stopIndex && currentIndex != -1)
                {
                    currentIndex = source.IndexOf('\n', currentIndex + 1);
                    line++;
                }

                if (currentIndex == -1)
                    break;
            }

            m_LineCount = line-1;
        }

        // Binary search that behaves like C++'s std::lower_bound()
        public AbstractMaterialNode FindNode(int line)
        {
            // line is 1-based throughout this function
            if (line > m_LineCount || line <= 0)
                return null;
            var l = 0;
            var r = m_LineStarts.Count - 1;
            while (l <= r)
            {
                var m = (l + r) / 2;
                var lineStart = m_LineStarts[m];
                var lineStop = m == m_LineStarts.Count-1 ? m_LineCount+1 : m_LineStarts[m + 1];
                if (line >= lineStop)
                    l = m + 1;
                else if (line < lineStart)
                    r = m - 1;
                else
                    return m_Nodes[m];
            }
            throw new Exception("Something went wrong in binary search");
        }
    }
}
