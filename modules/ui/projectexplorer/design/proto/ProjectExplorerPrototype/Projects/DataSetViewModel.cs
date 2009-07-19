using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectExplorerPrototype.Projects
{
    public class DataSetViewModel
    {
        private readonly List<DataSetViewModel> m_Children = new List<DataSetViewModel>();

        private string m_Name;

        private string m_Description;

        public DataSetViewModel()
        { }

        public DataSetViewModel(DataSetViewModel child)
        {
            m_Children.Add(child);
        }

        public DataSetViewModel(IEnumerable<DataSetViewModel> children)
        {
            m_Children.AddRange(children);
        }

        public DataSetViewModel(params DataSetViewModel[] children)
        {
            m_Children.AddRange(children);
        }

        public void Add(DataSetViewModel newChild)
        {
            m_Children.Add(newChild);
        }

        public List<DataSetViewModel> Children 
        { 
            get 
            { 
                return m_Children; 
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

        public string Description
        {
            get 
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
            }
        }

        public bool IsRunning
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }

        public ProcessState ProcessState
        {
            get;
            set;
        }

        public LockState LockState
        {
            get;
            set;
        }

        public int PercentComplete
        {
            get;
            set;
        }
    }
}
