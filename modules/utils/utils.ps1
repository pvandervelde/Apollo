# Globals
function global:Get-ScriptLocation{
	Split-Path $MyInvocation.ScriptName
}

function global:Get-MsBuildExe
{
	'msbuild'
}

function global:Get-BzrExe{
	'bzr'
}

function global:Get-BzrVersion{
	$bzrPath = Get-BzrExe
	
	# Get the bzr output in xml format
	$output = & $bzrPath version-info
	$versionInfo = [string]::Join([Environment]::NewLine, $output)
	if ($LastExitCode -ne 0)
	{
		throw "Getting the version number failed with exitcode: $LastExitCode"
	}
	
	#extract the revision text. This is hiding in:
	# revision-id: petrik@silversurfer-pc-20090625085718-zj10htj8ooan78sp
	# date: 2009-06-25 20:57:18 + 1200
	# build-date: 2009-06-27 00:41:58 +1200
	# revno: 66
	# branch-nick: prealpha
	
	# Find the revno section and remove all that is before
	$searchString = 'revno: '
	$index = $versionInfo.IndexOf($searchString)
	$versionInfo = $versionInfo.SubString($index + $searchString.length)
	# find the first NewLine and remove all that is after
	$index = $versionInfo.IndexOf([Environment]::NewLine)
	$versionInfo.SubString(0, $index)
}

function global:Create-VersionResourceFile([string]$path, [string]$newPath, [string[]]$versionNumber){
	if (!($versionNumber.Length -eq 4))
	{
		throw "Incorrect version number provided. Version number is: $versionNumber"
	}
	
	$text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
	$text = $text -replace '@MAJOR@', $versionNumber[0]
	$text = $text -replace '@MINOR@', $versionNumber[1]
	$text = $text -replace '@BUILD@', $versionNumber[2]
	$text = $text -replace '@REVISION@', $versionNumber[3]

	Set-Content $newPath $text
}

function global:Create-InternalsVisibleToFile([string]$path, [string]$newPath, [string]$assemblyName){
	# only do this when we run the tests

	$text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
	$text = $text -replace '@ASSEMBLYNAME@', $assemblyName
	
	Set-Content $newPath $text
}

# Properties
properties{
	"Setting properties.."
	$dirBase = Get-ScriptLocation

	# solution directories
	$dirSrc = Join-Path $dirBase 'src'

	$dirBin = Join-Path $dirBase 'bin'
	$dirBuild = Join-Path $dirBin 'build'
	
	$dirResource = Join-Path $dirBase 'resource'
	
	$dirLib = Join-Path $dirBase 'lib'
	$dirLib3rdParty = Join-Path $dirLib 'thirdparty'
	
	$dirTemp = Join-Path $dirBin "temp"
	$dirLogs = Join-Path $dirBin "logs"
	$dirReports = Join-Path $dirBin 'reports'
	
	# assembly names
	$assemblyNameUnitTest = 'Apollo.Utils.Test.Unit'
	
	# templates dirs
	$dirTemplates = Join-Path ((Get-Item $dirBase).parent.parent.fullname) 'templates'
	
	# tools dirs
	$dirTools = Join-Path (Join-Path ((Get-Item $dirBase).parent.parent.fullname) 'tools') 'thirdparty'
	$dirStyleCop = Join-Path $dirTools 'StyleCop'
	$dirFxCop = Join-Path $dirTools 'FxCop'
	$dirMsbuildExtensionPack = Join-Path $dirTools 'MsBuild'
	$dirMbUnit = Join-Path $dirTools 'MbUnit'
	
	# solution files
	$slnUtils = Join-Path $dirSrc 'Apollo.Utils.sln'
	
	$msbuildStyleCop = Join-Path $dirTemplates 'StyleCop.msbuild'
	$configFxCop = Join-Path $dirBase 'Apollo.Utils.fxcop'
	$msbuildApiDoc = Join-Path $dirBase 'Apollo.Utils.shfbproj'

	# template files
	$versionFile = Join-Path $dirBase 'Version.xml'
	$versionTemplateFile = Join-Path $dirTemplates 'AssemblyInfo.VersionNumber.cs.in'
	$versionAssemblyFile = Join-Path $dirSrc 'AssemblyInfo.VersionNumber.cs'
	
	$internalsVisibleToTemplateFile = Join-Path $dirTemplates 'AssemblyInfo.InternalsVisibleTo.cs.in'
	$internalsVisibleToFile = Join-Path $dirSrc 'AssemblyInfo.InternalsVisibleTo.cs'
	
	# output files
	$logMsiBuild = 'utils_msi.log'
	$logMsBuild = 'utils_msbuild.log'
	$logFxCop = 'utils_fxcop.xml'
	$logNCover = 'utils_ncover.xml'
	
	# version numbers
	$versionMajor = 1
	$versionMinor = 0
	$versionBuild = 0
	$versionRevision = 0
	
	# script-wide variables
	$shouldClean = $true
	$shouldCheckCoverage = $false
	$configuration = 'debug'
}

