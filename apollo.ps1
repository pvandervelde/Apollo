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

function global:Unzip-Files([string]$file, [string]$targetDirectory){
	"Uncompressing..."
	
	if (!(Test-Path -Path $targetDirectory -PathType Container))
	{
		New-Item $targetDirectory -ItemType directory | Out-Null # Don't display the directory information
	}
	
	$currentDir = $PWD
	try
	{
		sl $targetDirectory
		
		# zip the hudson temp dir
		$7zipExe = "$Env:ProgramW6432\7-Zip\7z.exe"
		$command = '& $7zipExe x ' + '"' + $file + '"'
		$command
		Invoke-Expression $command
		if ($LastExitCode -ne 0)
		{
			throw "Failed to uncompress the binaries in $file."
		}
	}
	finally
	{
		sl $currentDir
	}
}

# Properties
properties{
	$dirBase = Get-ScriptLocation

	# solution directories
	$dirBin = Join-Path $dirBase 'bin'
	$dirDeploy = Join-Path $dirBin 'deploy'
	$dirInstall = Join-Path $dirBase 'install'

	# Modules directories
	$dirModules = Join-Path $dirBase 'modules'
	$dirModuleCore = Join-Path $dirModules 'core'
	$dirModulesUiCommon = Join-Path $dirModules (Join-Path 'ui' 'common')
	$dirModulesUiProjectExplorer = Join-Path $dirModules (Join-Path 'Ui' 'projectexplorer')
	$dirModulesUiBatchService = Join-Path $dirModules (Join-Path 'ui' 'batchservice')
	$dirModulesUiRhino = Join-Path $dirModules (Join-Path 'ui' 'rhino')
	$dirModulesUtils = Join-Path $dirModules 'utils'
	
	# Default paths
	$pathDefaultDeploy = 'bin\release'

	# projects
	$projects = @{
					'core'= Join-Path $dirModuleCore 'core.ps1'; 
					'uicommon' = Join-Path $dirModulesUiCommon 'ui.common.ps1';
					'projectexplorer' = Join-Path $dirModulesUiProjectExplorer 'projectexplorer.ps1';
					'batchservice' = Join-Path $dirModulesUiBatchService 'batchservice.ps1';
					'rhino' = Join-Path $dirModulesUiRhino 'rhino.ps1';
					'utils' = Join-Path $dirModulesUtils 'utils.ps1';
				 }
	
	# output files
	$logMsiBuild = 'msi.log'
	
	# settings
	# Version number
	$versionNumber = New-Object -TypeName System.Version -ArgumentList "1.0.0.0"
	$versionFile = Join-Path $dirBase 'Version.xml'	
	
	# script-wide variables
	$shouldClean = $true
	$shouldRunUnitTests = $false
	$shouldRunVerify = $false
	$shouldBuildApiDocs = $false
	$shouldBuildUserDocs = $false
	$shouldBuildInstaller = $false
	$shouldDeployToTest = $false
	$configuration = 'debug'
	
	# internal variables
	$tasks = New-Object System.Collections.Generic.List``1[System.String]
}

# The default task doesn't do anything. This just calls the help function. Useful
#   for new people
task default -depends Help

# Configuration tasks
task Incremental -action{
	Set-Variable -Name shouldClean -Value $false -Scope 2
}

task Debug -action{
	Set-Variable -Name configuration -Value 'debug' -Scope 2
}

task Release -action{
	Set-Variable -Name configuration -Value 'release' -Scope 2
}

task UnitTest -action{
	Set-Variable -Name shouldRunUnitTests -Value $true -Scope 2
}

task Verify -action{
	Set-Variable -Name shouldRunVerify -Value $true -Scope 2
}

task ApiDoc -action{
	Set-Variable -Name shouldBuildApiDocs -Value $true -Scope 2
}

task UserDoc -action{
	Set-Variable -Name shouldBuildUserDocs -Value $true -Scope 2
}

task Install -action{
	Set-Variable -Name shouldBuildInstaller -Value $true -Scope 2
}

task DeployToTest -action{
	Set-Variable -Name shouldDeployToTest -Value $true -Scope 2
}

# Actual build tasks

# Clean all the generated files
task Clean -depends runClean, runScripts

# Run the build completely
task Build -depends runClean, runScripts, assembleApiDocs, buildUserDoc, assembleInstaller, deployToTestDirectory, collectMetrics

###############################################################################
# HELPER TASKS


###############################################################################
# EXECUTING TASKS

