using md2visio.mermaid.cmn;
using md2visio.mermaid.sequence;

namespace md2visio.mermaid.sequence._internal
{
    internal class SeSttWord : SynState
    {
        public override SynState NextState()
        {
            // keyword or text
            if (string.IsNullOrWhiteSpace(Buffer)) throw new SynException("syntax error", Ctx);

            if (SeSttKeyword.IsKeyword(Buffer)) return Forward<SeSttKeyword>();
            else return Forward<SeSttText>();
        }
    }
}