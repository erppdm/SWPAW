;~ http://help.solidworks.com/2014/english/api/epdmapi/epdm.interop.epdm~epdm.interop.epdm.edmobjecttype.html

EdmObject_Attribute:=10 ;~The object is an attribute, used in variables, and it supports the IEdmAttribute5 interface
EdmObject_BOM:=15 ;~The object is a Bill of Materials; see IEdmBom
EdmObject_Card:=5 ;~The object is a file/folder data card, which supports the IEdmCard5 interface
EdmObject_CardControl:=6 ;~The object is a control in a file/folder data card, and it supports the IEdmCardControl5 interface
EdmObject_Category:=14 ;~The object is a category; see IEdmCategory6
EdmObject_Dictionary:=12 ;~The object is a dictionary, and it supports the IEdmDictionary5 interface
EdmObject_File:=1 ;~The object is a file that supports the IEdmFile5 interface
EdmObject_Folder:=2 ;~ The object is a folder that supports the IEdmFolder5 interface
EdmObject_Invalid:=0  ;~This is not an object type; it is an error code
EdmObject_Item:=16 ;~ The object is an item; see IEdmItem
EdmObject_ItemFolder:=17 ;~ The object is a parent folder of an item; see IEdmFolder6
EdmObject_ItemRootFolder:=18 ;~ The object is the invisible root folder of all item folders; see IEdmFolder6
EdmObject_Label:=11 ;~The object is a label, and it supports the IEdmLabel5 interface
EdmObject_State:=3  ;~The object is a workflow state, and it supports the IEdmState5 interface
EdmObject_Transition:=4  ;~The object is a transition (i.e., a workflow state change) and, it supports the IEdmTransition5 interface
EdmObject_User:=7 ;~The object is a user, and it supports the IEdmUser5 interface
EdmObject_UserGroup:=8  ;~The object is a user group and it supports the IEdmUserGroup5 interface
EdmObject_Variable:=9 ;~The object is a variable (for file/folder data cards), and it supports the IEdmVariable5 interface
EdmObject_Workflow:=13  ;~The object is a workflow; see IEdmWorkflow5 and IEdmWorkflow6

