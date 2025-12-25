using md2visio.mermaid.cmn;

namespace md2visio.mermaid.er
{
    internal class ErSttChar : SynState
    {
        public override SynState NextState()
        {
            string? next = Ctx.Peek();
            if (next == null) return EndOfFile;

            // For ER diagrams, we just consume all characters since we'll use SimpleErParser
            if (next == "\n") { return Forward<SttFinishFlag>(); }

            return Take().Forward<ErSttChar>();
        }
    }
}
