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
	if(callUDF = "EdmCmd_PostAdd") 
	{
		EdmCmd_PostAdd()
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

EdmCmd_PostAdd()
{
	{
		Global EdmObject_Folder
		Global EdmObject_File
		Global errorCode
		Global errorMsg
		Global iniFile
		Global IniFileSection
		Global BiIfileName
		Global BiIfileID
		Global BiImainWindowHandleInt
		Global BiIpostAddParentFolderID
		Global BiIfileID
		;~ Connects to the vault if necessary.
		ConnectToVault(1)
		;~ Reads the main window handle
		IniRead, mainHhWnd, %iniFile%, %iniFileSection%, %BiImainWindowHandleInt%,
		If(mainHhWnd == "" | mainHhWnd = "ERROR")
		{
			errorCode := 1
			errorMsg := "The main window handle cannot be read from the INI file in function 'EdmCmd_PostAdd'."
			return
		}
		;~ Reads the parent folder of the new file
		IniRead, postAddParentFolderID, %iniFile%, %IniFileSection%, %BiIpostAddParentFolderID%,
		If(postAddParentFolderID = "" | postAddParentFolderID = "ERROR")
		{
			errorCode := 1
			errorMsg := "The parent folder id cannot be read from the INI file in function 'EdmCmd_PostAdd'."
			return
		}
		;~ Reads the file id of the new file
		IniRead, fileID, %iniFile%, %IniFileSection%, %BiIfileID%,
		If(fileID = "" | fileID = "ERROR")
		{
			errorCode := 1
			errorMsg := "The file id cannot be read from the INI file in function 'EdmCmd_PostAdd'."
			return
		}
		;~ Gets the folder object from vault
		folder := vault.GetObject(EdmObject_Folder, postAddParentFolderID)
		If(folder = "" )
		{
			errorCode := 1
			errorMsg := "The object for the parent folder cannot be fetched in function 'EdmCmd_PostAdd'."
			return
		}
		try
		{
			folderDelimiter := "\"
			;~ Array for the file names matching the pattern
			fileNameArray := []
			;~ Gets the file's name and directory
			SplitPath, iniFileSection,fName, fDir, fExt
			;~ Gets the directory name in which the file is stored
			folderArray := StrSplit(fDir, folderDelimiter)
			;~ Creates the RegEx pattern for file names
			fDirRegExPattern := ".*" . folderArray[folderArray.Length()] ".*"
			;~ Loop over files in the directory
			loopOver :=  fDir . "\*.*"
			;~ Checks the folders length
			if(StrLen(folderArray[folderArray.Length()]) != 7)
			{
				errorCode := 0
				return
			}
			;~ Gets all files in current folder
			Loop Files, %loopOver%
			{
				;~ Checks if the filename matches the pattern
				if(RegExMatch(A_LoopFileName, fDirRegExPattern) = 1)
				{
					StringMid, fileNum, A_LoopFileName, 9, 3
					Number := (fileNum . String) , Number += 0
					fileNameArray.Push(Number)
				}
			}
			;~ Gets the highest file number
			lastFileNumber := 0
			Loop % fileNameArray.Length()
			{
				tmpNumber := fileNameArray[A_Index]
				if (tmpNumber > lastFileNumber)
				lastFileNumber := tmpNumber
			}
			;~ Creates the new file name
			newFileName := folderArray[folderArray.Length()] . "-" . Format("{:03}", lastFileNumber + 1) . "." . fExt
		}
		catch e
		{
			errorCode := 1
			errorMsg := "The new file name cannot be determined in function 'EdmCmd_PostAdd'."
			return
		}
		;~ Renames the new file
		try
		{
			file := vault.GetObject(EdmObject_File, fileId)
			file.RenameEx(mainHhWnd,newFileName,0)
		}
		catch e
		{
			errorCode := 1
			errorMsg := "The file cannot be renamed in function 'EdmCmd_PostAdd'."
			return
		}
		if(!file)
		{
			errorCode := 1
			errorMsg := "The file cannot be renamed in function 'EdmCmd_PostAdd'."
			return
		}  
	}
}
;~ </User functions>
