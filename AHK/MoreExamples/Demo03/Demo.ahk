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
	Global EdmCmd_MenuFinished
	doIt := 1
	if(doIt = 1 && !EdmCmd_MenuFinished)
	{
		;~ Sets the value to 1 to ensure that this routine is executed only once even with multiple selection
		EdmCmd_MenuFinished := 1
		;~ Confirmation prompt
		MsgBox, 262436, SWPAW-Rename files, All selected files will be renamed. Are you sure?
		IfMsgBox No
		{
			return
		}
		;~ Global variables
		Global errorCode
		Global errorMsg
		Global iniFile
		Global BiIfileID
		Global BiImainWindowHandleInt
		Global BiIfileIsLocked
		Global EdmObject_File
		Global EdmObject_Folder
		;~ Local variables
		msgErrorLocked := "File '_file_' is locked."
		msgErrorRename := "The file '_file_' could not be renamed."
		msgInformation := "No serial number found for file '_file_'."

		;~ <Replace these variables with yours>
		varSerialNumber := "<variable name>"
		varSerialNumberConfig := "<configuration name>"
		;~ </Replace these variables with yours>
		
		;~ <Checks if all files are unlocked>
		;~ Loop over all sections in the INI file
		for sectionIndex, sectionItem in iniFileSections 
		{	
			;~ Reads if the file is locked
			IniRead, fileIsLocked, %iniFile%, %sectionItem%, %BiIfileIsLocked%,
			If(fileIsLocked == "" | fileIsLocked = "ERROR")
			{
				errorCode := 1
				errorMsg := "The status if the file is locked cannot be read from the INI."
				return
			}
			;~ Creates the error message if the file is locked
			if(fileIsLocked = "True")
			{
				tmpMsg := StrReplace(msgErrorLocked, "_file_", sectionItem)
				errorMsg := errorMsg . tmpMsg . newLine
			}
		}
		if(errorMsg)
		{
			errorCode := 2
			errorMsg := errorMsg . newLine . "The renaming will be canceled."
			return
		}
		;~ </Checks if all files are unlocked>
		
		;~ Reads the main window handle
		IniRead, mainHhWnd, %iniFile%, %sectionItem%, %BiImainWindowHandleInt%,
		If(mainHhWnd == "" | mainHhWnd = "ERROR")
		{
			errorCode := 3
			errorMsg := "The main window handle cannot be read from the INI."
			return
		}
		;~ Connects to the vault if necessary
		ConnectToVault(1)
		if(errorCode != 0)
		{      
			errorCode := 4
			errorMsg := "Cannot connenct to the vault."
			return
		}
		;~ <Renames all files in selection>
		tmpMsg := ""
		errorMsg := ""
		informationMsg := ""
		errorRenamMsg := ""
		;~ Loop over all sections in the INI file
		for sectionIndex, sectionItem in iniFileSections 
		{	
			newFileName := ""
			;~ Reads the file id
			IniRead, fileId, %iniFile%, %sectionItem%, %BiIfileID%,
			if(fileId = "" || fileId = "ERROR")
			{
				errorCode := 5
				errorMsg := "The file id cannot be read from the INI."
				return
			}
			;~ Gets the object for the file in the vault
			file := vault.GetObject(EdmObject_File, fileId)
			if(!file)
			{
				errorCode := 7
				errorMsg := "The object for the file in the vault cannot be created."
				return
			}
			;~ Reads the serial number in the variable 'varSerialNumber' from the file
			fileSerialNumber := GetFileVarVal(file, varSerialNumber, varSerialNumberConfig)
			if(!fileSerialNumber)
			{
				tmpMsg := StrReplace(msgInformation, "_file_", sectionItem)
				informationMsg := informationMsg . tmpMsg . newLine
			}			
			if(fileSerialNumber)
			{
				;~ <Creates the new file name>
				try
				{
					SplitPath, sectionItem,,, fExt, fName
					newFileName := fileSerialNumber . "." . fExt
				}
				catch e
				{
					errorCode := 8
					errorMsg := "Cannot read the file name."
					return
				}
			
				;~ Renames the file
				try
				{
					;~ Checks whether the file has been checked out in the meantime
					file := vault.GetObject(EdmObject_File, fileId)
					iSLocked := file.IsLocked
					if(isLocked = 0)
					{
						if(file.name != newFileName)
						{
							file.RenameEx(mainHhWnd, newFileName, 0)      
						}
					}
				}
				catch e
				{
					errorCode := 9
					errorMsg := "Cannot rename the file '" . sectionItem . "'."
					return
				}
				;~ Checks if the renaming of the file was successful
				try
				{
					file := vault.GetObject(EdmObject_File, fileId)
					if(!file)
					{
						errorCode := 10
						errorMsg := "The object for the renamed file in the vault cannot be get."
						return
					}
					if(file.Name && file.Name != newFileName && newFileName)
					{
						;~ Creates the information message if the file cannot be renamed
						tmpMsg := StrReplace(msgErrorRename, "_file_", sectionItem)
						errorRenamMsg := errorRenamMsg . tmpMsg . newLine
					}
				}
				catch e
				{
					errorCode := 11
					errorMsg := "The file " . iniFileSection . "could not be renamed to '" . newFileName . "'in function 'EdmCmd_Menu003'."
					return
				}
			}
		}
		;~ </Renames all files in selection>
		;~ <Creates and displays the output message>
		if(informationMsg || errorRenamMsg)
		{
			If(informationMsg && errorRenamMsg)
			{
				errorMsg := RegExReplace(informationMsg . newLine . errorRenamMsg, newLine, "`r`n")
			}
			else if(informationMsg)
			{
				errorMsg := RegExReplace(informationMsg, newLine, "`r`n")
			}
			else if(errorRenamMsg)
			{
				errorMsg := RegExReplace(errorRenamMsg, newLine, "`r`n")
			}
		}
		if(errorMsg)
		{
			;~ Output the affected files 
			MsgBox, 262192, SWPAW-Rename files, %errorMsg%
		}
		else
		{	
			msg := "All selected files renamed."
			MsgBox, 262208, SWPAW-Rename files, %msg%
		}
		;~ </Creates and displays the output message>
		Controlsend, ahk_parent,{f5}, ahk_id %mainHhWnd%
		;~ </Renames all files in selection>
	}
}
;~ </User functions>
