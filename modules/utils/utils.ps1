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

function global:Create-ConfigurationResourceFile([string]$path, [string]$newPath, [string]$config){
	$text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
	$text = $text -replace '@CONFIGURATION@', $config

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
	
	$dirTemp = Join-Path $dirBin "temp"
	$dirLogs = Join-Path $dirBin "logs"
	$dirReports = Join-Path $dirBin 'reports'
	$dirDeploy = Join-Path $dirBin 'release'
	
	# assembly names
	$assemblyNameUnitTest = 'Apollo.Utils.Test.Unit, PublicKey=' + `
	'0024000004800000940000000602000000240000525341310004000001000100cf9cb2eef36547' + `
	'0a150da8bd50d1f7ca65ad3ca14fe30f3fbb8cc005b4ea399a5cc88aa271e8fd69222e0cb43d5c' + `
	'04a1fa8ac57a3fc033fe7ab98881ad3287ed268d8bea2c9b08f76e197062ceef8f713b09eb4917' + `
	'25404461f4ca754cbe5ab7fa7892a14a1b986c1b225e5a6529d385bbd803c2f9f6bc75d3ba4de1' + `
	'896b24e2'
	
	# templates dirs
	$dirTemplates = Join-Path $dirBase 'templates'
	
	# tools dirs
	$dirTools = Join-Path $dirBase 'tools'
	$dirStyleCop = Join-Path $dirTools 'StyleCop'
	$dirFxCop = Join-Path $dirTools 'FxCop'
	$dirMsbuildExtensionPack = Join-Path $dirTools 'MsBuild'
	$dirMbUnit = Join-Path $dirTools 'MbUnit'
	$dirNCoverExplorer = Join-Path (Join-Path $dirMbUnit 'NCover') 'NCoverExplorer'
	$dirSandcastle = Join-Path $dirTools 'sandcastle'
	
	# solution files
	$slnUtils = Join-Path $dirSrc 'Apollo.Utils.sln'
	
	$msbuildStyleCop = Join-Path $dirTemplates 'StyleCop.msbuild'
	$msbuildApiDoc = Join-Path $dirBase 'Apollo.Utils.shfbproj'
	$msbuildSandcastleReferenceData = Join-Path $dirSandcastle 'fxReflection.proj'
	
	# template files
	$versionFile = Join-Path $dirBase 'Version.xml'
	$versionTemplateFile = Join-Path $dirTemplates 'AssemblyInfo.VersionNumber.cs.in'
	$versionAssemblyFile = Join-Path $dirSrc 'AssemblyInfo.VersionNumber.cs'
	
	$configurationTemplateFile = Join-Path $dirTemplates 'AssemblyInfo.Configuration.cs.in'
	$configurationAssemblyFile = Join-Path $dirSrc 'AssemblyInfo.Configuration.cs'
	
	$signTemplateFile = Join-Path $dirTemplates 'AssemblyInfo.Signing.cs.in'
	$signAssemblyFile = Join-Path $dirSrc 'AssemblyInfo.Signing.cs'
	
	$internalsVisibleToTemplateFile = Join-Path $dirTemplates 'AssemblyInfo.InternalsVisibleTo.cs.in'
	$internalsVisibleToFile = Join-Path $dirSrc 'AssemblyInfo.InternalsVisibleTo.cs'
	
	# output files
	$logMsiBuild = 'utils_msi.log'
	$logMsBuild = 'utils_msbuild.log'
	$logFxCop = 'utils_fxcop.xml'
	$logNCover = 'utils_ncover.xml'
	$logNCoverHtml = 'utils_ncover.html'
	
	# settings
	$levelMinCoverage = 85
	
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

# Creates the zip file of the deliverables
task Package -depends buildPackage

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
# note that the weird outlining is so that it works in the console ...
task Help -action{
@"
In order to run this build script please call a specific target.
The following build tasks are available
	'incremental':		Turns on the incremental building of the binaries
	'coverage':			Turns on the code coverage for the unit tests
	'debug':			Runs the script in debug mode. Mutually exclusive with the 'release' task
	'release':			Runs the script in release mode. Mutually exclusive with the 'debug' task
	'clean':			Cleans the output directory
	'build':			Cleans the output directory and builds the binaries
	'unittest':			Cleans the output directory, builds the binaries and runs the unit tests
	'integrationtest':		Cleans the output directory, builds the binaries and runs the integration tests
	'apidoc':			Builds the API documentation from the source comments
	'verify':			Runs the source and binary verification. Returning one or more reports
					describing the flaws in the source / binaries.
	'package':			Packages the deliverables into a single zip file

	./build.ps1 <TARGET>
Multiple build tasks can be specified separated by a comma. Also build tasks can be combined 
in any order. In most cases the build script will ensure that the tasks are executed in the
correct order. Note that this is NOT the case for the 'incremental', 'debug' and 'release' tasks.
In order to get a correct effect these tasks need to be the first tasks being called!
       
In order to run this build script please call this script via PSAKE like:
	invoke-psake utils.ps1 incremental,debug,clean,build,unittest,verify -framework 4.0 -timing -noexit -showfullerror
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
	
	if (!(Test-Path -Path $dirDeploy -PathType Container))
	{
		New-Item $dirDeploy -ItemType directory | Out-Null # Don't display the directory information
	}
}

task buildBinaries -depends runInit, getVersion -action{
	"Building Apollo.Utils..."	
	
	# Set the version numbers
	Create-VersionResourceFile $versionTemplateFile $versionAssemblyFile $versionNumber
	
	# Set the configuration
	Create-ConfigurationResourceFile $configurationTemplateFile $configurationAssemblyFile $configuration
	
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
	
	$mbunitExe = Join-Path $dirMbUnit 'Gallio.Echo.x86.exe'
	
	$files = ""
	$assemblies = Get-ChildItem -path $dirBuild -Filter "*.dll" | Where-Object { ((($_.Name -like "*Apollo*") -and ( $_.Name -like "*Test*") -and !($_.Name -like "*vshost*")))}
	$assemblies | ForEach-Object -Process { $files += '"' + $_.FullName + '" '}
	$command = '& "' + "$mbunitExe" + '" ' + '/hd:"' + $dirMbUnit + '" /sc '
	if ($shouldCheckCoverage)
	{
		$coverageFiles = ""
		$coverageAssemblies = Get-ChildItem -path $dirBuild |
			Where-Object { ((($_.Name -like "*Apollo*") -and `
							!( $_.Name -like "*Test.*") -and `
							!($_.Name -like "*vshost*")) -and `
							($_.Extension -match ".dll"))}
		$coverageAssemblies | ForEach-Object -Process { $coverageFiles += [System.IO.Path]::GetFileNameWithoutExtension($_.FullName) + ";"}
		
		# Run mbunit in an isolated process. On a 64-bit machine gallio ALWAYS starts as a 64-bit
		#   process. This means we can't load explicit 32-bit binaries. However using the 
		#   isolated process runner we can.
		# Turn on the code coverage and specify which files need coverage checked
		$command += "/r:NCover /rp:NCoverArguments='//a " + $coverageFiles
		
		# Specify where the XML log file should be written to
		$command += " //x " + '\"' + (Join-Path $dirReports $logNCover) + '\"'
	
		# Indicate which Attribute is used to exclude classes / methods from coverage
		$command += " //ea Apollo.Utils.ExcludeFromCoverageAttribute' "
	}
	else
	{
		# Run mbunit in an isolated process. On a 64-bit machine gallio ALWAYS starts as a 64-bit
		#   process. This means we can't load explicit 32-bit binaries. However using the 
		#   isolated process runner we can
		$command += "/r:IsolatedProcess " 
	}
	
	# add the files.
	$command += ' /rd:"' + $dirReports + '" /v:Verbose /rt:XHtml-Condensed /rt:Xml-inline ' + $files
	
	# run the tests
	$command
	Invoke-Expression $command
	if ($shouldCheckCoverage)
	{
		$ncoverFile = Join-Path $dirBase 'Coverage.log'
		if (Test-Path -Path $ncoverFile)
		{
			# The directory does not exist. Create it
			Remove-Item $ncoverFile -Force
		}
		
		Move-Item -Path (Join-Path $dirBase 'Coverage.xml') -Destination (Join-Path $dirReports $logNCover)
	}
	
	if ($LastExitCode -ne 0)
	{
		throw "MbUnit failed on Apollo.Utils with return code: $LastExitCode"
	}
	
	# Generate the code coverage HTML report
	if ($shouldCheckCoverage)
	{
		$ncoverExplorer = Join-Path $dirNCoverExplorer 'NCoverExplorer.Console.exe'
		$command = '& "' + "$ncoverExplorer" + '" ' + ' "' + (Join-Path $dirReports $logNCover) + '"' + " /h:" + '"' + (Join-Path $dirReports $logNCoverHtml) + '"' + " /r:ModuleClassSummary" + " /m:" + $levelMinCoverage
		$command
		
		Invoke-Expression $command
		if ($LastExitCode -ne 0)
		{
			throw "NCoverExplorer failed on Apollo.Utils with return code: $LastExitCode"
		}
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
	
	$assemblies = Get-ChildItem -path $dirBuild -Filter "*.dll" | Where-Object { (($_.Name -like "*Apollo*") -and !( $_.Name -like "*SrcOnly*") -and !( $_.Name -like "*Test*"))}

	$files = ""
	$assemblies | ForEach-Object -Process { $files += "/file:" + '"' + $_.FullName + '" '}
	
	$command = "& '" + "$fxcopExe" + "' " + "$files /rule:" + "'" + "$rulesDir" + "'" + " /out:" + "'" + "$outFile" + "'"
	$command
	Invoke-Expression $command
	if ($LastExitCode -ne 0)
	{
		throw "FxCop failed on Apollo.Utils with return code: $LastExitCode"
	}
}

