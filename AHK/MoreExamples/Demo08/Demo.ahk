;~ ##########################################################################
;~ Copyright (c) 2014 Ulf-Dirk Stockburger
;~ ##########################################################################

;~ Demo script for Autohotkey
;~ Autohotkey and AutoIt are one of the simplest and most powerful scripting languages for Windows I have ever worked. They are awesome.

;~ <Global variables>
	{
		;~ <Change the variable names with the real names in your vault>
		Global varConfig := "<your configuration name>"
		Global varNew := "<your variable name>"
		Global varOld01 := "<your variable name>"
		Global varOld02 := "<your variable name>"
		;~ </Change the variable names with the real names in your vault>
		Global varLegacyData := "legacyData"
	}
	#Include %A_ScriptDir%\_include\BiIConsts.ahk 
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
	#Include %A_ScriptDir%\_include\ComByefParameters.ahk
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
	#Include %A_ScriptDir%\_include\BiIMainScript.ahk
}
;~ </Main Script>

GetFileVarVal(Byref file, varName, ConfigName) {
	try {
		objVarValue := ComVar()
		var := file.GetEnumeratorVariable("")
		var.GetVar(varName, configName, objVarValue.ref)
		varVal := objVarValue[]
	}
	catch e	{
		errorCode := 1
		errorMsg := "Error in 'GetFileVarVal'." 
		return
	}
	return varVal
}

SetFileVarVal(Byref file, varName, varValue, varConfig) {
	try {
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

UserDefinedChecks() {
	CheckDebugMode(0,"")
	Global errorCode
	Global errorMsg
	Global vault
	Global iniFileSection
	Global callUDF
	Global errorCode
	if(callUDF = "EdmCmd_PostLock") {
		EdmCmd_PostLock()
	}
	else if(callUDF = "EdmCmd_PreUnlock") {
		EdmCmd_PreUnlock()
	}
	else {
		errorCode := 1
		errorMsg := "The function is not available: "%callUDF%
		ScriptExit(vault)
	}
	debugMsg := "AHK - Time needed to run the function 'UserDefinedChecks': "
	CheckDebugMode(1,debugMsg)	
}

EdmCmd_PostLock() {
	Global EdmCmd_PostLockFinished
	if(!EdmCmd_PostLockFinished) {
		EdmCmd_PostLockFinished := 1
		Global errorCode
		Global errorMsg
		Global iniFile
		Global BiIfileName
		Global BiIfileID
		Global EdmObject_File
		ConnectToVault(1)
		if(errorCode != 0) {
			errorCode := 1
			errorMsg := "Cannot connect to the vault."
			return
		}
		try {
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
				varNewVal := GetFileVarVal(file, varNew, varConfig)
				if(!varNewVal) {
					SetFileVarVal(file, varLegacyData, 1, varConfig)
				}
				if(errorCode != 0) {
					errorCode := 4
					errorMsg := "Error while writing the variable '" . varLegacyDatalegacyData . "'."
					return
				}
			}
		}
		catch e {
			errorCode := 666
			errorMsg := "A fatal error occured during execution in function 'EdmCmd_PostLock'."	
			return			
		}
	}	
}

EdmCmd_PreUnlock() {
	Global EdmCmd_PreUnlockFinished
	if(!EdmCmd_PreUnlockFinished) {
		EdmCmd_PreUnlockFinished := 1
		Global errorCode
		Global errorMsg
		Global iniFile
		Global BiIfileName
		Global BiIfileID
		Global EdmObject_File
		ConnectToVault(1)
		if(errorCode != 0) {
			errorCode := 1
			errorMsg := "Cannot connect to the vault."
			return
		}
		try{
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
				varNewVal := GetFileVarVal(file, varNew, varConfig)
				varOld01Val := GetFileVarVal(file, varOld01, varConfig)
				varOld02Val := GetFileVarVal(file, varOld02, varConfig)
				if(varNewVal != (varOld01Val . "/" . varOld02Val)) {
					varNewVal := varOld01Val . "/" . varOld02Val
					SetFileVarVal(file, varNew, varNewVal, varConfig)
					if(errorCode != 0) {
						errorCode := 4
						errorMsg := "Error while writing the variable."
						return
					}
				}
			}
		}
		catch e {
			errorCode := 666
			errorMsg := "A fatal error occured during execution in function 'EdmCmd_PreUnlock'."	
			return			
		}
	}
}
