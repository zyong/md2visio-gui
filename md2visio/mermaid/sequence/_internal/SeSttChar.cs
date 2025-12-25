using md2visio.mermaid.cmn;

namespace md2visio.mermaid.sequence._internal
{
    internal class SeSttChar : SynState
    {
        public override SynState NextState()
        {
            string? next = Ctx.Peek();
            if (next == null) return EndOfFile;

            if (next == ";") { return Forward<SeSttWordFlag>(); }
            if (next == "\n") { return Forward<SeSttWordFlag>(); }
            if (next == "\t") { return Forward<SeSttWordFlag>(); }
            if (next == " ") { return Forward<SeSttWordFlag>(); }
            if (next == "-") { return Forward<SeSttArrow>(); }
            if (next == ":") { return Forward<SeSttColon>(); }

            return Take().Forward<SeSttChar>();
        }
    }
}