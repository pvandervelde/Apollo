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

# Properties
properties{
	"Setting properties.."
	$dirBase = Get-ScriptLocation

	# solution directories
	$dirSrc = Join-Path $dirBase 'src'
	$dirBin = Join-Path $dirBase 'bin'
	
	$dirResource = Join-Path $dirBase 'resource'
	$dirInstall = Join-Path $dirBase 'install'
	
	$dirLib = Join-Path $dirBase 'lib'
	$dirLib3rdParty = Join-Path $dirLib 'thirdparty'
	
	$dirTemp = Join-Path $dirBin "temp"
	$dirLogs = Join-Path $dirBin "logs"
	$dirReports = Join-Path $dirBin 'reports'
	
	# tools dirs
	$dirTools = Join-Path (Join-Path ((Get-Item $dirBase).parent.parent.fullname) 'tools') 'thirdparty'
	$dirStyleCop = Join-Path $dirTools 'StyleCop'
	$dirFxCop = Join-Path $dirTools 'FxCop'
	$dirMsbuildExtensionPack = Join-Path $dirTools 'MsBuild'
	$dirMbUnit = Join-Path $dirTools 'MbUnit'
	
	# solution files
	$slnCore = Join-Path $dirSrc 'Apollo.Core.sln'
	
	$msbuildStyleCop = Join-Path (Join-Path((Get-Item $dirBase).parent.parent.fullname) 'templates') 'StyleCop.msbuild'
	$configFxCop = Join-Path $dirBase 'Apollo.Core.fxcop'
	
	$msbuildApiDoc = Join-Path $dirBase 'Apollo.Core.shfbproj'
	
	$versionFile = Join-Path $dirBase 'Version.xml'
	$versionTemplateFile = Join-Path $dirSrc 'AssemblyInfo.VersionNumber.cs.in'
	$versionAssemblyFile = Join-Path $dirSrc 'AssemblyInfo.VersionNumber.cs'
	
	# output files
	$logMsiBuild = 'core_msi.log'
	$logMsBuild = 'core_msbuild.log'
	$logFxCop = 'core_fxcop.xml'
	$logNCover = 'core_ncover.xml'
	
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
task Verify -depends runVerify

# Builds the installer modules
task Installer -depends buildBinaries

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

task runClean{
	if ($shouldClean)
	{
		"Cleaning..."
		
		$msbuildExe = Get-MsbuildExe
		& $msbuildExe $slnCallImport /t:Clean
		
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

	# build the core binaries
	"Building Apollo.Core..."	
	$logPath = Join-Path $dirLogs $logMsBuild
	
	$msbuildExe = Get-MsbuildExe
	& $msbuildExe $slnCore /p:Configuration=$configuration /clp:Summary /clp:ShowTimeStamp /clp:Verbosity=normal /fileLoggerParameters:LogFile=$logPath	
	if ($LastExitCode -ne 0)
	{
		throw "Apollo.Core build failed with return code: $LastExitCode"
	}
	
	# Copy the binaries
	$dirBinCore = Join-Path (Join-Path (Join-Path $dirSrc 'core') 'bin') $configuration
	$dirBinUnit = Join-Path (Join-Path (Join-Path $dirSrc 'core.test.unit') 'bin') $configuration
	$dirBinIntegration = Join-Path (Join-Path (Join-Path $dirSrc 'core.test.integration') 'bin') $configuration
	$dirBinPerf = Join-Path (Join-Path (Join-Path $dirSrc 'core.test.perf') 'bin') $configuration
	
	Copy-Item (Join-Path $dirBinCore '*') $dirBin
	Copy-Item (Join-Path $dirBinUnit '*') $dirBin
	Copy-Item (Join-Path $dirBinIntegration '*') $dirBin
	Copy-Item (Join-Path $dirBinPerf '*') $dirBin
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
		& $mbunitExe /hd:$dirMbUnit /wd:$dirBin /sc /rd:$dirReports /rt:XHtml-Condensed /r:NCover /rp:'NCoverCoverageFile:$logFile' /rp:"NCoverArguments:'//a Apollo.Core.dll'" (Join-Path $dirBin 'Apollo.Core.Test.Unit.dll')
	}
	else
	{
		# Run mbunit in an isolated process. On a 64-bit machine gallio ALWAYS starts as a 64-bit
		#   process. This means we can't load explicit 32-bit binaries. However using the 
		#   isolated process runner we can
		& $mbunitExe /hd:$dirMbUnit /wd:$dirBin /sc /rd:$dirReports /rt:XHtml-Condensed /r:IsolatedProcess (Join-Path $dirBin 'Apollo.Core.Test.Unit.dll')	
	}
	
	#
	# FIX THIS: NEED TO HAVE BUILD FAILURES!!!!
	#	
	
	#if ($LastExitCode -ne 0)
	#{
	#	throw "MbUnit failed on Apollo.Core with return code: $LastExitCode"
	#}
}

task runIntegrationTests -depends buildBinaries{
	"Running integration tests..."
	# ???
}

task buildApiDoc -depends buildBinaries{
	"Build the API docs..."
	
	$msbuildExe = Get-MsbuildExe
	& $msbuildExe $msbuildApiDoc
	if ($LastExitCode -ne 0)
	{
		throw "Sandcastle help file builder failed on Apollo.Core with return code: $LastExitCode"
	}
}

task runStyleCop -depends buildBinaries{
	$msbuildExe = Get-MsbuildExe
	
	& $msbuildExe $msbuildStyleCop /p:StyleCopForMsBuild=$dirStyleCop /p:MsBuildExtensionPack=$dirMsbuildExtensionPack /p:ProjectDir=$dirBase /p:SrcDir=$dirBase /p:ReportsDir=$dirReports
	if ($LastExitCode -ne 0)
	{
		throw "Stylecop failed on Apollo.Core with return code: $LastExitCode"
	}
	
	# FAIL THE BUILD IF THERE IS ANYTHING WRONG
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
		throw "FxCop failed on Apollo.Core with return code: $LastExitCode"
	}
	
	# FAIL THE BUILD IF THERE IS ANYTHING WRONG
}

task runDuplicateFinder -depends buildBinaries{
	"Running duplicate check ..."
	
	# FAIL THE BUILD IF THERE IS ANYTHING WRONG
}

task runVerify -depends runStyleCop, runFxCop, runDuplicateFinder

task buildInstaller -depends buildBinaries{
	"Building installer..."
	
	# Use Wix to build all the merge modules
}