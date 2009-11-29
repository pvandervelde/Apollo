# Globals
function global:Get-ScriptLocation{
	Split-Path $MyInvocation.ScriptName
}

function global:Get-MsBuildExe
{
	'msbuild'
}

function global:Invoke-MsBuild([string]$solution, [string]$configuration, [string]$logPath, [string]$verbosity, [string[]]$parameters){
	$msbuildExe = Get-MsBuildExe
	$msBuildParameters = "/p:Configuration=$configuration"
	if ($parameters.Length -ne 0)
	{
		$msBuildParameters += ' /p:' +  ([string]::Join(" /p:", ( $parameters )))
	}
	
	$command = "$msbuildExe"
	$command += " '"
	$command += "$solution"
	$command += "'"
	$command += " $msbuildParameters"
	$command += " /m"
	$command += " /clp:Verbosity=$verbosity /clp:Summary /clp:NoItemAndPropertyList /clp:ShowTimestamp"
	$command += " /flp:LogFile='$logPath' /flp:verbosity=$verbosity /flp:Summary /flp:ShowTimestamp"

	"Building $solution with command: $command"
	Invoke-Expression $command
	if ($LastExitCode -ne 0)
	{
		throw "$solution build failed with return code: $LastExitCode"
	}
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

function global:Create-VersionResourceFile([string]$path, [string]$newPath, [System.Version]$versionNumber){
	$text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
	$text = $text -replace '@MAJOR@', $versionNumber.Major
	$text = $text -replace '@MINOR@', $versionNumber.Minor
	$text = $text -replace '@BUILD@', $versionNumber.Build
	$text = $text -replace '@REVISION@', $versionNumber.Revision

	Set-Content -Path $newPath -Value $text
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
	$dirTemplates = Join-Path $dirBase 'templates'
	
	# tools dirs
	$dirTools = Join-Path $dirBase 'tools'
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
	
	# settings
	# Version number
	$versionNumber = New-Object -TypeName System.Version -ArgumentList "1.0.0.0"
	
	# incremental state
	$shouldClean = $true
	$configuration = 'debug'
	$shouldCheckCoverage = $false
}

# The default task doesn't do anything. This just calls the help function. Useful
#   for new people
task default -depends Help

# Configuration tasks
task Incremental -action{
	Set-Variable -Name shouldClean -Value $true -Scope 2
}

task Coverage -action{
	Set-Variable -Name shouldCheckCoverage -Value $true -Scope 2
}

task Debug -action{
	Set-Variable -Name configuration -Value 'debug' -Scope 2
}

task Release -action{
	Set-Variable -Name configuration -Value 'release' -Scope 2
}

# Cleans all the generated files
task Clean -depends runClean

# Builds all the binaries
task Build -depends buildBinaries

# Runs the unit tests
task UnitTest -depends runUnitTests

# Runs the integration tests
task IntegrationTest -depends runIntegrationTests

# Builds the API documentation
task ApiDoc -depends buildApiDoc

# Runs the verifications
task Verify -depends runStyleCop, runFxCop, runDuplicateFinder

###############################################################################
# HELPER TASKS

task getVersion -action{
	#Get the file version from the version.xml file
	[xml]$xmlFile = Get-Content $versionFile
	$major = $xmlFile.version | %{$_.major} | Select-Object -Unique
	$minor = $xmlFile.version | %{$_.minor} | Select-Object -Unique
	$build = $xmlFile.version | %{$_.build} | Select-Object -Unique
	$revision = Get-BzrVersion

	$version = New-Object -TypeName System.Version -ArgumentList "$major.$minor.$build.$revision"
	"version is: $version"
	
	Set-Variable -Name versionNumber -Value $version -Scope 2
}

###############################################################################
# EXECUTING TASKS

# The Help task displays the available commandline arguments
task Help -action{
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

task runClean  -precondition{ $shouldClean } -action{
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

task runInit -depends runClean -action{
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

task buildBinaries -depends runInit, getVersion -action{
	"Building Apollo.Utils..."	
	
	# Set the version numbers
	Create-VersionResourceFile $versionTemplateFile $versionAssemblyFile $versionNumber
	
	# Set the InternalsVisibleTo attribute
	Create-InternalsVisibleToFile $internalsVisibleToTemplateFile $internalsVisibleToFile $assemblyNameUnitTest

	$logPath = Join-Path $dirLogs $logMsBuild
	
	$msbuildExe = Get-MsbuildExe
	Invoke-MsBuild $slnUtils $configuration $logPath 'minimal' ("platform='Any CPU'")
	if ($LastExitCode -ne 0)
	{
		throw "Apollo.Utils build failed with return code: $LastExitCode"
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

task runUnitTests -depends buildBinaries -action{
	"Running unit tests..."
	
	$mbunitExe = Join-Path $dirMbUnit 'Gallio.Echo.exe'
	
	$files = ""
	$assemblies = Get-ChildItem -path $dirBuild -Filter "*.dll" | Where-Object { ((($_.Name -like "*Apollo*") -and ( $_.Name -like "*Test*") -and !($_.Name -like "*vshost*")))}
	$assemblies | ForEach-Object -Process { $files += '"' + $_.FullName + '" '}
	$command = "& '" + "$mbunitExe" + "' " + "/hd:'" + $dirMbUnit + "' /sc /rd:'" + $dirReports + "' /rt:XHtml-Condensed /rt:Xml-inline /r:IsolatedProcess " + $files
	
	if ($shouldCheckCoverage)
	{
		#
		# FIX THIS.... NEED COVERAGE!!!!!!!!!!
		#
	
		throw "Code coverage doesn't work at the moment. Please run without coverage"
		# Run mbunit in an isolated process. On a 64-bit machine gallio ALWAYS starts as a 64-bit
		#   process. This means we can't load explicit 32-bit binaries. However using the 
		#   isolated process runner we can
		#$logFile = Join-Path $dirReports $logNCover
		#& $mbunitExe /hd:$dirMbUnit /wd:$dirBuild /sc /rd:$dirReports /rt:XHtml-Condensed /r:NCover /rp:'NCoverCoverageFile:$logFile' /rp:"NCoverArguments:'//a Apollo.Utils.dll'" (Join-Path $dirBuild 'Apollo.Utils.Test.Unit.dll')
	}
	else
	{
		# Run mbunit in an isolated process. On a 64-bit machine gallio ALWAYS starts as a 64-bit
		#   process. This means we can't load explicit 32-bit binaries. However using the 
		#   isolated process runner we can
		Invoke-Expression $command	
	}

	if ($LastExitCode -ne 0)
	{
		throw "MbUnit failed on Apollo.Utils with return code: $LastExitCode"
	}
}

task runIntegrationTests -depends buildBinaries -action{
	"Running integration tests..."
	"There are no integration tests."
}

task buildApiDoc -depends buildBinaries -action{
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

task runStyleCop -depends buildBinaries -action{
	$msbuildExe = Get-MsbuildExe
	
	& $msbuildExe $msbuildStyleCop /p:StyleCopForMsBuild=$dirStyleCop /p:ProjectDir=$dirBase /p:SrcDir=$dirSrc /p:ReportsDir=$dirReports /verbosity:normal /clp:NoSummary
	if ($LastExitCode -ne 0)
	{
		throw "Stylecop failed on Apollo.Utils with return code: $LastExitCode"
	}
	
	# Check the MsBuild file (in the templates directory) for failure conditions	
}

task runFxCop -depends buildBinaries -action{
	# could set different targets depending on configuration:
	# - skip some rules if in debug mode
	# - fail if in release mode
	
	$fxcopExe = Join-Path $dirFxCop 'FxCopcmd.exe'
	$rulesDir = Join-Path $dirFxCop 'Rules'
	$outFile = Join-Path $dirReports $logFxCop
	
	$assemblies = Get-ChildItem -path $dirBuild -Filter "*.dll" | Where-Object { (($_.Name -like "*Apollo*") -and !( $_.Name -like "*Test*"))}

	$files = ""
	$assemblies | ForEach-Object -Process { $files += "/file:" + '"' + $_.FullName + '" '}
	
	$command = "& '" + "$fxcopExe" + "' " + "$files /rule:" + "'" + "$rulesDir" + "'" + " /out:" + "'" + "$outFile" + "'"
	$command
	Invoke-Expression $command
	if ($LastExitCode -ne 0)
	{
		throw "FxCop failed on NSarrac.Framework with return code: $LastExitCode"
	}
}

task runDuplicateFinder -depends buildBinaries -action{
	"Running duplicate check ..."
	
	# FAIL THE BUILD IF THERE IS ANYTHING WRONG
}