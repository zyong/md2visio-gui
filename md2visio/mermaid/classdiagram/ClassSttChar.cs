using md2visio.mermaid.cmn;

namespace md2visio.mermaid.classdiagram
{
    internal class ClassSttChar : SynState
    {
        public override SynState NextState()
        {
            string? next = Ctx.Peek();
            if (next == null) return EndOfFile;

            // For class diagrams, we consume all characters since we'll use SimpleClassParser
            if (next == "\n") { return Forward<SttFinishFlag>(); }

            return Take().Forward<ClassSttChar>();
        }
    }
}
