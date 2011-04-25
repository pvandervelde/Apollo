function Get-ScriptLocation{
    Split-Path $MyInvocation.ScriptName
}

function Print-PrettyPrintHeader([string]$value){
    "=" * 15 + " " + $value + " " + "="*15
}

function Invoke-PsakeScript([string]$script, [String[]]$targets){

    Print-PrettyPrintHeader "Starting $script"
    ""
    & invoke-psake $script $targets 4.0
    if (!$psake.build_success)
    {
        throw "$scriptName failed with return code: $LastExitCode"
    }
    
    Print-PrettyPrintHeader "Finished $script"
    ""
}

function Get-MsBuildExe
{
    'msbuild'
}

function Invoke-MsBuild([string]$solution, [string]$configuration, [string]$logPath, [string]$verbosity, [string[]]$parameters, [bool]$incremental){
	$msbuildExe = Get-MsBuildExe
	$msBuildParameters = "/p:Configuration=$configuration"
	if ($parameters.Length -ne 0)
	{
		$msBuildParameters += ' /p:' +  ([string]::Join(" /p:", ( $parameters )))
	}
	
	# for the moment we will not be using the /m switch because that creates
	# problems in several projects (with random linker errors etc.)
	$command = "$msbuildExe"
	$command += " '"
	$command += "$solution"
	$command += "'"
	$command += " $msbuildParameters"
    $command += " /m"
	
	if (!$incremental){
		$command += " /t:rebuild"
	}
	
	$command += " /clp:Verbosity=$verbosity /clp:Summary /clp:NoItemAndPropertyList /clp:ShowTimestamp"
	$command += " /flp:LogFile='$logPath' /flp:verbosity=$verbosity /flp:Summary /flp:ShowTimestamp"

	"Building $solution with command: $command"
	Invoke-Expression $command
	if ($LastExitCode -ne 0)
	{
		throw "$solution build failed with return code: $LastExitCode"
	}
}

function Get-BzrExe{
    'bzr'
}

