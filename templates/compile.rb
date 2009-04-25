# compile.rb
# Runs the csc compiler on a specified set of sources

require 'dependencies'

task :default => [:help]

task :build => [:compile, :copy]

desc "Provides the help for the current task"
task :help do
	# Describe how to use these tasks here
end

desc "Runs the MsBuild script on the VS solutions"
task :compile => [:clobber] do
	msbuild 
end

# Define the method that will run the MSBuild script
def msbuild(solution)
	sh "#{MSBUILD} #{solution}.sln"
end

desc ""
task :copy do
	# Copy the newly created binaries to the BIN folder
end