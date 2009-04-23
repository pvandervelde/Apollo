#build.rb

require 'rake'

# The default task is the help task which explains about the 
# switches
task :default => [:help]

# Debug
# - Build all source
desc "Build the debug version"
task :debug => [:buildSrc]
# Check
# - Build all source
# - Build all documentation
# - Build installer
# - Run unit tests
# - Run code coverage
# - Run FxCop
desc "Build the check version"
task :check => [:buildSrc, :buildDoc, :buildInstall, 
                :runUnit, :runCoverage, :runFxCop]
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
                   :runUnit, :runCoverage, :runFxCop, :runCheck,
				   :runPerf, :runVV]
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
               :runUnit, :runCoverage, :runFxCop, :runCheck,
			   :runPerf, :runVV]
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
                  :runUnit, :runCoverage, :runFxCop, :runCheck,
			      :runPerf, :runVV]

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