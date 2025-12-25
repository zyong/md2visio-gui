using md2visio.struc.sequence;
using md2visio.vsdx.@base;

namespace md2visio.vsdx
{
    internal class VBuilderSe(Sequence figure) : VFigureBuilder<Sequence>(figure)
    {
        override protected void ExecuteBuild() 
        {
            new VDrawerSe(figure, VisioApp).Draw();
        }        
    }
}