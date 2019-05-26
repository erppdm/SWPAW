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
	if(callUDF = "EdmCmd_CardButton") 
	{
		EdmCmd_CardButton()
	}
	else if(callUDF = "EdmCmd_Menu") 
	{
		EdmCmd_Menu()
	}
	else if(callUDF = "EdmCmd_PreState") 
	{
		EdmCmd_PreState()
	}
	else if(callUDF = "EdmCmd_PreUnlock") 
	{
		EdmCmd_PreUnlock()
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

EdmCmd_CardButton()
{
	if(errorCode = 0)
	{
		EdmCmd_CardButton001()
	}
}

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
				errorMsg := "The menu id cannot be read from the INI file in function 'EdmCmd_Menu'."
				return
			}
			if(menuMenuID = 10000) 
			{
				EdmCmd_Menu001()
			}
			if(menuMenuID = 10001)
			{
				EdmCmd_Menu002()
			}
			if(menuMenuID = 10002) 
			{
				EdmCmd_Menu003()
			}			
		}
		catch e
		{
			errorCode := 666
			errorMsg := "A fatal error occured during execution in function 'EdmCmd_Menu'."
		}		
	}
}

EdmCmd_PreState()
{
	if(errorCode = 0)
	{
		;~ <Require comment for specific transisiton: https://forum.solidworks.com/thread/215419>
		EdmCmd_PreState001()
	}
	if(errorCode = 0)
	{
		;~ <How can EPDM Prompt for ECN form During Transition? What kind of Transition action i have to create or add to fullfill the ECO requirement During transition?: https://forum.solidworks.com/thread/216707>
		EdmCmd_PreState002()
	}
}

EdmCmd_PreUnlock()
{
	if(errorCode = 0)
	{
		;~ <Script as an example of how to read and compare variables and cancel a Check-In if necessary.>
		EdmCmd_PreUnlock001()
	}	
}

EdmCmd_CardButton001()
{
	doIt := 1
	if(doIt = 1)
	{
		try
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
			If(mainHhWnd == "" || mainHhWnd = "ERROR")
			{
				errorCode := 1
				errorMsg := "The main window handle cannot be read from the INI file in function 'EdmCmd_CardButton001'."
				return
			}
			;~ Reads the filename
			IniRead, fileName, %iniFile%, %IniFileSection%, %BiIfileName%,
			If(fileName == "" || fileName = "ERROR")
			{
				errorCode := 2
				errorMsg := "Error in function 'EdmCmd_CardButton001.'"
				return
			}
			;~ Reads the file id
			IniRead, fileId, %iniFile%, %iniFileSection%, %BiIfileID%,
			if(fileId = "" || fileId = "ERROR")
			{
				errorCode := 4
				errorMsg := "The file id cannot be read from the INI file in function 'EdmCmd_CardButton001'."
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
				errorMsg := "The object for the file in the vault cannot be created in function 'EdmCmd_CardButton001'."
				return
			}
			fId := file.id
			MsgBox,, SWPAW-Hello World, INI file was read by 'EdmCmd_CardButton', filename: %fileName%, file id from file: %fId%, file id from ini file: %fileId%
		}
		catch e
		{
			errorCode := 666
			errorMsg := "A fatal error occured during execution in function 'EdmCmd_CardButton'."
		}
	}		
}

EdmCmd_Menu001()
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
			Global BiImainWindowHandleInt
			Global BiIfileName			
			;~ Reads the main window handle
			IniRead, mainHhWnd, %iniFile%, %iniFileSection%, %BiImainWindowHandleInt%,
			If(mainHhWnd == "" || mainHhWnd = "ERROR")
			{
				errorCode := 1
				errorMsg := "The main window handle cannot be read from the INI file in function 'EdmCmd_Menu'."
				return
			}
			;~ Reads the filename
			IniRead, fileName, %iniFile%, %IniFileSection%, %BiIfileName%,
			If(fileName == "" || fileName = "ERROR")
			{
				errorCode := 3
				errorMsg := "Error in function 'EdmCmd_Menu'"
				return
			}		
			MsgBox,, SWPAW-Hello World, INI file was read by 'EdmCmd_Menu' for files, filename: %fileName%
		}
		catch e
		{
			errorCode := 666
			errorMsg := "A fatal error occured during execution in function 'EdmCmd_Menu001'."
		}		
	}
}

EdmCmd_Menu002()
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
			Global BiImainWindowHandleInt
			Global BiIfileName						
			;~ Reads the main window handle
			IniRead, mainHhWnd, %iniFile%, %iniFileSection%, %BiImainWindowHandleInt%,
			If(mainHhWnd == "" || mainHhWnd = "ERROR")
			{
				errorCode := 1
				errorMsg := "The main window handle cannot be read from the INI file in function 'EdmCmd_Menu'."
				return
			}
			MsgBox,, SWPAW-Hello World, INI file was read by 'EdmCmd_Menu'for folders, folder name: %IniFileSection%
		}
		catch e
		{
			errorCode := 666
			errorMsg := "A fatal error occured during execution in function 'EdmCmd_Menu002'."
		}		
	}
}

