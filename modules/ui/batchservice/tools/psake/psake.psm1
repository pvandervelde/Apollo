# psake v0.23
# Copyright © 2009 James Kovacs
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in
# all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
# THE SOFTWARE.

#-- Start Exported Functions
function Task
{
<#
.Synopsis
    Defines a build task to be executed by psake 
.Description
    This function contains parameters that will be used by the psake engine to execute a build task.
	Note: There must be at least one task called 'default' in the build script 
.Parameter Name 
	The name of the task	
	Required
.Parameter Action 
	A scriptblock containing the statements to execute
	Optional 
.Parameter PreAction
	A scriptblock to be executed before the 'Action' scriptblock.
	Note: This parameter is ignored if the 'Action' scriptblock is not defined.
	Optional 
.Parameter PostAction 
	A scriptblock to be executed after the 'Action' scriptblock.
	Note: This parameter is ignored if the 'Action' scriptblock is not defined.
	Optional 
.Parameter Precondition 
	A scriptblock that is executed to determine if the task is executed or skipped.
	This scriptblock should return $true or $false
	Optional
.Parameter Postcondition
	A scriptblock that is executed to determine if the task completed its job correctly.
	An exception is thrown if the scriptblock returns false.	
	Optional
.Parameter ContinueOnError
	If this switch parameter is set then the task will not cause the build to fail when an exception is thrown
.Parameter Depends
	An array of tasks that this task depends on.  They will be executed before the current task is executed.
.Parameter Description
	A description of the task.
.Example
	task default -depends Test
	
	This is the 'default' task and should not contain an 'Action' parameter.
	Uses the 'depends' parameter to specify that 'Test' is a dependency

.Example
	task Test -depends Compile, Clean { 
	  $testMessage
	} 	
	
	This task uses the 'depends' parameter to specify that 'Compile' and 'Clean' are dependencies
	
	The 'Action' parameter is defaulted to the script block following the 'Clean'. 
	The equivalen is shown below:
	
	task Test -depends Compile, Clean -Action { 
	  $testMessage
	}
	
.ReturnValue
      
.Link	
	Invoke-psake
.Notes
 NAME:      Invoke-psake
 AUTHOR:    Jorge Matos
 LASTEDIT:  05/12/2009
#Requires -Version 2.0
#>
[CmdletBinding(
    SupportsShouldProcess=$False,
    SupportsTransactions=$False, 
    ConfirmImpact="None",
    DefaultParameterSetName="")]
	param(
		[string]$name = $null, 
		[scriptblock]$action = $null, 
		[scriptblock]$preaction = $null,
		[scriptblock]$postaction = $null,
		[scriptblock]$precondition = $null, 
		[scriptblock]$postcondition = $null, 
		[switch]$continueOnError = $false, 
		[string[]]$depends = @(), 
		[string]$description = $null
		)
	if ([string]::IsNullOrEmpty($name)) 
	{
		throw "Error: Task must have a name"	
	}
	if (($name.ToLower() -eq 'default') -and ($action -ne $null)) 
	{
		throw "Error: Default task cannot specify an action"
	}
	$newTask = @{
		Name = $name
		DependsOn = $depends
		PreAction = $preaction
		Action = $action
		PostAction = $postaction
		Precondition = $precondition
		Postcondition = $postcondition
		ContinueOnError = $continueOnError
		Description = $description
		Duration = 0
	}
	if ($global:tasks.$name -ne $null) 
	{ 
		throw "Error: Task, $name, has already been defined." 
	}
	$global:tasks.$name = $newTask
}

function Properties
{
	param([scriptblock]$propertyBlock)
	$global:properties += $propertyBlock
}

function Include
{
	param([string]$include)
	if (!(test-path $include)) 
	{ 
		throw "Error: $include not found."
	} 	
	$global:includes.Enqueue((Resolve-Path $include));
}

function FormatTaskName 
{
	param([string]$format)
	$global:formatTaskNameString = $format
}

function TaskSetup 
{
	param([scriptblock]$setup)
	$global:taskSetupScriptBlock = $setup
}

function TaskTearDown 
{
	param([scriptblock]$teardown)
	$global:taskTearDownScriptBlock = $teardown
}


#-- END Exported Functions

