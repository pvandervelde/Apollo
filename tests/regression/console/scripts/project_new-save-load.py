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

# verify that we can set the name
newname = 'project_newname'
newproject.Name = newname
if newproject.Name != newname:
    print 'Fail: Project name was not updated'
    sys.exit(1)
else:
    print 'Pass: Project name updated successfully'

# verify that we can set the description
newsummary = 'project_newsummary'
newproject.Summary = newsummary
if newproject.Summary != newsummary:
    print 'Fail: Project summary was not updated'
    sys.exit(1)
else:
    print 'Pass: Project summary updated successfully'

# Save the project

# close the current project
projects.UnloadProject()
if projects.HasActiveProject():
    print 'Fail: Could not unload project'
    sys.exit(1)
else:
    print 'Pass: Project unloaded successfully'

# load the saved project

# Verify that everything matches