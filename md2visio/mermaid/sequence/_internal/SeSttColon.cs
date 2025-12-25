using md2visio.mermaid.cmn;

namespace md2visio.mermaid.sequence._internal
{
    internal class SeSttColon : SynState
    {
        public override SynState NextState()
        {
            Take(); // consume :
            return Forward<SeSttChar>();
        }
    }
}