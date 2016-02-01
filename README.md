# smarter-balanced-glossary-service

## Project setup
1. Clone project and open in Visual Studio 2015
2. In the Solution Explorer, right click on the IllustrationGlossaryPackage.App project and select "Set As StartUp Project"
3. In the Solution Explorer, right click on the IllustrationGlossaryPackage.App project. Open the Debug tab and enter two command line arguments
  1. Path to test package zip file
  2. Path to illustration csv
4. Click Start to run project

## Project Details
This is what happens when the project is executed. The project is still in development, so this list will be updated as the project is developed. This list reflects the current state of the project. 
### On Execution
1. The files given as command line arguments are validated
  * Still working on creating more extensive validation
2. An archive of the given test package is created in a directory called Archive, located in the same directory as the given test package
3. The given csv containing illustrations is parsed
  
