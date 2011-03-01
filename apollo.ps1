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
    & invoke-psake $script $targets 4.0
    if (!$psake.build_success)
    {
        throw "$scriptName failed with return code: $LastExitCode"
    }
    
    Print-PrettyPrintHeader "Finished $script"
    ""
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

function global:Get-PublicKeySignatureFromKeyFile([string]$tempDir, [string]$pathToKeyFile)
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

function global:Get-PublicKeySignatureFromAssembly([string]$pathToAssembly)
{
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

function global:Create-InternalsVisibleToFile([string]$path, [string]$newPath, [string[]]$assemblyNames){
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

function global:Create-ConcordionConfigFile([string]$path, [string]$newPath, [string]$concordionOutputPath){
    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
    $text = $text -replace '@OUTPUT_DIR@', $concordionOutputPath
    
    Set-Content $newPath $text
}

function global:Create-SandcastleConfigFile([string]$path, [string]$newPath, [string]$dirTools, [string]$dirDoc, [string]$dirLogs, [string]$dirBin){
    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
    
    $text = $text -replace '@TOOLS_DIR@', $dirTools
    $text = $text -replace '@DOC_DIR@', $dirDoc
    $text = $text -replace '@LOGS_DIR@', $dirLogs
    
    $text = $text -replace '@BIN_DIR@', $dirBin
    
    Set-Content $newPath $text
}

function global:Create-PartCoverConfigFile(
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

function global:Create-LicenseVerificationSequencesFile([string]$generatorTemplate, [string]$yieldTemplate, [string]$newPath){
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

# Properties
properties{
    $dirBase = Get-ScriptLocation

    # solution directories
    $dirBin = Join-Path $dirBase 'bin'
    $dirBuild = Join-Path $dirBin 'build'
    $dirDeploy = Join-Path $dirBin 'deploy'
    $dirLogs = Join-Path $dirBin "logs"
    $dirReports = Join-Path $dirBin 'reports'
    $dirTemp = Join-Path $dirBin 'temp'
    $dirDoc = Join-Path $dirBin 'doc'

    # contents directories
    $dirInstall = Join-Path $dirBase 'install'
    $dirResource = Join-Path $dirBase 'resource'
    $dirTemplates = Join-Path $dirBase 'templates'
    $dirConfiguration = Join-Path $dirBase 'config'
    $dirSrc = Join-Path $dirBase 'src'
   
    # tools directories
    $dirTools = Join-Path $dirBase 'tools'
    $dirBabel = Join-Path $dirTools 'babel'
    $dirMbunit = Join-Path $dirTools 'mbunit'
    $dirNCoverExplorer = Join-Path $dirTools 'ncoverexplorer'
    $dirSandcastle = Join-Path $dirTools 'sandcastle'
    $dirFxCop = Join-Path $dirTools 'FxCop'
    $dirMoq = Join-Path $dirTools 'Moq'
    $dirConcordion = Join-Path $dirTools 'Concordion'
    $dirPartCover = Join-Path $dirTools 'PartCover'
    $dirPartCoverExclusionWriter = Join-Path $dirTools 'partcoverexclusionwriter'
    
    # solutions
    $slnApollo = Join-Path $dirSrc 'Apollo.sln'
    $msbuildSandcastleReferenceData = Join-Path $dirSandcastle 'fxReflection.proj'
    
    # assembly names
    $assemblyNameUnitTest = 'Test.Unit, PublicKey='
    #$assemblyNameSpecTest = 'Test.Spec, PublicKey='
    $assemblyNameDynamicProxy = 'DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7'
    $assemblyNameMoq = 'Moq, PublicKey='
    
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
    
    $partCoverConfigTemplateFile = Join-Path $dirTemplates 'PartCover.Settings.xml.in'
    $partCoverConfigFile = Join-Path $dirTemp 'PartCover.Settings.xml'
    
    $concordionConfigTemplateFile = Join-Path $dirTemplates 'concordion.config.in'
    $sandcastleTemplateFile = Join-Path $dirTemplates 'sandcastle.shfbproj.in'
    
    # output files
    $logMsiBuild = 'msi.log'
    $logMsBuild = 'msbuild.log'
    $logFxCop = 'fxcop.xml'
    $logPartCover = 'partcover.xml'
    $logPartCoverHtml = 'partcover.html'
    
    # output directories
    $dirPartCoverHtml = 'partcoverhtml'
    
    # settings
    $levelMinCoverage = 85
    
    # Version number
    $versionNumber = New-Object -TypeName System.Version -ArgumentList "1.0.0.0"
    $versionFile = Join-Path $dirBase 'Version.xml' 
    
    # script-wide variables
    $shouldCheckCoverage = $false
    $shouldClean = $true
    $configuration = 'debug'
    $dirOutput = ''
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
    Set-Variable -Name dirOutput -Value (Join-Path $dirBuild 'debug') -Scope 2
}

task Release -action{
    Set-Variable -Name configuration -Value 'release' -Scope 2
    Set-Variable -Name dirOutput -Value (Join-Path $dirBuild 'release') -Scope 2
}

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
    'incremental':      Turns on the incremental building of the binaries
    'coverage':         Turns on the code coverage for the unit tests
    'debug':            Runs the script in debug mode. Mutually exclusive with the 'release' task
    'release':          Runs the script in release mode. Mutually exclusive with the 'debug' task
    'clean':            Cleans the output directory
    'build':            Builds the binaries
    'unittest':         Runs the unit tests
    'spectest':         Runs the specification tests
    'integrationtest':  Runs the integration tests
    'verify':           Runs the source and binary verification. Returning one or more reports
                        describing the flaws in the source / binaries.
    'doc':              Runs the documentation build
    'package':          Packages the deliverables into a single zip file

    ./build.ps1 <TARGET>
Multiple build tasks can be specified separated by a comma. Also build tasks can be combined 
in any order. In most cases the build script will ensure that the tasks are executed in the
correct order. Note that this is NOT the case for the 'incremental', 'debug' and 'release' tasks.
In order to get a correct effect these tasks need to be the first tasks being called!
       
In order to run this build script please call this script via PSAKE like:
    invoke-psake apollo.ps1 incremental,debug,clean,build,unittest,verify,doc,package 4.0
"@
}

task runClean  -precondition{ $shouldClean } -action{
    "Cleaning..."

    $msbuildExe = Get-MsbuildExe
    & $msbuildExe $slnApollo /t:Clean /verbosity:minimal
    
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
    
    if (!(Test-Path -Path $dirDoc -PathType Container))
    {
        New-Item $dirDoc -ItemType directory | Out-Null # Don't display the directory information
    }
}

task buildBinaries -depends runInit, getVersion -action{
    "Building Apollo..."
    
    # Set the version numbers
    Create-VersionResourceFile $versionTemplateFile $versionAssemblyFile $versionNumber
    
    # Set the configuration
    Create-ConfigurationResourceFile $configurationTemplateFile $configurationAssemblyFile $configuration
    
    # Set the InternalsVisibleTo attribute
    $publicKeyToken = Get-PublicKeySignatureFromKeyFile $dirTemp $env:SOFTWARE_SIGNING_KEY_PATH
    $unitTestAssemblyName = $assemblyNameUnitTest + $publicKeyToken
    
    $publicKeyToken = Get-PublicKeySignatureFromAssembly (Join-Path $dirMoq 'Moq.dll')
    $moqAssemblyName = $assemblyNameMoq + $publicKeyToken
    Create-InternalsVisibleToFile $internalsVisibleToTemplateFile $internalsVisibleToFile ($unitTestAssemblyName, $moqAssemblyName, $assemblyNameDynamicProxy)
    
    # Create the license verification sequence file
    Create-LicenseVerificationSequencesFile $licenseVerificationSequencesTemplateFile $licenseVerificationSequencesYieldTemplateFile $licenseVerificationSequencesFile

    $logPath = Join-Path $dirLogs $logMsBuild
    
    $msbuildExe = Get-MsbuildExe
    Invoke-MsBuild $slnApollo $configuration $logPath 'minimal' ("platform='Any CPU'")
    if ($LastExitCode -ne 0)
    {
        throw "Apollo build failed with return code: $LastExitCode"
    }
}

task runUnitTests -depends buildBinaries -action{
    "Running unit tests..."
    
    $gallioExe = 'Gallio.Echo.x86.exe'
    
    $files = ""
    $assemblies = Get-ChildItem -path $dirOutput -Filter "*.dll" | 
        Where-Object { (( $_.Name -like "*Test*") -and `
                        ( $_.Name -like "*Unit*"))}
    $assemblies | ForEach-Object -Process { $files += '"' + $_.FullName + '" '}
    
    $command = ""
    if ($shouldCheckCoverage)
    {
        $coverageFiles = ""
        $coverageAssemblies = Get-ChildItem -path $dirOutput |
            Where-Object { ((($_.Name -like "*Apollo*") -and `
                            !( $_.Name -like "*Test.*") -and `
                            !($_.Name -like "*vshost*")) -and `
                            (($_.Extension -match ".dll") -or `
                             ($_.Extension -match ".exe")))}
                             
        $coverageAssemblies
        $coverageAssemblies | ForEach-Object -Process { $coverageFiles += '"' + [System.IO.Path]::GetFullPath($_.FullName) + '" '}
        $coverageFiles
        
        $reportFile = Join-Path $dirReports $logPartCover
        
        # Create the config file
        Create-PartCoverConfigFile $partCoverConfigTemplateFile $partCoverConfigFile $dirMbUnit $gallioExe $dirOutput $dirReports $files $reportFile

        # Add the exclusions
        $writer = Join-Path $dirPartCoverExclusionWriter 'partcoverexclusionwriter.exe'
        $writerCommand = '& "' + $writer + '" ' + "/i " + '"' + $partCoverConfigFile + '" ' + "/o " + '"' + $partCoverConfigFile + '"'
        $writerCommand += " /e Apollo.Utils.ExcludeFromCoverageAttribute System.Runtime.CompilerServices.CompilerGeneratedAttribute"
        $writerCommand += " /a " + $coverageFiles
        
        $writerCommand
        Invoke-Expression $writerCommand
        if ($LastExitCode -ne 0)
        {
           throw 'PartCoverExclusionWriter failed on Apollo with return code: $LastExitCode'
        }
        
        $partCoverExe = Join-Path $dirPartCover 'PartCover.exe'
        $command += '& "' + "$partCoverExe" + '" --register' 
        $command += ' --settings "' + $partCoverConfigFile + '" '
        
        # run the tests
        $command
        
        #Invoke-Expression $command | out-null
        & $partCoverExe --register --settings $partCoverConfigFile
        "" # Add an extra line because PartCover is retarded and doesn't do a writeline at the end
        if ($LastExitCode -ne 0)
        {
            throw "MbUnit failed on Apollo with return code: $LastExitCode"
        }
        
        $transformExe = Join-Path (Join-Path $dirSandcastle "ProductionTools")"xsltransform.exe"
        $partCoverXslt = Join-Path (Join-Path $dirPartCover 'xslt') "partcoverfullreport.xslt"
        $partCoverHtml = Join-Path $dirReports $logPartCoverHtml
        $command = '& "' + $transformExe + '" "' + $reportFile + '" /xsl:"' + $partCoverXslt + '" /out:"' + $partCoverHtml + '"'
        
        $command
        Invoke-Expression $command
        if ($LastExitCode -ne 0)
        {
            throw "XSLT transformation failed on Apollo with return code: $LastExitCode"
        }
    }
    else
    {
        $mbunitExe = Join-Path $dirMbUnit $gallioExe
        
        $command = '& "' + "$mbunitExe" + '" ' + '/hd:"' + $dirMbUnit + '" /sc '
    
        # Run mbunit in an isolated process. On a 64-bit machine gallio ALWAYS starts as a 64-bit
        #   process. This means we can't load explicit 32-bit binaries. However using the 
        #   isolated process runner we can
        $command += "/r:IsolatedProcess " 
        
        # add the files.
        $command += ' /rd:"' + $dirReports + '" /v:Verbose /rt:XHtml /rt:Xml-inline ' + $files
        
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
    $mbunitExe = Join-Path $dirMbUnit 'Gallio.Echo.exe'
    
    $files = ""
    $assemblies = Get-ChildItem -path $dirOutput -Filter "*.dll" | Where-Object { ((( $_.Name -like "*Test*") -and ( $_.Name -like "*Spec*") -and !($_.Name -like "*vshost*")))}

    # Create the concordion config file and copy it
    $specAssembly = $assemblies | select -First 1
    $configFile = Join-Path $dirOutput ([System.IO.Path]::GetFileNameWithoutExtension($specAssembly.FullName) + '.config')
    Create-ConcordionConfigFile $concordionConfigTemplateFile $configFile $dirReports    
    
    $assemblies | ForEach-Object -Process { $files += '"' + $_.FullName + '" '}
    $command = '& "' + "$mbunitExe" + '" ' + '/hd:"' + $dirMbUnit + '" /sc /pd:"' + $dirConcordion + '" '
    
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
    
    $fxcopExe = Join-Path $dirFxCop 'FxCopcmd.exe'
    $rulesDir = Join-Path $dirFxCop 'Rules'
    $outFile = Join-Path $dirReports $logFxCop
    
    $assemblies = Get-ChildItem -path $dirOutput -Filter "*.dll" | 
        Where-Object { ((($_.Name -like "*Apollo*") -and `
                        !( $_.Name -like "*Test.*") -and `
                        !($_.Name -like "*vshost*")) -and `
                        (($_.Extension -match ".dll") -or `
                        ($_.Extension -match ".exe")))}

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

task buildApiDocs -depends buildBinaries -action{
    "Build the API docs..."
    
    # generate the sandcastle file
    $sandcastleFile = Join-Path $dirTemp 'apollo.shfbproj'
    Create-SandcastleConfigFile $sandcastleTemplateFile $sandcastleFile $dirTools $dirDoc $dirLogs $dirOutput
    
    # Set the DXROOT Environment variable
    $Env:DXROOT = $dirSandcastle
    
    $msbuildExe = "c:\windows\Microsoft.NET\Framework\v3.5\MSBuild.exe"
    
    # See if we need to create the reference data.
    $dirSandcastleReference = Join-Path $dirSandcastle 'Data'
    if (!(Test-Path -Path $dirSandcastleReference -PathType Container))
    {
        "Building the Sandcastle reference data. This may take a while ... "
        & $msbuildExe $msbuildSandcastleReferenceData
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
    
    if( $configuration -eq 'release')
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
    $assemblies = Get-ChildItem -path $dirOutput | 
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
    Copy-Item (Join-Path $dirOutput $lokadFile) -Destination (Join-Path $dirTempZip $lokadFile)
   
    $autofacFile = 'Autofac.dll'
    Copy-Item (Join-Path $dirOutput $autofacFile) -Destination (Join-Path $dirTempZip $autofacFile)
    
    $autofacStartableFile = 'AutofacContrib.Startable.dll'
    Copy-Item (Join-Path $dirOutput $autofacStartableFile) -Destination (Join-Path $dirTempZip $autofacStartableFile)
    
    $quickgraph = 'QuickGraph.dll'
    Copy-Item (Join-Path $dirOutput $quickgraph) -Destination (Join-Path $dirTempZip $quickgraph)    
    
    $nlog = 'NLog.dll'
    Copy-Item (Join-Path $dirOutput $nlog) -Destination (Join-Path $dirTempZip $nlog)    
    
    $prismFile = 'Microsoft.Practices.Composite.dll'
    Copy-Item (Join-Path $dirOutput $prismFile) -Destination (Join-Path $dirTempZip $prismFile)
    
    $prismPresentationFile = 'Microsoft.Practices.Composite.Presentation.dll'
    Copy-Item (Join-Path $dirOutput $prismPresentationFile) -Destination (Join-Path $dirTempZip $prismPresentationFile)
    
    $serviceLocationFile = 'Microsoft.Practices.ServiceLocation.dll'
    Copy-Item (Join-Path $dirOutput $serviceLocationFile) -Destination (Join-Path $dirTempZip $serviceLocationFile)
    
    $graphSharpFile = 'GraphSharp.dll'
    Copy-Item (Join-Path $dirOutput $graphSharpFile) -Destination (Join-Path $dirTempZip $graphSharpFile)
    
    $graphSharpControlsFile = 'GraphSharp.Controls.dll'
    Copy-Item (Join-Path $dirOutput $graphSharpControlsFile) -Destination (Join-Path $dirTempZip $graphSharpControlsFile)
    
    $wpfExtensionsFile = 'WPFExtensions.dll'
    Copy-Item (Join-Path $dirOutput $wpfExtensionsFile) -Destination (Join-Path $dirTempZip $wpfExtensionsFile)
    
    $greyableImageFile = 'GreyableImage.dll'
    Copy-Item (Join-Path $dirOutput $greyableImageFile) -Destination (Join-Path $dirTempZip $greyableImageFile)
    
    $pixelLabCommonFile = 'PixelLab.Common.dll'
    Copy-Item (Join-Path $dirOutput $pixelLabCommonFile) -Destination (Join-Path $dirTempZip $pixelLabCommonFile)
    
    $pixelLabWpfFile = 'PixelLab.Wpf.dll'
    Copy-Item (Join-Path $dirOutput $pixelLabWpfFile) -Destination (Join-Path $dirTempZip $pixelLabWpfFile)
    
    $wpfLocalizationFile = 'WPFLocalizeExtension.dll'
    Copy-Item (Join-Path $dirOutput $wpfLocalizationFile) -Destination (Join-Path $dirTempZip $wpfLocalizationFile)
    
    # zip them
    # Name the zip: Apollo_<DATE>
    $output = Join-Path $dirDeploy ("Apollo_" + [System.DateTime]::Now.ToString("yyyy_MM_dd-HH_mm_ss") + ".zip")

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
    "Assembling installer..."
    
    # Grab all the merge modules and make them into a single installer
    # Installers are created per UI. Each UI will have a different installer?
    
}