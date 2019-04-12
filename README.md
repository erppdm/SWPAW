# SWPAW
**S**olid**W**orks **P**DM **A**dd-In **W**rapper

A wrapper to create SolidWorks PDM add-ins in any language

## Workflow
![](http://bii.erppdm.com/images/Ulf-Dirk%20Stockburger_bii_pdm_add-in.svg)
[Further information about this project](http://bii.erppdm.com/BiIUniversalExistingFunctionalityLong.html "Further Information")

## Minimum Prerequisites
- C# 6 
- .NET 2
- SolidWorks Enterprise PDM 2014 SP0

## Getting Started
- Add the EPDM **Interop.epdm.dll/Runtime Version: v2.0.50727** to the references.
- Sign the assembly. 
- Compile the add-in.
- Create the user environment variable **SWPAW.ini** and assign the full filename to it.
- Log out and log in again to apply changes.
- Fit the **SWPAW.ini** to your needs.
- Add the add-in to the vault.
- Restart the system.
- Write your own scripts to extend SolidWorks PDM.

## How to debug DLLs
[Hitting breakpoints in .NET Class Libraries while debugging with Visual Studio 2010](http://through-the-interface.typepad.com/through_the_interface/2010/04/hitting-breakpoints-in-net-class-libraries-while-debugging-with-visual-studio-2010.html)

## How to use the demo script for AutoHotkey
The demo script intercepts button clicks and extends the context menu of the right mouse button for selected files and folders.
- Create the directory **C:\SWPAW** and copy all files and directories into it.
- Create a button with the following properties on a PDM data card.
  - Command type: **Run Add-in**
  - Name of add-in: **AHK:HelloWorld** - The entry **EdmCmd_CardButtonExecutionCondition=.<@1>^AHK:.*$** in the **SWPAW.ini** ensures that only buttons beginning with the name **AHK:** are used by the add-in.

## How to debug scripts in AutoHotkey
- Set the value **DebugMode=true** in the **[System]** section in **SWPAW.ini**.
- Execute the action to be debugged in PDM.
- Use the new created **temp file** and the **name of the executed hook** by filling in the required information in the **</Only for debugging>** area in **BiIMainScript.ahk**.
- Run the debugger for [AutoHotkey](https://www.autohotkey.com).
- If you are looking for an **IDE**, you will probably find it [**here**](https://github.com/ahkscript/awesome-AutoHotkey#integrated-development-environment).
