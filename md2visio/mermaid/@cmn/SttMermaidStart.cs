namespace md2visio.mermaid.cmn
{
    internal class SttMermaidStart : SynState
    {
        public override SynState NextState()
        {
            return Run(Ctx);
        }

        public static SynState Run(SynContext ctx)
        {
            if (ctx.Until(@"^\s*(?<bquote>`{3,})\s*mermaid\s*(?=\n)"))
            {
                SynState state = new SttMermaidStart();
                state.Ctx = ctx;
                state.Fragment = ctx.ExpectedGroups["bquote"].Value;
                ctx.AddState(state);

                return state.Forward<SttIntro>();
            }
            else
            {
                // No ```mermaid, assume bare Mermaid content
                SynState state = new SttMermaidStart();
                state.Ctx = ctx;
                state.Fragment = "";
                ctx.AddState(state);

                return state.Forward<SttIntro>();
            }
        }
    }
}
