using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Ui.Common.ViewModels
{
    /// <summary>
    /// Defines the interface for view models that hide <c>IProcessableData</c> objects.
    /// </summary>
    public interface IProcessableDataViewModel
    { 
        // Must have:
        // - Progress notification
        // - Progress state notification: Not running, running, paused, cancelled, error, finished
        // - Actions: Start, Pause, Stop
    }
}
