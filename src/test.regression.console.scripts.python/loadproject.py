# The following variables are already present, courtesy of the environment
# - projects: An object of type ILinkScriptsToProjects
# - scriptCancellationToken: An object of type CancellationToken

# $Source$
import clr
clr.AddReference("Apollo.Core.Scripting")

import Apollo.Core.Scripting

# Verify that we don't have an active project
hasactiveproject = projects.HasActiveProject()
if hasActiveProject:
    raise Exception('Project already loaded')

# Create a project
projectpath = ""
newproject = projects.LoadProject(projectpath)

# Verify that the project was created

# verify that we can change the name

# verify that we can change the description