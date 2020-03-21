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

SetFileVarVal(Byref file, varName, varValue, varConfig) {
	try	{
		var := file.GetEnumeratorVariable("")
		var.SetVar(varName, varConfig, varValue)
		var.Flush()
	}
	catch e	{
		errorCode := 1
		errorMsg := "Error in 'SetFileVarVal'."
		return
	}
}
;~ <User functions>
;~ <This function will be called for each file or folder in the INI file>
UserDefinedChecks()
{
	;~ <Stops the time if debug mode is enabeld>
	CheckDebugMode(0,"")
	Global errorCode
	Global errorMsg
	Global vault
	Global iniFileSection
	Global callUDF
	Global errorCode
	if(callUDF = "EdmCmd_PreUnlock") {
		EdmCmd_PreUnlock()
	}	
	;~ Writes the time difference and message to the debug file if debug mode is enabeld
	debugMsg := "AHK - Time needed to run the function 'UserDefinedChecks': "
	CheckDebugMode(1,debugMsg)	
}
;~ </This function will be called for each file or folder in the INI file>

;~ <This function will be called once for all sections in the INI file>
EdmCmd_PreUnlock(){
	Global EdmCmd_PreUnlockFinished
	if(!EdmCmd_PreUnlockFinished){		
		EdmCmd_PreUnlockFinished := 1
		Global errorCode
		Global errorMsg
		Global iniFile
		Global BiIfileName
		Global BiIfileID
		Global EdmObject_File
		ConnectToVault(1)
		if(errorCode != 0) {
			return
		}
		try{
			;~ <Replace these variables with yours>
			varConfig := "<configuration name>"
			varDataCard := "<data card variable name>"
			;~ </Replace these variables with yours>
			FormatTime, t, %A_Now%, dd.MM.yyyy HH:mm:ss
			for sectionIndex, sectionItem in iniFileSections {
				IniRead, fileId, %iniFile%, %sectionItem%, %BiIfileID%,
				if(fileId = "" || fileId = "ERROR") {
					errorCode := 2
					errorMsg := "The file id cannot be read from the INI file."
					return
				}
				file := vault.GetObject(EdmObject_File, fileId)
				if(file = "") {
					errorCode := 3
					errorMsg := "The object for the file cannot be created."
					return
				}
				SetFileVarVal(file, varDataCard, file.LockedByUser.FullName . " - " . t, varConfig)
				if(errorCode != 0) {
					errorCode := 4
					errorMsg := "Error while writing the variable."
					return
				}
			}
		}
		catch e {
			errorCode := 666
			errorMsg := "A fatal error occured during execution in function 'EdmCmd_PreUnlock002'."	
			return			
		}
	}
}
;~ </This function will be called once for the INI file>

;~ </User functions>
