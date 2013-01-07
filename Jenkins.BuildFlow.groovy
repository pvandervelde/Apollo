out.println ''
out.println ''
out.println "Building Apollo for revision: " + build.environment["BZR_REVISION"]

archiveLocation = "%JENKINS_WORKSPACE_CLONE_PATH%\\apollo"
testResultPrefix = "Apollo-TestResult-" + build.number + "-"
prepareWorkspaceClone = "Apollo-Prepare-Workspace-" + build.number
testResultWorkspaceClone = prepareWorkspaceClone

buildResult = hudson.model.Result.SUCCESS

prepareWorkspaceBuild = build("Apollo-Prepare-Workspace", WORKSPACE_ARCHIVE_NAME: prepareWorkspaceClone, ARCHIVE_LOCATION: archiveLocation)
buildResult = prepareWorkspaceBuild.build.result.combine(buildResult)

out.println ''
out.println ''
out.println "Build result is: " + buildResult

guard {
    buildBinariesWorkspaceClone = "Apollo-BuildBinaries-" + build.number
    if (buildResult.isBetterOrEqualTo(hudson.model.Result.SUCCESS)) {
        parallel(
            { 
                b = build("Apollo-Analyze-Ccm", CLONED_WORKSPACE: prepareWorkspaceClone, ARCHIVE_LOCATION: archiveLocation, BZR_REVISION: build.environment["BZR_REVISION"], TESTRESULTS_PREFIX: testResultPrefix) 
                buildResult = b.build.result.combine(buildResult)
            },
            { 
                b = build("Apollo-Analyze-SourceMonitor", CLONED_WORKSPACE: prepareWorkspaceClone, ARCHIVE_LOCATION: archiveLocation, BZR_REVISION: build.environment["BZR_REVISION"], TESTRESULTS_PREFIX: testResultPrefix) 
                buildResult = b.build.result.combine(buildResult)
            },
            { 
                b = build("Apollo-Build-Binaries", CLONED_WORKSPACE: prepareWorkspaceClone, WORKSPACE_ARCHIVE_NAME: buildBinariesWorkspaceClone, ARCHIVE_LOCATION: archiveLocation, BZR_REVISION: build.environment["BZR_REVISION"]) 
                buildResult = b.build.result.combine(buildResult)
                
                testResultWorkspaceClone = buildBinariesWorkspaceClone
            },
        )
    }

    out.println ''
    out.println ''
    out.println "Build result is: " + buildResult

    if (buildResult.isBetterOrEqualTo(hudson.model.Result.SUCCESS)) {
        parallel(
            {
                b = build("Apollo-Analyze-FxCop", CLONED_WORKSPACE: buildBinariesWorkspaceClone, ARCHIVE_LOCATION: archiveLocation, BZR_REVISION: build.environment["BZR_REVISION"], TESTRESULTS_PREFIX: testResultPrefix) 
                buildResult = b.build.result.combine(buildResult)
            },
            {
                b = build("Apollo-Analyze-Moma", CLONED_WORKSPACE: buildBinariesWorkspaceClone, ARCHIVE_LOCATION: archiveLocation, BZR_REVISION: build.environment["BZR_REVISION"], TESTRESULTS_PREFIX: testResultPrefix) 
                buildResult = b.build.result.combine(buildResult)
            },
            {
                b = build("Apollo-Build-ApiDocumentation", CLONED_WORKSPACE: buildBinariesWorkspaceClone, ARCHIVE_LOCATION: archiveLocation, BZR_REVISION: build.environment["BZR_REVISION"], TESTRESULTS_PREFIX: testResultPrefix) 
                buildResult = b.build.result.combine(buildResult)
            },
            {
                b = build("Apollo-Test-UnitTest", CLONED_WORKSPACE: buildBinariesWorkspaceClone, ARCHIVE_LOCATION: archiveLocation, BZR_REVISION: build.environment["BZR_REVISION"], TESTRESULTS_PREFIX: testResultPrefix) 
                buildResult = b.build.result.combine(buildResult)
            },
        )
    }

    packageMsiWorkspaceClone = "Apollo-PackageMsi-" + build.number
    if (buildResult.isBetterOrEqualTo(hudson.model.Result.SUCCESS)){
        b = build("Apollo-Package-Msi", CLONED_WORKSPACE: buildBinariesWorkspaceClone, WORKSPACE_ARCHIVE_NAME: packageMsiWorkspaceClone, ARCHIVE_LOCATION: archiveLocation, BZR_REVISION: build.environment["BZR_REVISION"]) 
        buildResult = b.build.result.combine(buildResult)
        
        testResultWorkspaceClone = packageMsiWorkspaceClone
    }

    if (buildResult.isBetterOrEqualTo(hudson.model.Result.SUCCESS)) {
        b = build("Apollo-Deploy-ToMsiRepository", CLONED_WORKSPACE: packageMsiWorkspaceClone, ARCHIVE_LOCATION: archiveLocation, BZR_REVISION: build.environment["BZR_REVISION"]) 
        buildResult = b.build.result.combine(buildResult)
    }
} rescue {
    b = build("Apollo-Gather-BuildResults", CLONED_WORKSPACE: testResultWorkspaceClone, ARCHIVE_LOCATION: archiveLocation, BZR_REVISION: build.environment["BZR_REVISION"], TESTRESULTS_PREFIX: testResultPrefix)
    buildResult = b.build.result.combine(buildResult)
}

out.println ''
out.println ''
out.println "Build result is: " + buildResult
build.setResult(build.result.combine(buildResult))
