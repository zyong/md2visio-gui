using md2visio.vsdx.@base;

namespace md2visio.mxgraph
{
    internal class MxGraphFigure
    {
        private List<MxCell> cells;

        public MxGraphFigure(List<MxCell> cells)
        {
            this.cells = cells;
        }

        public void ToVisio(string outputPath)
        {
            var builder = new MxGraphVBuilder(this);
            builder.Build(outputPath);
        }

        public List<MxCell> GetVertices()
        {
            return cells.Where(c => c.Vertex).ToList();
        }

        public List<MxCell> GetEdges()
        {
            return cells.Where(c => c.Edge).ToList();
        }
    }
}