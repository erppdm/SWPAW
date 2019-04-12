;~ ##########################################################################
;~ Copyright (c) 2014 Ulf-Dirk Stockburger
;~ ##########################################################################

;~ <Checks if the script was started in debug mode>
CheckIfDebugModeIsRquierd()
{
	Global debugMode
	Global startTickCount
	Global tickCount
	Global debugFile
	Global iniFile
	Global BiIDebugInfoExtension
	Global BiIdebugMode	
	;~ If debug file exist
	debugFile = %iniFile%%BiIDebugInfoExtension%
	firstSectionFound := 0
	Loop, Read, %iniFile%
	{
		Loop, Parse, A_LoopReadLine, %A_Tab%
		{
			if(RegExMatch(A_LoopField, BiIRegExPatternSection) = 1)
			{
				;~ Getting the section without brackets
				firstSection := SubStr(A_LoopField, 2, StrLen(A_LoopField)-2)
				firstSectionFound = 1
			}
		}
		if(firstSectionFound = 1)
		{
			break
		}
	}
	;~ Reads the main window handle
	IniRead, debugMode, %iniFile%, %firstSection%, %BiIdebugMode%,
	If(debugMode == "" | debugMode = "ERROR")
	{
		errorCode := 1
		errorMsg := "The value for the debugMode cannot be read from the ini file."
		return
	}
	;~ Writing the script start time and the difference in the debug file
	if(debugMode != 0)
	{		
		Loop, read, %debugFile%
		{
			startTickCount := A_LoopReadLine
		}
		if(debugMode != 0)
		{
			startTickCount := RegExReplace(startTickCount, "\D")
			tickCount := A_TickCount
			FileAppend, `nAHK - TickCount: %tickCount%, %debugFile%
			If(ErrorLevel != 0)
			{
				;~ Resets the debugMode in case of an error
				AppendErrorMsg()
			}
		}	
		if(debugMode != 0)
		{
			durationDifferenceTicks := A_TickCount - startTickCount
			FileAppend, `nAHK - Time needed to run the shell: %durationDifferenceTicks% milliseconds, %debugFile%
			If(ErrorLevel != 0)
			{
				;~ Resets the debugMode in case of an error
				AppendErrorMsg()		
			}
		}
		if(debugMode != 0)
		{
			FormatTime, startTickCount, %A_Now%, dd.MM.yyyy HH:mm:ss
			FileAppend, `nAHK - Time when the script is started: %startTickCount%, %debugFile%
			If(ErrorLevel != 0)
			{
				;~ Resets the debugMode in case of an error
				AppendErrorMsg()
			}		
		}
	}
}
;~ </Checks if the script was started in debug mode>

;~ <Writes debug messages to the debug file if required>
CheckDebugMode(setTime, msg)
{
	Global debugMode
	Global tickCount
	Global debugFile
	if(debugMode != 0)
	{	
		If(setTime = 0)
		{
			tickCount := A_TickCount
		}
		else
		{
			tickCountDifference := A_TickCount - tickCount
			FileAppend, `n%msg%%tickCountDifference% milliseconds, %debugFile%
			If(ErrorLevel != 0)
			{
				AppendErrorMsg()
			}		
		}
	}
}
;~ </Writes debug messages to the debug file if required>

;~ <Shows a message box if debug file cannot be written>
AppendErrorMsg()
{
	Global debugFile
	Global debugMode
	Msgbox, 16, BiI Error, The debug information cannot be written to the file: '%debugFile%'`nThe following steps will not be logged.
	debugMode := 0
}
;~ </Shows a message box if debug file cannot be written>
