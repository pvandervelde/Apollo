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

function Create-GeneratedItemsList([string]$path, [string]$outputDir = $null) {
    Out-Host -InputObject $path
    Out-Host -InputObject $outputDir
    
    $startingDirectory = Split-Path $path -Parent
    $paths = New-Object System.Collections.Generic.List``1[System.String]
    Get-Content -Path $path | ForEach-Object {
        if ($_.StartsWith("**\"))
        {
            $filter = $_.Trim("**\")
            Get-ChildItem -Path $startingDirectory -Filter $filter -Recurse | ForEach-Object { $paths.Add($_.FullName) }
        }
        else
        {
            if ($_.StartsWith("{OUTPUT_DIR}\"))
            {
                $filter = $_.Trim("{OUTPUT_DIR}\")
                $item = Join-Path $outputDir $filter
                if (Test-Path -Path $item)
                {
                    $paths.Add($item)
                }
            }
            else
            {
                if ($_ -ne "")
                {
                    $item = Join-Path $startingDirectory $_
                    if (Test-Path -Path $item)
                    {
                        $paths.Add($item)
                    }
                }
            }
        }
    }
    
    return $paths
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

function Get-BuildNumber
{
    if ($env:BUILD_NUMBER -ne $null)
    {
        return $env:BUILD_NUMBER
    }
    
    return 0
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

function Get-SnExe
{
	if (${Env:ProgramFiles(x86)} -ne $null)
	{
		"${Env:ProgramFiles(x86)}\Microsoft SDKs\Windows\v7.0A\bin\sn.exe"
	}
	else
	{
		"$Env:ProgramFiles\Microsoft SDKs\Windows\v7.0A\bin\sn.exe"
	}
}

function Get-PublicKeySignatureFromKeyFile([string]$tempDir, [string]$pathToKeyFile)
{
    # NOTE: Do not put output text in this method because it will be appended
    # to the return value, which is very unhelpful
    
    $sn = Get-SnExe
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
    
    $sn = Get-SnExe

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

function Get-7ZipExe
{
	if ($Env:ProgramW6432 -ne $null)
	{
		"$Env:ProgramW6432\7-Zip\7z.exe"
	}
	else
	{
		"$Env:ProgramFiles\7-Zip\7z.exe"
	}
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

function Create-SourceMonitorInputFile([string]$path, [string]$newPath, [string]$tempDir, [string]$srcDir, [string]$outputFile){
    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
    $text = $text -replace '@PROJECT_FILE_NAME@', (Join-Path $tempDir 'apollo.smp')
    $text = $text -replace '@SOURCE_DIR@', $srcDir
    $text = $text -replace '@DATE@', ([System.DateTimeOffset]::Now.ToString("yyyy-MM-ddTHH:mm:ss"))
    $text = $text -replace '@OUTPUT_FILE@', $outputFile
    
    Set-Content $newPath $text
}

function Create-CcmInputFile([string]$path, [string]$newPath, [string]$srcDir){
    $text = [string]::Join([Environment]::NewLine, (Get-Content -Path $path))
    $text = $text -replace '@SOURCE_FOLDER_NAME@', $srcDir
    
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
	$reloadPackages = $false
    $configuration = 'debug'
    $platform = 'Any CPU'
    
    # Store the defaults in the hashtable for later use
    $props.coverage = $coverage
    $props.incremental = $incremental
	$props.reloadPackages = $reloadPackages
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
    $props.dirMsiInstall = Join-Path $props.dirInstall 'msi'
    $props.dirZipInstall = Join-Path $props.dirInstall 'zip'
    $props.dirResource = Join-Path $props.dirBase 'resource'
    $props.dirTemplates = Join-Path $props.dirBase 'templates'
    $props.dirConfiguration = Join-Path $props.dirBase 'config'
    $props.dirSrc = Join-Path $props.dirBase 'src'
    
    $props.dirBinMsiInstall = Join-Path $props.dirMsiInstall 'bin'
   
    # tools directories
	$props.dirPackages = Join-Path $props.dirBase 'packages'
    $props.dirMbunit = Join-Path $props.dirPackages 'Gallio.Complete'
    $props.dirNCoverExplorer = Join-Path $props.dirPackages 'ncoverexplorer'
    $props.dirSandcastle = Join-Path $props.dirPackages 'sandcastle'
    $props.dirFxCop = Join-Path $props.dirPackages 'FxCop'
    $props.dirMoq = Join-Path (Join-Path (Join-Path $props.dirPackages 'Moq') 'lib') 'NET40'
    $props.dirConcordion = Join-Path (Join-Path $props.dirPackages 'Concordion.Net') 'lib'
    $props.dirPartCover = Join-Path $props.dirPackages 'PartCover'
    $props.dirPartCoverExclusionWriter = Join-Path $props.dirPackages 'partcoverexclusionwriter'
    $props.dirSourceMonitor = Join-Path $props.dirPackages 'SourceMonitor'
    $props.dirCcm = Join-Path $props.dirPackages 'Ccm'
	$props.dirNTreva = Join-Path $props.dirPackages 'nTreva'
	
    $props.dirTools = Join-Path $props.dirBase 'tools'
    
    # solutions
    $props.slnApollo = Join-Path $props.dirSrc 'Apollo.sln'
    $props.slnApolloWix = Join-Path $props.dirMsiInstall 'Apollo.sln'
    $props.msbuildSandcastleReferenceData = Join-Path $props.dirSandcastle 'fxReflection.proj'
    
    # assembly names
    $props.assemblyNameTestUnitHost = 'Test.Unit.Core.Host, PublicKey='
	$props.assemblyNameTestUnitBase = 'Test.Unit.Core.Base, PublicKey='
	$props.assemblyNameTestUnitUi = 'Test.Unit.UI, PublicKey='
	$props.assemblyNameTestUnitUtils = 'Test.Unit.Utilities, PublicKey='
    $props.assemblyNameTestUnitDataset = 'Test.Unit.Dataset, PublicKey='
    
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
    
	$props.licenseInfoFile = Join-Path $props.dirSrc 'licenses.xml'
	
    $props.partCoverConfigTemplateFile = Join-Path $props.dirTemplates 'PartCover.Settings.xml.in'
    $props.partCoverConfigFile = Join-Path $props.dirTemp 'PartCover.Settings.xml'
    
    $props.concordionConfigTemplateFile = Join-Path $props.dirTemplates 'concordion.config.in'
    $props.sandcastleTemplateFile = Join-Path $props.dirTemplates 'sandcastle.shfbproj.in'
    
    $props.sourceMonitorTemplateFile = Join-Path $props.dirTemplates 'sourcemonitor.xml.in'
    $props.sourceMonitorFile = Join-Path $props.dirTemp 'sherlock.sourcemonitor.xml'
    
    $props.ccmTemplateFile = Join-Path $props.dirTemplates 'ccm.xml.in'
    $props.ccmFile = Join-Path $props.dirTemp 'sherlock.ccm.xml'
    
    $props.wixVersionTemplateFile = Join-Path $props.dirMsiInstall 'VersionNumber.wxi.in'
    $props.wixVersionFile = Join-Path $props.dirMsiInstall 'VersionNumber.wxi'
    
    $props.dependenciesTemplateFile = Join-Path $props.dirMsiInstall 'Dependencies.wxi.in'
    $props.dependenciesFile = Join-Path $props.dirMsiInstall 'Dependencies.wxi'
    
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
}

# The default task doesn't do anything. This just calls the help function. Useful
#   for new people
task default -depends Help

# Cleans all the generated files
task Clean -depends runClean, runCleanPackages

# Builds all the binaries
task Build -depends buildBinaries

# Runs the unit tests
task UnitTest -depends runUnitTests

# Runs the Specification tests
task SpecTest -depends runSpecificationTests

# Runs the verifications
task Verify -depends runFxCop, runDuplicateFinder, runCcm

# Runs the documentation build
task Doc -depends buildApiDocs, buildUserDoc

# Creates the zip file of the deliverables
task Package -depends buildPackage, assembleInstaller

# Collects the statistics of the build
task Statistics -depends runSourceMonitor

###############################################################################
# HELPER TASKS

task getVersion -action{
    #Get the file version from the version.xml file
    [xml]$xmlFile = Get-Content $props.versionFile
    $major = $xmlFile.version | %{$_.major} | Select-Object -Unique
    $minor = $xmlFile.version | %{$_.minor} | Select-Object -Unique
    $build = Get-BuildNumber
    $revision = Get-BzrVersion
	
	$input = "$major.$minor.$build.$revision"
    $props.versionNumber = New-Object -TypeName System.Version -ArgumentList $input
    ("version is: " + $props.versionNumber )
}

task getBuildDependencies -action{
    # Pull in all the packages
    $packages = Get-ChildItem -Path $props.dirBase -Filter "packages.config" -Recurse
    
    foreach($package in $packages)
    {
        $nuget = 'nuget.exe'
        $command = '& "' + $nuget + '" '
        $command += 'install "' + ($package.FullName) + '" '
        $command += '-ExcludeVersion '
        $command += '-OutputDirectory "' + $props.dirPackages + '"'
        
        ("Grabbing package from: " + $package.FullName)
        Invoke-Expression $command
        if ($LastExitCode -ne 0)
        {
            throw "NuGet failed on Apollo with return code: $LastExitCode"
        }
    }
}

task getLicenses -depends getBuildDependencies -action{
	$ntrevaExe = Join-Path $props.dirNTreva 'ntreva.exe'
	
	$command = '& "' + $ntrevaExe + '" '
	$command += '-p "' + $props.dirPackages + '" '
	$command += '-o "' + $props.licenseInfoFile + '" '
	
	# for each directory in the src directory
	# that doesn't contain the string 'test'
	$nonTestDirectories = Get-ChildItem -path $props.dirSrc |
            Where-Object { (($_.PSIsContainer) -and
                            !( $_.Name -like "*Test.*"))}
	foreach($dir in $nonTestDirectories)
	{
		$command += '-c "' + $dir.FullName + '" '
	}
	
	$command
	Invoke-Expression $command
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
	'reloadpackages':   Turns on or off the reloading of the Nuget packages. Default is off.
    'configuration':    Defines the configuration for the build. Valid values are 'debug' and 'release', default value is 'debug'.
    'platform':         Defines the platform for the build. Valid values are 'Any CPU', default value is 'Any CPU'.

The following build tasks are available
    'clean':            Cleans the output directory
    'build':            Builds the binaries
    'unittest':         Runs the unit tests
    'spectest':         Runs the specification tests
    'verify':           Runs the source and binary verification. Returning one or more reports
                        describing the flaws in the source / binaries.
    'doc':              Runs the documentation build
    'package':          Packages the deliverables into a single zip file and into an MSI installer file
    'statistics':       Collects statistics of the current build.

Multiple build tasks can be specified separated by a comma. 
       
In order to run this build script please call this script via PSAKE like:
    invoke-psake apollo.ps1 -properties @{ "incremental"=$trueText;"coverage"=$trueText;"reloadpackages"=$trueText;"configuration"="debug";"platform"="Any CPU" } clean,build,unittest,spectest,verify,doc,package,statistics 4.0
"@
}

task runInit -action{
    $props.incremental = $incremental
    $props.coverage = $coverage
	$props.reloadPackages = $reloadpackages;
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
	("Incremental:     " + $props.incremental)
	("Reload packages: " + $props.reloadpackages)
    ""
}

# Note that the precondition is defined based on the $incremental property because
# it seems that psake determines the values of these preconditions based on values 
# available when the script is started, not values becoming available later on.
task runClean -depends displayInfo -precondition{ !$incremental } -action{
    "Removing generated items ..."
    $itemsToRemove = Create-GeneratedItemsList (Join-Path $props.dirbase 'generateditems.txt')
    if ($itemsToRemove -ne $null)
    {
        $itemsToRemove | foreach {
            if (Test-Path $_)
            {
				"Removing: $_"
                Remove-Item $_ -Force -Recurse
            }
        }
    }
	
	""
}

task runCleanPackages -depends displayInfo -precondition { $reloadpackages } -action{
	if (Test-Path $props.dirPackages)
	{
		"Removing packages"
		Remove-Item $props.dirPackages -Force -Recurse
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
	
	""
}

task buildBinaries -depends runPrepareDisk, getBuildDependencies, getLicenses, getVersion -action{
    "Building Apollo..."
    
    # Set the version numbers
    Create-VersionResourceFile $props.versionTemplateFile $props.versionAssemblyFile $props.versionNumber
    
    # Set the configuration
    Create-ConfigurationResourceFile $props.configurationTemplateFile $props.configurationAssemblyFile $props.configuration
    
    # Set the InternalsVisibleTo attribute
    $publicKeyToken = Get-PublicKeySignatureFromKeyFile $props.dirTemp $env:SOFTWARE_SIGNING_KEY_PATH
    $testUnitHostAssemblyName = $props.assemblyNameTestUnitHost + $publicKeyToken
	$testUnitBaseAssemblyName = $props.assemblyNameTestUnitBase + $publicKeyToken
	$testUnitUIAssemblyName = $props.assemblyNameTestUnitUi + $publicKeyToken
	$testUnitUtilsAssemblyName = $props.assemblyNameTestUnitUtils + $publicKeyToken
	$testUnitDatasetAssemblyName = $props.assemblyNameTestUnitDataset + $publicKeyToken
    
    $manualTestAssemblyName = $props.assemblyNameManualTest + $publicKeyToken
    
    $publicKeyToken = Get-PublicKeySignatureFromAssembly (Join-Path $props.dirMoq 'Moq.dll')
    $moqAssemblyName = $props.assemblyNameMoq + $publicKeyToken
    Create-InternalsVisibleToFile $props.internalsVisibleToTemplateFile $props.internalsVisibleToFile ($testUnitHostAssemblyName, $testUnitBaseAssemblyName, $testUnitUIAssemblyName, $testUnitUtilsAssemblyName, $testUnitDatasetAssemblyName, $manualTestAssemblyName, $moqAssemblyName, $props.assemblyNameDynamicProxy)
    
    $logPath = Join-Path $props.dirLogs $props.logMsBuild
    $msbuildExe = Get-MsbuildExe
    Invoke-MsBuild $props.slnApollo $props.configuration $logPath 'minimal' (("platform='" + $props.Platform+ "'")) $props.incremental
    if ($LastExitCode -ne 0)
    {
        throw "Apollo build failed with return code: $LastExitCode"
    }
	
	""
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
        $writerCommand += " /e System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute System.Runtime.CompilerServices.CompilerGeneratedAttribute"
        $writerCommand += " /a " + $coverageFiles
        
        $writerCommand
        Invoke-Expression $writerCommand
        if ($LastExitCode -ne 0)
        {
           throw 'PartCoverExclusionWriter failed on Apollo with return code: $LastExitCode'
        }
        
        $partCoverExe = Join-Path $props.dirPartCover 'PartCover.exe'
        $command += '& "' + "$partCoverExe" + '" --register' 
        $command += ' --settings "' + $props.partCoverConfigFile + '"'
        
        # run the tests
        $command
        
        Invoke-Expression $command
        "" # Add an extra line because PartCover is retarded and doesn't do a writeline at the end
        if ($LastExitCode -ne 0)
        {
            throw "MbUnit failed on Apollo with return code: $LastExitCode"
        }
        
        $partCoverLogFile = Join-Path $props.dirBase 'partcover.driver.log'
        if (Test-Path -Path $partCoverLogFile)
        {
            Remove-Item $partCoverLogFile -Force
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
	
	""
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
        throw "Concordion failed on Apollo.Core.Host with return code: $LastExitCode"
    }
	
	""
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
        throw "FxCop failed on Apollo.Core.Host with return code: $LastExitCode"
    }
	
	""
}

task runDuplicateFinder -depends buildBinaries -action{
    "Running duplicate check ..."
    ""
    # FAIL THE BUILD IF THERE IS ANYTHING WRONG
}

task buildApiDocs -depends buildBinaries -action{
    "Build the API docs..."
    
    # generate the sandcastle file
    $sandcastleFile = Join-Path $props.dirTemp 'apollo.shfbproj'
    Create-SandcastleConfigFile $props.sandcastleTemplateFile $sandcastleFile $props.dirPackages $props.dirDoc $props.dirLogs $props.dirOutput
    
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
    
	""
}

task buildUserDoc -depends buildBinaries -action{
    "Building user docs..."
	""
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
    
    "Copying files to zip ..."
    $itemsToPack = Create-GeneratedItemsList (Join-Path $props.dirZipInstall 'ItemsToPack.txt') $props.dirOutput
    if ($itemsToPack -ne $null)
    {
        $itemsToPack | foreach {
            if (Test-Path $_)
            {
				"Copying: $_"
                Copy-Item $_ -Destination (Join-Path $dirTempZip ($_.Name)) -Force
            }
        }
    }
    
    # zip them
    # Name the zip: Apollo_<DATE>
    $output = Join-Path $props.dirDeploy ("Apollo_" + $props.Platform.Replace(" ", "") + "_" + $props.Configuration + "_" + [System.DateTime]::Now.ToString("yyyy_MM_dd-HH_mm_ss") + ".zip")

    "Compressing..."

    # zip the temp dir
    $7zipExe = Get-7ZipExe
    & $7zipExe a -tzip $output (Get-ChildItem $dirTempZip | foreach { $_.FullName })
    if ($LastExitCode -ne 0)
    {
        throw "Failed to compress the Apollo binaries."
    }
	
	""
}

task assembleInstaller -depends buildBinaries -action{
    $versionString = [string]::Format('{0}.{1}.{2}.{3}', $props.versionNumber.Major, $props.versionNumber.Minor, $props.versionNumber.Build, $props.versionNumber.Revision)
    $props.versionNumber
    
    "Setting the version number..."
    Create-VersionResourceFile $props.wixVersionTemplateFile $props.wixVersionFile $props.versionNumber
    
    "Creating the dependencies file..."
    Create-InstallerDependencyFile $props.dependenciesTemplateFile $props.dependenciesFile $props.dirOutput $props.dirResource $props.dirInstall
    
    $logPath = Join-Path $props.dirLogs $props.logMsiBuild
    
    "Building 32-bit Apollo installer"
    $msbuildExe = Get-MsbuildExe
    Invoke-MsBuild $props.slnApolloWix 'release' $logPath 'normal' ("platform='x86'") $false

    $dirBinConfig = Join-Path (Join-Path $props.dirBinMsiInstall 'x86') 'release'
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.msi') -Destination (Join-Path $props.dirDeploy "apollo - x86 - $versionString.msi")
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.batchservice.msi') -Destination (Join-Path $props.dirDeploy "apollo.batchservice - x86 - $versionString.msi")
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.loaderapplication.msi') -Destination (Join-Path $props.dirDeploy "apollo.loaderapplication - x86 - $versionString.msi")

    "Building 64-bit Apollo installer"
    Invoke-MsBuild $props.slnApolloWix 'release' $logPath 'normal' ("platform='x64'") $false
    
    $dirBinConfig = Join-Path (Join-Path $props.dirBinMsiInstall 'x64') 'release'
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.msi') -Destination (Join-Path $props.dirDeploy "apollo - x64 - $versionString.msi")
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.batchservice.msi') -Destination (Join-Path $props.dirDeploy "apollo.batchservice - x64 - $versionString.msi")
    Copy-Item -Force -Path (Join-Path $dirBinConfig 'apollo.loaderapplication.msi') -Destination (Join-Path $props.dirDeploy "apollo.loaderapplication - x64 - $versionString.msi")
	
	""
}

task runCcm -depends buildBinaries -action{
    Create-CcmInputFile $props.ccmTemplateFile $props.ccmFile $props.dirSrc
    
    $ccm = Join-Path $props.dirCcm 'ccm.exe'
    & $ccm $props.ccmFile | Out-File (Join-Path $props.dirReports 'apollo.ccm.xml')
    if ($LastExitCode -ne 0)
    {
        throw "Ccm failed on Apollo with return code: $LastExitCode"
    }
	
	""
}

task runSourceMonitor -depends buildBinaries -action{
    $outputXmlFile = Join-Path $props.dirReports 'apollo.sourcemonitor.xml'
    Create-SourceMonitorInputFile $props.sourceMonitorTemplateFile $props.sourceMonitorFile $props.dirTemp $props.dirSrc $outputXmlFile
    
    $sourceMonitor = Join-Path $props.dirSourceMonitor 'SourceMonitor.exe'
    $command = ' /C "' + $props.sourceMonitorFile + '"'
    
    "Getting code statistics with command: SourceMonitor.exe $command"
    $smProcess = [diagnostics.process]::start($sourceMonitor, $command)
    
    ""
    ("Waiting for " + $smProcess.ProcessName + " to exit ...")
    
    # now wait for it to exit. Somehow when the process starts it does that asynchronously. And that screws up
    # getting the return code. After the process completes (which happens at some point in the future that we don't know of)
    # the $LastExitCode has value $null .... not good.
    # So in order to combat this retardedness we do things the hard way. First we create an instance of the .NET 
    # Diagnostics.Process class and provided that with our command line for SourceMonitor. Then we start the process and wait
    # for it to exit. Fortunately .NET is clever enough to defeat SourceMonitor and we get an actual exit code at the end ...
    # Victory is ours.
    $smProcess.WaitForExit()
    
    $exitCode = $smProcess.ExitCode
    if ($exitCode -ne 0)
    {
        throw "SourceMonitor failed on Apollo with return code: $exitCode"
    }
    
    # process the output and turn it into a CSV file for Jenkins to read
    "Writing data to CSV file"
    [xml]$xmlFile = Get-Content $outputXmlFile
    $metrics = $xmlFile.sourcemonitor_metrics.project.checkpoints.checkpoint.metrics.metric
    $numberOfLines = $metrics[0].InnerText
    $percentComments = $metrics[2].InnerText
    $percentDocs = $metrics[3].InnerText
    $numberOfElements = $metrics[4].InnerText
    $methodsPerClass = $metrics[5].InnerText
    $callsPerMethod = $metrics[6].InnerText
    $statementsPerMethod = $metrics[7].InnerText
    $maximumComplexity = $metrics[10].InnerText
    $averageComplexity = $metrics[14].InnerText
    $maximumBlockDepth = $metrics[12].InnerText
    $averageBlockDepth = $metrics[13].InnerText
    
    $text = '"Number of Lines"'
    $text += [System.Environment]::NewLine
    $text += "$numberOfLines"
    Set-Content (Join-Path $props.dirReports 'apollo.sourcemonitor.linecount.csv') $text
    
    $text = '"Percent comment lines", "Percent documentation lines"'
    $text += [System.Environment]::NewLine
    $text += "$percentComments, $percentDocs"
    Set-Content (Join-Path $props.dirReports 'apollo.sourcemonitor.percentages.csv') $text
    
    $text = '"Methods per class", "Calls per method", "Statements per method", "Maximum complexity", "Average complexity", "Average block depth", "Maximum block depth"'
    $text += [System.Environment]::NewLine
    $text += "$methodsPerClass, $callsPerMethod, $statementsPerMethod, $maximumComplexity, $averageComplexity, $averageBlockDepth, $maximumBlockDepth"
    Set-Content (Join-Path $props.dirReports 'apollo.sourcemonitor.complexity.csv') $text
	
	""
}
