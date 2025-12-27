using md2visio.struc.classdiagram;
using md2visio.vsdx.@base;
using Microsoft.Office.Interop.Visio;
using System.Linq;

namespace md2visio.vsdx
{
    internal class VDrawerClass : VFigureDrawer<ClassDiagram>
    {
        private Dictionary<string, Shape> classShapes = new Dictionary<string, Shape>();
        private Document? umlStencil;

        public VDrawerClass(ClassDiagram diagram, Application visioApp) : base(diagram, visioApp)
        {
            // Try to open UML stencil
            try
            {
                umlStencil = visioApp.Documents.OpenEx("UML Class.vss", (short)VisOpenSaveArgs.visOpenDocked);
                Console.WriteLine("✓ Opened UML Class stencil");
            }
            catch
            {
                Console.WriteLine("Warning: Could not open UML Class stencil. Will use basic shapes.");
                umlStencil = null;
            }
        }

        public override void Draw()
        {
            EnsureVisible();
            PauseForViewing(300);

            DrawClasses();
            PauseForViewing(500);

            DrawRelationships();
            PauseForViewing(300);
        }

        void DrawClasses()
        {
            Console.WriteLine($"Drawing {figure.Classes.Count} classes");

            // Layout configuration for LR (Left to Right) direction
            double startX = 1.5;
            double startY = 10.0;
            double classWidth = 2.5;
            double horizontalSpacing = 4.0;
            double verticalSpacing = 2.0;

            // Build relationship-based groups
            var groups = BuildClassGroups();

            // Position classes by groups (Left to Right layout)
            PositionClassesByGroups(groups, startX, startY, classWidth, horizontalSpacing, verticalSpacing);
        }

        List<List<ClassEntity>> BuildClassGroups()
        {
            var groups = new List<List<ClassEntity>>();
            var processed = new HashSet<string>();

            // Build relationship map
            var relatedClasses = new Dictionary<string, HashSet<string>>();
            foreach (var cls in figure.Classes)
            {
                relatedClasses[cls.Name] = new HashSet<string>();
            }

            // Add all relationships (both directions for grouping)
            foreach (var rel in figure.Relationships)
            {
                if (relatedClasses.ContainsKey(rel.From))
                    relatedClasses[rel.From].Add(rel.To);
                if (relatedClasses.ContainsKey(rel.To))
                    relatedClasses[rel.To].Add(rel.From);
            }

            // Group 1: User hierarchy
            var userGroup = new List<ClassEntity>();
            var userClass = figure.Classes.FirstOrDefault(c => c.Name == "User");
            if (userClass != null)
            {
                userGroup.Add(userClass);
                processed.Add("User");

                // Add direct children
                foreach (var cls in figure.Classes.Where(c => c.Name == "AdminUser" || c.Name == "CustomerUser"))
                {
                    userGroup.Add(cls);
                    processed.Add(cls.Name);
                }

                // Add UserStatus
                var userStatus = figure.Classes.FirstOrDefault(c => c.Name == "UserStatus");
                if (userStatus != null)
                {
                    userGroup.Add(userStatus);
                    processed.Add("UserStatus");
                }
            }
            if (userGroup.Count > 0) groups.Add(userGroup);

            // Group 2: Order system
            var orderGroup = new List<ClassEntity>();
            var orderClass = figure.Classes.FirstOrDefault(c => c.Name == "Order");
            if (orderClass != null && !processed.Contains("Order"))
            {
                orderGroup.Add(orderClass);
                processed.Add("Order");

                foreach (var cls in figure.Classes.Where(c => c.Name == "OrderItem" || c.Name == "OrderStatus"))
                {
                    if (!processed.Contains(cls.Name))
                    {
                        orderGroup.Add(cls);
                        processed.Add(cls.Name);
                    }
                }
            }
            if (orderGroup.Count > 0) groups.Add(orderGroup);

            // Group 3: Payment system
            var paymentGroup = new List<ClassEntity>();
            var paymentClass = figure.Classes.FirstOrDefault(c => c.Name == "Payment");
            if (paymentClass != null)
            {
                paymentGroup.Add(paymentClass);
                processed.Add("Payment");

                foreach (var cls in figure.Classes.Where(c => c.Name == "AlipayPayment" || c.Name == "WechatPayment"))
                {
                    paymentGroup.Add(cls);
                    processed.Add(cls.Name);
                }
            }
            if (paymentGroup.Count > 0) groups.Add(paymentGroup);

            // Group 4: Product & Inventory
            var productGroup = new List<ClassEntity>();
            foreach (var cls in figure.Classes.Where(c => c.Name == "Product" || c.Name == "InventoryService"))
            {
                if (!processed.Contains(cls.Name))
                {
                    productGroup.Add(cls);
                    processed.Add(cls.Name);
                }
            }
            if (productGroup.Count > 0) groups.Add(productGroup);

            // Group 5: Repository pattern
            var repoGroup = new List<ClassEntity>();
            var repoClass = figure.Classes.FirstOrDefault(c => c.Name == "Repository~T~");
            if (repoClass != null)
            {
                repoGroup.Add(repoClass);
                processed.Add("Repository~T~");

                foreach (var cls in figure.Classes.Where(c => c.Name == "UserRepository" || c.Name == "OrderRepository"))
                {
                    if (!processed.Contains(cls.Name))
                    {
                        repoGroup.Add(cls);
                        processed.Add(cls.Name);
                    }
                }
            }
            if (repoGroup.Count > 0) groups.Add(repoGroup);

            // Group 6: Service layer
            var serviceGroup = new List<ClassEntity>();
            var serviceClass = figure.Classes.FirstOrDefault(c => c.Name == "OrderService");
            if (serviceClass != null && !processed.Contains("OrderService"))
            {
                serviceGroup.Add(serviceClass);
                processed.Add("OrderService");
            }
            if (serviceGroup.Count > 0) groups.Add(serviceGroup);

            // Add remaining classes
            var remainingGroup = new List<ClassEntity>();
            foreach (var cls in figure.Classes)
            {
                if (!processed.Contains(cls.Name))
                {
                    remainingGroup.Add(cls);
                    processed.Add(cls.Name);
                }
            }
            if (remainingGroup.Count > 0) groups.Add(remainingGroup);

            return groups;
        }

