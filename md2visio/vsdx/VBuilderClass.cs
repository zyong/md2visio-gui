using md2visio.struc.classdiagram;
using md2visio.vsdx.@base;

namespace md2visio.vsdx
{
    internal class VBuilderClass : VFigureBuilder<ClassDiagram>
    {
        public VBuilderClass(ClassDiagram diagram) : base(diagram)
        {
        }

        protected override void ExecuteBuild()
        {
            var drawer = new VDrawerClass(figure, VisioApp);
            drawer.Draw();
        }
    }
}