function Get-BzrVersion{
    # NOTE: Do not put output text in this method because it will be appended
    # to the return value, which is very unhelpful

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

function Get-PublicKeySignatureFromKeyFile([string]$tempDir, [string]$pathToKeyFile)
{
    # NOTE: Do not put output text in this method because it will be appended
    # to the return value, which is very unhelpful
    
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

function Get-PublicKeySignatureFromAssembly([string]$pathToAssembly)
{
    # NOTE: Do not put output text in this method because it will be appended
    # to the return value, which is very unhelpful
    
    $sn = "${Env:ProgramFiles(x86)}\Microsoft SDKs\Windows\v7.0A\bin\sn.exe"

    # use snk to get the public key bit
    $output = & $sn -Tp $pathToAssembly
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

function Create-VersionResourceFile([string]$path, [string]$newPath, [System.Version]$versionNumber){
    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
    $text = $text -replace '@MAJOR@', $versionNumber.Major
    $text = $text -replace '@MINOR@', $versionNumber.Minor
    $text = $text -replace '@BUILD@', $versionNumber.Build
    $text = $text -replace '@REVISION@', $versionNumber.Revision

    Set-Content -Path $newPath -Value $text
}

function Create-ConfigurationResourceFile([string]$path, [string]$newPath, [string]$config){
    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
    $text = $text -replace '@COPYRIGHTYEAR@', [DateTimeOffset]::Now.Year
    
    $text = $text -replace '@CONFIGURATION@', $config
    
    $now = [DateTimeOffset]::Now
    $text = $text -replace '@BUILDTIME@', $now.ToString("o")

    Set-Content -Path $newPath -Value $text
}

function Create-InternalsVisibleToFile([string]$path, [string]$newPath, [string[]]$assemblyNames){
    $attribute = '[assembly: InternalsVisibleTo("@ASSEMBLYNAME@")]'
    
    $inputText = ''
    $assemblyNames | foreach{
        $inputText += $attribute -replace '@ASSEMBLYNAME@', $_
        $inputText += [System.Environment]::NewLine
    }
    
    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
    $text = $text -replace '@ATTRIBUTES@', $inputText
    
    Set-Content $newPath $text
}

function Create-ConcordionConfigFile([string]$path, [string]$newPath, [string]$concordionOutputPath){
    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
    $text = $text -replace '@OUTPUT_DIR@', $concordionOutputPath
    
    Set-Content $newPath $text
}

function Create-SandcastleConfigFile([string]$path, [string]$newPath, [string]$dirTools, [string]$dirDoc, [string]$dirLogs, [string]$dirBin){
    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
    
    $text = $text -replace '@TOOLS_DIR@', $dirTools
    $text = $text -replace '@DOC_DIR@', $dirDoc
    $text = $text -replace '@LOGS_DIR@', $dirLogs
    
    $text = $text -replace '@BIN_DIR@', $dirBin
    
    Set-Content $newPath $text
}

function Create-PartCoverConfigFile(
    [string]$path, 
    [string]$newPath,
    [string]$dirGallio,
    [string]$exeGallio,
    [string]$dirBuild,
    [string]$dirReportsGallio,
    [string]$files,
    [string]$reportFile)
{
    $inputText = ''
    $assemblyNames | foreach{
        $inputText += $attribute -replace '@ASSEMBLYNAME@', $_
        $inputText += [System.Environment]::NewLine
    }
    
    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
	$text = $text -replace '@{GALLIO_DIR}@', $dirGallio
    $text = $text -replace '@{GALLIO_EXE}@', $exeGallio
    $text = $text -replace '@{BIN_DIR}@', $dirBuild
    $text = $text -replace '@{FILES}@', $files
    $text = $text -replace '@{REPORTS_DIR}@', $dirReportsGallio
    $text = $text -replace '@{REPORT}@', $reportFile
	
	Set-Content $newPath $text
}

function Create-LicenseVerificationSequencesFile([string]$generatorTemplate, [string]$yieldTemplate, [string]$newPath){
    $yieldText = [string]::Join([Environment]::NewLine, (Get-Content -Path $yieldTemplate))
    
    # generate the sequences
    # Check each hour after build
    $sequenceText += $yieldText
    $sequenceText = $sequenceText -replace '@REPEATPERIOD@', 'Hourly'
    $sequenceText = $sequenceText -replace '@MODIFIER@', '1'
    $sequenceText = $sequenceText -replace '@ISPERIODIC@', 'true'
    $sequenceText = $sequenceText -replace '@START_TIME@', 'BuildTime()'
    $sequenceText += [Environment]::NewLine
    $sequenceText += [Environment]::NewLine
    
    # Check each hour after install
    $sequenceText += $yieldText
    $sequenceText = $sequenceText -replace '@REPEATPERIOD@', 'Hourly'
    $sequenceText = $sequenceText -replace '@MODIFIER@', '1'
    $sequenceText = $sequenceText -replace '@ISPERIODIC@', 'true'
    $sequenceText = $sequenceText -replace '@START_TIME@', 'InstallTime()'
    $sequenceText += [Environment]::NewLine
    $sequenceText += [Environment]::NewLine
    
    # Check each hour after start
    $sequenceText += $yieldText
    $sequenceText = $sequenceText -replace '@REPEATPERIOD@', 'Hourly'
    $sequenceText = $sequenceText -replace '@MODIFIER@', '1'
    $sequenceText = $sequenceText -replace '@ISPERIODIC@', 'true'
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
    $sequenceText = $sequenceText -replace '@ISPERIODIC@', 'true'
    $sequenceText = $sequenceText -replace '@START_TIME@', $timeText
    $sequenceText += [Environment]::NewLine
    $sequenceText += [Environment]::NewLine
    
    # Check within 3 minutes after start-up
    $sequenceText += $yieldText
    $sequenceText = $sequenceText -replace '@REPEATPERIOD@', 'Minutely'
    $sequenceText = $sequenceText -replace '@MODIFIER@', '1'
    $sequenceText = $sequenceText -replace '@ISPERIODIC@', 'false'
    $sequenceText = $sequenceText -replace '@START_TIME@', 'ProcessStartTime()'
    $sequenceText += [Environment]::NewLine
    $sequenceText += [Environment]::NewLine
    
    # Check on a bunch of random dates
    $timeText = 'new DateTimeOffset(2010, 08, 10, 23, 43, 05, 00, new TimeSpan(' + $time.Offset.Ticks + '))'
    $sequenceText += $yieldText
    $sequenceText = $sequenceText -replace '@REPEATPERIOD@', 'Hourly'
    $sequenceText = $sequenceText -replace '@MODIFIER@', '1'
    $sequenceText = $sequenceText -replace '@ISPERIODIC@', 'false'
    $sequenceText = $sequenceText -replace '@START_TIME@', $timeText
    
    # Write the sequences to the file
    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $generatorTemplate))
    $text = $text -replace '@YIELD_STATEMENTS@', $sequenceText
    
    Set-Content $newPath $text
}

# Create the dependecy.wxs file. This file contains all the
# directory paths for the binaries, documentation etc. etc.
function Create-InstallerDependencyFile(
    [string]$path,
    [string]$newPath,
    [string]$binFolder,
    [string]$resourceFolder,
    [string]$setupFolder)
{
    # Store the text that is displayed at the top of the file
    # to notify users that the file is generated
    $warningText = 
@"
   <!--
      This is a generated file.
      Do NOT make changes to this file.
      They will be undone next time the file is generated.
   -->
"@

    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
    
    $text = $text -replace '@WARNINGTEXT@', $warningText
    
    $text = $text -replace '@APOLLO_BINFOLDER@', $binFolder
    $text = $text -replace '@APOLLO_RESOURCEFOLDER@', $resourceFolder
    $text = $text -replace '@APOLLO_SETUPFOLDER@', $setupFolder

    Set-Content -Path $newPath -Value $text
}

properties{
    # Define a hashtable that will be used to store all the build information
    # like the configuration, platform and all the directory paths etc.
    $props = @{}
    
    # define variables for the commandline properties. These provide
    # the default values which will be used in case the user doesn't 
    # provide a value.
    $coverage = $false
    $incremental = $false
    $configuration = 'debug'
    $platform = 'Any CPU'
    
    # Store the defaults in the hashtable for later use
    $props.coverage = $coverage
    $props.incremental = $incremental
    $props.configuration = $configuration
    $props.platform = $platform

    # solution directories
    $props.dirBase = Get-ScriptLocation
    $props.dirBuild = Join-Path $props.dirBase 'build'
    $props.dirBin = Join-Path $props.dirBuild 'bin'
    $props.dirDeploy = Join-Path $props.dirBuild 'deploy'
    $props.dirLogs = Join-Path $props.dirBuild "logs"
    $props.dirReports = Join-Path $props.dirBuild 'reports'
    $props.dirTemp = Join-Path $props.dirBuild 'temp'
    $props.dirDoc = Join-Path $props.dirBuild 'doc'
    
    # contents directories
    $props.dirInstall = Join-Path $props.dirBase 'install'
    $props.dirResource = Join-Path $props.dirBase 'resource'
    $props.dirTemplates = Join-Path $props.dirBase 'templates'
    $props.dirConfiguration = Join-Path $props.dirBase 'config'
    $props.dirSrc = Join-Path $props.dirBase 'src'
    
    $props.dirBinInstall = Join-Path $props.dirInstall 'bin'
   
    # tools directories
    $props.dirTools = Join-Path $props.dirBase 'tools'
    $props.dirBabel = Join-Path $props.dirTools 'babel'
    $props.dirMbunit = Join-Path $props.dirTools 'mbunit'
    $props.dirNCoverExplorer = Join-Path $props.dirTools 'ncoverexplorer'
    $props.dirSandcastle = Join-Path $props.dirTools 'sandcastle'
    $props.dirFxCop = Join-Path $props.dirTools 'FxCop'
    $props.dirMoq = Join-Path $props.dirTools 'Moq'
    $props.dirConcordion = Join-Path $props.dirTools 'Concordion'
    $props.dirPartCover = Join-Path $props.dirTools 'PartCover'
    $props.dirPartCoverExclusionWriter = Join-Path $props.dirTools 'partcoverexclusionwriter'
    
    # solutions
    $props.slnApollo = Join-Path $props.dirSrc 'Apollo.sln'
    $props.slnApolloWix = Join-Path $props.dirInstall 'Apollo.sln'
    $props.msbuildSandcastleReferenceData = Join-Path $props.dirSandcastle 'fxReflection.proj'
    
    # assembly names
    $props.assemblyNameUnitTest = 'Test.Unit, PublicKey='
    #$props.assemblyNameSpecTest = 'Test.Spec, PublicKey='
    $props.assemblyNameManualTest = 'Test.Manual.Console, PublicKey='
    $props.assemblyNameDynamicProxy = 'DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7'
    $props.assemblyNameMoq = 'Moq, PublicKey='

    
    # file templates
    $props.versionFile = Join-Path $props.dirBase 'Version.xml'
    $props.versionTemplateFile = Join-Path $props.dirTemplates 'AssemblyInfo.VersionNumber.cs.in'
    $props.versionAssemblyFile = Join-Path $props.dirSrc 'AssemblyInfo.VersionNumber.cs'
    
    $props.configurationTemplateFile = Join-Path $props.dirTemplates 'AssemblyInfo.BuildInformation.cs.in'
    $props.configurationAssemblyFile = Join-Path $props.dirSrc 'AssemblyInfo.BuildInformation.cs'
    
    $props.internalsVisibleToTemplateFile = Join-Path $props.dirTemplates 'AssemblyInfo.InternalsVisibleTo.cs.in'
    $props.internalsVisibleToFile = Join-Path $props.dirSrc 'AssemblyInfo.InternalsVisibleTo.cs'
    
    $props.licenseVerificationSequencesTemplateFile = Join-Path $props.dirTemplates 'ValidationSequenceGenerator.cs.in'
    $props.licenseVerificationSequencesYieldTemplateFile = Join-Path $props.dirTemplates 'ValidationSequenceGenerator.YieldStatement.cs.in'
    $props.licenseVerificationSequencesFile = Join-Path $props.dirSrc 'ValidationSequenceGenerator.cs'
    
    $props.partCoverConfigTemplateFile = Join-Path $props.dirTemplates 'PartCover.Settings.xml.in'
    $props.partCoverConfigFile = Join-Path $props.dirTemp 'PartCover.Settings.xml'
    
    $props.concordionConfigTemplateFile = Join-Path $props.dirTemplates 'concordion.config.in'
    $props.sandcastleTemplateFile = Join-Path $props.dirTemplates 'sandcastle.shfbproj.in'
    
    $props.wixVersionTemplateFile = Join-Path $props.dirInstall 'VersionNumber.wxi.in'
    $props.wixVersionFile = Join-Path $props.dirInstall 'VersionNumber.wxi'
    
    $props.dependenciesTemplateFile = Join-Path $props.dirInstall 'Dependencies.wxi.in'
    $props.dependenciesFile = Join-Path $props.dirInstall 'Dependencies.wxi'
    
    # output files
    $props.logMsiBuild = 'msi.log'
    $props.logMsBuild = 'msbuild.log'
    $props.logFxCop = 'fxcop.xml'
    $props.logPartCover = 'partcover.xml'
    $props.logPartCoverHtml = 'partcover.html'
    
    # output directories
    $props.dirPartCoverHtml = 'partcoverhtml'
    
    # settings
    $props.levelMinCoverage = 85
    
    # Version number
    $props.versionNumber = New-Object -TypeName System.Version -ArgumentList "1.0.0.0"
    $props.versionFile = Join-Path $props.dirBase 'Version.xml' 
}

# The default task doesn't do anything. This just calls the help function. Useful
#   for new people
task default -depends Help

# Cleans all the generated files
task Clean -depends runClean

# Builds all the binaries
task Build -depends buildBinaries

# Runs the unit tests
task UnitTest -depends runUnitTests

# Runs the Specification tests
task SpecTest -depends runSpecificationTests

# Runs the integration tests
task IntegrationTest -depends runIntegrationTests

# Runs the verifications
task Verify -depends runFxCop, runDuplicateFinder

# Runs the documentation build
task Doc -depends buildApiDocs, buildUserDoc

# Creates the zip file of the deliverables
task Package -depends buildPackage, assembleInstaller

###############################################################################
# HELPER TASKS

task getVersion -action{
    #Get the file version from the version.xml file
    [xml]$xmlFile = Get-Content $props.versionFile
    $major = $xmlFile.version | %{$_.major} | Select-Object -Unique
    $minor = $xmlFile.version | %{$_.minor} | Select-Object -Unique
    $build = $xmlFile.version | %{$_.build} | Select-Object -Unique
    $revision = Get-BzrVersion
    $props.versionNumber = New-Object -TypeName System.Version -ArgumentList "$major.$minor.$build.$revision"
    ("version is: " + $props.versionNumber )
}

###############################################################################
# EXECUTING TASKS

# The Help task displays the available commandline arguments
task Help -action{
    # because powershell is being cunning we define two strings for the $true and $false boolean values
    $trueText = '$true' # note the use of the single quotes to stop powershell from expanding the 'variable'
    $falseText = '$false'
@"
In order to run this build script please call a specific target.
The following build properties are available:
    'incremental':      Turns on or off the incremental building of the binaries. Default is off.
    'coverage':         Turns on or off the unit testing coverage check. Default is off.
    'configuration':    Defines the configuration for the build. Valid values are 'debug' and 'release', default value is 'debug'.
    'platform':         Defines the platform for the build. Valid values are 'Any CPU', default value is 'Any CPU'.

The following build tasks are available
    'clean':            Cleans the output directory
    'build':            Builds the binaries
    'unittest':         Runs the unit tests
    'spectest':         Runs the specification tests
    'integrationtest':  Runs the integration tests
    'verify':           Runs the source and binary verification. Returning one or more reports
                        describing the flaws in the source / binaries.
    'doc':              Runs the documentation build
    'package':          Packages the deliverables into a single zip file and into an MSI installer file

Multiple build tasks can be specified separated by a comma. 
       
In order to run this build script please call this script via PSAKE like:
    invoke-psake apollo.ps1 -properties @{ "incremental"=$trueText;"coverage"=$trueText;"configuration"="debug";"platform"="Any CPU" } clean,build,unittest,spectest,integrationtest,verify,doc,package 4.0
"@
}

task runInit -action{
    $props.incremental = $incremental
    $props.coverage = $coverage
    $props.configuration = $configuration
    $props.platform = $platform
    
    $props.platformForPaths = $props.platform.Replace(" ", "")
    $props.dirOutput = Join-Path (Join-Path $props.dirBin $props.platformForPaths) $props.configuration    
}

# Displays the starting information for the build, including the start time
task displayInfo -depends runInit -action{
    $date = [System.DateTime]::Now
    $user = [Security.Principal.WindowsIdentity]::GetCurrent().Name

    ""
    $date
    "Starting build of Apollo"
    "Running as user: $user"
    ("Configuration:   " + $props.configuration)
    ("Platform:        " + $props.platform)
    ""
}

# Note that the precondition is defined based on the $incremental property because
# it seems that psake determines the values of these preconditions based on values 
# available when the script is started, not values becoming available later on.
task runClean -depends displayInfo -precondition{ !$incremental } -action{
    $msbuildExe = Get-MsbuildExe
    
    "Cleaning the apollo solution directories..."
    & $msbuildExe $props.slnApollo /t:Clean /verbosity:minimal
    
    "Cleaning the installer solution directories..."
    & $msbuildExe $props.slnApolloWix /t:Clean /verbosity:minimal
    
    if (Test-Path -Path $props.dirBuild -PathType Container)
    {
        Remove-Item $props.dirBuild -Force -Recurse
    }
}

task runPrepareDisk -depends displayInfo,runClean -action{
    "Initializing build..."

    if (!(Test-Path -Path $props.dirBuild -PathType Container))
    {
        New-Item $props.dirBuild -ItemType directory | Out-Null
    }
    
    if (!(Test-Path -Path $props.dirBin -PathType Container))
    {
        New-Item $props.dirBin -ItemType directory | Out-Null
    }
    
    if (!(Test-Path -Path $props.dirTemp -PathType Container))
    {
        New-Item $props.dirTemp -ItemType directory | Out-Null
    }
    
    if (!(Test-Path -Path $props.dirLogs -PathType Container))
    {
        New-Item $props.dirLogs -ItemType directory | Out-Null
    }
    
    if (!(Test-Path -Path $props.dirReports -PathType Container))
    {
        New-Item $props.dirReports -ItemType directory | Out-Null
    }
    
    if (!(Test-Path -Path $props.dirDeploy -PathType Container))
    {
        New-Item $props.dirDeploy -ItemType directory | Out-Null
    }
    
    if (!(Test-Path -Path $props.dirDoc -PathType Container))
    {
        New-Item $props.dirDoc -ItemType directory | Out-Null
    }
}

task buildBinaries -depends runPrepareDisk, getVersion -action{
    "Building Apollo..."
    
    # Set the version numbers
    Create-VersionResourceFile $props.versionTemplateFile $props.versionAssemblyFile $props.versionNumber
    
    # Set the configuration
    Create-ConfigurationResourceFile $props.configurationTemplateFile $props.configurationAssemblyFile $props.configuration
    
    # Set the InternalsVisibleTo attribute
    $publicKeyToken = Get-PublicKeySignatureFromKeyFile $props.dirTemp $env:SOFTWARE_SIGNING_KEY_PATH
    $unitTestAssemblyName = $props.assemblyNameUnitTest + $publicKeyToken
    $manualTestAssemblyName = $props.assemblyNameManualTest + $publicKeyToken
    
    $publicKeyToken = Get-PublicKeySignatureFromAssembly (Join-Path $props.dirMoq 'Moq.dll')
    $moqAssemblyName = $props.assemblyNameMoq + $publicKeyToken
    Create-InternalsVisibleToFile $props.internalsVisibleToTemplateFile $props.internalsVisibleToFile ($unitTestAssemblyName, $manualTestAssemblyName, $moqAssemblyName, $props.assemblyNameDynamicProxy)
    
    # Create the license verification sequence file
    Create-LicenseVerificationSequencesFile $props.licenseVerificationSequencesTemplateFile $props.licenseVerificationSequencesYieldTemplateFile $props.licenseVerificationSequencesFile

    $logPath = Join-Path $props.dirLogs $props.logMsBuild
    
    $msbuildExe = Get-MsbuildExe
    Invoke-MsBuild $props.slnApollo $props.configuration $logPath 'minimal' (("platform='" + $props.Platform+ "'")) $props.incremental
    if ($LastExitCode -ne 0)
    {
        throw "Apollo build failed with return code: $LastExitCode"
    }
}

task runUnitTests -depends buildBinaries -action{
    "Running unit tests..."
    
    $gallioExe = 'Gallio.Echo.x86.exe'
    
    $files = ""
    $assemblies = Get-ChildItem -path $props.dirOutput -Filter "*.dll" | 
        Where-Object { (( $_.Name -like "*Test*") -and `
                        ( $_.Name -like "*Unit*"))}
    $assemblies | ForEach-Object -Process { $files += '"' + $_.FullName + '" '}
    
    $command = ""
    if ($props.Coverage)
    {
        $coverageFiles = ""
        $coverageAssemblies = Get-ChildItem -path $props.dirOutput |
            Where-Object { ((($_.Name -like "*Apollo*") -and `
                            !( $_.Name -like "*Test.*") -and `
                            !($_.Name -like "*vshost*")) -and `
                            (($_.Extension -match ".dll") -or `
                             ($_.Extension -match ".exe")))}
                             
        $coverageAssemblies | ForEach-Object -Process { $coverageFiles += '"' + [System.IO.Path]::GetFullPath($_.FullName) + '" '}
        $reportFile = Join-Path $props.dirReports $props.logPartCover
        
        # Create the config file
        Create-PartCoverConfigFile $props.partCoverConfigTemplateFile $props.partCoverConfigFile $props.dirMbUnit $gallioExe $props.dirOutput $props.dirReports $files $reportFile

        # Add the exclusions
        $writer = Join-Path $props.dirPartCoverExclusionWriter 'partcoverexclusionwriter.exe'
        $writerCommand = '& "' + $writer + '" ' + "/i " + '"' + $props.partCoverConfigFile + '" ' + "/o " + '"' + $props.partCoverConfigFile + '"'
        $writerCommand += " /e Apollo.Utils.ExcludeFromCoverageAttribute System.Runtime.CompilerServices.CompilerGeneratedAttribute"
        $writerCommand += " /a " + $coverageFiles
        
        $writerCommand
        Invoke-Expression $writerCommand
        if ($LastExitCode -ne 0)
        {
           throw 'PartCoverExclusionWriter failed on Apollo with return code: $LastExitCode'
        }
        
        $partCoverExe = Join-Path $props.dirPartCover 'PartCover.x86.exe'
        $command += '& "' + "$partCoverExe" + '" --register' 
        $command += ' --settings "' + $props.partCoverConfigFile + '"'
        
        # run the tests
        $command
        
        #Invoke-Expression $command | out-null
        & $partCoverExe --register --settings $partCoverConfigFile
        "" # Add an extra line because PartCover is retarded and doesn't do a writeline at the end
        if ($LastExitCode -ne 0)
        {
            throw "MbUnit failed on Apollo with return code: $LastExitCode"
        }
        
        $transformExe = Join-Path (Join-Path $props.dirSandcastle "ProductionTools")"xsltransform.exe"
        $partCoverXslt = Join-Path (Join-Path $props.dirPartCover 'xslt') "partcoverfullreport.xslt"
        $partCoverHtml = Join-Path $props.dirReports $props.logPartCoverHtml
        $command = '& "' + $transformExe + '" "' + $reportFile + '" /xsl:"' + $partCoverXslt + '" /out:"' + $partCoverHtml + '" '
        
        $command
        Invoke-Expression $command
        if ($LastExitCode -ne 0)
        {
            throw "XSLT transformation failed on Apollo with return code: $LastExitCode"
        }
    }
    else
    {
        $mbunitExe = Join-Path $props.dirMbUnit $gallioExe
        
        $command = '& "' + "$mbunitExe" + '" ' + '/hd:"' + $props.dirMbUnit + '" /sc '
    
        # Run mbunit in an isolated process. On a 64-bit machine gallio ALWAYS starts as a 64-bit
        #   process. This means we can't load explicit 32-bit binaries. However using the 
        #   isolated process runner we can
        $command += "/r:IsolatedProcess " 
        
        # add the files.
        $command += ' /rd:"' + $props.dirReports + '" /v:Verbose /rt:XHtml-Condensed /rt:Xml-inline ' + $files
        
        # run the tests
        $command
        
        Invoke-Expression $command
        "" # Add an extra line because PartCover is retarded and doesn't do a writeline at the end
        if ($LastExitCode -ne 0)
        {
            throw "MbUnit failed on Apollo with return code: $LastExitCode"
        }
    }
}

task runSpecificationTests -depends buildBinaries -action{
    "Running specification tests ..."
    
    # Start the integration tests. First setup the commandline for
    # Concordion
    $mbunitExe = Join-Path $props.dirMbUnit 'Gallio.Echo.exe'
    
    $files = ""
    $assemblies = Get-ChildItem -path $props.dirOutput -Filter "*.dll" | Where-Object { ((( $_.Name -like "*Test*") -and ( $_.Name -like "*Spec*") -and !($_.Name -like "*vshost*")))}

    # Create the concordion config file and copy it
    $specAssembly = $assemblies | select -First 1
    $configFile = Join-Path $props.dirOutput ([System.IO.Path]::GetFileNameWithoutExtension($specAssembly.FullName) + '.config')
    Create-ConcordionConfigFile $props.concordionConfigTemplateFile $configFile $props.dirReports    
    
    $assemblies | ForEach-Object -Process { $files += '"' + $_.FullName + '" '}
    $command = '& "' + "$mbunitExe" + '" ' + '/hd:"' + $props.dirMbUnit + '" /sc /pd:"' + $props.dirConcordion + '" '
    
    # Run mbunit in an isolated process. On a 64-bit machine gallio ALWAYS starts as a 64-bit
    #   process. This means we can't load explicit 32-bit binaries. However using the 
    #   isolated process runner we can
    $command += "/r:Local " 
    
    # add the files.
    $command += $files
    
    # run the tests
    $command
    Invoke-Expression $command
    if ($LastExitCode -ne 0)
    {
        throw "Concordion failed on Apollo.Core with return code: $LastExitCode"
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
    
    $fxcopExe = Join-Path $props.dirFxCop 'FxCopcmd.exe'
    $rulesDir = Join-Path $props.dirFxCop 'Rules'
    $dictionaryFile = Join-Path $props.dirBase 'CustomDictionary.xml'
    $outFile = Join-Path $props.dirReports $props.logFxCop
    
    $assemblies = Get-ChildItem -path $props.dirOutput | 
        Where-Object { ((($_.Name -like "*Apollo*") -and `
                        !( $_.Name -like "*Test.*") -and `
                        !($_.Name -like "*vshost*")) -and `
                        (($_.Extension -like ".dll") -or `
                        ($_.Extension -like ".exe")))}

    $files = ""
    $assemblies | ForEach-Object -Process { $files += "/file:" + '"' + $_.FullName + '" '}
    
    # exclude the fxcop rules we don't want to use.
    # 1006: do not nest generic types in member signatures
    # 1030: use events where appropriate
    $excludedRules = " /ruleid:-Microsoft.Rules.Managed.CA1006 /ruleid:-Microsoft.Rules.Managed.CA1030"
    
    $command = "& '" + "$fxcopExe" + "' " + "$files /rule:+" + "'" + "$rulesDir" + "'" + $excludedRules + " /out:" + "'" + "$outFile" + "' /forceoutput /dictionary:'" + "$dictionaryFile" + "'"
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

task buildApiDocs -depends buildBinaries -action{
    "Build the API docs..."
    
    # generate the sandcastle file
    $sandcastleFile = Join-Path $props.dirTemp 'apollo.shfbproj'
    Create-SandcastleConfigFile $props.sandcastleTemplateFile $sandcastleFile $props.dirTools $props.dirDoc $props.dirLogs $props.dirOutput
    
    # Set the DXROOT Environment variable
    $Env:DXROOT = $props.dirSandcastle
    
    $msbuildExe = "c:\windows\Microsoft.NET\Framework\v3.5\MSBuild.exe"
    
    # See if we need to create the reference data.
    $dirSandcastleReference = Join-Path $props.dirSandcastle 'Data'
    if (!(Test-Path -Path $dirSandcastleReference -PathType Container))
    {
        "Building the Sandcastle reference data. This may take a while ... "
        & $msbuildExe $props.msbuildSandcastleReferenceData
        if ($LastExitCode -ne 0)
        {
            throw "Could not generate the Sandcastle reference data. Return code from MsBuild: $LastExitCode"
        }
    }
    
    & $msbuildExe $sandcastleFile
    if ($LastExitCode -ne 0)
    {
        throw "Sandcastle help file builder failed on Apollo with return code: $LastExitCode"
    }
    
    if( $props.configuration -eq 'release')
    {
        # Should fail are release build if there's anything missing?
    }
}

task buildUserDoc -depends buildBinaries -action{
    "Building user docs..."
    # Build the user docs
}

task buildPackage -depends buildBinaries -action{
    "Packaging the files into a zip ..."
    
    $dirTempZip = Join-Path $props.dirTemp 'zip'
    if((Test-Path -Path $dirTempZip -PathType Container))
    {
        Remove-Item $dirTempZip -Force -Recurse
    }
    
    # The directory does not exist. Create it
    New-Item $dirTempZip -ItemType directory | Out-Null # Don't display all the information
    
    # Copy the files to the temp dir
    # match all files that:
    # - Are DLL, EXE or config files
    # - Have the term Sherlock in their name
    # - Don't have the terms 'Test' or 'vshost' in their name
    $assemblies = Get-ChildItem -path $props.dirOutput | 
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
    Copy-Item (Join-Path $props.dirOutput $lokadFile) -Destination (Join-Path $dirTempZip $lokadFile)
   
    $autofacFile = 'Autofac.dll'
    Copy-Item (Join-Path $props.dirOutput $autofacFile) -Destination (Join-Path $dirTempZip $autofacFile)
    
    $autofacStartableFile = 'AutofacContrib.Startable.dll'
    Copy-Item (Join-Path $props.dirOutput $autofacStartableFile) -Destination (Join-Path $dirTempZip $autofacStartableFile)
    
    $quickgraph = 'QuickGraph.dll'
    Copy-Item (Join-Path $props.dirOutput $quickgraph) -Destination (Join-Path $dirTempZip $quickgraph)    
    
    $nlog = 'NLog.dll'
    Copy-Item (Join-Path $props.dirOutput $nlog) -Destination (Join-Path $dirTempZip $nlog)    
    
    $prismFile = 'Microsoft.Practices.Prism.dll'
    Copy-Item (Join-Path $props.dirOutput $prismFile) -Destination (Join-Path $dirTempZip $prismFile)
    
    $prismInteractivityFile = 'Microsoft.Practices.Prism.Interactivity.dll'
    Copy-Item (Join-Path $props.dirOutput $prismInteractivityFile) -Destination (Join-Path $dirTempZip $prismInteractivityFile)
    
    $serviceLocationFile = 'Microsoft.Practices.ServiceLocation.dll'
    Copy-Item (Join-Path $props.dirOutput $serviceLocationFile) -Destination (Join-Path $dirTempZip $serviceLocationFile)
    
    $graphSharpFile = 'GraphSharp.dll'
    Copy-Item (Join-Path $props.dirOutput $graphSharpFile) -Destination (Join-Path $dirTempZip $graphSharpFile)
    
    $graphSharpControlsFile = 'GraphSharp.Controls.dll'
    Copy-Item (Join-Path $props.dirOutput $graphSharpControlsFile) -Destination (Join-Path $dirTempZip $graphSharpControlsFile)
    
    $wpfExtensionsFile = 'WPFExtensions.dll'
    Copy-Item (Join-Path $props.dirOutput $wpfExtensionsFile) -Destination (Join-Path $dirTempZip $wpfExtensionsFile)
    
    $greyableImageFile = 'GreyableImage.dll'
    Copy-Item (Join-Path $props.dirOutput $greyableImageFile) -Destination (Join-Path $dirTempZip $greyableImageFile)
    
    $pixelLabCommonFile = 'PixelLab.Common.dll'
    Copy-Item (Join-Path $props.dirOutput $pixelLabCommonFile) -Destination (Join-Path $dirTempZip $pixelLabCommonFile)
    
    $pixelLabWpfFile = 'PixelLab.Wpf.dll'
    Copy-Item (Join-Path $props.dirOutput $pixelLabWpfFile) -Destination (Join-Path $dirTempZip $pixelLabWpfFile)
    
    $wpfLocalizationFile = 'WPFLocalizeExtension.dll'
    Copy-Item (Join-Path $props.dirOutput $wpfLocalizationFile) -Destination (Join-Path $dirTempZip $wpfLocalizationFile)
    
    # zip them
    # Name the zip: Apollo_<DATE>
    $output = Join-Path $props.dirDeploy ("Apollo_" + [System.DateTime]::Now.ToString("yyyy_MM_dd-HH_mm_ss") + ".zip")

    "Compressing..."

    # zip the temp dir
    $7zipExe = "$Env:ProgramW6432\7-Zip\7z.exe"
    & $7zipExe a -tzip $output (Get-ChildItem $dirTempZip | foreach { $_.FullName })
    if ($LastExitCode -ne 0)
    {
        throw "Failed to compress the Apollo.Core binaries."
    }
}

task assembleInstaller -depends buildBinaries -action{
    $versionString = [string]::Format('{0}.{1}.{2}.{3}', $props.versionNumber.Major, $props.versionNumber.Minor, $props.versionNumber.Build, $props.versionNumber.Revision)
    $props.versionNumber
    
    "Setting the version number..."
    Create-VersionResourceFile $props.wixVersionTemplateFile $props.wixVersionFile $props.versionNumber
    
    "Creating the dependencies file..."
    Create-InstallerDependencyFile $props.dependenciesTemplateFile $props.dependenciesFile $props.dirOutput $props.dirResource $props.dirInstall
    
    $logPath = Join-Path $props.dirLogs $props.logMsiBuild
    
    #"Building 32-bit Apollo installer"
    $msbuildExe = Get-MsbuildExe
    Invoke-MsBuild $props.slnApolloWix 'release' $logPath 'normal' ("platform='x86'") $false

    $dirBinConfig = Join-Path (Join-Path $props.dirBinInstall 'x86') 'release'
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.msi') -Destination (Join-Path $props.dirDeploy "apollo - x86 - $versionString.msi")
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.batchservice.msi') -Destination (Join-Path $props.dirDeploy "apollo.batchservice - x86 - $versionString.msi")
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.loaderapplication.msi') -Destination (Join-Path $props.dirDeploy "apollo.loaderapplication - x86 - $versionString.msi")

    "Building 64-bit Apollo installer"
    Invoke-MsBuild $props.slnApolloWix 'release' $logPath 'normal' ("platform='x64'") $false
    
    $dirBinConfig = Join-Path (Join-Path $props.dirBinInstall 'x64') 'release'
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.msi') -Destination (Join-Path $props.dirDeploy "apollo - x64 - $versionString.msi")
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.batchservice.msi') -Destination (Join-Path $props.dirDeploy "apollo.batchservice - x64 - $versionString.msi")
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.loaderapplication.msi') -Destination (Join-Path $props.dirDeploy "apollo.loaderapplication - x64 - $versionString.msi")
}