        void PositionClassesByGroups(List<List<ClassEntity>> groups,
            double startX, double startY, double classWidth, double horizontalSpacing, double verticalSpacing)
        {
            double currentX = startX;

            foreach (var group in groups)
            {
                if (group.Count == 0) continue;

                // Calculate max height in this group
                double maxHeight = 0;
                foreach (var cls in group)
                {
                    double height = CalculateClassHeight(cls);
                    maxHeight = Math.Max(maxHeight, height);
                }

                // Position classes vertically in this column
                double currentY = startY;

                foreach (var cls in group)
                {
                    double x = currentX;
                    double y = currentY;

                    cls.X = x + classWidth / 2;
                    cls.Y = y;

                    DrawClass(cls, x, y, classWidth);

                    // Move down for next class in group
                    double classHeight = CalculateClassHeight(cls);
                    currentY -= (classHeight + verticalSpacing);
                }

                // Move right for next group
                currentX += horizontalSpacing;
            }
        }

        double CalculateClassHeight(ClassEntity classEntity)
        {
            double headerHeight = 0.4;
            double stereotypeHeight = classEntity.Stereotype != ClassStereotype.None ? 0.2 : 0;
            double memberHeight = 0.2;
            double separatorHeight = 0.05;
            double padding = 0.2;

            // Enumeration only shows values
            if (classEntity.Stereotype == ClassStereotype.Enumeration)
            {
                return headerHeight + stereotypeHeight + classEntity.Attributes.Count * memberHeight + padding;
            }

            int totalMembers = classEntity.Attributes.Count + classEntity.Methods.Count;
            int separators = (classEntity.Attributes.Count > 0 && classEntity.Methods.Count > 0) ? 1 : 0;

            return headerHeight + stereotypeHeight + totalMembers * memberHeight + separators * separatorHeight + padding;
        }