;~ <Rename the selected files in the vault based on their serial numbers>
EdmCmd_Menu003()
{
	;~ This script only works reliably within a folder in Windows Explorer. It does not work from an Add-In, e.g. the PDM search.
	Global EdmCmd_MenuFinished003
	doIt := 1
	if(doIt = 1 && !EdmCmd_MenuFinished003)
	{
		;~ Sets the value to 1 to ensure that this routine is executed only once even with multiple selection
		EdmCmd_MenuFinished003 := 1
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
		;~ varSerialNumber := "BiISerialNumber"
		varSerialNumber := "BiIPdmNo"
		varSerialNumberConfig := "Default"
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
				errorMsg := "The status if the file is locked cannot be read from the INI file in function 'EdmCmd_Menu003'."
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
			errorMsg := "The main window handle cannot be read from the INI file in function 'EdmCmd_Menu003'."
			return
		}
		;~ Connects to the vault if necessary
		ConnectToVault(1)
		if(errorCode != 0)
		{      
			errorCode := 4
			errorMsg := "Cannot connenct to the vault in function 'EdmCmd_Menu003'."
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
				errorMsg := "The file id cannot be read from the INI file in function 'EdmCmd_Menu003'."
				return
			}
			;~ Gets the object for the file in the vault
			file := vault.GetObject(EdmObject_File, fileId)
			if(!file)
			{
				errorCode := 7
				errorMsg := "The object for the file in the vault cannot be created in function 'EdmCmd_Menu003'."
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
					errorMsg := "Cannot read the file name in function 'EdmCmd_Menu003'."
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
					errorMsg := "Cannot rename the file '" . sectionItem . "' in function 'EdmCmd_Menu003'."
					return
				}
				;~ Checks if the renaming of the file was successful
				try
				{
					file := vault.GetObject(EdmObject_File, fileId)
					if(!file)
					{
						errorCode := 10
						errorMsg := "The object for the renamed file in the vault cannot be get in function 'EdmCmd_Menu003'."
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
;~ </Rename the selected files in the vault based on their serial numbers>

;~ <Require comment for specific transisiton: https://forum.solidworks.com/thread/215419>
EdmCmd_PreState001() 
{
	Global EdmCmd_PreStateFinished001
	doIt := 1
	if(doIt = 1 && !EdmCmd_PreStateFinished001)
	{
		;~ Sets the value to 1 to ensure that this routine is executed only once even with multiple selection
		EdmCmd_PreStateFinished001 := 1
		try
		{
			;~ Global variables
			Global errorCode
			Global errorMsg
			Global iniFile
			Global IniFileSection
			Global BiIpreStateDestinationStateID
			Global BiIpreStateCommentText
			;~ Defines the destination status that requires a comment
			;~ Use this query to get the destination state ids from the data base: Select [StatusID], [Name] From [dbo].[Status]
			destinationStateIDs := ["13","16"]
			;~ Reads the destination status id
			IniRead, preStateDestinationStateID, %iniFile%, %IniFileSection%, %BiIpreStateDestinationStateID%,
			If(preStateDestinationStateID = "" || preStateDestinationStateID = "ERROR")
			{
				errorCode := 1
				errorMsg := "The destination status cannot be read from the INI file in function 'EdmCmd_PreState001'."
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
			errorMsg := "A fatal error occured during execution in function 'EdmCmd_PreState001'."
		}
	}	
}
;~ <Require comment for specific transisiton: https://forum.solidworks.com/thread/215419>

;~ <How can EPDM Prompt for ECN form During Transition? What kind of Transition action i have to create or add to fullfill the ECO requirement During transition?: https://forum.solidworks.com/thread/216707>
EdmCmd_PreState002()
{
	;~ # This script opens a document template for a specific transition and waits until it is closed. If the document was not edited and simply closed, the transition would be aborted. 
	Global EdmCmd_PreStateFinished002
	doIt := 1
	if(doIt = 1 && !EdmCmd_PreStateFinished002)
	{
		;~ Sets the value to 1 to ensure that this routine is executed only once even with multiple selection
		;~ Comment out this line if you want to create an ECN for each file in a multiple selection
		;~ EdmCmd_PreStateFinished002 := 1
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
			;~ </Replace these variables with yours>

			;~ Defines the destination status that requires a comment
			;~ Use this query to get the destination state ids from the data base: Select [StatusID], [Name] From [dbo].[Status]
			destinationStateIDs := ["13","16"]
			;~ Reads the destination status from the INI file
			IniRead, preStateDestinationStateID, %iniFile%, %IniFileSection%, %BiIpreStateDestinationStateID%,
			If(preStateDestinationStateID  = "" || preStateDestinationStateID  = "ERROR")
			{
				errorCode := 1
				errorMsg := "The destination status cannot be read from the INI file in function 'EdmCmd_PreState002'."
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
				errorMsg := "The files current version cannot be read from the INI file in function 'EdmCmd_PreState002'."
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
				errorMsg := "The file '" . destFileName . "' cannot be written in function 'EdmCmd_PreState002'."
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
				errorMsg := "The file '" . destFileName . "' has been deleted. This is a message from function 'EdmCmd_PreState002'."
				return        
			}
			;~ Gets the size and the time of the file again to compare
			FileGetTime, fileTimeNew, %destFileName%
			FileGetSize, fileSizeNew, %destFileName%
			If(fileTimeOrg = fileTimeNew)
			{
				errorCode := 5
				errorMsg := "The file date of '" . destFileName . "' hasn't been changed. This is a message from function 'EdmCmd_PreState002'."
				return              
			}
			;~ Checks old and new file size to check for changes
			If(fileSizeOrg = fileSizeNew)
			{
				errorCode := 6
				errorMsg := "The file size of '" . destFileName . "' hasn't been changed. This is a message from function 'EdmCmd_PreState002'." 
				return        
			}
		}
		catch e
		{
			errorCode := 666
			errorMsg := "A fatal error occured during execution in function 'EdmCmd_PreState002'."
		}
	}
}
;~ </How can EPDM Prompt for ECN form During Transition? What kind of Transition action i have to create or add to fullfill the ECO requirement During transition?: https://forum.solidworks.com/thread/216707>

;~ <Script as an example of how to read and compare variables and cancel a Check-In if necessary.>
EdmCmd_PreUnlock001()
{
	Global EdmCmd_PreUnlockFinished001
	doIt := 1
	if(doIt = 1 && !EdmCmd_PreUnlockFinished001)
	{
		;~ Sets the value to 1 to ensure that this routine is executed only once even with multiple selection
		EdmCmd_PreUnlockFinished001 := 1
		Global errorCode
		Global errorMsg
		Global iniFile
		Global BiIfileName
		Global BiIfileID
		Global EdmObject_File
		;~ Connects to the vault
		ConnectToVault(1)
		if(errorCode != 0)
		{			
			errorCode := 1
			errorMsg := "Cannot connent to the vault."
			return
		}
		try
		{
			;~ <Replace these variables with yours>
			customPropertyName := "BiIRevision"
			customPorpertyConfig := "Default"
			msg := "Message to user - file _file_ custom property _customPropertyName_ = _customPropertyValue_, should be _shouldBeValue_"
			;~ <Replace these variables with yours>
			
			;~ Loop over all sections in the INI file
			for sectionIndex, sectionItem in iniFileSections 
			{	
				;~ Reads the file id
				IniRead, fileId, %iniFile%, %sectionItem%, %BiIfileID%,
				if(fileId = "" || fileId = "ERROR")
				{
					errorCode := 2
					errorMsg := "The file id cannot be read from the INI file in function 'EdmCmd_PreUnlock001'."
					return
				}
				;~ Gets the file object from the vault
				file := vault.GetObject(EdmObject_File, fileId)
				if(file = "")
				{
					errorCode := 3
					errorMsg := "The object for the file in the vault cannot be created in function 'EdmCmd_PreUnlock001'."
					return
				}
				;~ Gets the values to be compared
				shouldBeValue := file.CurrentRevision
				varVal := GetFileVarVal(file, customPropertyName, customPorpertyConfig)
				if(varVal != shouldBeValue)
				{
					;~ Reads the filename
					IniRead, fileName, %iniFile%, %sectionItem%, %BiIfileName%,
					If(fileName == "" || fileName = "ERROR")
					{
						errorCode := 4
						errorMsg := "Error in function 'EdmCmd_PreUnlock001'"
						return
					}
					;~ Creates the error message
					tmpMsg := StrReplace(msg, "_file_", fileName)
					tmpMsg := StrReplace(tmpMsg, "_customPropertyName_", customPropertyName)
					tmpMsg := StrReplace(tmpMsg, "_customPropertyValue_", varVal)
					tmpMsg := StrReplace(tmpMsg, "_shouldBeValue_", shouldBeValue)
					errorMsg := errorMsg . tmpMsg . newLine
				}
			}
		}
		catch e
		{
			errorCode := 666
			errorMsg := "A fatal error occured during execution in function 'EdmCmd_PreUnlock001'."	
			return			
		}
		if(errorMsg)
		{
			errorCode := 5
			errorMsg := errorMsg . newLine . "The transition will be canceled."
			return
		}
	}
}
;~ </Script as an example of how to read and compare variables and cancel a Check-In if necessary.>

;~ </User functions>
