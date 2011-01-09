# Ensure that we can run the code coverage for the unit tests. This must be done in a 32 bit
# powershell. So we switch to one if we haven't got one.
if ($env:Processor_Architecture -ne "x86")
{
    # We're on a 64bit powershell. Switch to 32bit. We'll call the current script in the new
    # shell through $MyInvocation.Line
    write-warning "Running x86 PowerShell..."
    Write-Warning $MyInvocation.Line
    & "$env:WINDIR\syswow64\windowspowershell\v1.0\powershell.exe" -NonInteractive -NoProfile $MyInvocation.Line
    exit
}

"Running as: $env:Processor_Architecture"
"running from $pwd"
. ./build.ps1
Build-ReleaseFull