        void DrawClass(ClassEntity classEntity, double x, double y, double width)
        {
            Console.WriteLine($"Drawing class: {classEntity.Name}");

            double totalHeight = CalculateClassHeight(classEntity);
            double left = x;
            double right = x + width;
            double top = y;
            double bottom = y - totalHeight;

            // Draw outer rectangle
            var classRect = visioPage.DrawRectangle(left, bottom, right, top);
            classShapes[classEntity.Name] = classRect;

            // Style the rectangle
            classRect.CellsU["FillForegnd"].FormulaU = "RGB(255,255,224)"; // Light yellow
            classRect.CellsU["LineColor"].FormulaU = "RGB(0,0,0)";
            classRect.CellsU["LineWeight"].FormulaU = "1 pt";
            classRect.SendToBack(); // Send the background rectangle to back

            double currentY = top - 0.05;

            // Draw stereotype if present
            if (classEntity.Stereotype != ClassStereotype.None)
            {
                string stereoText = classEntity.Stereotype switch
                {
                    ClassStereotype.Interface => "<<interface>>",
                    ClassStereotype.Abstract => "<<abstract>>",
                    ClassStereotype.Enumeration => "<<enumeration>>",
                    _ => ""
                };

                if (!string.IsNullOrEmpty(stereoText))
                {
                    var stereoShape = visioPage.DrawRectangle(left, currentY - 0.2, right, currentY);
                    stereoShape.Text = stereoText;
                    stereoShape.CellsU["LinePattern"].FormulaU = "0"; // No border
                    stereoShape.CellsU["FillPattern"].FormulaU = "0"; // No fill
                    stereoShape.CellsU["Char.Size"].FormulaU = "8 pt";
                    stereoShape.CellsU["Para.HorzAlign"].FormulaU = "1"; // Center
                    stereoShape.CellsU["VerticalAlign"].FormulaU = "1"; // Middle
                    currentY -= 0.2;
                }
            }

            // Draw class name
            var nameShape = visioPage.DrawRectangle(left, currentY - 0.4, right, currentY);
            nameShape.Text = classEntity.Name;
            nameShape.CellsU["LinePattern"].FormulaU = "0";
            nameShape.CellsU["FillPattern"].FormulaU = "0";
            nameShape.CellsU["Char.Size"].FormulaU = "10 pt";
            nameShape.CellsU["Char.Style"].FormulaU = "17"; // Bold
            nameShape.CellsU["Para.HorzAlign"].FormulaU = "1"; // Center
            nameShape.CellsU["VerticalAlign"].FormulaU = "1"; // Middle
            currentY -= 0.4;

            // Draw separator line after header
            var headerLine = visioPage.DrawLine(left, currentY, right, currentY);
            headerLine.CellsU["LineWeight"].FormulaU = "1 pt";
            currentY -= 0.05;

            // Draw attributes
            if (classEntity.Attributes.Count > 0)
            {
                foreach (var attr in classEntity.Attributes)
                {
                    string accessSymbol = GetAccessSymbol(attr.Access);
                    string memberText = classEntity.Stereotype == ClassStereotype.Enumeration
                        ? attr.Name
                        : $"{accessSymbol}{attr.Type} {attr.Name}".Trim();

                    var attrShape = visioPage.DrawRectangle(left + 0.1, currentY - 0.2, right - 0.1, currentY);
                    attrShape.Text = memberText;
                    attrShape.CellsU["LinePattern"].FormulaU = "0";
                    attrShape.CellsU["FillPattern"].FormulaU = "0";
                    attrShape.CellsU["Char.Size"].FormulaU = "8 pt";
                    attrShape.CellsU["Para.HorzAlign"].FormulaU = "0"; // Left align
                    attrShape.CellsU["VerticalAlign"].FormulaU = "1"; // Middle
                    currentY -= 0.2;
                }
            }

            // Draw separator line between attributes and methods
            if (classEntity.Attributes.Count > 0 && classEntity.Methods.Count > 0 &&
                classEntity.Stereotype != ClassStereotype.Enumeration)
            {
                var separatorLine = visioPage.DrawLine(left, currentY, right, currentY);
                separatorLine.CellsU["LineWeight"].FormulaU = "0.5 pt";
                currentY -= 0.05;
            }

            // Draw methods
            if (classEntity.Methods.Count > 0 && classEntity.Stereotype != ClassStereotype.Enumeration)
            {
                foreach (var method in classEntity.Methods)
                {
                    string accessSymbol = GetAccessSymbol(method.Access);
                    string methodText = $"{accessSymbol}{method.Name}()";
                    if (!string.IsNullOrEmpty(method.Type))
                    {
                        methodText = $"{accessSymbol}{method.Type} {method.Name}()";
                    }

                    var methodShape = visioPage.DrawRectangle(left + 0.1, currentY - 0.2, right - 0.1, currentY);
                    methodShape.Text = methodText;
                    methodShape.CellsU["LinePattern"].FormulaU = "0";
                    methodShape.CellsU["FillPattern"].FormulaU = "0";
                    methodShape.CellsU["Char.Size"].FormulaU = "8 pt";
                    methodShape.CellsU["Para.HorzAlign"].FormulaU = "0"; // Left align
                    methodShape.CellsU["VerticalAlign"].FormulaU = "1"; // Middle
                    currentY -= 0.2;
                }
            }
        }