task runDuplicateFinder -depends buildBinaries -action{
	"Running duplicate check ..."
	
	# FAIL THE BUILD IF THERE IS ANYTHING WRONG
}

task buildPackage -depends buildBinaries -action{
	"Packaging the files into a zip ..."
	
	$dirTempZip = Join-Path $dirTemp 'zip'
	if((Test-Path -Path $dirTempZip -PathType Container))
	{
		Remove-Item $dirTempZip -Force
	}
	
	# The directory does not exist. Create it
	New-Item $dirTempZip -ItemType directory | Out-Null # Don't display all the information
	
	# Copy the files to the temp dir
	# match all files that:
	# - Are DLL, EXE or config files
	# - Have the term Sherlock in their name
	# - Don't have the terms 'Test' or 'vshost' in their name
	$assemblies = Get-ChildItem -path $dirBuild | 
		Where-Object { ((($_.Name -like "*Apollo*") -and `
						!( $_.Name -like "*SrcOnly.*") -and `
						!( $_.Name -like "*Test.*") -and `
						!($_.Name -like "*vshost*")) -and `
						(($_.Extension -match ".dll") -or `
						 ($_.Extension -match ".exe") -or `
						 ($_.Extension -match ".config") -or `
						 ($_.Extension -match ".pdb")))}

	foreach ($file in $assemblies){
		$newFilePath = Join-Path $dirTempZip $file.Name
		Copy-Item $file.FullName -Destination $newFilePath -Force
	}
	
	# Copy the dependencies
	$lokadFile = 'Lokad.Shared.dll'
	Copy-Item (Join-Path $dirBuild $lokadFile) -Destination (Join-Path $dirTempZip $lokadFile)
	
	# zip them
	# Name the zip: Apollo.Utils_<DATE>
	$output = Join-Path $dirDeploy ("Apollo.Utils_" + [System.DateTime]::Now.ToString("yyyy_MM_dd-HH_mm_ss") + ".zip")

	"Compressing..."

	# zip the hudson temp dir
	$7zipExe = "$Env:ProgramW6432\7-Zip\7z.exe"
	& $7zipExe a -tzip $output (Get-ChildItem $dirTempZip | foreach { $_.FullName })
	if ($LastExitCode -ne 0)
	{
		throw "Failed to compress the Apollo.Utils binaries."
	}	
}