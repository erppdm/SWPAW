;~ http://help.solidworks.com/2014/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.EdmAddFlag.html

EdmAdd_DeleteSource := 64 ;~Delete the source file once it has been added to the vault
EdmAdd_DontAddCorrupt := 2 ;~Refuse to add corrupt files; IEdmFolder5::AddFile returns the error code E_EDM_INVALID_FILE
EdmAdd_ForceGenerateSerialNumbers := 128 ;~Force regeneration of serial numbers when values already exist; if this flag is not set, SolidWorks Enterprise PDM will only generate values that are missing in the file
EdmAdd_GetInterface := 256 ;~Return the file interface; only works with IEdmBatchAdd
EdmAdd_KeepExistingSerialNumbers := 4 ;~Do not replace existing serial numbers with new ones; SolidWorks Enterprise PDM still creates serial numbers for empty fields; if this flag is not set, SolidWorks Enterprise PDM writes over existing serial numbers with new ones
EdmAdd_Refresh := 1 ;~Make all file listings in Windows Explorer and Open/Save As dialog boxes refresh so that the new file is displayed
EdmAdd_Simple := 0 ;~Add the file
EdmAdd_UniqueVarClearDuplicate := 16 ;~Clear duplicated unique constrained variables instead of failing
EdmAdd_UniqueVarDelayCheck := 32 ;~Delay the unique variable check until the next check-in