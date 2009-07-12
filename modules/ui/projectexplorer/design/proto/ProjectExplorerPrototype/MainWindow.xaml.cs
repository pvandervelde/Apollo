using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Annotations;
using SpiderTreeControl.Diagram;
using BarberBornander.UI.Physics;
using BarberBornander.UI.Physics.SpringRenderers;
using ProjectExplorerPrototype.Filters;
using ProjectExplorerPrototype.Projects;
using System.Diagnostics;

namespace ProjectExplorerPrototype
{
    /// <summary>
    /// Interaction logic for the main window.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FilterCollection m_Filters = new FilterCollection();

        private List<DataSetViewModel> m_Project;

        public FilterCollection Filters { get { return m_Filters; } }

        public List<DataSetViewModel> Project 
        { 
            get 
            {
                return m_Project;
            } 
        }

        /// <summary>
        /// Constructor for the main window
        /// </summary>
        public MainWindow()
        {
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(new DefaultTraceListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.All;

            m_Filters.Add(new IsRunningFilter());
            m_Filters.Add(new IsSystemGeneratedFilter());

            DataSetViewModel child = null;
            DataSetViewModel parent = null;
            for (int i = 0; i < 10; i++)
            {
                parent = new DataSetViewModel(child);
                parent.Name = "This is child: " + (10 - i).ToString();
                parent.Description = "This is the description for child:" + (10 - i).ToString();

                child = parent;
            }

            var projectData = new DataSetViewModel(parent);
            projectData.Name = "This is the project data.";
            projectData.Description = "This is the description for the project data ... bla bla bla bla";

            m_Project = new List<DataSetViewModel>();
            m_Project.Add(projectData);

            InitializeComponent();

            
        }

    }
}
