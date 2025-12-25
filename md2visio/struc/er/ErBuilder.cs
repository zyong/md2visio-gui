using System;
using System.IO;
using System.Text.RegularExpressions;
using md2visio.mermaid.er;
using md2visio.struc.figure;

namespace md2visio.struc.er
{
    internal class ErBuilder : FigureBuilder
    {
        private ErDiagram? diagram;

        public ErBuilder(mermaid.cmn.SttIterator iter) : base(iter)
        {
        }

        public override void Build(string outputFile)
        {
            Console.WriteLine("Building ER diagram");
            Console.WriteLine("Using SimpleErParser (fast path)");

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

                // Parse the ER diagram
                var parser = new SimpleErParser();
                diagram = parser.Parse(mermaidCode);

                if (diagram == null || diagram.Entities.Count == 0)
                {
                    throw new Exception("Failed to parse ER diagram or no entities found");
                }

                Console.WriteLine($"✓ Parsed successfully: {diagram.Entities.Count} entities, {diagram.Relationships.Count} relationships");

                // Output to Visio
                Output(outputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ ER diagram parsing failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        protected void Output(string outputFile)
        {
            if (diagram == null)
            {
                throw new Exception("ER diagram not built yet");
            }

            Console.WriteLine($"Outputting ER diagram to: {outputFile}");
            new md2visio.vsdx.VBuilderEr(diagram).Build(outputFile);
        }
    }
}
