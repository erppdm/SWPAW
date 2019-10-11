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
		EdmCmd_PreStateFinished := 1
		try
		{
			;~ Global variables
			Global errorCode
			Global errorMsg
			Global iniFile
			Global IniFileSection
			Global BiIpreStateDestinationStateID
			Global BiIpreStateCommentText
			;~ <Replace these variables with yours>
			;~ Defines the destination status that requires a comment
			destinationStateIDs := ["13","16"]
			;~ </Replace these variables with yours>
			;~ Reads the destination status id
			IniRead, preStateDestinationStateID, %iniFile%, %IniFileSection%, %BiIpreStateDestinationStateID%,
			If(preStateDestinationStateID = "" || preStateDestinationStateID = "ERROR")
			{
				errorCode := 1
				errorMsg := "The destination status cannot be read from the INI file."
				return
			}
			;~ Checks if the status requires a comment
			Loop % destinationStateIDs.Length()
			{
				;~ Loop over all destinationStateIDs
				if(destinationStateIDs[A_Index] = preStateDestinationStateID)
				{
					;~ Checks if a comment is given for the requiered destination status id
					IniRead, preStateCommentText, %iniFile%, %IniFileSection%, %BiIpreStateCommentText%,
					if(preStateCommentText = "" || preStateCommentText = "ERROR")
					{
						errorCode := 1
						errorMsg := "A comment to change to this status is mandatory. The transition will be canceled."
					}
				}
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
