using md2visio.struc.er;
using md2visio.vsdx.@base;

namespace md2visio.vsdx
{
    internal class VBuilderEr : VFigureBuilder<ErDiagram>
    {
        public VBuilderEr(ErDiagram diagram) : base(diagram)
        {
        }

        protected override void ExecuteBuild()
        {
            var drawer = new VDrawerEr(figure, VisioApp);
            drawer.Draw();
        }
    }
}