# The Help task displays the available commandline arguments
task Help -action{
@"
In order to run this build script please call a specific target.
The following build tasks are available
	'incremental':		Turns on the incremental building of the binaries
	'debug':			Switches the script to debug mode. Mutually exclusive with the 'release' task
	'release':			Switches the script to release mode. Mutually exclusive with the 'debug' task
	'unittest':			Turns on the unit testing of the binaries.
	'verify':			Turns on the verification of the binaries. 
	'apidoc':			Turns on the generation of the API documentation.
	'userdoc':			Turns on the generation of the user documentation.
	'install':			Turns on the generation of the installer package.
	'deploytotest':		Turns on the deployment to the test directory.
	
	'Clean':			Cleans the output directories.
	'Build':			Builds the binaries with the options given by the user.

Multiple build tasks can be specified separated by a comma. Also build tasks can be combined 
in any order. In most cases the build script will ensure that the tasks are executed in the
correct order. Note that this is NOT the case for the 'Clean' and 'Build' tasks.
In order to get a correct effect these tasks need to be the last tasks being called!
       
In order to run this build script please call this script via PSAKE like:
	Invoke-psake build.ps1 incremental,debug,unittest,verify,apidoc,userdoc,install,build -framework 4.0 -timing
"@
}

task runClean  -precondition{ $shouldClean } -action{
	"Cleaning..."

	# Clean the bin dir
	if (Test-Path -Path $dirBin -PathType Container)
	{
		"Removing the bin directory..."
		Remove-Item $dirBin -Force -Recurse
	}
}

task createTasks -action{
	$tasks.Clear()

	if ($configuration -eq 'debug') { $tasks.Add('Debug') | Out-Null }
	if ($configuration -eq 'release') { $tasks.Add('Release') | Out-Null }

	if (!$shouldClean) { $tasks.Add('Incremental') | Out-Null }
	$tasks.Add('Clean') | Out-Null
	$tasks.Add('Build') | Out-Null
	
	if ($shouldRunUnitTests) { $tasks.Add('UnitTest') | Out-Null }
	if ($shouldRunVerify) { $tasks.Add('Verify') | Out-Null }
	if ($shouldBuildApiDocs) { $tasks.Add('ApiDoc') | Out-Null }
	if ($shouldDeployToTest) { $tasks.Add('Package') | Out-Null }
}

task runScripts -depends createTasks -action{
	Invoke-PsakeScript $projects['utils'] $tasks
	Invoke-PsakeScript $projects['core'] $tasks
	Invoke-PsakeScript $projects['uicommon'] $tasks
	Invoke-PsakeScript $projects['projectexplorer'] $tasks
	#Invoke-PsakeScript $projects['batchservice'] $tasks
	#Invoke-PsakeScript $projects['rhino'] $tasks
}

task createTestDirectory -action{
	"Creating the test directory..."
	
	if (!(Test-Path -Path $dirBin -PathType Container))
	{
		New-Item $dirBin -ItemType directory | Out-Null # Don't display the directory information
	}
}

task assembleApiDocs -depends runScripts -precondition{ return $shouldBuildApiDocs } -action{
	"Assembling API docs..."
	
	# Collect API docs from different folders and assemble them
}

task buildUserDoc -precondition{ return $shouldBuildUserDocs } -action{
	"Building user docs..."
	# Build the user docs
}

task assembleInstaller -depends runScripts -precondition{ return $shouldBuildInstaller } -action{
	"Assembling installer..."
	
	# Grab all the merge modules and make them into a single installer
	# Installers are created per UI. Each UI will have a different installer?
	
}

task deployToTestDirectory -depends runScripts,createTestDirectory -precondition{ return $shouldDeployToTest } -action{
	"Deploying to the test directory ..."
	
	# copy all the zip files to the test directory
	$utils = Get-ChildItem -Path (Join-Path $dirModulesUtils $pathDefaultDeploy) | Where-Object { ($_.Extension -match ".zip") }
	foreach ($file in $utils){
		Unzip-Files $file.FullName $dirDeploy
	}
	
	# copy all the zip files to the test directory
	$core = Get-ChildItem -Path (Join-Path $dirModuleCore $pathDefaultDeploy) |	Where-Object { ($_.Extension -match ".zip") }
	foreach ($file in $core){
		Unzip-Files $file.FullName $dirDeploy
	}
	
	# copy all the zip files to the test directory
	$uiCommon = Get-ChildItem -Path (Join-Path $dirModulesUiCommon $pathDefaultDeploy) | Where-Object { ($_.Extension -match ".zip") }
	foreach ($file in $uiCommon){
		Unzip-Files $file.FullName $dirDeploy
	}
	
	# copy all the zip files to the test directory
	$projectExplorer = Get-ChildItem -Path (Join-Path $dirModulesUiProjectExplorer $pathDefaultDeploy) | Where-Object { ($_.Extension -match ".zip") }
	foreach ($file in $projectExplorer){
		Unzip-Files $file.FullName $dirDeploy
	}
}

task collectMetrics -depends runScripts{
	"Collecting statistics..."
	
	# collect metrics and so something with them ...
}