function ExecuteTask 
{
	param([string]$name,
		  [System.Collections.Hashtable]$availableTasks = $null,
			$executedTasks = $null,
			$callStack = $null)
#	"Running execute task: $name"
#	
#	$name
#	"------------"
#	"availabletasks"
#	$availableTasks -ne $null
#	$availableTasks.Count
#	$availableTasks
#	"------------"
#	"executedtasks"
#	$executedTasks -ne $null
#	$executedTasks.Count
#	"------------"
#	"callstack"
#	$callStack -ne $null
#	$callStack.Count
#	""
	
	
	if (!$availableTasks.Contains($name)) 
	{
		throw "task [$name] does not exist"
	}

	if ($executedTasks.Contains($name)) 
	{ 
		return 
	}
  
	if ($callStack.Contains($name)) 
	{
		throw "Error: Circular reference found for task, $name"
	}
  
	$callStack.Push($name)
  
	$task = $availableTasks.$name
	
	$precondition_is_valid = if ($task.Precondition -ne $null) {& $task.Precondition} else {$true}
	
	if (!$precondition_is_valid) 
	{
		"Precondition was false not executing $name"		
	}
	else
	{
		if ($name.ToLower() -ne 'default') 
		{
			$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
			
			if ( ($task.PreAction -ne $null) -or ($task.PostAction -ne $null) -and ($task.Action -eq $null) )
			{
				throw "Error: Action must be specified when using PreAction or PostAction parameters"
			}
			
			if ($task.Action -ne $null) 
			{
				try
				{						
					foreach($childTask in $task.DependsOn) 
					{
						ExecuteTask $childTask $availableTasks $executedTasks $callStack
					}
					
					if ($global:formatTaskNameString -ne $null) 
					{
						$global:formatTaskNameString -f $name
					} 
					else 
					{	
						"Executing task, $name..."
					}
					
					if ($global:taskSetupScriptBlock -ne $null) 
					{
						& $global:taskSetupScriptBlock
					}
					
					if ($task.PreAction -ne $null) 
					{
						& $task.PreAction
					}
					
					& $task.Action
					
					if ($task.PostAction -ne $null) 
					{
						& $task.PostAction
					}
					
					if ($global:taskTearDownScriptBlock -ne $null) 
					{
						& $global:taskTearDownScriptBlock
					}					
				}
				catch
				{
					if ($task.ContinueOnError) 
					{
						"-"*70
						"Error in Task [$name] $_"
						"-"*70
						continue
					} 
					else 
					{
						throw $_
					}
				}			
			} # if ($task.Action -ne $null)
			else
			{
				#no Action was specified but we still execute all the dependencies
				foreach($childTask in $task.DependsOn) 
				{
					ExecuteTask $childTask $availableTasks $executedTasks $callStack
				}
			}
			$stopwatch.stop()
			$task.Duration = $stopwatch.Elapsed
		} # if ($name.ToLower() -ne 'default') 
		else 
		{ 
			foreach($childTask in $task.DependsOn) 
			{
				ExecuteTask $childTask $availableTasks $executedTasks $callStack
			}
		}
		
		$postcondition = if ($task.Postcondition -ne $null) {(& $task.Postcondition)} else {$true} 
					
		if (!$postcondition) 
		{
			throw "Error: Postcondition failed for $name"
		}
	}
	
    if ($callStack.Count -gt 0)
    {
    	$poppedTask = $callStack.Pop()
    	if($poppedTask -ne $name) 
    	{
    		throw "Error: CallStack was corrupt. Expected $name, but got $poppedTask."
    	}
    }
	$executedTasks.Push($name)
}

function Configure-BuildEnvironment 
{
	$version = $null
	switch ($framework) 
	{
		'1.0' { $version = 'v1.0.3705'  }
		'1.1' { $version = 'v1.1.4322'  }
		'2.0' { $version = 'v2.0.50727' }
		'3.0' { $version = 'v2.0.50727' } # .NET 3.0 uses the .NET 2.0 compilers
		'3.5' { $version = 'v3.5'       }
		'4.0' { $version = 'v4.0.20506' }
		default { throw "Error: Unknown .NET Framework version, $framework" }
	}
	$frameworkDir = "$env:windir\Microsoft.NET\Framework\$version\"
	if(!(test-path $frameworkDir)) 
	{
		throw "Error: No .NET Framework installation directory found at $frameworkDir"
	}
	$env:path = "$frameworkDir;$env:path"
	$global:ErrorActionPreference = "Stop"
}

