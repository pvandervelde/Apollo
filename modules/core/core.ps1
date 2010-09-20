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

function global:Get-PublicKeySignature([string]$tempDir, [string]$pathToKeyFile)
{
	$sn = "${Env:ProgramFiles(x86)}\Microsoft SDKs\Windows\v7.0A\bin\sn.exe"
	$publicKeyFile = Join-Path $tempDir ([System.IO.Path]::GetRandomFileName())

	# use snk to get the public key bit
	& $sn -p $pathToKeyFile $publicKeyFile | Out-Null
	$output = & $sn -tp $publicKeyFile
	$publicKeyInfo = [string]::Join("", $output)
	
	# extract the public key text. This is hiding in:
	# Microsoft (R) .NET Framework Strong Name Utility  Version 3.5.30729.1
	# Copyright (c) Microsoft Corporation.  All rights reserved.
	# 
	# Public key is
	# 0024000004800000940000000602000000240000525341310004000001000100cf9cb2eef36547
	# 0a150da8bd50d1f7ca65ad3ca14fe30f3fbb8cc005b4ea399a5cc88aa271e8fd69222e0cb43d5c
	# 04a1fa8ac57a3fc033fe7ab98881ad3287ed268d8bea2c9b08f76e197062ceef8f713b09eb4917
	# 25404461f4ca754cbe5ab7fa7892a14a1b986c1b225e5a6529d385bbd803c2f9f6bc75d3ba4de1
	# 896b24e2
	# 
	# Public key token is ee5b68ec5ad4ef93
	
	$startString = 'Public key is'
	$endString = 'Public key token is'
	$startIndex = $publicKeyInfo.IndexOf($startString)
	$endIndex = $publicKeyInfo.IndexOf($endString)
	$publicKeyInfo.SubString($startIndex + $startString.length, $endIndex - ($startIndex + $startString.length))
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
    $text = $text -replace '@COPYRIGHTYEAR@', [DateTimeOffset]::Now.Year
    
	$text = $text -replace '@CONFIGURATION@', $config
	
	$now = [DateTimeOffset]::Now
	$text = $text -replace '@BUILDTIME@', $now.ToString("o")

	Set-Content -Path $newPath -Value $text
}

function global:Create-InternalsVisibleToFile([string]$path, [string]$newPath, [string]$assemblyName){
	# only do this when we run the tests

	$text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
	$text = $text -replace '@ASSEMBLYNAME@', $assemblyName
	
	Set-Content $newPath $text
}

