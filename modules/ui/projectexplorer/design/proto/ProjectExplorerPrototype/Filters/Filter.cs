using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;

namespace ProjectExplorerPrototype.Filters
{
    internal sealed class ChangeActiveStateCommand : ICommand
    {
        private readonly Filter m_Owner;

        public ChangeActiveStateCommand(Filter owner)
        {
            m_Owner = owner;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        private void RaiseCanExecuteChanged()
        {
            EventHandler local = CanExecuteChanged;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        public void Execute(object parameter)
        {
            m_Owner.ChangeActiveState();
        }
    }

    public abstract class Filter : INotifyPropertyChanged
    {
        private readonly ChangeActiveStateCommand m_Command;
        private string m_Name;
        private bool m_IsActive;

        protected Filter() : this(string.Empty)
        { }

        protected Filter(string name) : this(name, true)
        { }

        protected Filter(bool isActive) : this(string.Empty, isActive)
        { }

        protected Filter(string name, bool isActive)
        {
            m_Command = new ChangeActiveStateCommand(this);

            m_Name = name;
            m_IsActive = isActive;
        }

        public void ChangeActiveState()
        {
            m_IsActive = !m_IsActive;
            RaisePropertyChanged("IsActive");
        }

        public ICommand ChangeActiveStateCommand {get {return m_Command; } }

        public string Name 
        { 
            get
            {
                return m_Name;
            } 
            set
            {
                m_Name=value;
                RaisePropertyChanged("Name");
            } 
        }

        public bool IsActive 
        { 
            get
            {
                return m_IsActive;
            } 
        }
    
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler local = PropertyChanged;
            if (local != null)
            {
                local(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
