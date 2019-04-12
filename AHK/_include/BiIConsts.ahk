;~ ##########################################################################
;~ Copyright (c) 2014 Ulf-Dirk Stockburger
;~ ##########################################################################

;~ Basics set of conststants provided by the add-in.
BiIDebugInfoExtension := ".SWPAW.Debug.Info" ;~ // File extension for the aditional file in debug mode
BiIRegExPatternSection := "^\[.+\]$" ;~ RegEx pattern for sections in the INI file
BiIErrorMsg = `nerrorMsg= ;~ Last entry in the INI file if an error occurs.
Global configsDelimiter := "{@0}" ;Delimiter in configurations string