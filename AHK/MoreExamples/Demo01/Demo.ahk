;~ ##########################################################################
;~ Copyright (c) 2014 Ulf-Dirk Stockburger
;~ ##########################################################################

;~ Demo script for Autohotkey
;~ Autohotkey and AutoIt are one of the simplest and most powerful scripting languages for Windows I have ever worked. They are awesome.

;~ <Global variables>
	#Include %A_ScriptDir%\_include\BiIConsts.ahk ;~
	{
		Global newLine := "{newLine}" ;~ Required to display multiline error messages.
		Global configsDelimiter
		Global callUDF = 
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
	#Include %A_ScriptDir%\_include\BiIVariables.ahk
	#Include %A_ScriptDir%\_include\BiIDebugMode.ahk
	;~ EPDM_Enums
	#Include %A_ScriptDir%\_include\EdmObjectType_Enumeration.ahk
	#Include %A_ScriptDir%\_include\EdmUtility_Enumeration.ahk
}
;~ </Include>

;~ <Main Script>
{
	;~ Include the main script to start at this point. It was outsourced to improve the clarity of the script
	#Include %A_ScriptDir%\_include\BiIMainScript.ahk
}
;~ </Main Script>

GetFileVarVal(Byref file, varName, ConfigName)
{
	try
	{
		objVarValue := ComVar()
		var := file.GetEnumeratorVariable("")
		var.GetVar(varName, configName, objVarValue.ref)
		varVal := objVarValue[]
	}
	catch e
	{
		errorCode := 1
		errorMsg := "Error in 'GetFileVarVal'." 
		return
	}
	return varVal
}

;~ <User functions>
;~ <This function will be called for each file or folder in the INI file>
UserDefinedChecks()
{
	;~ <Stops the time if debug mode is enabeld
	CheckDebugMode(0,"")
	Global errorCode
	Global errorMsg
	Global vault
	Global iniFileSection
	Global callUDF
	Global errorCode
	if(callUDF = "EdmCmd_Menu") 
	{
		EdmCmd_Menu()
	}
	else
	{
		errorCode := 1
		errorMsg := "The function is not available: "%callUDF%
		ScriptExit(vault)
	}
	;~ Writes the time difference and message to the debug file if debug mode is enabeld
	debugMsg := "AHK - Time needed to run the function 'UserDefinedChecks': "
	CheckDebugMode(1,debugMsg)	
}
;~ </This function will be called for each file or folder in the INI file>

EdmCmd_Menu()
{
	doIt := 1
	if(doIt = 1)
	{
		try
		{
			;~ Global variables
			Global errorCode
			Global errorMsg
			Global iniFile
			Global IniFileSection
			Global BiImenuMenuID
			;~ Reads the menu id
			IniRead, menuMenuID, %iniFile%, %IniFileSection%, %BiImenuMenuID%,
			If(menuMenuID = "" || menuMenuID = "ERROR")
			{
				errorCode := 1
				errorMsg := "Error while fetching the menu id"
				return
			}
			if(menuMenuID = 10000) 
			{
				;~ Concept
				EdmCmd_Menu001("<configuration name>", "<variable name>", 0)
			}
			if(menuMenuID = 10001) 
			{
				;~ Production
				EdmCmd_Menu001("<configuration name>", "<variable name>", 1)
			}
		}
		catch e
		{
			errorCode := 666
			errorMsg := "A fatal error occured during execution"
		}		
	}
}

;~ <User functions>
EdmCmd_Menu001(configName, varName, varValue)
{
	doIt := 1
	if(doIt = 1)
	{
		needle := "i)^.*([.]sld(asm)|(prt))$"
		if(RegExMatch(iniFileSection, needle) = 0)
		{
			return
		}
		try
		{
			Global errorCode
			Global errorMsg
			Global iniFile
			Global IniFileSection
			Global BiImainWindowHandleInt
			Global BiIfileID
			Global EdmObject_File
			IniRead, mainHhWnd, %iniFile%, %iniFileSection%, %BiImainWindowHandleInt%,
			If(mainHhWnd == "" || mainHhWnd = "ERROR")
			{
				errorCode := 1
				errorMsg := "Error while fetching the main window handle"
				return
			}
			ConnectToVault(1)
			if(errorCode != 0)
			{      
				errorCode := 2
				errorMsg := "Error while connecting to the vault"
				return
			}
			IniRead, fileId, %iniFile%, %iniFileSection%, %BiIfileID%,
			if(fileId = "" || fileId = "ERROR")
			{
				errorCode := 3
				errorMsg := "Error while fetching the file id"
				return
			}
			file := vault.GetObject(EdmObject_File, fileId)
			if(!file)
			{
				errorCode := 4
				errorMsg := "Error while fetching the file object"
				return
			}
			if(file.IsLocked = 0)
			{
				errorCode := 5
				errorMsg := "Error, file '" . iniFileSection . "' is locked"
				return
			}
			vars := file.GetEnumeratorVariable("")
			if(!vars)
			{
				errorCode := 6
				errorMsg := "Error while fetching the file variables"
				return
			}
			try
			{
				vars.SetVar(varName, configName, varValue)
				vars.CloseFile(1)
			}
			catch e
			{
				errorCode := 7
				errorMsg := "Error while setting the variable value"
				return
			}
		}
		catch e
		{
			errorCode := 666
			errorMsg := "A fatal error occured during execution"
		}		
	}
}
;~ </User functions>