function Cleanup-Environment 
{
	param([System.Collections.Hashtable]$availableTasks = $null,
		  $callStack,
		  $properties,
		  $originalEnvPath,
		  $originalDirectory,
		  $originalErrorActionPreference)
		  
#	"Cleaning environment ..."
#	"Current values ..."
#	"Tasks: " 
#	foreach ($task in $availableTasks) { $task }
#	""
#	"Call-stack: " + $callstack.Count
#	"Properties: " + $properties

	$env:path = $originalEnvPath	
	Set-Location $originalDirectory
	$global:ErrorActionPreference = $originalErrorActionPreference
	remove-variable tasks -scope "global" -ErrorAction SilentlyContinue
	remove-variable properties -scope "global" -ErrorAction SilentlyContinue
	remove-variable includes -scope "global" -ErrorAction SilentlyContinue
	remove-variable psake_version -scope "global" -ErrorAction SilentlyContinue 
	remove-variable psake_buildScript -scope "global" -ErrorAction SilentlyContinue 
	remove-variable formatTaskNameString -scope "global" -ErrorAction SilentlyContinue 
	remove-variable taskSetupScriptBlock -scope "global" -ErrorAction SilentlyContinue 
	remove-variable taskTearDownScriptBlock -scope "global" -ErrorAction SilentlyContinue 
	remove-variable psake_frameworkVersion -scope "global" -ErrorAction SilentlyContinue 
	if (!$noexit) 
	{
		remove-variable psake_buildSucceeded -scope "global" -ErrorAction SilentlyContinue 
	}  
}

#borrowed from Jeffrey Snover http://blogs.msdn.com/powershell/archive/2006/12/07/resolve-error.aspx
function Resolve-Error($ErrorRecord=$Error[0]) 
{	
	$ErrorRecord #| Format-List * -Force
	$ErrorRecord.InvocationInfo  #| Format-List *
	$Exception = $ErrorRecord.Exception
	for ($i = 0; $Exception; $i++, ($Exception = $Exception.InnerException)) 
	{
		"$i" * 70
		$Exception #| Format-List * -Force
	}
}

function Write-Documentation 
{
	$list = New-Object System.Collections.ArrayList
	foreach($key in $global:tasks.Keys) 
	{
		if($key -eq "default") 
		{
		  continue
		}
		$task = $global:tasks.$key
		$content = "" | Select-Object Name, Description
		$content.Name = $task.Name        
		$content.Description = $task.Description
		$index = $list.Add($content)
	}

	$list | Sort 'Name' | Format-Table -Auto
}

function Write-TaskTimeSummary
{
	param($availableTasks,
	      $executedTasks)

	"-"*70
	"Build Time Report"
	"-"*70	
	$list = @()
	while ($executedTasks.Count -gt 0) 
	{
		$name = $executedTasks.Pop()
		$task = $availableTasks.$name
		if($name -eq "default") 
		{
		  continue
		}    
		$list += "" | Select-Object @{Name="Name";Expression={$name}}, @{Name="Duration";Expression={$task.Duration}}
	}
	[Array]::Reverse($list)
	$list += "" | Select-Object @{Name="Name";Expression={"Total:"}}, @{Name="Duration";Expression={$stopwatch.Elapsed}}
	$list | Format-Table -Auto | Out-String -Stream | ? {$_}  # using "Out-String -Stream" to filter out the blank line that Format-Table prepends 
}

