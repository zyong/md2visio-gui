using md2visio.struc.sequence;
using md2visio.vsdx.@base;
using Microsoft.Office.Interop.Visio;

namespace md2visio.vsdx
{
    internal class VDrawerSe : VFigureDrawer<Sequence>
    {
        public VDrawerSe(Sequence figure, Application visioApp) : base(figure, visioApp)
        {
        }

        public override void Draw()
        {
            EnsureVisible();
            PauseForViewing(300);

            DrawParticipants();
            PauseForViewing(500);

            DrawMessages();
            PauseForViewing(300);
        }

        void DrawParticipants()
        {
            Console.WriteLine("Drawing participants: " + figure.Participants.Count);
            double x = 1.5; // start x in inches (increased for better spacing)
            double topY = 10.0; // top of the diagram
            double actorBoxHeight = 0.5; // height of actor box
            double actorBoxWidth = 1.2; // width of actor box

            // Calculate lifeline length based on number of messages
            // Each message takes 0.5 inches, plus 1 inch padding at bottom
            double messageSpacing = 0.5;
            double lifelineLength = Math.Max(8.0, figure.Messages.Count * messageSpacing + 1.0);
            Console.WriteLine($"Calculated lifeline length: {lifelineLength} inches for {figure.Messages.Count} messages");

            foreach (var participant in figure.Participants)
            {
                Console.WriteLine($"Drawing participant: {participant.Name}");

                // 1. Draw actor box (rectangle) at the top
                double boxLeft = x - actorBoxWidth / 2;
                double boxRight = x + actorBoxWidth / 2;
                double boxTop = topY;
                double boxBottom = topY - actorBoxHeight;

                var actorBox = visioPage.DrawRectangle(boxLeft, boxBottom, boxRight, boxTop);
                actorBox.Text = participant.Alias;

                // Style the actor box
                actorBox.CellsU["FillForegnd"].FormulaU = "RGB(230, 240, 255)"; // Light blue fill
                actorBox.CellsU["LineWeight"].FormulaU = "1.5 pt"; // Border weight
                actorBox.CellsU["LineColor"].FormulaU = "RGB(0, 0, 0)"; // Black border

                // Center text
                actorBox.CellsU["Para.HorzAlign"].FormulaU = "1"; // Center horizontal
                actorBox.CellsU["VerticalAlign"].FormulaU = "1"; // Center vertical

                // 2. Draw lifeline (dashed vertical line) from bottom of actor box
                double lifelineTop = boxBottom;
                double lifelineBottom = topY - actorBoxHeight - lifelineLength;

                var lifeline = visioPage.DrawLine(x, lifelineTop, x, lifelineBottom);

                // Style the lifeline as dashed
                lifeline.CellsU["LinePattern"].FormulaU = "2"; // Dashed line
                lifeline.CellsU["LineWeight"].FormulaU = "0.5 pt"; // Thin line
                lifeline.CellsU["LineColor"].FormulaU = "RGB(100, 100, 100)"; // Gray color

                // 3. Draw actor box at the bottom (mirror) - optional but common in UML
                double bottomBoxTop = lifelineBottom;
                double bottomBoxBottom = lifelineBottom - actorBoxHeight;

                var bottomActorBox = visioPage.DrawRectangle(boxLeft, bottomBoxBottom, boxRight, bottomBoxTop);
                bottomActorBox.Text = participant.Alias;

                // Style the bottom actor box (same as top)
                bottomActorBox.CellsU["FillForegnd"].FormulaU = "RGB(230, 240, 255)";
                bottomActorBox.CellsU["LineWeight"].FormulaU = "1.5 pt";
                bottomActorBox.CellsU["LineColor"].FormulaU = "RGB(0, 0, 0)";
                bottomActorBox.CellsU["Para.HorzAlign"].FormulaU = "1";
                bottomActorBox.CellsU["VerticalAlign"].FormulaU = "1";

                // Store position for messages (center of lifeline)
                participant.X = x;

                // Move to next participant position
                x += 2.5; // increased spacing between participants
            }
        }

        void DrawMessages()
        {
            Console.WriteLine("Drawing messages: " + figure.Messages.Count);
            double y = 9.0; // start y from top
            foreach (var message in figure.Messages)
            {
                Console.WriteLine("Message: " + message.From + " " + message.Type + " " + message.To + " : " + message.Text);
                // Check both Name and Alias fields to handle "participant X as Y" syntax
                var fromPart = figure.Participants.FirstOrDefault(p => p.Name == message.From || p.Alias == message.From);
                var toPart = figure.Participants.FirstOrDefault(p => p.Name == message.To || p.Alias == message.To);
                if (fromPart == null || toPart == null)
                {
                    Console.WriteLine($"  ! Skipping message: fromPart={fromPart?.Name}, toPart={toPart?.Name}");
                    continue;
                }

                double x1 = fromPart.X;
                double x2 = toPart.X;
                var line = visioPage.DrawLine(x1, y, x2, y);

                // Set arrow style based on type
                if (message.Type == MessageType.Solid)
                {
                    line.CellsU["EndArrow"].FormulaU = "2"; // solid arrow
                }
                else if (message.Type == MessageType.Dashed)
                {
                    line.CellsU["LinePattern"].FormulaU = "2"; // dashed
                    line.CellsU["EndArrow"].FormulaU = "2";
                }
                else
                {
                    line.CellsU["EndArrow"].FormulaU = "1"; // open arrow
                }

                // Add text
                if (!string.IsNullOrEmpty(message.Text))
                {
                    line.Text = message.Text;

                    // Fix text orientation - always horizontal, never upside down
                    // TxtAngle: 0 degrees = horizontal text
                    line.CellsU["TxtAngle"].FormulaU = "0 deg";

                    // For lines going right-to-left (x1 > x2), flip the text block
                    if (x1 > x2)
                    {
                        // Flip text to read left-to-right even on reversed arrows
                        line.CellsU["FlipX"].FormulaU = "TRUE";
                    }

                    // Position text above the line
                    line.CellsU["Para.HorzAlign"].FormulaU = "1"; // Center horizontal
                }

                y -= 0.5; // next message lower
            }
        }
    }
}