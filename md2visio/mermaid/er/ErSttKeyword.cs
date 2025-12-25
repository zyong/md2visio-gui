using md2visio.mermaid.cmn;
using System.Text.RegularExpressions;

namespace md2visio.mermaid.er
{
    internal class ErSttKeyword : SynState
    {
        public override SynState NextState()
        {
            if (!IsKeyword(Buffer)) throw new SynException("syntax error", Ctx);

            Save(Buffer).ClearBuffer().SlideSpaces();
            return Forward<ErSttChar>();
        }

        static Regex regKW = new("^(erDiagram)$", RegexOptions.Compiled);

        public static bool IsKeyword(string word)
        {
            return regKW.IsMatch(word);
        }
    }
}
