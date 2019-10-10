This script shows a simple solution for updating variables in a selection of files with the same values.

Request: [Can you modify info on multiple data cards at once?](https://forum.solidworks.com/thread/234757)

This script can be executed via the context menu of the explorer. Depending on which item in the menu is selected, the variable is set to 0 or 1.
The files to be changed have to be checked out. If a file does not match the pattern [i)^.*([.]sld(asm)|(prt))$](https://www.autohotkey.com/docs/commands/RegExMatch.htm), it will be ignored.

If an error occurs, the routine will be aborted at this point with an error message. If no error occurs, the routine will be terminated without feedback.
