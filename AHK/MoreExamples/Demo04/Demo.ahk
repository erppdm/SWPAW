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
	if(callUDF = "EdmCmd_PreState") 
	{
		EdmCmd_PreState()
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

EdmCmd_PreState()
{
	Global EdmCmd_PreStateFinished
	doIt := 1
	if(doIt = 1 && !EdmCmd_PreStateFinished)
	{
		;~ Sets the value to 1 to ensure that this routine is executed only once even with multiple selection
		;~ Comment out this line if you want to create an ECN for each file in a multiple selection
		;~ EdmCmd_PreStateFinished := 1
		try
		{
			;~ Global variables
			Global errorCode
			Global errorMsg
			Global iniFile
			Global IniFileSection
			Global BiIpreStateDestinationStateID
			Global BiIfileCurrentVersion
			;~ Local variables
			nextStep := 0
			;~ <Replace these variables with yours>
			destFileExtension := ".docx"
			templateName := "C:\SWPAW\ECN\template\template.docx"
			destFolder := "C:\SWPAW\ECN\"
			;~ Defines the destination status that requires a comment
			destinationStateIDs := ["10","11"]
			;~ </Replace these variables with yours>
			;~ Reads the destination status from the INI file
			IniRead, preStateDestinationStateID, %iniFile%, %IniFileSection%, %BiIpreStateDestinationStateID%,
			If(preStateDestinationStateID  = "" || preStateDestinationStateID  = "ERROR")
			{
				errorCode := 1
				errorMsg := "The destination status cannot be read from the INI file."
				return
			}
			;~ Checks if destination status is in array destinationStateIDs
			Loop % destinationStateIDs.Length()
			{
				if(destinationStateIDs[A_Index] = preStateDestinationStateID)
				{
					nextstep := 1
					break
				}
			}
			;~ Checks if destination status is processable
			if(nextStep = 0)
			{
				errorCode := 0
				return
			}
			;~ Reads the files current version from the INI file
			IniRead, fileCurrentVersion, %iniFile%, %IniFileSection%, %BiIfileCurrentVersion%,
			If(fileCurrentVersion = "" || fileCurrentVersion = "ERROR")
			{
				errorCode := 2
				errorMsg := "The files current version cannot be read from the INI file."
				return
			}
			;~ Saves the new file and overwrite if it already exists
			SplitPath, iniFileSection, fName, fDir, fExt
			;~ Creates the new file Name and copy the template into the destination folder
			destFileName := destFolder . fName . ".V" . fileCurrentVersion . destFileExtension
			FileCopy, %templateName%, %destFileName%, 1
			;~ Checks if the new file has been written
			IF(!FileExist(destFileName))
			{
				errorCode := 3
				errorMsg := "The file '" . destFileName . "' cannot be written."
				return        
			}
			;~ Gets the size and the time of the file to compare after RunWait
			FileGetTime, fileTimeOrg, %destFileName%
			FileGetSize, fileSizeOrg, %destFileName%
			;~ Opens the file and waits until the user has saved the file
			RunWait, %destFileName%,, UseErrorLevel, destProghWnd
			;~ Checks again if the new file exists
			IF(!FileExist(destFileName))
			{
				errorCode := 4
				errorMsg := "The file '" . destFileName . "' has been deleted."
				return        
			}
			;~ Gets the size and the time of the file again to compare
			FileGetTime, fileTimeNew, %destFileName%
			FileGetSize, fileSizeNew, %destFileName%
			If(fileTimeOrg = fileTimeNew)
			{
				errorCode := 5
				errorMsg := "The file date of '" . destFileName . "' hasn't been changed."
				return              
			}
			;~ Checks old and new file size to check for changes
			If(fileSizeOrg = fileSizeNew)
			{
				errorCode := 6
				errorMsg := "The file size of '" . destFileName . "' hasn't been changed." 
				return        
			}
		}
		catch e
		{
			errorCode := 666
			errorMsg := "A fatal error occured during execution."
		}
	}
}
;~ </User functions>
