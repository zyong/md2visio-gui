using md2visio.mermaid.cmn;
using md2visio.struc.figure;
using md2visio.vsdx;

namespace md2visio.struc.sequence
{
    internal class Sequence : Figure
    {
        public List<Participant> Participants { get; } = new List<Participant>();
        public List<Message> Messages { get; } = new List<Message>();

        public Sequence()
        {
        }

        public override void ToVisio(string path)
        {
            var builder = new VBuilderSe(this);
            builder.Build(path);
        }
    }

    internal class Participant
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public double X { get; set; } // position for drawing

        public Participant(string name, string alias = "")
        {
            Name = name;
            Alias = string.IsNullOrEmpty(alias) ? name : alias;
        }
    }

    internal class Message
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Text { get; set; }
        public MessageType Type { get; set; }

        public Message(string from, string to, string text, MessageType type)
        {
            From = from;
            To = to;
            Text = text;
            Type = type;
        }
    }

    internal enum MessageType
    {
        Solid,
        Dashed,
        Open
    }
}