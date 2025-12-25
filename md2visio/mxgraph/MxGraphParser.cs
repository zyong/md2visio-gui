using System.Xml.Linq;

namespace md2visio.mxgraph
{
    public class MxGraphParser
    {
        public List<MxCell> Parse(string xmlContent)
        {
            var cells = new List<MxCell>();
            var doc = XDocument.Parse(xmlContent);
            var root = doc.Root;

            foreach (var cellElement in root.Elements("mxCell"))
            {
                var cell = new MxCell
                {
                    Id = cellElement.Attribute("id")?.Value,
                    Value = cellElement.Attribute("value")?.Value,
                    Style = cellElement.Attribute("style")?.Value,
                    Vertex = cellElement.Attribute("vertex")?.Value == "1",
                    Parent = cellElement.Attribute("parent")?.Value,
                    Source = cellElement.Attribute("source")?.Value,
                    Target = cellElement.Attribute("target")?.Value,
                    Edge = cellElement.Attribute("edge")?.Value == "1"
                };

                var geometry = cellElement.Element("mxGeometry");
                if (geometry != null)
                {
                    cell.Geometry = new MxGeometry
                    {
                        X = double.Parse(geometry.Attribute("x")?.Value ?? "0"),
                        Y = double.Parse(geometry.Attribute("y")?.Value ?? "0"),
                        Width = double.Parse(geometry.Attribute("width")?.Value ?? "0"),
                        Height = double.Parse(geometry.Attribute("height")?.Value ?? "0"),
                        As = geometry.Attribute("as")?.Value
                    };
                }

                cells.Add(cell);
            }

            return cells;
        }
    }

    public class MxCell
    {
        public string? Id { get; set; }
        public string? Value { get; set; }
        public string? Style { get; set; }
        public bool Vertex { get; set; }
        public string? Parent { get; set; }
        public string? Source { get; set; }
        public string? Target { get; set; }
        public bool Edge { get; set; }
        public MxGeometry? Geometry { get; set; }
    }

    public class MxGeometry
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string? As { get; set; }
    }
}