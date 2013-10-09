# The following variables are already present, courtesy of the hosting environment
# - projects: An object of type ILinkScriptsToProjects
# - scriptCancellationToken: An object of type CancellationToken

# $Source$
import clr
clr.AddReference("System")
clr.AddReference("System.Core")
clr.AddReference("Apollo.Core.Scripting")
clr.AddReference("Apollo.Core.Base")
clr.AddReference("System.Management")

import sys

import System.Diagnostics
import System.Threading
import Apollo.Core.Base
import Apollo.Core.Base.Activation
import Apollo.Core.Scripting

from System.Diagnostics import Process
from System.Management import (
    ManagementObject,
    ManagementObjectSearcher
)
from System.Threading import AutoResetEvent
from Apollo.Core.Base.Activation import DistributionLocations
from Apollo.Core.Scripting.Projects import DistributionSuggestionProxy

def SelectFrom(options):
    return options[0]

activateevent = AutoResetEvent(False)
def switchonactivate(sender, eventargs):
    activateevent.Set()

deactivateevent = AutoResetEvent(False)
def switchondeactivate(sender, eventargs):
    deactivateevent.Set()

# Verify that we don't have an active project
hasactiveproject = projects.HasActiveProject()
if hasactiveproject:
    print 'Fail: Active project already exists'
    sys.exit(1)

# Create a project
try:
    projects.NewProject()
except Exception, e:
    print 'Fail: Could not create new project'
    sys.exit(1)

# Verify that the project was created
newproject = projects.ActiveProject()
if newproject is None:
    print 'Fail: No new project was created'
    sys.exit(1)
else:
    print 'Pass: New project created successfully'

# Create a sub-dataset
rootdataset = newproject.Root()
if rootdataset is None:
    print 'Fail: No root dataset was created'
    sys.exit(1)

childdataset = rootdataset.AddChild()
if childdataset is None:
    print 'Fail: No child dataset was created'
    sys.exit(1)
else:
    print 'Pass: New child dataset created'

# verify that we can set the name
newname = 'dataset_newname'
childdataset.Name = newname
if childdataset.Name != newname:
    print 'Fail: Dataset name was not updated'
else:
    print 'Pass: Dataset name updated successfully'

# verify that we can set the description
newsummary = 'dataset_newsummary'
childdataset.Summary = newsummary
if childdataset.Summary != newsummary:
    print 'Fail: Dataset summary was not updated'
else:
    print 'Pass: Dataset summary updated successfully'

# Activate dataset on local machine
processid = 0
try:
    childdataset.OnActivated += switchonactivate
    childdataset.Activate(DistributionLocations.Local, SelectFrom)

    # Wait for the activation event to fire
    activateevent.WaitOne(2 * 60 * 1000)

    parentprocessid = Process.GetCurrentProcess().Id
    mos = ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Process WHERE ParentProcessId = " + parentprocessid.ToString())
    for result in mos.Get():
        processid = result.GetPropertyValue("ProcessId")

    if processid != 0:
        print 'Pass: Dataset was activated successfully'
    else:
        print 'Fail: Dataset was not activated'
        sys.exit(1)
except System.Exception, e:
    print 'Fail: Dataset activation threw exception'
    print e.ToString()
    sys.exit(1)

# Deactivate dataset on local machine
try:
    childdataset.OnDeactivated += switchondeactivate

    # Grab the child process so that we can watch it
    process = Process.GetProcessById(processid)

    # Deactivate it
    childdataset.Deactivate()

    # Wait for the deactivate event
    deactivateevent.WaitOne(2 * 60 * 1000)

    # See if the child process is gone
    if not process.HasExited:
        process.WaitForExit(2 * 60 * 1000)

    if not process.HasExited:
        print 'Fail: Dataset was not deactivated'
        sys.exit(1)
    else:
        print 'Pass: Dataset was deactivated successfully'
except System.Exception, e:
    print 'Fail: Dataset deactivation threw exception'
    print e.ToString()
    sys.exit(1)

# Save

# Close
projects.UnloadProject()
if projects.HasActiveProject():
    print 'Fail: Could not unload project'
    sys.exit(1)
else:
    print 'Pass: Project unloaded successfully'

# Reload from disk

# Activate dataset on remote machine