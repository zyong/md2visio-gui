using md2visio.mermaid.cmn;
using md2visio.mermaid.sequence._internal;
using System.Text.RegularExpressions;

namespace md2visio.mermaid.sequence
{
    internal class SeSttKeyword : SynState
    {
        public override SynState NextState()
        {
            if (!IsKeyword(Buffer)) throw new SynException("syntax error", Ctx);
            
            // Special handling for sequenceDiagram keyword
            if (Buffer == "sequenceDiagram")
            {
                Save(Buffer).ClearBuffer().SlideSpaces();
                return Forward<SeSttChar>();
            }
            
            Save(Buffer).ClearBuffer().SlideSpaces();
            
            // For sequence diagrams, keywords like "participant" may be followed by parameters
            if (Buffer == "participant")
            {
                return Forward<SeSttWord>(); // participant name
            }
            
            return Forward<SeSttChar>();
        }

        static Regex regKW =
            new("^(sequenceDiagram|participant|note|activate|deactivate|loop|alt|else|opt|par|and|critical|break|rect|ref|autonumber)$", RegexOptions.Compiled);
        public static bool IsKeyword(string word)
        {
            return regKW.IsMatch(word);
        }
    }
}