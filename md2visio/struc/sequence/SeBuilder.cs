using md2visio.mermaid.cmn;
using md2visio.mermaid.sequence;
using md2visio.mermaid.sequence._internal;
using md2visio.struc.figure;
using md2visio.struc.sequence;
using System.Text.RegularExpressions;

namespace md2visio.struc.sequence
{
    internal class SeBuilder : FigureBuilder
    {
        Sequence sequence = new Sequence();

        public SeBuilder(SttIterator iter) : base(iter)
        {
        }

        public override void Build(string outputFile)
        {
            Console.WriteLine("Building sequence diagram");
            Console.WriteLine("Using SimpleSequenceParser (fast path)");

            // 使用简化解析器 - 直接从文件读取并解析
            try
            {
                string inputFile = iter.Context.InputFile;
                string content = File.ReadAllText(inputFile);

                // 提取 mermaid 代码块内容
                var match = Regex.Match(content, @"```mermaid\s*\r?\n(.*?)```", RegexOptions.Singleline);
                string mermaidCode;

                if (match.Success)
                {
                    mermaidCode = match.Groups[1].Value;
                    Console.WriteLine("Extracted mermaid code block");
                }
                else
                {
                    // 没有代码块包裹，假设整个文件都是 mermaid 代码
                    mermaidCode = content;
                    Console.WriteLine("No code block found, using entire file");
                }

                // 使用简化解析器
                var parser = new SimpleSequenceParser();
                sequence = parser.Parse(mermaidCode);

                Console.WriteLine($"✓ Parsed successfully: {sequence.Participants.Count} participants, {sequence.Messages.Count} messages");

                // 输出到 Visio
                Output(outputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error in SimpleSequenceParser: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // 回退到原始解析逻辑
                Console.WriteLine("Falling back to original parser...");
                BuildWithOriginalParser(outputFile);
            }
        }

        // 保留原始解析逻辑作为后备
        void BuildWithOriginalParser(string outputFile)
        {
            Console.WriteLine($"Total states in context: {iter.Context.StateList.Count}");
            int count = 0;
            while (iter.HasNext())
            {
                SynState cur = iter.Next();
                count++;
                Console.WriteLine($"[{count}] Processing: {cur.GetType().Name} - '{cur.Fragment}'");
                if (cur is SttMermaidStart)
                {
                    Console.WriteLine("  -> Found MermaidStart, continuing...");
                }
                else if (cur is SttMermaidClose)
                {
                    Console.WriteLine("  -> Found MermaidClose, outputting...");
                    Output(outputFile);
                    break;
                }
                else if (cur is SeSttKeyword)
                {
                    Console.WriteLine("  -> Found Keyword, building...");
                    BuildKeyword();
                }
                else if (cur is SeSttText)
                {
                    Console.WriteLine("  -> Found Text, building message...");
                    BuildText();
                }
                else if (cur is SttComment)
                {
                    Console.WriteLine("  -> Found Comment, loading directive...");
                    sequence.Config.LoadUserDirectiveFromComment(cur.Fragment);
                }
                else if (cur is SttFrontMatter)
                {
                    Console.WriteLine("  -> Found FrontMatter, loading...");
                    sequence.Config.LoadUserFrontMatter(cur.Fragment);
                }
                else if (cur is SttFinishFlag)
                {
                    Console.WriteLine("  -> Found FinishFlag (line end), continuing...");
                    // FinishFlag just marks end of line, not end of diagram - continue processing
                }
                else
                {
                    Console.WriteLine($"  -> Unknown state type: {cur.GetType().Name}");
                }
            }
            Console.WriteLine($"Finished processing. Total states processed: {count}");
        }

        void BuildKeyword()
        {
            string kw = iter.Current.Fragment;
            Console.WriteLine($"  BuildKeyword: '{kw}'");
            if (kw == "participant")
            {
                // participant name [as alias]
                Console.WriteLine($"    Found 'participant' keyword");
                iter.Next(); // skip participant
                Console.WriteLine($"    After skip participant: {iter.Current.GetType().Name} - '{iter.Current.Fragment}'");
                if (iter.Current is SeSttText)
                {
                    string name = iter.Current.Fragment;
                    string alias = name;
                    Console.WriteLine($"    Participant name: '{name}'");
                    if (iter.HasNext())
                    {
                        iter.Next();
                        Console.WriteLine($"    Next token: '{iter.Current.Fragment}'");
                        if (iter.Current.Fragment == "as" && iter.HasNext())
                        {
                            iter.Next();
                            alias = iter.Current.Fragment;
                            Console.WriteLine($"    Alias: '{alias}'");
                        }
                    }
                    Console.WriteLine($"    ✓ Adding participant: name='{name}', alias='{alias}'");
                    sequence.Participants.Add(new Participant(name, alias));
                }
                else
                {
                    Console.WriteLine($"    ✗ Expected SeSttText but got {iter.Current.GetType().Name}");
                }
            }
            else
            {
                Console.WriteLine($"    ! Unknown keyword: '{kw}'");
            }
        }

        void BuildText()
        {
            // Assume it's a message: From ->> To : text
            string from = iter.Current.Fragment;
            Console.WriteLine($"  BuildText: from='{from}'");
            AddParticipant(from);
            if (iter.HasNext())
            {
                iter.Next();
                Console.WriteLine($"    Next: {iter.Current.GetType().Name} - '{iter.Current.Fragment}'");
                if (iter.Current is SeSttArrow)
                {
                    string arrow = iter.Current.Fragment;
                    Console.WriteLine($"    Found arrow: '{arrow}'");
                    if (iter.HasNext())
                    {
                        iter.Next();
                        Console.WriteLine($"    Next: {iter.Current.GetType().Name} - '{iter.Current.Fragment}'");
                        if (iter.Current is SeSttText)
                        {
                            string to = iter.Current.Fragment;
                            Console.WriteLine($"    To: '{to}'");
                            AddParticipant(to);
                            string text = "";
                            if (iter.HasNext())
                            {
                                iter.Next();
                                Console.WriteLine($"    Next: {iter.Current.GetType().Name} - '{iter.Current.Fragment}'");
                                if (iter.Current is SeSttColon && iter.HasNext())
                                {
                                    iter.Next();
                                    text = iter.Current.Fragment;
                                    Console.WriteLine($"    Message text: '{text}'");
                                }
                            }
                            MessageType type = GetMessageType(arrow);
                            Console.WriteLine($"    ✓ Adding message: {from} {arrow} {to} : {text} (Type: {type})");
                            sequence.Messages.Add(new Message(from, to, text, type));
                        }
                        else
                        {
                            Console.WriteLine($"    ✗ Expected SeSttText for 'to' but got {iter.Current.GetType().Name}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"    ✗ Expected SeSttArrow but got {iter.Current.GetType().Name}");
                }
            }
        }

        void AddParticipant(string name)
        {
            if (!sequence.Participants.Any(p => p.Alias == name))
            {
                sequence.Participants.Add(new Participant(name));
            }
        }

        MessageType GetMessageType(string arrow)
        {
            if (arrow.Contains(">>")) return MessageType.Solid;
            if (arrow.Contains(">")) return MessageType.Dashed;
            return MessageType.Open;
        }

        void Output(string outputFile)
        {
            sequence.ToVisio(outputFile);
        }
    }
}