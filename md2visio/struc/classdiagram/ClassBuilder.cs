using System.Text.RegularExpressions;
using md2visio.mermaid.classdiagram;
using md2visio.struc.figure;

namespace md2visio.struc.classdiagram
{
    internal class ClassBuilder : FigureBuilder
    {
        private ClassDiagram? diagram;

        public ClassBuilder(mermaid.cmn.SttIterator iter) : base(iter)
        {
        }

        public override void Build(string outputFile)
        {
            Console.WriteLine("Building Class diagram");
            Console.WriteLine("Using SimpleClassParser (fast path)");

            try
            {
                // Read the input file
                string inputFile = iter.Context.InputFile;
                string content = File.ReadAllText(inputFile);

                // Try to extract mermaid code block, otherwise use entire content
                var match = Regex.Match(content, @"```mermaid\s*\r?\n(.*?)```", RegexOptions.Singleline);
                string mermaidCode;
                if (match.Success)
                {
                    mermaidCode = match.Groups[1].Value;
                    Console.WriteLine("Found mermaid code block");
                }
                else
                {
                    mermaidCode = content;
                    Console.WriteLine("No code block found, using entire file");
                }

                // Parse the Class diagram
                var parser = new SimpleClassParser();
                diagram = parser.Parse(mermaidCode);

                if (diagram == null || diagram.Classes.Count == 0)
                {
                    throw new Exception("Failed to parse Class diagram or no classes found");
                }

                Console.WriteLine($"✓ Parsed successfully: {diagram.Classes.Count} classes, {diagram.Relationships.Count} relationships");

                // Output to Visio
                Output(outputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Class diagram parsing failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        protected void Output(string outputFile)
        {
            if (diagram == null)
            {
                throw new Exception("Class diagram not built yet");
            }

            Console.WriteLine($"Outputting Class diagram to: {outputFile}");
            diagram.ToVisio(outputFile);
        }
    }
}
