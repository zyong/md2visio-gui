using md2visio.mermaid.cmn;
using System.Text.RegularExpressions;

namespace md2visio.mermaid.classdiagram
{
    internal class ClassSttKeyword : SynState
    {
        public override SynState NextState()
        {
            if (!IsKeyword(Buffer)) throw new SynException("syntax error", Ctx);

            Save(Buffer).ClearBuffer().SlideSpaces();
            return Forward<ClassSttChar>();
        }

        static Regex regKW = new("^(classDiagram)$", RegexOptions.Compiled);

        public static bool IsKeyword(string word)
        {
            return regKW.IsMatch(word);
        }
    }
}
