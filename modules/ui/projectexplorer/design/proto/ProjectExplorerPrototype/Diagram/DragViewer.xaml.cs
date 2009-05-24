using System.Windows.Controls;

namespace SpiderTreeControl.Diagram
{
    /// <summary>
    /// Interaction logic for DragViewer.xaml
    /// </summary>
    public partial class DragViewer : UserControl
    {

        public DragViewer()
        {
            InitializeComponent();
            diagramViewer.FrictionScrollViewer = this.sv;
        }

        public DiagramNode Root
        {
            set 
            {
                diagramViewer.RootNode = value;
            }
        }
    }
}
