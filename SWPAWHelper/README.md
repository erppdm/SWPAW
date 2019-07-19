# SWPAWHelper
**S**olid**W**orks **P**DM **A**dd-In **W**rapper**Helper**

An example of a COM-Server to extend SolidWorks PDM functionality which can be used for late binding.

## How to build and register the COM-Server
**Visual Studio**
- **Assembly Information**: Set 'Make Assembly COM-Visible'
- **Build**: Set 'Register For COM interop'
- Sign the assembly.
- Compile the add-in.

**Windows**
- Register the DLL in codebase ([Using regasm.exe to register COM types (Outdated)](https://github.com/rubberduck-vba/Rubberduck/wiki/Using-regasm.exe-to-register-COM-types-(Outdated))).

## Why to programme an own COM-Server?
As soon as the given functionality is no longer sufficient, or to optimise the process.

## Why to programme a COM-Server for SolidWorks PDM?
Unfortunately, several interfaces cannot be used via COM. As far as I know, these utilities are not available up to version 2018:

- EdmUtil_RawReferenceMgr
- EdmUtil_RevisionMgr
- EdmUtil_SerNoGen
- EdmUtil_TemplateMgr
- EdmUtil_CategoryMgr
- EdmUtil_BomMgr
- EdmUtil_UserMgr
- EdmUtil_VariableMgr
- EdmUtil_WorkflowMgr

Fortunately, these functions or parts of them can be provided by a separate DLL with little effort.

## How to use the COM-Server in AutoHotkey/VBA?
This example demonstrates how to get the user information provided by the COM-Interface.

**AutoHotkey**
``` AutoHotkey
;~ Creates the COM-Server
SWPAWHelper := ComObjCreate("SWPAWHelper.Helper")
;~ Sets the debug mode to no
SWPAWHelper.debug := false
;~ Login into the vault
vault := SWPAWHelper.VaultLogin("BiIEpdmVault")
;~ Gets the information for the current logged in user
userInfo := SWPAWHelper.UserGetLoggedInUserInfo(vault)
;~ Shows the users information in a message box
wCounter := 0
while(wCounter <= userInfo.MaxIndex()) {
	if(wCounter < userInfo.MaxIndex()){
		output := userInfo[wCounter][0] . " : " . userInfo[wCounter][1]
	}
	else{
		output := userInfo[wCounter][0] . " : "
		for element, index In userInfo[wCounter][1]{
			output := output . " | " . element . " | "
		}
	}
	MsgBox,,SWPAWHelper.Helper: users groups, %output%
	wCounter++
}
```

**VBA**
``` VBA
Dim output As String
Dim element As Variant
'Creates the COM-Server
Dim SWPAWHelper As Object: Set SWPAWHelper = CreateObject("SWPAWHelper.Helper")
'Sets the debug mode to no
SWPAWHelper.debug = False
'Login into the vault
Dim vault As Object: Set vault = SWPAWHelper.VaultLogin("BiIEpdmVault")
'Gets the information for the current logged in user
Dim userInfo As Variant: userInfo = SWPAWHelper.UserGetLoggedInUserInfo(vault)
'Shows the users information in a message box
Dim wCounter As Integer: wCounter = 0
While wCounter <= UBound(userInfo)
    If wCounter < UBound(userInfo) Then
        output = userInfo(wCounter)(0) & " : " & userInfo(wCounter)(1)
    Else
        output = userInfo(wCounter)(0) & " : "
        For Each element In userInfo(wCounter)(1)
            output = output & " | " & element & " | "
        Next
    End If
    MsgBox output, vbInformation, "SWPAWHelper.Helper: users groups"
    wCounter = wCounter + 1
Wend
``` 
