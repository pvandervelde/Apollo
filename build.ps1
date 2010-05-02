# this is the normal build script. This will call the psake tool to run the build

# Globals
function Get-ScriptLocation{
	Split-Path $MyInvocation.ScriptName
}

function Get-ApolloScriptFullPath{
	$scriptPath = Get-ScriptLocation
	Join-Path $scriptPath 'apollo.ps1'
}

function Get-PsakePath{
	Join-Path (Join-Path (Get-ScriptLocation) 'tools') 'psake'
}

function Build-DebugDev{
	Import-Psake (Get-PsakePath)
	$script = Get-ApolloScriptFullPath

	'Running debug developer build'
	"Running script from: $script"
	& invoke-psake $script Debug,UnitTests,Verify,DeveloperBuild 4.0
}

function Build-ReleaseDev{
	Import-Psake (Get-PsakePath)
	$script = Get-ApolloScriptFullPath

	'Running release developer build'
	'Running script from: $script'
	& invoke-psake $script Release,UnitTests,Verify,DeveloperBuild 4.0
}

function Build-DebugFull{
	Import-Psake (Get-PsakePath)
	$script = Get-ApolloScriptFullPath

	'Running debug full build'
	'Running script from: $script'
	& invoke-psake $script Debug,UnitTests,Verify,ApiDocs,UserDocs,Installer,FullBuild 4.0
}

function Build-ReleaseFull{
	Import-Psake (Get-PsakePath)
	$script = Get-ApolloScriptFullPath

	'Running release full build'
	'Running script from: $script'
	& invoke-psake $script Release,UnitTests,Verify,ApiDocs,UserDocs,Installer,FullBuild 4.0
}

function Import-Psake([string] $psakeDirectory){
	# See if psake is loaded. If it is we unload it and reload the correct
	# one. We do this because that's currently the only way to guarantuee
	# that we get the right version of the psake module
	$modules = Get-Module
	foreach ($module in $modules) {
		if ($module.Name -eq 'psake'){
			$path = Split-Path $module.Path
			if ($path -eq $psakeDirectory)
			{
				'Correct version of Psake found. Done loading.'
				return
			}
			
			# Found a psake but it was not the correct version
			# so unload it
			"Found incorrect version of Psake at path: " + $module.Path
			"Unloading ..."
			Remove-Module -ModuleInfo ($module)
		}
	}
	
	'Psake not found, importing...'
	
	# import the psake module if we didn't find it
	Import-Module (Join-Path $psakeDirectory 'psake.psm1')
	$loadedModule = Get-Module -Name psake
	"Loaded psake from: " + $loadedModule.Path
}