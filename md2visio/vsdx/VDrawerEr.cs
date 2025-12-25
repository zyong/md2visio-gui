using System;
using System.Collections.Generic;
using System.Linq;
using md2visio.struc.er;
using md2visio.vsdx.@base;
using Microsoft.Office.Interop.Visio;

namespace md2visio.vsdx
{
    internal class VDrawerEr : VFigureDrawer<ErDiagram>
    {
        private Dictionary<string, Shape> entityShapes = new Dictionary<string, Shape>();

        public VDrawerEr(ErDiagram diagram, Application visioApp) : base(diagram, visioApp)
        {
        }

        public override void Draw()
        {
            EnsureVisible();
            PauseForViewing(300);

            DrawEntities();
            PauseForViewing(500);

            DrawRelationships();
            PauseForViewing(300);
        }

        void DrawEntities()
        {
            Console.WriteLine($"Drawing {figure.Entities.Count} entities");

            // Calculate layout
            double startX = 2.0;
            double startY = 10.0;
            double entityWidth = 3.0;
            double columnSpacing = 4.5; // entityWidth (3.0) + half entity spacing (1.5)
            int entitiesPerRow = 3;

            // Pre-calculate entity heights to determine row heights
            double headerHeight = 0.4;
            double attrHeight = 0.25;
            var entityHeights = new Dictionary<int, double>();

            int index = 0;
            double currentY = startY;
            int currentRow = 0;
            double maxHeightInRow = 0;

            foreach (var entity in figure.Entities)
            {
                int row = index / entitiesPerRow;
                int col = index % entitiesPerRow;

                // Calculate entity height
                double totalHeight = headerHeight + entity.Attributes.Count * attrHeight + 0.1;

                // Track maximum height in current row
                if (row != currentRow)
                {
                    // Move to next row
                    currentY -= (maxHeightInRow + 1.5); // Add 1.5 inches spacing between rows
                    currentRow = row;
                    maxHeightInRow = totalHeight;
                }
                else
                {
                    maxHeightInRow = Math.Max(maxHeightInRow, totalHeight);
                }

                double x = startX + col * columnSpacing;
                double y = currentY;

                entity.X = x + entityWidth / 2; // Center X
                entity.Y = y;

                DrawEntity(entity, x, y, entityWidth);
                index++;
            }
        }

        void DrawEntity(Entity entity, double x, double y, double width)
        {
            Console.WriteLine($"Drawing entity: {entity.Name}");

            // Calculate height based on number of attributes
            double headerHeight = 0.4;
            double attrHeight = 0.25;
            double totalHeight = headerHeight + entity.Attributes.Count * attrHeight + 0.1;

            double left = x;
            double right = x + width;
            double top = y;
            double bottom = y - totalHeight;

            // Draw outer rectangle
            var entityRect = visioPage.DrawRectangle(left, bottom, right, top);
            entityShapes[entity.Name] = entityRect;

            // Style the rectangle
            entityRect.CellsU["FillForegnd"].FormulaU = "RGB(255, 255, 255)"; // White fill
            entityRect.CellsU["LineWeight"].FormulaU = "2 pt";
            entityRect.CellsU["LineColor"].FormulaU = "RGB(0, 0, 0)"; // Black border

            // Draw header section (entity name)
            double headerBottom = top - headerHeight;
            var headerLine = visioPage.DrawLine(left, headerBottom, right, headerBottom);
            headerLine.CellsU["LineWeight"].FormulaU = "2 pt";

            // Add entity name text
            var nameText = visioPage.DrawRectangle(left, headerBottom, right, top);
            nameText.Text = entity.Name;
            nameText.CellsU["FillPattern"].FormulaU = "0"; // No fill
            nameText.CellsU["LinePattern"].FormulaU = "0"; // No border
            nameText.CellsU["Char.Style"].FormulaU = "17"; // Bold
            nameText.CellsU["Char.Size"].FormulaU = "12 pt";
            nameText.CellsU["Para.HorzAlign"].FormulaU = "1"; // Center
            nameText.CellsU["VerticalAlign"].FormulaU = "1"; // Middle

            // Draw attributes
            double currentY = headerBottom;
            foreach (var attr in entity.Attributes)
            {
                double attrTop = currentY;
                double attrBottom = currentY - attrHeight;

                // Draw separator line
                if (attr != entity.Attributes.First())
                {
                    var attrLine = visioPage.DrawLine(left, attrTop, right, attrTop);
                    attrLine.CellsU["LineWeight"].FormulaU = "0.5 pt";
                    attrLine.CellsU["LineColor"].FormulaU = "RGB(200, 200, 200)"; // Light gray
                }

                // Format attribute text
                string attrText = FormatAttribute(attr);
                var attrTextBox = visioPage.DrawRectangle(left + 0.1, attrBottom, right - 0.1, attrTop);
                attrTextBox.Text = attrText;
                attrTextBox.CellsU["FillPattern"].FormulaU = "0"; // No fill
                attrTextBox.CellsU["LinePattern"].FormulaU = "0"; // No border
                attrTextBox.CellsU["Char.Size"].FormulaU = "9 pt";
                attrTextBox.CellsU["Para.HorzAlign"].FormulaU = "0"; // Left align
                attrTextBox.CellsU["VerticalAlign"].FormulaU = "1"; // Middle

                // Highlight primary key
                if (attr.IsPrimaryKey)
                {
                    attrTextBox.CellsU["Char.Style"].FormulaU = "17"; // Bold
                    var pkRect = visioPage.DrawRectangle(left, attrBottom, right, attrTop);
                    pkRect.CellsU["FillForegnd"].FormulaU = "RGB(255, 255, 200)"; // Light yellow
                    pkRect.CellsU["FillPattern"].FormulaU = "1";
                    pkRect.CellsU["LinePattern"].FormulaU = "0"; // No border
                    pkRect.SendToBack();
                }

                currentY -= attrHeight;
            }
        }

