# this is the normal build script. This will call the psake tool to run the build

# Globals
function Get-ScriptLocation{
	Split-Path $MyInvocation.ScriptName
}

function Get-ApolloScriptFullPath{
	$scriptPath = Get-ScriptLocation
	Join-Path $scriptPath 'apollo.ps1'
}

function Build-DebugDev{
	$script = Get-ApolloScriptFullPath

	'Running debug developer build'
	"Running script from: $script"
	& invoke-psake $script -noexit -showfullerror -timing -framework 4.0 Debug,UnitTests,Verify,DeveloperBuild
}

function Build-ReleaseDev{
	$script = Get-ApolloScriptFullPath

	'Running release developer build'
	'Running script from: $script'
	& invoke-psake $script -noexit -showfullerror -timing -framework 4.0 Release,UnitTests,Verify,DeveloperBuild | Out-Host
}

function Build-DebugFull{
	$script = Get-ApolloScriptFullPath

	'Running debug full build'
	'Running script from: $script'
	& invoke-psake $script -noexit -showfullerror -timing -framework 4.0 Debug,UnitTests,Verify,ApiDocs,UserDocs,Installer,FullBuild | Out-Host
}

function Build-ReleaseFull{
	$script = Get-ApolloScriptFullPath

	'Running release full build'
	'Running script from: $script'
	& invoke-psake $script -noexit -showfullerror -timing -framework 4.0 Release,UnitTests,Verify,ApiDocs,UserDocs,Installer,FullBuild
}

'Checking existence of PSake scripts...'
# See if psake is loaded
$modules = Get-Module
foreach ($module in $modules) {
	if ($module.Name -eq 'psake'){
		'Psake found'
		return
	}
}

'Psake not found, importing...'

# import the psake module if we didn't find it
Import-Module Psake

'Psake imported'