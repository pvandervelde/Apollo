# Globals
function global:Get-ScriptLocation{
	Split-Path $MyInvocation.ScriptName
}

# Properties
properties{
	$dirBase = Get-ScriptLocation

	# solution directories
	$dirBin = Join-Path $dirBase 'bin'
	$dirInstall = Join-Path $dirBase 'install'

	# Modules directories
	$dirModules = Join-Path $dirBase 'modules'
	$dirModuleCore = Join-Path $dirModules 'core'
	$dirModulesUiCommon = Join-Path $dirModules (Join-Path 'ui' 'common')
	$dirModulesUiProjectExplorer = Join-Path $dirModules (Join-Path 'Ui' 'projectexplorer')
	$dirModulesUiBatchService = Join-Path $dirModules (Join-Path 'ui' 'batchservice')
	$dirModulesUiRhino = Join-Path $dirModules (Join-Path 'ui' 'rhino')
	$dirModulesUtils = Join-Path $dirModules 'utils'

	# projects
	$projects = @{
					'core'= Join-Path $dirModuleCore 'core.ps1'; 
					'uicommon' = Join-Path $dirModulesUiCommon 'common.ps1';
					'projectexplorer' = Join-Path $dirModulesUiProjectExplorer 'projectexplorer.ps1';
					'batchservice' = Join-Path $dirModulesUiBatchService 'batchservice.ps1';
					'rhino' = Join-Path $dirModulesUiRhino 'rhino.ps1';
					'utils' = Join-Path $dirModulesUtils 'utils.ps1';
					
				 }
	
	# script-wide variables
	$shouldClean = $true
	$shouldBuildApiDocs = $false
	$shouldBuildUserDocs = $false
	$shouldBuildInstaller = $false
	$configuration = 'debug'
}

# Configuration tasks
task Incremental{
	Set-Variable -Name shouldClean -Value $false -Scope 2
}

task Debug{
	Set-Variable -Name configuration -Value 'debug' -Scope 2
}

task Release{
	Set-Variable -Name configuration -Value 'release' -Scope 2
}

task ApiDocs{
	Set-Variable -Name shouldBuildApiDocs -Value $true -Scope 2
}

task UserDocs{
	Set-Variable -Name shouldBuildUserDocs -Value $true -Scope 2
}

task Installer{
	Set-Variable -Name shouldBuildInstaller -Value $true -Scope 2
}

# Actual build tasks

# Clean all the generated files
task Clean -depends runClean

# Run the build
task Build -depends assembleApiDocs, buildUserDoc, assembleInstaller, collectMetrics

###############################################################################
# HELPER TASKS


###############################################################################
# EXECUTING TASKS

task runClean{
	"Cleaning..."
	
}

task runInit -depends runClean{
	"Initializing build..."
}

task buildBinaries -depends runInit{
	"Calling module build scripts..."
}

task assembleApiDocs -depends buildBinaries{
	"Assembling API docs..."
}

task buildUserDoc{
}

task assembleInstaller -depends buildBinaries{
	"Assembling installer..."
}

task collectMetrics -depends buildBinaries{
	"Collecting statistics..."
}