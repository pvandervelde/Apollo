# Globals
function global:Get-ScriptLocation{
	Split-Path $MyInvocation.ScriptName
}

function global:Print-PrettyPrintHeader([string]$value){
	"=" * 15 + " " + $value + " " + "="*15
}

function global:Invoke-PsakeScript([string]$script, [String[]]$targets){

	Print-PrettyPrintHeader "Starting $script"
	""
	& invoke-psake $script $targets -noexit -showfullerror -timing -framework 4.0
	if (!$psake_buildSucceeded)
	{
		throw "$scriptName failed with return code: $LastExitCode"
	}
	
	Print-PrettyPrintHeader "Finished $script"
	""
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
	$shouldRunUnitTests = $false
	$shouldRunVerify = $false
	$shouldBuildApiDocs = $false
	$shouldBuildUserDocs = $false
	$shouldBuildInstaller = $false
	$configuration = 'debug'
	
	$tasks = New-Object System.Collections.Specialized.StringCollection
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

task UnitTest{
	Set-Variable -Name shouldRunUnitTests -Value $true -Scope 2
}

task Verify{
	Set-Variable -Name shouldRunVerify -Value $true -Scope 2
}

task ApiDoc{
	Set-Variable -Name shouldBuildApiDocs -Value $true -Scope 2
}

task UserDoc{
	Set-Variable -Name shouldBuildUserDocs -Value $true -Scope 2
}

task Install{
	Set-Variable -Name shouldBuildInstaller -Value $true -Scope 2
}

# Actual build tasks

# Clean all the generated files
task Clean -depends runScripts

# Run the developer build. Doesn't do installers and documentation
task DeveloperBuild -depends runScripts

# Runs a full build
task FullBuild -depends runScripts, assembleApiDocs, buildUserDoc, assembleInstaller, collectMetrics

###############################################################################
# HELPER TASKS


###############################################################################
# EXECUTING TASKS

task createTasks{
	$tasks.Clear()

	if ($configuration -eq 'debug')
	{ $tasks.Add('Debug') | Out-Null }
	else
	{ $tasks.Add('Release') | Out-Null }
		
	if (!$shouldClean) { $tasks.Add('Incremental') | Out-Null }
	if ($shouldRunUnitTests) { $tasks.Add('UnitTest') | Out-Null }
	if ($shouldRunVerify) { $tasks.Add('Verify') | Out-Null }
	if ($shouldBuildApiDocs) { $tasks.Add('ApiDoc') | Out-Null }
}

task runScripts -depends createTasks{
	Invoke-PsakeScript $projects['utils'] $tasks
	Invoke-PsakeScript $projects['core'] $tasks
	#Invoke-PsakeScript $projects['uicommon'] $tasks
	#Invoke-PsakeScript $projects['projectexplorer'] $tasks
	#Invoke-PsakeScript $projects['batchservice'] $tasks
	#Invoke-PsakeScript $projects['rhino'] $tasks
}

task assembleApiDocs -depends runScripts{
	"Assembling API docs..."
	
	# Collect API docs from different folders and assemble them
}

task buildUserDoc{
	"Building user docs..."
	# Build the user docs
}

task assembleInstaller -depends runScripts{
	"Assembling installer..."
	
	# Grab all the merge modules and make them into a single installer
}

task collectMetrics -depends runScripts{
	"Collecting statistics..."
	
	# collect metrics and so something with them ...
}