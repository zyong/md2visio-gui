using md2visio.mermaid.cmn;
using md2visio.mermaid.sequence._internal;

namespace md2visio.mermaid.sequence
{
    internal class SeSttKeywordParam : SynState
    {
        public override SynState NextState()
        {
            if (!Ctx.WithinKeyword()) throw new SynException("syntax error", Ctx);
            if (!HasParam(Ctx)) return SlideSpaces().Forward<SeSttChar>();

            SlideSpaces();
            SaveParams();
            return SlideSpaces().Forward<SeSttChar>();
        }

        public static bool HasParam(SynContext ctx)
        {
            bool match = ctx.Test(@"^(?<param>[^\n]+?)(?=\n|;)");
            return match && ctx.TestGroups["param"].Value.Trim().Length > 0;
        }

        void SaveParams()
        {
            var text = new System.Text.StringBuilder();
            while (true)
            {
                string? next = Peek();
                if (next == null || next == "\n" || next == ";")
                {
                    AddText(text);
                    break;
                }

                if (SttQuoted.TestQuoted(Ctx))
                {
                    AddText(text).AddCompo("\"", Ctx.TestGroups["quote"].Value);
                    Slide(Ctx.TestGroups["0"].Length);
                }
                else
                {
                    Take();
                    text.Append(next);
                }
            }
        }

        SeSttKeywordParam AddText(System.Text.StringBuilder text)
        {
            if (text.Length > 0)
            {
                this.AddCompo("text", text.ToString());
                text.Clear();
            }
            return this;
        }
    }
}