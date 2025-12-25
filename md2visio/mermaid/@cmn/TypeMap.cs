using md2visio.mermaid.graph;
using md2visio.mermaid.graph.@internal;
using md2visio.mermaid.journey;
using md2visio.mermaid.packet;
using md2visio.mermaid.pie;
using md2visio.mermaid.sequence;
using md2visio.mermaid.sequence._internal;
using md2visio.mermaid.xy;
using md2visio.struc.graph;
using md2visio.struc.journey;
using md2visio.struc.packet;
using md2visio.struc.pie;
using md2visio.struc.sequence;
using md2visio.struc.xy;

namespace md2visio.mermaid.cmn
{
    internal class TypeMap
    {
        public static readonly Dictionary<string, Type> KeywordMap = new()
        {
            { "graph", typeof(GSttKeyword) }, { "flowchart", typeof(GSttKeyword) },
            { "journey", typeof(JoSttKeyword) },
            { "pie", typeof(PieSttKeyword) },
            { "packet-beta", typeof(PacSttKeyword) }, { "packet", typeof(PacSttKeyword) },
            { "xychart-beta", typeof(XySttKeyword) }, { "xychart", typeof(XySttKeyword) },
            { "sequenceDiagram", typeof(SeSttKeyword) },
        };

        public static readonly Dictionary<string, Type> CharMap = new()
        {
            { "graph", typeof(GSttChar) }, { "flowchart", typeof(GSttChar) },
            { "journey", typeof(JoSttChar) },
            { "pie", typeof(PieSttChar) },
            { "packet-beta", typeof(PaSttChar) }, { "packet", typeof(PaSttChar) },
            { "xychart-beta", typeof(XySttChar) }, { "xychart", typeof(XySttChar) },
            { "sequenceDiagram", typeof(SeSttChar) },
        };

        public static readonly Dictionary<string, Type> BuilderMap = new()
        {
            { "graph", typeof(GBuilder) }, { "flowchart", typeof(GBuilder) },
            { "journey", typeof(JoBuilder) },
            { "pie", typeof(PieBuilder) },
            { "packet-beta", typeof(PacBuilder) }, { "packet", typeof(PacBuilder) },
            { "xychart-beta", typeof(XyBuilder) }, { "xychart", typeof(XyBuilder) },
            { "sequenceDiagram", typeof(SeBuilder) },
        };

        public static readonly Dictionary<string, string> ConfigMap = new()
        {
            {typeof(Graph).Name, "flowchart"},
            {typeof(Journey).Name, "journey"},
            {typeof(Packet).Name, "packet"},
            {typeof(Pie).Name, "pie"},
            {typeof(XyChart).Name, "xyChart"},
            {typeof(Sequence).Name, "sequence"},
        };
    }
}
