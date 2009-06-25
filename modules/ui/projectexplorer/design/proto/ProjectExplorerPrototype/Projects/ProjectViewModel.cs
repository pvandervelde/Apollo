using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectExplorerPrototype.Projects
{
    public class ProjectViewModel
    {
        private readonly DataSetViewModel m_ProjectData;

        private string m_Name;

        public ProjectViewModel(DataSetViewModel projectData)
        {
            m_ProjectData = projectData;
        }

        public DataSetViewModel ProjectData
        {
            get 
            {
                return m_ProjectData;
            }
        }

        public string Name
        {
            get 
            {
                return m_Name;
            }
            set 
            {
                m_Name = value;
            }
        }
    }
}
