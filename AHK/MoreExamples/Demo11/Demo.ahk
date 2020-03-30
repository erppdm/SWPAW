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

GetFileVarVal(Byref file, varName, varConfig) {
    try    {
        objVarValue := ComVar()
        var := file.GetEnumeratorVariable("")
        var.GetVar(varName, varConfig, objVarValue.ref)
        varVal := objVarValue[]
    }
    catch e    {
        errorCode := 1
        errorMsg := "Error in 'GetFileVarVal'." 
        return
    }
    return varVal
}

SetFileVarVal(Byref file, varName, varValue, varConfig, flush) {
    try    {
        var := file.GetEnumeratorVariable("")
        var.SetVar(varName, varConfig, varValue)
        if(flush = 1){
            var.Flush()
        }
    }
    catch e    {
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
        UpdateConfigsWithVarValues()
    }    
    ;~ Writes the time difference and message to the debug file if debug mode is enabeld
    debugMsg := "AHK - Time needed to run the function 'UserDefinedChecks': "
    CheckDebugMode(1,debugMsg)    
}
;~ </This function will be called for each file or folder in the INI file>

;~ <This function will be called once for all sections in the INI file>
UpdateConfigsWithVarValues(){
    Global UpdateConfigsWithVarValuesFinished
    if(!UpdateConfigsWithVarValuesFinished){        
        UpdateConfigsWithVarValuesFinished := 1
        Global errorCode
        Global errorMsg
        Global iniFile
        Global BiIfileID
        Global EdmObject_File
        ConnectToVault(1)
        try{
            for sectionIndex, sectionItem in iniFileSections {
                ;~ One configuration (varConfigs := ["Default"]) means that all existing configurations
                ;~ will be updated. The values of the first configuration are copied to the others.
                ;~ <Replace these variables with yours>
                varConfigs := ["Default", "@"]
                varVars := ["<variable name>", "<variable name>"]
                ;~ No regExPattern := "" means to ignore RegEx
                regExPattern := ""
                varVarsVal := []
                ;~ </Replace these variables with yours>
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
                ;~ Gets all configurations from the current file                                
                if (varConfigs.MaxIndex() = 1)
                {
                    configs := file.GetConfigurations()
                    pos := configs.GetHeadPosition()
                    while (!pos.IsNull) {
                        config := configs.GetNext(pos)
                        if(config != varConfigs[1]) {
                            varConfigs.Push(config)
                        }
                    }                    
                }
                ;~ Gets all variable values from the source configuration
                for i, e in varVars {
                    varVarsVal.Push(GetFileVarVal(file, e, varConfigs[1]))
                }
                ;~ Sets all variables in all defined configurations
                flush := 0
                canFlush := 0
                for iC, eC in varConfigs{
                    if(iC = varConfigs.MaxIndex()) {
                        canFlush := 1
                    }
                    for iV, eV in varVars
                    {
                        if(varConfigs[1] = varConfigs[iC]) {
                            break
                        }
                        if(regExPattern != ""){
                            if(!RegExMatch(varConfigs[iC], regExPattern) = 1) {
                                break
                            }
                        }
                        if(iV = varVars.MaxIndex()) {
                            if(canFlush = 1){
                                flush := 1
                            }
                        }
                        try {
                            SetFileVarVal(file, eV, varVarsVal[iV], varConfigs[iC], flush)
                        }
                        catch e {
                            errorCode := 4
                            errorMsg := "The variable values cannot be written."
                            return                        
                        }
                    }
                }
            }
        }
        catch e {
            errorCode := 666
            errorMsg := "A fatal error occured during execution in function 'UpdateConfigsWithVarValues'."    
            return            
        }
    }
}
;~ </This function will be called once for the INI file>

;~ </User functions>
