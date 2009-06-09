#light

// include FAKE libraries
#I "tools/thirdparty/FAKE"
#r "FakeLib.dll"
open Fake

// Define the build directory
let buildDir = @".\bin\"

// Targets
Target "Clean" (fun() ->
    CleanDir buildDir
    // Should really call Msbuild here with the clean target
)


Target "Default" (fun() ->
    trace "Hello world from FAKE"
)

// Dependencies
"Default" <== ["Clean"]

run "Default"