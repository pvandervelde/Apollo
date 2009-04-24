#build.rb

require 'rake'

# The default task is the help task which explains about the 
# switches
task :default => [:help]

# Call the build for the different sub-parts
task :build => [:buildCore, :buildUI, :buildPlugins, :buildUtils]

# Can simply call the template build script with the data from the different
# core build scripts
