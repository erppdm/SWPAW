;~ ##########################################################################
;~ Copyright (c) 2014 Ulf-Dirk Stockburger
;~ ##########################################################################

;~ Demo script for Autohotkey
;~ Autohotkey and AutoIt are one of the simplest and most powerful scripting languages for Windows I have ever worked. They are awesome.

;~ <Global variables>
	#Include %A_ScriptDir%\_include\BiIConsts.ahk ;~
	{
		Global configsDelimiter
		Global callUDF = ;~ 
		Global iniFileSection = ;~ Full filename to each file.
		Global debugFile = ;~ Full name of the debug file.
		Global iniFile = ;~ Full name of the INI file.
		Global debugMode := 0 ;~ !=0 = The script was started in debug mode.
		Global startTickCount ;~ Ticks at the beginning of the script.
		Global tickCount ;~ Ticks between the checks
		Global iniFileSections := [] ;~ All [Sections] in the INI file without brackets
		Global vault = ;~ ConisioLib.EdmVault
		Global errorCode := 0 ;~ Return value to the BiI add-in.
		Global errorMsg = ;~ Return value to the BiI add-in will be written to the INI file.
		Global infoMessage = ;~ Returns a list files, that cannot be processed
		Global iniFileSectionsCount := 0 ;~ The number of ini file sections to process
		Global iniFileSectionsCurrentInProcess := 0 ;~ The number of current ini file section in process
	}
;~ </Global variables>

;~ <Include>
{
	;~ Functions
	#Include %A_ScriptDir%\_include\ComByefParameters.ahk ;- Used to get variant values ByRef. In c# it's the 'out' parameter
	;~ BiI
	#Include %A_ScriptDir%\_include\BiIVariables.ahk ;~
	#Include %A_ScriptDir%\_include\BiIDebugMode.ahk ;~
	;~ EPDM_Enums
	#Include %A_ScriptDir%\_include\EdmObjectType_Enumeration.ahk ;~
	#Include %A_ScriptDir%\_include\EdmUtility_Enumeration.ahk ;~
}
;~ </Include>

;~ <Main Script>
{
	;~ Include the main script to start at this point. It was outsourced to improve the clarity of the script
	#Include %A_ScriptDir%\_include\BiIMainScript.ahk ;~
}
;~ </Main Script>

;~ <User functions>
;~ <This function will be called for each file or folder in the INI file>
UserDefinedChecks()
{
	;~ <tops the time if debug mode is enabeld
	CheckDebugMode(0,"")
	Global errorCode
	Global errorMsg
	Global vault
	Global iniFileSection
	Global callUDF
	Global errorCode
	if(callUDF = "EdmCmd_CardButton") 
	{
		EdmCmd_CardButton()
	}
	else if(callUDF = "EdmCmd_Menu") 
	{
		EdmCmd_Menu()
	}
	else
	{
		errorCode := 1
		errorMsg := "The function isn't available: "%callUDF%
		ScriptExit(vault)
	}
	;~ Writes the time difference and message to the debug file if debug mode is enabeld
	debugMsg := "AHK - Time needed to run the function 'UserDefinedChecks': "
	CheckDebugMode(1,debugMsg)	
}
;~ </This function will be called for each file or folder in the INI file>

EdmCmd_CardButton()
{
	Global errorCode
	Global errorMsg
	Global iniFile
	Global IniFileSection
	Global BiIfileName
	Global BiIfileID
	Global EdmObject_File
	;~ Reads the main window handle
	IniRead, mainHhWnd, %iniFile%, %iniFileSection%, %BiImainWindowHandleInt%,
	If(mainHhWnd == "" | mainHhWnd = "ERROR")
	{
		errorCode := 1
		errorMsg := "The main window handle cannot be read from the INI file in function 'EdmCmd_Menu'."
		return
	}
	;~ Reads the filename
	IniRead, fileName, %iniFile%, %IniFileSection%, %BiIfileName%,
	If(fileName == "" | fileName = "ERROR")
	{
		errorCode := 2
		errorMsg := "Error in function 'EdmCmd_CardButton'"
		return
	}
	;~ Reads the file id
	IniRead, fileId, %iniFile%, %iniFileSection%, %BiIfileID%,
	if(fileId = "" | fileId = "ERROR")
	{
		errorCode := 4
		errorMsg := "The file id cannot be read from the INI file in function 'EdmCmd_Menu'."
		return
	}
	;~ Connects to the vault if necessary.
	ConnectToVault(1)
	if(errorCode != 0)
	{			
		errorCode := 3
		errorMsg := "Cannot connent to the vault."
		return
	}
	;~ Gets the file object from the vault
	file := vault.GetObject(EdmObject_File, fileId)
	if(file = "")
	{
		errorCode := 4
		errorMsg := "The object for the file in the vault cannot be created in function 'EdmCmd_Menu'."
		return
	}
	fId := file.id
	MsgBox,, SWPAW-Hello World, INI file was read by 'EdmCmd_CardButton', filename: %fileName%, file id from file: %fId%, file id from ini file: %fileId%
}

EdmCmd_Menu()
{
	;~ Global variables
	Global errorCode
	Global errorMsg
	Global iniFile
	Global IniFileSection
	Global BiImenuMenuID
	Global BiImainWindowHandleInt
	Global BiIfileName	
	;~ Reads the main window handle
	IniRead, mainHhWnd, %iniFile%, %iniFileSection%, %BiImainWindowHandleInt%,
	If(mainHhWnd == "" | mainHhWnd = "ERROR")
	{
		errorCode := 1
		errorMsg := "The main window handle cannot be read from the INI file in function 'EdmCmd_Menu'."
		return
	}
	;~ Reads the menu id
	IniRead, menuMenuID, %iniFile%, %IniFileSection%, %BiImenuMenuID%,
	If(menuMenuID = "" | menuMenuID = "ERROR")
	{
		errorCode := 2
		errorMsg := "The menu id cannot be read from the INI file in function 'EdmCmd_Menu'."
		return
	}
	;~ Runs the routine for the menu with the number 10000
	if(menuMenuID = 10000) 
	{
		;~ Reads the filename
		IniRead, fileName, %iniFile%, %IniFileSection%, %BiIfileName%,
		If(fileName == "" | fileName = "ERROR")
		{
			errorCode := 3
			errorMsg := "Error in function 'EdmCmd_Menu'"
			return
		}		
		MsgBox,, SWPAW-Hello World, INI file was read by 'EdmCmd_Menu' for files, filename: %fileName%
	}
	;~ Runs the routine for the menu with the number 10001
	if(menuMenuID = 10001)
	{
		MsgBox,, SWPAW-Hello World, INI file was read by 'EdmCmd_Menu'for folders, folder name: %IniFileSection%
	}
}
;~ </User functions>