# Configuration tasks
task Incremental{
	Set-Variable -Name shouldClean -Value $true -Scope 2
}

task Coverage{
	Set-Variable -Name shouldCheckCoverage -Value $true -Scope 2
}

task Debug{
	Set-Variable -Name configuration -Value 'debug' -Scope 2
}

task Release{
	Set-Variable -Name configuration -Value 'release' -Scope 2
}

# Build
# Cleans all the generated files
task Clean -depends runClean

# Builds all the binaries
task Build -depends buildBinaries

# Runs the unit tests
task UnitTests -depends runUnitTests

# Runs the integration tests
task IntegrationTests -depends runIntegrationTests

# Builds the API documentation
task ApiDoc -depends buildApiDoc

# Runs the verifications
task Verify -depends runStyleCop, runFxCop, runDuplicateFinder

###############################################################################
# HELPER TASKS

task getVersion{
	# Get the version number from the bzr repository
	[xml]$xmlFile = Get-Content $versionFile
	$major = $xmlFile.version | %{$_.major} | Select-Object -Unique
	Set-Variable -Name versionMajor -Value $major -Scope 2
	"major version number at:	$versionMajor"
	
	$minor = $xmlFile.version | %{$_.minor} | Select-Object -Unique
	Set-Variable -Name versionMinor -Value $minor -Scope 2
	"minor version number at:	$versionMinor"
	
	$build = $xmlFile.version | %{$_.build} | Select-Object -Unique
	Set-Variable -Name versionBuild -Value $build -Scope 2
	"build version number at:	$versionBuild"

	$revision = Get-BzrVersion
	Set-Variable -Name versionRevision -Value $revision -Scope 2
	"revision version number at:	$versionRevision"	
}

###############################################################################
# EXECUTING TASKS

# The Help task displays the available commandline arguments
task Help{
@"
In order to run this build script please call a specific target.
The following build tasks are available
	'incremental':		Turns on the incremental building of the binaries
	'debug':			Runs the script in debug mode. Mutually exclusive with the 'release' task
	'release':			Runs the script in release mode. Mutually exclusive with the 'debug' task
	'clean':			Cleans the output directory
	'build':			Cleans the output directory and builds the binaries
	'unittest':			Cleans the output directory, builds the binaries and runs the unit tests
	'integrationtest':	Cleans the output directory, builds the binaries and runs the integration tests
	'apidoc':			Builds the API documentation from the source comments
	'verify':			Runs the source and binary verification. Returning one or more reports
						describing the flaws in the source / binaries.

	./build.ps1 <TARGET>
Multiple build tasks can be specified separated by a comma. Also build tasks can be combined 
in any order. In most cases the build script will ensure that the tasks are executed in the
correct order. Note that this is NOT the case for the 'incremental', 'debug' and 'release' tasks.
In order to get a correct effect these tasks need to be the first tasks being called!
       
In order to run this build script please call this script via PSAKE like:
	invoke-psake utils.ps1 incremental,debug,clean,build,unittest,verify -framework 4.0 -timing
"@
}

task runClean{
	if ($shouldClean)
	{
		"Cleaning..."
		
		$msbuildExe = Get-MsbuildExe
		& $msbuildExe $slnUtils /t:Clean /verbosity:minimal
		
		# Clean the bin dir
		if (Test-Path -Path $dirBin -PathType Container)
		{
			"Removing the bin directory..."
			Remove-Item $dirBin -Force -Recurse
		}
	}
}

task runInit -depends runClean{
	"Initializing build..."
	
	if (!(Test-Path -Path $dirBin -PathType Container))
	{
		New-Item $dirBin -ItemType directory | Out-Null # Don't display the directory information
	}
	
	if (!(Test-Path -Path $dirBuild -PathType Container))
	{
		New-Item $dirBuild -ItemType directory | Out-Null # Don't display the directory information
	}
	
	if (!(Test-Path -Path $dirTemp -PathType Container))
	{
		New-Item $dirTemp -ItemType directory | Out-Null # Don't display the directory information
	}
	
	if (!(Test-Path -Path $dirLogs -PathType Container))
	{
		New-Item $dirLogs -ItemType directory | Out-Null # Don't display the directory information
	}
	
	if (!(Test-Path -Path $dirReports -PathType Container))
	{
		New-Item $dirReports -ItemType directory | Out-Null # Don't display the directory information
	}
}