        string FormatAttribute(EntityAttribute attr)
        {
            string result = $"{attr.Name}: {attr.Type}";

            if (attr.IsPrimaryKey)
            {
                result = $"🔑 {result}";
            }

            if (!string.IsNullOrWhiteSpace(attr.Comment))
            {
                result = $"{result} ({attr.Comment})";
            }

            return result;
        }

        void DrawRelationships()
        {
            Console.WriteLine($"Drawing {figure.Relationships.Count} relationships");

            foreach (var rel in figure.Relationships)
            {
                var fromEntity = figure.Entities.FirstOrDefault(e => e.Name == rel.From);
                var toEntity = figure.Entities.FirstOrDefault(e => e.Name == rel.To);

                if (fromEntity == null || toEntity == null)
                {
                    Console.WriteLine($"  ! Skipping relationship: {rel.From} -> {rel.To} (entity not found)");
                    continue;
                }

                DrawRelationship(rel, fromEntity, toEntity);
            }
        }

        void DrawRelationship(Relationship rel, Entity fromEntity, Entity toEntity)
        {
            Console.WriteLine($"Drawing relationship: {rel.From} -> {rel.To} ({rel.Label})");

            // Get the entity shapes
            if (!entityShapes.ContainsKey(rel.From) || !entityShapes.ContainsKey(rel.To))
            {
                Console.WriteLine($"  ! Cannot find shapes for relationship");
                return;
            }

            var fromShape = entityShapes[rel.From];
            var toShape = entityShapes[rel.To];

            // Drop a dynamic connector from the stencil
            // Use the built-in Dynamic Connector master
            var connector = visioPage.Drop(visioApp.ConnectorToolDataObject, 0, 0);

            // Connect the connector to the shapes
            // GlueToConnectionPoint: glues the connector begin/end to the shape
            connector.CellsU["BeginX"].GlueTo(fromShape.CellsU["PinX"]);
            connector.CellsU["EndX"].GlueTo(toShape.CellsU["PinX"]);

            // Set line style based on relationship type
            if (rel.Type == RelationshipType.NonIdentifying)
            {
                connector.CellsU["LinePattern"].FormulaU = "2"; // Dashed
            }
            else
            {
                connector.CellsU["LinePattern"].FormulaU = "1"; // Solid
            }

            connector.CellsU["LineWeight"].FormulaU = "1.5 pt";
            connector.CellsU["LineColor"].FormulaU = "RGB(100, 100, 100)";

            // Set arrow endings based on cardinality
            SetCardinalityArrow(connector, "BeginArrow", rel.FromCardinality);
            SetCardinalityArrow(connector, "EndArrow", rel.ToCardinality);

            // Add relationship label
            if (!string.IsNullOrWhiteSpace(rel.Label))
            {
                connector.Text = rel.Label;
                connector.CellsU["Char.Size"].FormulaU = "9 pt";
                connector.CellsU["TxtAngle"].FormulaU = "0 deg";
                connector.CellsU["Para.HorzAlign"].FormulaU = "1"; // Center
            }
        }

        void SetCardinalityArrow(Shape line, string arrowCell, Cardinality cardinality)
        {
            // Set arrow type based on cardinality
            // 0 = none, 1 = open, 2 = filled, 3 = open, 4 = filled
            string arrowType = cardinality switch
            {
                Cardinality.ExactlyOne => "13",    // Exactly one (perpendicular line)
                Cardinality.ZeroOrOne => "22",     // Zero or one (circle + line)
                Cardinality.ZeroOrMore => "3",     // Zero or more (crow's foot)
                Cardinality.OneOrMore => "3",      // One or more (crow's foot)
                _ => "0"
            };

            line.CellsU[arrowCell].FormulaU = arrowType;
        }
    }
}
