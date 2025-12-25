using md2visio.mxgraph;

namespace md2visio.mxgraph
{
    public class MxGraphConverter
    {
        public void ConvertToVisio(string xmlContent, string outputPath)
        {
            var parser = new MxGraphParser();
            var cells = parser.Parse(xmlContent);
            var figure = new MxGraphFigure(cells);
            figure.ToVisio(outputPath);
        }
    }
}