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
	try	{
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
	if(callUDF = "EdmCmd_CardButton") {
		EdmCmd_CardButton()
	}	
	;~ Writes the time difference and message to the debug file if debug mode is enabeld
	debugMsg := "AHK - Time needed to run the function 'UserDefinedChecks': "
	CheckDebugMode(1,debugMsg)	
}
;~ </This function will be called for each file or folder in the INI file>

EdmCmd_CardButton()
{
	;~ MsgBox,Debug
	try	{	
		Global errorCode
		Global errorMsg
		Global iniFile
		Global IniFileSection
		Global BiIfileName
		Global BiImainWindowHandleInt
		Global BiIcardButtonNameOfAddInToCall
		Global BiIvaultRootFolder
		fileFilter := "All SolidWorks files(*.sld*)|*.sld*"
		buttonName := "AHK:AddRefs"
		;~ Reads the button's id to execute
		IniRead, cardButtonNameOfAddInToCall, %iniFile%, %iniFileSection%, %BiIcardButtonNameOfAddInToCall%
		If(cardButtonNameOfAddInToCall == "" || cardButtonNameOfAddInToCall = "ERROR") {
			errorCode := 1
			errorMsg := "The buttons's id cannot be read from the INI file."
			return
		}
		if (cardButtonNameOfAddInToCall != buttonName) {
			errorCode := 0
			return					
		}
		;~ Reads the main window handle
		IniRead, mainHhWnd, %iniFile%, %iniFileSection%, %BiImainWindowHandleInt%
		If(mainHhWnd == "" || mainHhWnd = "ERROR") {
			errorCode := 2
			errorMsg := "The main window handle cannot be read from the INI file."
			return
		}
		;~ Reads the filename
		IniRead, fileName, %iniFile%, %IniFileSection%, %BiIfileName%
		If(fileName == "" || fileName = "ERROR") {
			errorCode := 5
			errorMsg := "The filename cannot be read from the INI file."
			return
		}
		;~ Reads the vault root folder
		IniRead, vaultRootFolder, %iniFile%, %iniFileSection%, %BiIvaultRootFolder%
		if(vaultRootFolder = "" || vaultRootFolder = "ERROR") {
			errorCode := 6
			errorMsg := "The vault root folder cannot be read from the INI file."
			return
		}
		;~ Gets the reference to add it to the file
		aCR := ComObjCreate("AddCustomReference.CustomRef")
		errorCode := aCR.Add(fileName, fileFilter, vaultRootFolder) 
		if (errorCode != 0) {
			errorMsg := "Error code: " . errorCode
		}
		else {
			MsgBox,,Add reference, Done
		}
		aCR =
	}
	catch e {
		errorCode := 666
		errorMsg := "A fatal error occured during execution."
	}
}
;~ </User functions>
