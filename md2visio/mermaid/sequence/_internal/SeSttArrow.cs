using md2visio.mermaid.cmn;

namespace md2visio.mermaid.sequence._internal
{
    internal class SeSttArrow : SynState
    {
        public override SynState NextState()
        {
            if (Ctx.Test(@"^->>")) { Save("->>").Slide(3); return Forward<SeSttChar>(); }
            if (Ctx.Test(@"^-->>")) { Save("-->>").Slide(4); return Forward<SeSttChar>(); }
            if (Ctx.Test(@"^-\)")) { Save("-)").Slide(2); return Forward<SeSttChar>(); }
            if (Ctx.Test(@"^>>")) { Save(">>").Slide(2); return Forward<SeSttChar>(); }
            if (Ctx.Test(@"^>")) { Save(">").Slide(1); return Forward<SeSttChar>(); }

            return Take().Forward<SeSttChar>();
        }
    }
}