function global:Create-LicenseVerificationSequencesFile([string]$generatorTemplate, [string]$yieldTemplate, [string]$newPath){
	$yieldText = [string]::Join([Environment]::NewLine, (Get-Content -Path $yieldTemplate))
	
	# generate the sequences
	# Check each hour after build
	$sequenceText += $yieldText
	$sequenceText = $sequenceText -replace '@REPEATPERIOD@', 'Hourly'
	$sequenceText = $sequenceText -replace '@MODIFIER@', '1'
	$sequenceText = $sequenceText -replace '@START_TIME@', 'BuildTime()'
	$sequenceText += [Environment]::NewLine
	$sequenceText += [Environment]::NewLine
	
	# Check each hour after install
	$sequenceText += $yieldText
	$sequenceText = $sequenceText -replace '@REPEATPERIOD@', 'Hourly'
	$sequenceText = $sequenceText -replace '@MODIFIER@', '1'
	$sequenceText = $sequenceText -replace '@START_TIME@', 'InstallTime()'
	$sequenceText += [Environment]::NewLine
	$sequenceText += [Environment]::NewLine
	
	# Check each hour after start
	$sequenceText += $yieldText
	$sequenceText = $sequenceText -replace '@REPEATPERIOD@', 'Hourly'
	$sequenceText = $sequenceText -replace '@MODIFIER@', '1'
	$sequenceText = $sequenceText -replace '@START_TIME@', 'ProcessStartTime()'
	$sequenceText += [Environment]::NewLine
	$sequenceText += [Environment]::NewLine
	
	# Check each hour after some time
	$time = [DateTimeOffset]::Now.AddMinutes(10)
	$timeText = 'new DateTimeOffset(' + $time.Year + `
		', ' + $time.Month + `
		', ' + $time.Day + `
		', ' + $time.Hour + `
		', ' + $time.Minute + `
		', ' + $time.Second + `
		', ' + $time.Millisecond + `
		', new TimeSpan(' + $time.Offset.Ticks + '))'
	$sequenceText += $yieldText
	$sequenceText = $sequenceText -replace '@REPEATPERIOD@', 'Hourly'
	$sequenceText = $sequenceText -replace '@MODIFIER@', '1'
	$sequenceText = $sequenceText -replace '@START_TIME@', $timeText
	
	# Write the sequences to the file
	$text = [string]::Join([Environment]::NewLine, (Get-Content -Path $generatorTemplate))
	$text = $text -replace '@YIELD_STATEMENTS@', $sequenceText
	
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
	$dirInstall = Join-Path $dirBase 'install'
	
	$dirTemp = Join-Path $dirBin "temp"
	$dirLogs = Join-Path $dirBin "logs"
	$dirReports = Join-Path $dirBin 'reports'
	$dirDeploy = Join-Path $dirBin 'release'
	
	# assembly names
	$assemblyNameUnitTest = 'Apollo.Core.Test.Unit, PublicKey='
	
	# templates dirs
	$dirTemplates = Join-Path $dirBase 'templates'
	
	# configuration dir
	$dirConfiguration = Join-Path $dirBase 'config'
	
	# tools dirs
	$dirTools = Join-Path $dirBase 'tools'
	$dirStyleCop = Join-Path $dirTools 'StyleCop'
	$dirFxCop = Join-Path $dirTools 'FxCop'
	$dirMsbuildExtensionPack = Join-Path $dirTools 'MsBuild'
	$dirMbUnit = Join-Path $dirTools 'MbUnit'
	$dirNCoverExplorer = Join-Path (Join-Path (Join-Path $dirMbUnit 'NCover') 'libs') 'NCoverExplorer'
	
	# solution files
	$slnCore = Join-Path $dirSrc 'Apollo.Core.sln'
	$msbuildStyleCop = Join-Path $dirTemplates 'StyleCop.msbuild'

	# file templates
	$versionFile = Join-Path $dirBase 'Version.xml'
	$versionTemplateFile = Join-Path $dirTemplates 'AssemblyInfo.VersionNumber.cs.in'
	$versionAssemblyFile = Join-Path $dirSrc 'AssemblyInfo.VersionNumber.cs'
	
	$configurationTemplateFile = Join-Path $dirTemplates 'AssemblyInfo.BuildInformation.cs.in'
	$configurationAssemblyFile = Join-Path $dirSrc 'AssemblyInfo.BuildInformation.cs'
	
	$internalsVisibleToTemplateFile = Join-Path $dirTemplates 'AssemblyInfo.InternalsVisibleTo.cs.in'
	$internalsVisibleToFile = Join-Path $dirSrc 'AssemblyInfo.InternalsVisibleTo.cs'
	
	$licenseVerificationSequencesTemplateFile = Join-Path $dirTemplates 'ValidationSequenceGenerator.cs.in'
	$licenseVerificationSequencesYieldTemplateFile = Join-Path $dirTemplates 'ValidationSequenceGenerator.YieldStatement.cs.in'
	$licenseVerificationSequencesFile = Join-Path $dirSrc 'ValidationSequenceGenerator.cs'
	
	# output files
	$logMsiBuild = 'core_msi.log'
	$logMsBuild = 'core_msbuild.log'
	$logFxCop = 'core_fxcop.xml'
	$logStyleCop = 'core_stylecop.xml'
	$logNCover = 'core_ncover.xml'
	$logNCoverHtml = 'core_ncover.html'
	
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

# Runs the verifications
task Verify -depends runFxCop, runDuplicateFinder

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
	'integrationtest':	Cleans the output directory, builds the binaries and runs the integration tests
	'verify':			Runs the source and binary verification. Returning one or more reports
						describing the flaws in the source / binaries.
	'package':			Packages the deliverables into a single zip file

	./build.ps1 <TARGET>
Multiple build tasks can be specified separated by a comma. Also build tasks can be combined 
in any order. In most cases the build script will ensure that the tasks are executed in the
correct order. Note that this is NOT the case for the 'incremental', 'debug' and 'release' tasks.
In order to get a correct effect these tasks need to be the first tasks being called!
       
In order to run this build script please call this script via PSAKE like:
	invoke-psake core.ps1 incremental,debug,clean,build,unittest,verify 4.0
"@
}

task runClean -precondition{ $shouldClean } -action{
	"Cleaning..."
	
	$msbuildExe = Get-MsbuildExe
	& $msbuildExe $slnCore /t:Clean /verbosity:minimal
	
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
	"Building Apollo.Core..."
	
	# Set the version numbers
	Create-VersionResourceFile $versionTemplateFile $versionAssemblyFile $versionNumber
	
	# Set the configuration
	Create-ConfigurationResourceFile $configurationTemplateFile $configurationAssemblyFile $configuration
	
	# Set the InternalsVisibleTo attribute
	$publicKeyToken = Get-PublicKeySignature $dirTemp $env:SOFTWARE_SIGNING_KEY_PATH
	$friendAssemblyName = $assemblyNameUnitTest + $publicKeyToken
	Create-InternalsVisibleToFile $internalsVisibleToTemplateFile $internalsVisibleToFile $friendAssemblyName
	
	# Create the license verification sequence file
	Create-LicenseVerificationSequencesFile $licenseVerificationSequencesTemplateFile $licenseVerificationSequencesYieldTemplateFile $licenseVerificationSequencesFile

	$logPath = Join-Path $dirLogs $logMsBuild
	
	$msbuildExe = Get-MsbuildExe
	Invoke-MsBuild $slnCore $configuration $logPath 'minimal' ("platform='Any CPU'")
	if ($LastExitCode -ne 0)
	{
		throw "Apollo.Core build failed with return code: $LastExitCode"
	}
	
	# Copy the binaries
	$dirBinCore = Join-Path (Join-Path (Join-Path $dirSrc 'core') 'bin') $configuration
	$dirBinUnit = Join-Path (Join-Path (Join-Path $dirSrc 'core.test.unit') 'bin') $configuration
	$dirBinIntegration = Join-Path (Join-Path (Join-Path $dirSrc 'core.test.integration') 'bin') $configuration
	$dirBinPerf = Join-Path (Join-Path (Join-Path $dirSrc 'core.test.perf') 'bin') $configuration
	
	Copy-Item (Join-Path $dirBinCore '*') $dirBuild -Force
	Copy-Item (Join-Path $dirBinUnit '*') $dirBuild -Force
	Copy-Item (Join-Path $dirBinIntegration '*') $dirBuild -Force
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
		#throw "Code coverage doesn't work at the moment. Please run without coverage"
		
		$coverageFiles = ""
		$coverageAssemblies = Get-ChildItem -path $dirBuild |
			Where-Object { ((($_.Name -like "*Apollo*") -and `
							!( $_.Name -like "*Utils.*") -and `
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
		$command += " //ea Apollo.Utils.ExcludeFromCoverageAttribute;System.Runtime.CompilerServices.CompilerGeneratedAttribute' "
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
		
		if (!(Test-Path -Path (Join-Path $dirReports $logNCover)))
		{
			Move-Item -Path (Join-Path $dirBase 'Coverage.xml') -Destination (Join-Path $dirReports $logNCover)
		}
		else
		{
			Remove-Item -Path (Join-Path $dirBase 'Coverage.xml')
		}
	}
	
	if ($LastExitCode -ne 0)
	{
		throw "MbUnit failed on Apollo.Core with return code: $LastExitCode"
	}
	
	# Generate the code coverage HTML report
	if ($shouldCheckCoverage)
	{
		$ncoverExplorer = Join-Path $dirNCoverExplorer 'NCoverExplorer.Console.exe'
		$command = '& "' + "$ncoverExplorer" + '" ' + ' "' + (Join-Path $dirReports $logNCover) + '"' + " /h:" + '"' + (Join-Path $dirReports $logNCoverHtml) + '"' + " /r:ModuleClassSummary" + " /m:" + $levelMinCoverage
		if ($verbose)
		{
			$command
		}
		
		Invoke-Expression $command
		if ($LastExitCode -ne 0)
		{
			throw "NCoverExplorer failed on Apollo.Core with return code: $LastExitCode"
		}
	}
}

task runIntegrationTests -depends buildBinaries -action{
	"Running integration tests..."
	"There are currently no integration tests. You should make some ..."
	# ???
}

task runFxCop -depends buildBinaries -action{
	# could set different targets depending on configuration:
	# - skip some rules if in debug mode
	# - fail if in release mode
	
	$fxcopExe = Join-Path $dirFxCop 'FxCopcmd.exe'
	$rulesDir = Join-Path $dirFxCop 'Rules'
	$outFile = Join-Path $dirReports $logFxCop
	
	$assemblies = Get-ChildItem -path $dirBuild -Filter "*.dll" | 
		Where-Object { (($_.Name -like "*Apollo*") -and `
						!( $_.Name -like "*Utils*") -and `
						!( $_.Name -like "*Test*"))}

	$files = ""
	$assemblies | ForEach-Object -Process { $files += "/file:" + '"' + $_.FullName + '" '}
	
	$command = "& '" + "$fxcopExe" + "' " + "$files /rule:" + "'" + "$rulesDir" + "'" + " /out:" + "'" + "$outFile" + "'"
	$command
	Invoke-Expression $command
	if ($LastExitCode -ne 0)
	{
		throw "FxCop failed on Apollo.Core with return code: $LastExitCode"
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
						!( $_.Name -like "*Utils.*") -and `
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
	$autofacFile = 'Autofac.dll'
	$autofacStartableFile = 'AutofacContrib.Startable.dll'
	$quickgraph = 'QuickGraph.dll'
	$systemCoreEx = 'System.CoreEx.dll'
	$nlog = 'NLog.dll'
	
	Copy-Item (Join-Path $dirBuild $autofacFile) -Destination (Join-Path $dirTempZip $autofacFile)
	Copy-Item (Join-Path $dirBuild $autofacStartableFile) -Destination (Join-Path $dirTempZip $autofacStartableFile)
	Copy-Item (Join-Path $dirBuild $quickgraph) -Destination (Join-Path $dirTempZip $quickgraph)	
	Copy-Item (Join-Path $dirBuild $systemCoreEx) -Destination (Join-Path $dirTempZip $systemCoreEx)	
	Copy-Item (Join-Path $dirBuild $nlog) -Destination (Join-Path $dirTempZip $nlog)	
	
	# zip them
	# Name the zip: Apollo.Core_<DATE>
	$output = Join-Path $dirDeploy ("Apollo.Core_" + [System.DateTime]::Now.ToString("yyyy_MM_dd-HH_mm_ss") + ".zip")

	"Compressing..."

	# zip the hudson temp dir
	$7zipExe = "$Env:ProgramW6432\7-Zip\7z.exe"
	& $7zipExe a -tzip $output (Get-ChildItem $dirTempZip | foreach { $_.FullName })
	if ($LastExitCode -ne 0)
	{
		throw "Failed to compress the Apollo.Core binaries."
	}
}