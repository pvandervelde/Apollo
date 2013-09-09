# The following variables are already present, courtesy of the hosting environment
# - projects: An object of type ILinkScriptsToProjects
# - scriptCancellationToken: An object of type CancellationToken

# $Source$
import clr
clr.AddReference("Apollo.Core.Scripting")

import sys
import Apollo.Core.Scripting

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

# Save

# Close
projects.UnloadProject()
if projects.HasActiveProject():
    print 'Fail: Could not unload project'
    sys.exit(1)
else:
    print 'Pass: Project unloaded successfully'

# Reload from disk

# Activate dataset on local machine

# Activate dataset on remote machine