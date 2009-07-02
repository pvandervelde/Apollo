# this is the normal build script. This will call the psake tool to run the build

function Build-DebugDev{
	& run-psake apollo.ps1 -noexit -showfullerror -timing -framework 4.0 Debug,UnitTests,Verify,DeveloperBuild
}

function Build-ReleaseDev{
	& run-psake apollo.ps1 -noexit -showfullerror -timing -framework 4.0 Release,UnitTests,Verify,DeveloperBuild
}

function Build-DebugFull{
	& run-psake apollo.ps1 -noexit -showfullerror -timing -framework 4.0 Debug,UnitTests,Verify,ApiDocs,UserDocs,Installer,FullBuild
}

function Build-ReleaseFull{
	& run-psake apollo.ps1 -noexit -showfullerror -timing -framework 4.0 Release,UnitTests,Verify,ApiDocs,UserDocs,Installer,FullBuild
}