# build.rb
# Performs the build

task :default => [:help]

# Debug
# - Build all source
desc "Build the debug version"
task :debug => [:buildSrc, :deployLocal]
# Check
# - Build all source
# - Build all documentation
# - Build installer
# - Run unit tests
# - Run code coverage
# - Run FxCop
desc "Build the check version"
task :check => [:buildSrc, :buildDoc, :buildInstall, 
                :runUnit, :runXCop, :deployLocal]
# Internal
# - Build all source
# - Build all documentation
# - Build installer
# - Run unit tests
# - Run code coverage
# - Run FxCop
# - Check company data (plugins, documentation, assembly info, logos)
# - Run profile tests
# - Run verification & validation
desc "Build the internal test version"
task :internal => [:buildSrc, :buildDoc, :buildInstall, 
                   :runUnit, :runXCop, :runCheck,
				   :runPerf, :runVV, :deployLocal]
# Beta
# - Build all source
# - Build all documentation
# - Build installer
# - Run unit tests
# - Run code coverage
# - Run FxCop
# - Check company data (plugins, documentation, assembly info, logos)
# - Run profile tests
# - Run verification & validation
desc "Build the beta version"
task :beta => [:buildSrc, :buildDoc, :buildInstall, 
               :runUnit, :runXCop, :runCheck,
			   :runPerf, :runVV,
			   :deployLocal, :deployServer]
# Release
# - Build all source
# - Build all documentation
# - Build installer
# - Run unit tests
# - Run code coverage
# - Run FxCop
# - Check company data (plugins, documentation, assembly info, logos)
# - Run profile tests
# - Run verification & validation
desc "Build the release version"
task :release => [:buildSrc, :buildDoc, :buildInstall, 
                  :runUnit, :runXCop, :runCheck,
			      :runPerf, :runVV,
				  :deployLocal, :deployServer]

desc "Build the sources"
task :buildSrc do
	# Call into the individual build scripts here
end

desc "build the documentation, bot user and developer"
task :buildDoc do
	# Call into the individual developer doc scripts 
	# Call into the individual user doc scripts
end

desc "build the installer package(s)"
task :buildInstall do
	# Call into the WiX scripts here
end

desc "Runs the unit tests, with or without code coverage"
task :runUnit do
end

desc "Runs the different checkers"
task :runXCop do
end

desc "Runs the performance tests"
task :runPerf do
end

desc "Runs the verification and validation tests"
task :runVV do
end

desc "Deploys the installer package to a local machine"
task :deployLocal do
end

desc "Deploys the installer package to a server"
task :deployServer do
end