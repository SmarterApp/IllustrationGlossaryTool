# smarter-balanced-glossary-service

## Overview
This project adds illustrations to an existing test package.

## Dependencies
- Microsoft .NET 4.5.2

## Installation/Usage
1. Download the latest release .exe.
2. Run according to the following specifications.

####Required Arguments
1. Test Package
2. Illustrations csv

####Optional Arguments
1. `-n`: Does not create an archive of the current test package
2. `-h /? -? --help ?`: Show the help page

####Requirements for Illustrations csv
- Must have 3 columns: ItemId, Term, IllustrationFilename
- The first row in the csv is a header row

#####Example:
`IllustrationGlossaryPackage -n MyTestPackes/IrpContentPackage.zip MyIllustrations/Illustrations.csv`

## Contributing/Project setup
1. Clone project and open in Visual Studio 2015
2. In the Solution Explorer, right click on the IllustrationGlossaryPackage.App project and select "Set As StartUp Project"
3. In the Solution Explorer, right click on the IllustrationGlossaryPackage.App project. Open the Debug tab and enter two command line arguments
  1. Path to test package zip file
  2. Path to illustration csv
4. Click Start to run project 

## License
Mozilla Public License Version 2.0. See LICENSE for more details.