function Invoke-psake 
{
<#
.Synopsis
    Runs a psake build script.
.Description
    This function runs a psake build script 
.Parameter BuildFile 
	The psake build script to execute (default: default.ps1).	
.Parameter Framework 
	The version of the .NET framework you want to build
	Possible values: '1.0', '1.1', '2.0', '3.0',  '3.5', '4.0'
	Default = '3.5'
.Parameter ShowFullError
	Displays detailed error information when an error occurs
.Parameter Timing 
	Prints a report showing how long each task took to execute
.Parameter Docs 
	Prints a list of tasks and their descriptions
.Parameter NoExit
	Does not use 'exit' command when an error occurs or when printing documentation - useful when running a build interactively
	
.Example
    Invoke-psake 
	
	Runs the 'default' task in the 'default.ps1' build script in the current directory
.Example
	Invoke-psake '.\build.ps1'
	
	Runs the 'default' task in the '.build.ps1' build script
.Example
	Invoke-psake '.\build.ps1' Tests, Package
	
	Runs the 'Tests' and 'Package' tasks in the '.build.ps1' build script
.Example
	Invoke-psake '.\build.ps1' -timing
	
	Runs the 'default' task in the '.build.ps1' build script and prints a timing report
.Example
	Invoke-psake '.\build.ps1' -debuginfo
	
	Runs the 'default' task in the '.build.ps1' build script and prints a report of what includes, properties and tasks are in the build script
.Example
	Invoke-psake '.\build.ps1' -docs
	
	Prints a report of all the tasks and their descriptions and exits
	
.ReturnValue
    Calls exit() function with 0 for success and 1 for failure 
	If $noexit is $true then exit() is not called and no value is returned  
.Link	
.Notes
 NAME:      Invoke-psake
 AUTHOR:    Jorge Matos
 LASTEDIT:  05/04/2009
#Requires -Version 2.0
#>
[CmdletBinding(
    SupportsShouldProcess=$False,
    SupportsTransactions=$False, 
    ConfirmImpact="None",
    DefaultParameterSetName="")]
	
	param(
	  [string]$buildFile = 'default.ps1',
	  [string[]]$taskList = @(),
	  [string]$framework = '3.5',
	  [switch]$showFullError = $false,	 
	  [switch]$timing = $false,
	  [switch]$docs = $false,
	  [switch]$noexit = $false
	)

	Begin 
	{
		# Create the global variables. These can be clobbered at any time. 
		# The real values for this build will be stored in local values...
		$global:tasks = @{}
		$global:properties = @()
		$global:includes = New-Object System.Collections.Queue
		
		$global:psake_buildSucceeded = $true
		$global:formatTaskNameString = $null
		$global:taskSetupScriptBlock = $null
		$global:taskTearDownScriptBlock = $null
		
		# local variables
		$localTasks = @{}
		$localProperties = @()
		$localIncludes = $null
		$localPsake_buildScript = $buildFile
		$localPsake_frameworkVersion = $framework
		$localFormatTaskNameString = $null

		$localExecutedTasks = New-Object System.Collections.Stack
		$localCallStack = New-Object System.Collections.Stack
		
		$localOriginalEnvPath = $env:path
		$localOriginalDirectory = Get-Location
		$originalErrorActionPreference = $Global:ErrorActionPreference
	}
	
	Process 
	{	
		try 
		{
			$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

			# Execute the build file to set up the tasks and defaults
			if(test-path $buildFile) 
			{
				$buildFile = resolve-path $buildFile
				set-location (split-path $buildFile)
				& $buildFile
			} 
			else 
			{
				throw "Error: Could not find the build file, $buildFile."
			}
			
			# load all the values into the local parameters
			$localTasks = $global:tasks.clone()
			$localProperties = $global:properties.clone()
			$localIncludes = $global:includes.clone()
			$localFormatTaskNameString = $global:formatTaskNameString

			Configure-BuildEnvironment

			# N.B. The initial dot (.) indicates that variables initialized/modified
			#      in the propertyBlock are available in the parent scope.
			while ($localIncludes.Count -gt 0) 
			{
				$includeBlock = $localIncludes.Dequeue()
				. $includeBlock;
			}
			foreach($propertyBlock in $localProperties) 
			{
				. $propertyBlock
			}

			if($docs) 
			{
				Write-Documentation
				Cleanup-Environment  $localtasks $localcallStack $localproperties $localoriginalEnvPath $localoriginalDirectory $originalErrorActionPreference
				if ($noexit) 
				{ 					
					return
				} 
				else 
				{
					exit($LastExitCode)
				}				
			}

			# Execute the list of tasks or the default task
			if($taskList.Length -ne 0) 
			{
#				"TaskList:"
#				$taskList
#				"+++++++++++++++++"
				
				foreach($task in $taskList) 
				{
#					"About to execute"
#					$localTasks
					ExecuteTask $task $localTasks $localexecutedTasks $localcallStack
				}
			} 
			elseif ($localtasks.default -ne $null) 
			{
				ExecuteTask default $localTasks $localexecutedTasks $localcallStack
			} 
			else 
			{
				throw 'Error: default task required'
			}

			$stopwatch.Stop()

			if ($timing) 
			{	
				Write-TaskTimeSummary $localtasks $localexecutedTasks
			}
			
			$global:psake_buildSucceeded = $true
		} 
		catch 
		{			
			if ($showFullError)
			{
				"-" * 70 
				"{0}: An Error Occurred. See Error Details Below: " -f [DateTime]::Now
				"-" * 70 
				Resolve-Error $_
				"-" * 70
			}
			else
			{
				$file = split-path $global:psake_buildScript -leaf
				Write-Host -foregroundcolor Red ($file + ":" + $_)
			}
			
			Cleanup-Environment  $localtasks $localcallStack $localproperties $localoriginalEnvPath $localoriginalDirectory $originalErrorActionPreference

			if ($noexit) 
			{ 
				$global:psake_buildSucceeded = $false
			} 
			else 
			{
				# Set the last exit code, just to be clear
				exit($LastExitCode)
			}
		}
	} #Process
	
	End 
	{
		# Clear out any global variables
		Cleanup-Environment  $localtasks $localcallStack $localproperties $localoriginalEnvPath $localoriginalDirectory $originalErrorActionPreference
	}
}

Export-ModuleMember Invoke-psake, Task, Properties, Include, FormatTaskName, TaskSetup, TaskTearDown
