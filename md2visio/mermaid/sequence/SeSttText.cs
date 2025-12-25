using md2visio.mermaid.cmn;
using md2visio.mermaid.sequence._internal;

namespace md2visio.mermaid.sequence
{
    internal class SeSttText : SynState
    {
        public override SynState NextState()
        {
            if (!Ctx.FindContainerFrag("sequenceDiagram").Success)
                throw new SynException("syntax error", Ctx);

            string text = Buffer;
            ClearBuffer().SlideSpaces();
            if (string.IsNullOrWhiteSpace(text)) return Forward<SeSttChar>();

            return Save(text).Forward<SeSttChar>();
        }
    }
}