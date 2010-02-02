using System;
using System.Windows;

namespace ContactManager.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        public void GarbageCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void CloseApplication()
        {
            Application.Current.Shutdown();
        }
    }
}