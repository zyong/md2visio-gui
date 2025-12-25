using md2visio.vsdx.@base;
using Visio = Microsoft.Office.Interop.Visio;

namespace md2visio.mxgraph
{
    internal class MxGraphVBuilder : VBuilder
    {
        private MxGraphFigure figure;

        public MxGraphVBuilder(MxGraphFigure figure)
        {
            this.figure = figure;
        }

        public void Build(string outputFile)
        {
            ExecuteBuild();
            VShapeDrawer.RemoveShadow();
            SaveAndClose(outputFile);
        }

        private void ExecuteBuild()
        {
            var vertices = figure.GetVertices();
            var edges = figure.GetEdges();

            // Create shapes for vertices
            var shapeMap = new Dictionary<string, Visio.Shape>();
            foreach (var cell in vertices)
            {
                var shape = CreateShape(cell);
                if (shape != null && cell.Id != null)
                {
                    shapeMap[cell.Id] = shape;
                }
            }

            // Create connectors for edges
            foreach (var cell in edges)
            {
                if (cell.Source != null && cell.Target != null && shapeMap.ContainsKey(cell.Source) && shapeMap.ContainsKey(cell.Target))
                {
                    var connector = visioPage.Drop(visioDoc.Masters.ItemU[@"Dynamic connector"], 0, 0);
                    var sourceShape = shapeMap[cell.Source];
                    var targetShape = shapeMap[cell.Target];

                    connector.CellsU["BeginX"].GlueTo(sourceShape.CellsU["PinX"]);
                    connector.CellsU["EndX"].GlueTo(targetShape.CellsU["PinX"]);
                }
            }
        }

        private Visio.Shape? CreateShape(MxCell cell)
        {
            if (cell.Geometry == null) return null;

            string masterName = GetMasterName(cell.Style);
            var master = visioDoc.Masters.ItemU[masterName];
            double xInch = cell.Geometry.X / 96.0;
            double yInch = cell.Geometry.Y / 96.0;
            var shape = visioPage.Drop(master, xInch, yInch);

            if (masterName != "Text Box")
            {
                double widthInch = cell.Geometry.Width / 96.0;
                double heightInch = cell.Geometry.Height / 96.0;
                shape.CellsU["Width"].FormulaU = $"{widthInch} in";
                shape.CellsU["Height"].FormulaU = $"{heightInch} in";
            }

            if (!string.IsNullOrEmpty(cell.Value))
            {
                shape.Text = cell.Value;
            }

            return shape;
        }

        private string GetMasterName(string? style)
        {
            if (style == null) return "Rectangle";

            if (style.Contains("ellipse")) return "Ellipse";
            if (style.Contains("rhombus")) return "Diamond";
            if (style.Contains("rounded=1")) return "Rounded Rectangle";
            if (style.Contains("text")) return "Text Box"; // for labels
            return "Rectangle";
        }
    }
}