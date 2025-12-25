using md2visio.mermaid.cmn;

namespace md2visio.mermaid.sequence._internal
{
    internal class SeSttWordFlag : SynState
    {
        public override SynState NextState()
        {
            // a word flag may make a node string
            // left
            if (!string.IsNullOrWhiteSpace(Buffer)) { return Forward<SeSttWord>(); }

            // right            
            string? next = Peek();
            if (next == null) return EndOfFile;
            if (next == ";") { return Forward<SttFinishFlag>(); }
            if (next == "-") { return Forward<SeSttArrow>(); }

            // skip spaces, tabs, and newlines
            while (true)
            {
                next = Peek();
                if (next == null) return EndOfFile;
                if (next != " " && next != "\t" && next != "\n") break;
                Ctx.Slide();
            }

            return Forward<SeSttChar>();
        }
    }
}