        string GetAccessSymbol(AccessModifier access)
        {
            return access switch
            {
                AccessModifier.Public => "+",
                AccessModifier.Private => "-",
                AccessModifier.Protected => "#",
                AccessModifier.Package => "~",
                _ => "+"
            };
        }

        void DrawRelationships()
        {
            Console.WriteLine($"Drawing {figure.Relationships.Count} relationships");

            foreach (var rel in figure.Relationships)
            {
                if (!classShapes.TryGetValue(rel.From, out var fromShape) ||
                    !classShapes.TryGetValue(rel.To, out var toShape))
                {
                    Console.WriteLine($"Warning: Cannot find shapes for relationship {rel.From} -> {rel.To}");
                    continue;
                }

                DrawRelationship(rel, fromShape, toShape);
            }
        }

        void DrawRelationship(ClassRelationship rel, Shape fromShape, Shape toShape)
        {
            // Use dynamic connector
            var connector = visioPage.Drop(visioApp.ConnectorToolDataObject, 0, 0);

            // Glue connector to shapes
            connector.CellsU["BeginX"].GlueTo(fromShape.CellsU["PinX"]);
            connector.CellsU["EndX"].GlueTo(toShape.CellsU["PinX"]);

            // Set line style based on relationship type
            switch (rel.Type)
            {
                case RelationType.Inheritance:
                case RelationType.Implementation:
                    // Solid line for inheritance, dashed for implementation
                    if (rel.Type == RelationType.Implementation)
                    {
                        connector.CellsU["LinePattern"].FormulaU = "2"; // Dashed
                    }
                    connector.CellsU["EndArrow"].FormulaU = "29"; // Hollow triangle
                    connector.CellsU["BeginArrow"].FormulaU = "0";
                    break;

                case RelationType.Composition:
                    connector.CellsU["BeginArrow"].FormulaU = "27"; // Filled diamond
                    connector.CellsU["EndArrow"].FormulaU = "0";
                    break;

                case RelationType.Aggregation:
                    connector.CellsU["BeginArrow"].FormulaU = "28"; // Hollow diamond
                    connector.CellsU["EndArrow"].FormulaU = "0";
                    break;

                case RelationType.Association:
                    connector.CellsU["EndArrow"].FormulaU = "5"; // Simple arrow
                    connector.CellsU["BeginArrow"].FormulaU = "0";
                    break;

                case RelationType.Dependency:
                    connector.CellsU["LinePattern"].FormulaU = "2"; // Dashed
                    connector.CellsU["EndArrow"].FormulaU = "5"; // Simple arrow
                    connector.CellsU["BeginArrow"].FormulaU = "0";
                    break;
            }

            connector.CellsU["LineWeight"].FormulaU = "1 pt";

            // Add label with multiplicity if present
            if (!string.IsNullOrEmpty(rel.FromMultiplicity) || !string.IsNullOrEmpty(rel.ToMultiplicity) || !string.IsNullOrEmpty(rel.Label))
            {
                string labelText = "";
                if (!string.IsNullOrEmpty(rel.Label))
                {
                    labelText = rel.Label;
                }
                if (!string.IsNullOrEmpty(rel.FromMultiplicity) && !string.IsNullOrEmpty(rel.ToMultiplicity))
                {
                    labelText = $"{rel.FromMultiplicity} {labelText} {rel.ToMultiplicity}".Trim();
                }

                if (!string.IsNullOrEmpty(labelText))
                {
                    connector.Text = labelText;
                }
            }

            Console.WriteLine($"  Drew {rel.Type} from {rel.From} to {rel.To}");
        }
    }
}