task buildBinaries -depends runInit, getVersion{
	"Building binaries..."
	
	# Set the version numbers
	Create-VersionResourceFile $versionTemplateFile $versionAssemblyFile ($versionMajor, $versionMinor, $versionBuild, $versionRevision)
	
	# Set the InternalsVisibleTo attribute
	Create-InternalsVisibleToFile $internalsVisibleToTemplateFile $internalsVisibleToFile $assemblyNameUnitTest

	# build the core binaries
	"Building Apollo.Utils..."	
	$logPath = Join-Path $dirLogs $logMsBuild
	
	$msbuildExe = Get-MsbuildExe
	& $msbuildExe $slnUtils /p:Configuration=$configuration /clp:Summary /clp:ShowTimeStamp /clp:Verbosity=minimal /flp:LogFile=$logPath /flp:Verbosity=normal
	if ($LastExitCode -ne 0)
	{
		throw "Apollo.Core build failed with return code: $LastExitCode"
	}
	
	# Copy the binaries
	$dirBinUtils = Join-Path (Join-Path (Join-Path $dirSrc 'utils') 'bin') $configuration
	$dirBinUtilsSrcOnly = Join-Path (Join-Path (Join-Path $dirSrc 'utils.srconly') 'bin') $configuration
	$dirBinUnit = Join-Path (Join-Path (Join-Path $dirSrc 'utils.test.unit') 'bin') $configuration
	$dirBinPerf = Join-Path (Join-Path (Join-Path $dirSrc 'utils.test.perf') 'bin') $configuration
	
	Copy-Item (Join-Path $dirBinUtils '*') $dirBuild -Force
	Copy-Item (Join-Path $dirBinUtilsSrcOnly '*') $dirBuild -Force
	Copy-Item (Join-Path $dirBinUnit '*') $dirBuild -Force
	Copy-Item (Join-Path $dirBinPerf '*') $dirBuild -Force
}

task runUnitTests -depends buildBinaries{
	"Running unit tests..."
	
	$mbunitExe = Join-Path $dirMbUnit 'Gallio.Echo.exe'
	if ($shouldCheckCoverage)
	{
		#
		# FIX THIS.... NEED COVERAGE!!!!!!!!!!
		#
	
		throw "Code coverage doesn't work at the moment. Please run without coverage"
		# Run mbunit in an isolated process. On a 64-bit machine gallio ALWAYS starts as a 64-bit
		#   process. This means we can't load explicit 32-bit binaries. However using the 
		#   isolated process runner we can
		$logFile = Join-Path $dirReports $logNCover
		& $mbunitExe /hd:$dirMbUnit /wd:$dirBuild /sc /rd:$dirReports /rt:XHtml-Condensed /r:NCover /rp:'NCoverCoverageFile:$logFile' /rp:"NCoverArguments:'//a Apollo.Utils.dll'" (Join-Path $dirBuild 'Apollo.Utils.Test.Unit.dll')
	}
	else
	{
		# Run mbunit in an isolated process. On a 64-bit machine gallio ALWAYS starts as a 64-bit
		#   process. This means we can't load explicit 32-bit binaries. However using the 
		#   isolated process runner we can
		& $mbunitExe /hd:$dirMbUnit /wd:$dirBuild /sc /rd:$dirReports /rt:XHtml-Condensed /r:IsolatedProcess (Join-Path $dirBuild 'Apollo.Utils.Test.Unit.dll')	
	}

	if ($LastExitCode -ne 0)
	{
		throw "MbUnit failed on Apollo.Utils with return code: $LastExitCode"
	}
}

task runIntegrationTests -depends buildBinaries{
	"Running integration tests..."
	"There are no integration tests."
}

task buildApiDoc -depends buildBinaries{
	"Build the API docs..."
	
	$msbuildExe = Get-MsbuildExe
	& $msbuildExe $msbuildApiDoc
	if ($LastExitCode -ne 0)
	{
		throw "Sandcastle help file builder failed on Apollo.Utils with return code: $LastExitCode"
	}
	
	if( $configuration -eq 'release')
	{
		# Should fail are release build if there's anything missing?
	}
}

task runStyleCop -depends buildBinaries{
	$msbuildExe = Get-MsbuildExe
	
	& $msbuildExe $msbuildStyleCop /p:StyleCopForMsBuild=$dirStyleCop /p:MsBuildExtensionPack=$dirMsbuildExtensionPack /p:ProjectDir=$dirBase /p:SrcDir=$dirSrc /p:ReportsDir=$dirReports /verbosity:normal /clp:NoSummary
	if ($LastExitCode -ne 0)
	{
		throw "Stylecop failed on Apollo.Utils with return code: $LastExitCode"
	}
	
	# Check the MsBuild file (in the templates directory) for failure conditions	
}

task runFxCop -depends buildBinaries{
	# could set different targets depending on configuration:
	# - skip some rules if in debug mode
	# - fail if in release mode
	
	$fxcopExe = Join-Path $dirFxCop 'FxCopcmd.exe'
	$outFile = Join-Path $dirReports $logFxCop
	
	& $fxcopExe /project:$configFxCop /out:$outFile
	if ($LastExitCode -ne 0)
	{
		throw "FxCop failed on Apollo.Utils with return code: $LastExitCode"
	}
	
	if ($configuration -eq 'release')
	{
		# check that there were no violations
	}
}

task runDuplicateFinder -depends buildBinaries{
	"Running duplicate check ..."
	
	# FAIL THE BUILD IF THERE IS ANYTHING WRONG
}