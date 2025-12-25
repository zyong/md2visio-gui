using System.Text.RegularExpressions;
using md2visio.struc.sequence;

namespace md2visio.mermaid.sequence
{
    /// <summary>
    /// 简化的时序图解析器 - 使用正则表达式直接解析基本语法
    /// 这是一个临时解决方案，用于快速实现时序图功能
    /// </summary>
    internal class SimpleSequenceParser
    {
        // participant Alice
        // participant A as Alice
        private static readonly Regex ParticipantRegex = new Regex(
            @"^\s*participant\s+(\w+)(?:\s+as\s+([^\r\n]+))?\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        // Alice->>Bob: Hello
        // Alice-->>Bob: Hello
        // Alice->Bob: Hello
        // Alice-->Bob: Hello
        private static readonly Regex MessageRegex = new Regex(
            @"^\s*(\w+)\s*(--?>>?|->>|-->>|->|-->)\s*(\w+)\s*:\s*(.+?)\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        public Sequence Parse(string content)
        {
            var sequence = new Sequence();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"[SimpleSequenceParser] Parsing {lines.Length} lines");

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Skip empty lines and mermaid/sequenceDiagram keywords
                if (string.IsNullOrWhiteSpace(trimmedLine) ||
                    trimmedLine == "sequenceDiagram" ||
                    trimmedLine.StartsWith("```"))
                {
                    continue;
                }

                // Try to parse as participant
                var participantMatch = ParticipantRegex.Match(trimmedLine);
                if (participantMatch.Success)
                {
                    string name = participantMatch.Groups[1].Value;
                    string alias = participantMatch.Groups[2].Success
                        ? participantMatch.Groups[2].Value.Trim()
                        : name;

                    Console.WriteLine($"  Found participant: name='{name}', alias='{alias}'");
                    sequence.Participants.Add(new Participant(name, alias));
                    continue;
                }

                // Try to parse as message
                var messageMatch = MessageRegex.Match(trimmedLine);
                if (messageMatch.Success)
                {
                    string from = messageMatch.Groups[1].Value;
                    string arrow = messageMatch.Groups[2].Value;
                    string to = messageMatch.Groups[3].Value;
                    string text = messageMatch.Groups[4].Value.Trim();

                    Console.WriteLine($"  Found message: {from} {arrow} {to} : {text}");

                    // Auto-add participants if not explicitly declared
                    AddParticipantIfNotExists(sequence, from);
                    AddParticipantIfNotExists(sequence, to);

                    // Determine message type based on arrow
                    MessageType type = GetMessageType(arrow);

                    sequence.Messages.Add(new Message(from, to, text, type));
                    continue;
                }

                // Log unrecognized lines for debugging
                Console.WriteLine($"  ! Unrecognized line: {trimmedLine}");
            }

            Console.WriteLine($"[SimpleSequenceParser] Result: {sequence.Participants.Count} participants, {sequence.Messages.Count} messages");
            return sequence;
        }

        private void AddParticipantIfNotExists(Sequence sequence, string name)
        {
            // Check both Name and Alias to avoid duplicates
            // e.g., "participant frontend as 前端服务" creates Participant(Name="frontend", Alias="前端服务")
            // Messages use "frontend", so we need to check Name field as well
            if (!sequence.Participants.Any(p => p.Name == name || p.Alias == name))
            {
                Console.WriteLine($"  Auto-adding participant: {name}");
                sequence.Participants.Add(new Participant(name));
            }
        }

        private MessageType GetMessageType(string arrow)
        {
            // Solid arrows: ->> or -->>
            if (arrow.Contains(">>"))
            {
                return MessageType.Solid;
            }
            // Dashed arrows: -> or -->
            else if (arrow.Contains(">"))
            {
                return MessageType.Dashed;
            }
            // Default
            return MessageType.Open;
        }
    }
}
