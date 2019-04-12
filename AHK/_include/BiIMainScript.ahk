;~ ##########################################################################
;~ Copyright (c) 2014 Ulf-Dirk Stockburger
;~ ##########################################################################

;~ <Runs the main script for each section in the ini file>
{
	Global iniFile
	Global callUDF
	;~ Only for debugging existing ini files
	debug := 0
	;~ Check if ini file was given to the script
	if(debug=1)
	{
		;~ </Only for debugging>
		iniFile = C:\SWPAW\AHK\_debug\debug.tmp ;~ Filename of the INI file to debug
		callUDF = EdmCmd_CardButton ;~ Function name to call in UserDefinedChecks
		;~ </Only for debugging>
	}
	else
	{
		;~ Check if the INI file has been sent to the script
		If (%0%<2) 
		{
			Msgbox, 16, BiI Error, No INI file was sent to the script.
			ExitApp 1
		}
		else
		{
			IfNotExist %1%
			{
				Msgbox, 16, BiI Error, The given INI file doesn't exist: %1%
				ExitApp 1
			}
			else
			{
				iniFile =  %1%
			}
			if(%2% = "")
			{
				callUDF = %2%
			}
			else
			{
				Msgbox, 16, BiI Error, The given value to call the user defind function is empty.
				ExitApp 1					
			}
		}
	}
	CheckIfDebugModeIsRquierd()
	;~ Read the INI file and fill the array with sections
	GetAllIniFileSections() 
	;~ Loop over all sections in the INI file
	for sectionIndex, sectionItem in iniFileSections 
	{	
		Global iniFileSection 
		Global errorCode
		;~ Current section
		iniFileSection:= sectionItem
		;~ Your code in this function in the file Demo.ahk
		UserDefinedChecks()
		;~ If an error occurs, the loop will be terminated.
		if(errorCode != 0)
		{
			break
		}
	}
	if(errorCode = 0 && infoMessage != "")
	{
		MsgBox,,AHK script information, %infoMessage%
	}
	if(debugMode != 0)
	{
		if(errorCode = 0)
		{
			MsgBox,, SWPAW, Exit script without errors.
		}
		else
		{
			MsgBox, 16, SWPAW, Exit script with errors.
		}
	}
	ScriptExit(vault)
}
;~ <Runs the main script for each section in the ini file>

;~ <Reads the INI file and fill the array with sections>
GetAllIniFileSections()
{
	Global iniFile
	Global BiIRegExPatternSection
	Global iniFileSections
	Global iniFileSectionsCount
	Global errorCode
	
	Loop, Read, %iniFile%
	{
		Loop, Parse, A_LoopReadLine, %A_Tab%
		{
			if(RegExMatch(A_LoopField, BiIRegExPatternSection) = 1)
			{
				;~ Getting the section without brackets
				iniFileSections.Push(SubStr(A_LoopField, 2, StrLen(A_LoopField)-2))
				++iniFileSectionsCount
			}
		}
	}
}
;~ </Reads the INI file and fill the array with sections>

;~ <Connect to the vault>
ConnectToVault(exitIfError)
{
	;~ Using the global defined vault
	Global vault
	Global errorCode
	Global BiivaultName
	errorCode := 0
	if(!IsObject(vault))
	{
		;~ Stops the time if debug mode is enabeld
		CheckDebugMode(0,"")		
		;~ Default value: No connection to the vault
		errorCode := 1
		;~ Checks if the key in the INI file contains the vault name
		IniRead, vaultName, %iniFile%, %iniFileSection%, %BiivaultName%
		;~ Binds the COM Interface
		if(vault = "")
		{
			vault := ComObjCreate("ConisioLib.EdmVault")			
		}
		;~ Checks if a connection to the vault exists
		if(!vault.IsLoggedIn)
		{
			;~ Connect to the vault, if neccessary
			vault.LoginAuto[vaultName, 0]
			loggedIn := vault.IsLoggedIn
			if(exitIfError != 0)
			{
				if(!loggedIn || loggedIn = 0)
				{
					;~ Terminates the script with error message and error code
					SplitPath, A_ScriptName,,,, scriptName
					Msgbox, 16, BiI Error, Error in script '%scriptName%'.
					errorCode := 666
					ScriptExit(vault)
				}
			}
		}
		if(loggedIn != 0)
		{
			;~ Connection to vault successful
			errorCode := 0
		}
		;~ Writes the time difference and message to the debug file if debug mode is enabeld
		debugMsg := "AHK - Time needed to connect to the vault: "
		CheckDebugMode(1,debugMsg)
	}
	return errorCode
}

;~ <Cleaning up COM objects>
ReleaseObject(ByRef obj)
{
	if(IsObject(obj))
	{
		ObjRelease(obj)
	}
}
;~ </Cleaning up COM objects>

;~ <Finishing the script>
ScriptExit(vault)
{
	Global BiIErrorMsg
	Global errorCode
	Global debugMode
	Global iniFile

	ReleaseObject(vault)
	if(debugMode = 0 )
	{
		if(errorCode = 0)
		{
			FileDelete, %iniFile%
		}
	}
	if(errorCode != 0)
	{
		msg = %BiIErrorMsg%%errorMsg%
		FileAppend, %msg%,%iniFile%
	}
	ExitApp %errorCode%
}
;~ </Finishing the script>