using md2visio.vsdx.@base;
using Visio = Microsoft.Office.Interop.Visio;

namespace md2visio.mxgraph
{
    internal class MxGraphVBuilder : VBuilder
    {
        private MxGraphFigure figure;
        private MxStyleParser styleParser;
        private Visio.Document? stencilDoc;

        public MxGraphVBuilder(MxGraphFigure figure)
        {
            this.figure = figure;
            this.styleParser = new MxStyleParser();

            // Open the basic shapes stencil
            try
            {
                // Try to open the built-in Basic Shapes stencil
                stencilDoc = VisioApp!.Documents.OpenEx(
                    "Basic Shapes.vss",
                    (short)Visio.VisOpenSaveArgs.visOpenDocked
                );
                Console.WriteLine("✓ Opened Basic Shapes stencil");
            }
            catch
            {
                Console.WriteLine("Warning: Could not open Basic Shapes stencil. Using document masters.");
                stencilDoc = null;
            }
        }

        public void Build(string outputFile)
        {
            ExecuteBuild();
            CleanupPageDisplay();
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
                    // Use Visio's built-in ConnectorToolDataObject for dynamic connectors
                    // This is the correct way to create connectors in Visio
                    var connector = visioPage.Drop(VisioApp!.ConnectorToolDataObject, 0, 0);
                    var sourceShape = shapeMap[cell.Source];
                    var targetShape = shapeMap[cell.Target];

                    // Glue connector ends to shapes
                    connector.CellsU["BeginX"].GlueTo(sourceShape.CellsU["PinX"]);
                    connector.CellsU["EndX"].GlueTo(targetShape.CellsU["PinX"]);

                    // Set connector style
                    connector.CellsU["LineWeight"].FormulaU = "1 pt";
                    connector.CellsU["EndArrow"].FormulaU = "5";  // Arrow ending
                }
            }
        }

        private Visio.Shape? CreateShape(MxCell cell)
        {
            if (cell.Geometry == null) return null;

            string masterName = GetMasterName(cell.Style);

            // Try to get master from stencil first, then from document
            Visio.Master? master = null;
            try
            {
                if (stencilDoc != null)
                {
                    master = stencilDoc.Masters.ItemU[masterName];
                }
            }
            catch
            {
                // If not found in stencil, try document masters
                try
                {
                    master = visioDoc.Masters.ItemU[masterName];
                }
                catch
                {
                    Console.WriteLine($"Warning: Could not find master '{masterName}'. Using Rectangle.");
                    // Fallback to rectangle
                    if (stencilDoc != null)
                    {
                        master = stencilDoc.Masters.ItemU["Rectangle"];
                    }
                    else
                    {
                        // If we still can't find it, create a simple rectangle
                        return null;
                    }
                }
            }

            // Convert coordinates: mxGraph uses pixels from top-left, Visio uses inches from bottom-left
            double xInch = cell.Geometry.X / 96.0;
            // Visio Y-axis is inverted: we need to flip the Y coordinate
            // Assuming a standard page height of 11 inches (letter size)
            double pageHeight = 11.0;
            double yInch = pageHeight - (cell.Geometry.Y / 96.0);

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

            // Apply colors from style
            ApplyStyles(shape, cell.Style);

            // Remove connection points (the black dots on shape borders)
            RemoveConnectionPoints(shape);

            return shape;
        }

        private void ApplyStyles(Visio.Shape shape, string? styleString)
        {
            if (string.IsNullOrEmpty(styleString))
                return;

            var styles = styleParser.Parse(styleString);

            // Apply fill color
            var fillColor = styleParser.GetFillColor(styles);
            if (fillColor != null)
            {
                if (fillColor.ToLower() == "none")
                {
                    // Make fill transparent
                    shape.CellsU["FillPattern"].FormulaU = "0";  // No fill
                }
                else
                {
                    var rgbColor = MxStyleParser.ConvertColorToRGB(fillColor);
                    if (rgbColor != null)
                    {
                        shape.CellsU["FillForegnd"].FormulaU = rgbColor;
                    }
                }
            }

            // Apply stroke (border) color
            var strokeColor = styleParser.GetStrokeColor(styles);
            if (strokeColor != null)
            {
                if (strokeColor.ToLower() == "none")
                {
                    // Make border invisible
                    shape.CellsU["LinePattern"].FormulaU = "0";  // No line
                }
                else
                {
                    var rgbColor = MxStyleParser.ConvertColorToRGB(strokeColor);
                    if (rgbColor != null)
                    {
                        shape.CellsU["LineColor"].FormulaU = rgbColor;
                    }
                }
            }

            // Set line weight only if there's a visible border
            if (strokeColor == null || strokeColor.ToLower() != "none")
            {
                shape.CellsU["LineWeight"].FormulaU = "1.5 pt";
            }
        }

        private string GetMasterName(string? style)
        {
            if (style == null) return "Rectangle";

            // Map mxGraph styles to Visio built-in shapes
            // These names match the built-in Basic Shapes stencil
            if (style.Contains("ellipse")) return "Circle";  // Use Circle instead of Ellipse
            if (style.Contains("rhombus")) return "Diamond";
            if (style.Contains("rounded=1")) return "Rounded rectangle";  // Note: lowercase 'r'
            // For text labels, just use Rectangle - we'll add text to it anyway
            // "Text block" doesn't exist in Basic Shapes stencil
            return "Rectangle";
        }

        private void CleanupPageDisplay()
        {
            try
            {
                if (visioPage == null) return;

                // Remove any stray guide or connection point shapes that may have been created
                var shapesToDelete = new List<Visio.Shape>();

                foreach (Visio.Shape shape in visioPage.Shapes)
                {
                    try
                    {
                        // Check if this is a very small shape (likely a stray connection point)
                        double width = shape.Cells["Width"].ResultIU;
                        double height = shape.Cells["Height"].ResultIU;

                        // If shape is extremely small (less than 0.01 inches), it's likely a stray point
                        if (width < 0.01 && height < 0.01)
                        {
                            shapesToDelete.Add(shape);
                        }
                        // Also check for shapes with specific connection point characteristics
                        else if (shape.Master == null && string.IsNullOrEmpty(shape.Text))
                        {
                            // Check if it has no meaningful geometry (might be a guide or point)
                            if (width < 0.05 && height < 0.05)
                            {
                                shapesToDelete.Add(shape);
                            }
                        }
                    }
                    catch
                    {
                        // Skip shapes that can't be evaluated
                    }
                }

                // Delete all identified stray shapes
                foreach (var shape in shapesToDelete)
                {
                    try
                    {
                        shape.Delete();
                    }
                    catch
                    {
                        // Silently continue if deletion fails
                    }
                }
            }
            catch
            {
                // Silently continue if page cleanup fails
            }
        }

        private void RemoveConnectionPoints(Visio.Shape shape)
        {
            try
            {
                // Remove connection points by deleting the section's rows
                short sectionIndex = (short)Visio.VisSectionIndices.visSectionConnectionPts;

                if (shape.SectionExists[sectionIndex, (short)Visio.VisExistsFlags.visExistsAnywhere] != 0)
                {
                    // Use RowsSRC to delete rows from the section
                    short rowCount = shape.RowCount[sectionIndex];

                    // Delete rows from end to beginning to avoid index shifting issues
                    for (short i = (short)(rowCount - 1); i >= 0; i--)
                    {
                        try
                        {
                            shape.DeleteRow(sectionIndex, i);
                        }
                        catch
                        {
                            // Continue if a specific row can't be deleted
                        }
                    }
                }
            }
            catch
            {
                // Silently continue if connection point removal fails
            }
        }
    }
}