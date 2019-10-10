# SWPAW
**S**olid**W**orks **P**DM **A**dd-In **W**rapper

A wrapper making possible to create SolidWorks Professional PDM add-ins in any programming language, including scripting languages.

## Why
Actually I developed this PDM Add-In for my own prototype programming. After a short time it turned out that it is also suitable for everyday use. That's why I decided to make it available to the public to allow every PDM administrator developing own add-ins in the preferred language.

## Workflow
Before the script is executed, a set of default information for the executed event will be created and given to the script. Simple evaluations are possible without having access to the PDM-API. Further information can be collected via SQL and PDM-API. A prerequisite to use the PDM-API is that the chosen language supports [COM](https://en.wikipedia.org/wiki/Component_Object_Model).

![](http://bii.erppdm.com/images/Ulf-Dirk%20Stockburger_bii_pdm_add-in.svg)
[Further information about this project](http://bii.erppdm.com/BiIUniversalExistingFunctionalityLong.html "Further Information")

## Limitations
It is important to know that for time-critical add-ins or actions that cannot be executed outside the current thread, programming in .NET or C++ cannot be avoided. Nevertheless, there are a lot of tasks that can also be implemented without .NET or C++ knowledge. And this, from my point of view, with less code in a short time. The prerequisite is that you master your chosen language.

## Minimum Prerequisites
- C# 6 
- .NET 2
- SolidWorks Enterprise PDM 2014 SP0

## Getting Started

**.NET 2.0**
- Add the EPDM **Interop.epdm.dll/Runtime Version: v2.0.50727** to the references.
- Comment out the following in **SWPAWSourceCode\SWPAW.cs**

  **using EdmLib;**
  
  **public void OnCmd(ref EdmCmd poCmd, ref Array ppoData)**
  
  **System.Array user8Groups = null;**

**.NET 4.0**
- Add the EPDM **EPDM.Interop.epdm.dll/Runtime Version: v4.0.30319** to the references.
- Comment out the following in **SWPAWSourceCode\SWPAW.cs**

  **using EPDM.Interop.epdm;**

  **public void OnCmd(ref EdmCmd poCmd, ref file mdData[] ppoData)**
  
  **object[] user8Groups = null;**

Both versions
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

## Where to find an IDE for AutoHotkey
- Have a look [here](https://github.com/ahkscript/awesome-AutoHotkey#integrated-development-environment).

## How to start with SQL and AutoHotkey
- Have a look [here](https://github.com/Jim-VxE/AHK-Lib-ADOSQL) or [here](https://autohotkey.com/board/topic/83542-func-adosql-uses-ado-to-manage-sql-transactions-v503l/).

## How to start with PDM and AutoHotkey
- Logs in to the specified vault.
```AutoHotkey
_vault := ComObjCreate("ConisioLib.EdmVault")			
_vault.LoginAuto[<vault name>, 0]
```
- Gets the PDM file object via file id.
``` AutoHotkey
_file := _vault.GetObject(EdmObject_File, <file id>)
```
- Gets a variable value by ref. This [script](https://github.com/cocobelgica/AutoHotkey-ComDispatch/blob/master/ComVar.ahk) is required.
``` AutoHotkey
_objVarValue := ComVar()
_var := _file.GetEnumeratorVariable("")
_var.GetVar(<variable name>, <configuration name>, _objVarValue.ref)
_varValue := _objVarValue[]
ComVarDel(_objVarValue)

```
- Gets a list of names of the configurations for the specified version of this file.
``` AutoHotkey
_configList := _file.GetConfigurations()
_pos := _configList.GetHeadPosition()
while(!_pos.IsNull){
	_configurationName := _configList.GetNext(_pos)
}
```
- Searches the vault for files with specific conditions. Have a look at [EdmSearchToken Enumeration](http://help.solidworks.com/2014/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.EdmSearchToken.html) for possible tokens.

```AutoHotkey
_search := _vault.CreateSearch()
_search.SetToken(1, 1)
;~ Set all required tokens.
_searchResult := _search.GetFirstResult()
while(_searchResult){
	_searchResult := _search.GetNextResult()
}
```

## How to run scripts in AutoHotkey

To work correctly executed scripts have to return an **ErrorLevel** for the evaluation in AutoHotkey. All integers except 0 are errors.

- Starts a VBScript and waits for its completion. To return an **ErrorLevel** it ends with **WScript.Quit(0)**. 
```
cscript := "C:\Windows\System32\cscript.exe"
vbscript := "C:\SWPAW\VBS\Demo.vbs"	
RunWait, %cscript% %vbscript%,, Hide UseErrorLevel, scriptPid
```
- Starts a batch script and waits for its completion. To return an **ErrorLevel** it ends with **Exit 0**. 
```
batchscript := "C:\SWPAW\BAT\tmp.bat"
RunWait, %batchscript%,, Show UseErrorLevel, scriptPid
```

The calling of scripts and programs always follows the above pattern.

- After completion of the script, you can evaluate the **ErrorLevel** and handle errors. Error code and error message will be passed to SWPAW.
```
if(ErrorLevel != 0)
{
	errorCode := ErrorLevel
	if(errorCode = 1)
	{
		errorMsg := "Error 1 in script."
	}
	else if(errorCode = 2)
	{
		errorMsg := "Error 2 in script."
	}		
	else
	{
		errorMsg := "Unknown error " . errorCode . " in script."		
	}		
	return		
}
```
