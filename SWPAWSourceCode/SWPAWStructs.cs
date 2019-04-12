namespace SWPAW
{
	class Structs
    {
        public struct EdmSystemInformation
        {
            public string vaultCommandID;// Gets the command ID count.
            public string vaultIsLoggedIn;// Gets whether you are logged in to this vault.
            public string vaultLanguage;// Gets the language used by the SolidWorks Enterprise PDM client.
            public string vaultName;// Gets the name of this vault.
            public string vaultRootFolder;// Gets the root folder of this vault.
            public string vaultRootFolderID;// Gets the database ID of the root folder of this vault.
            public string vaultRootFolderPath;// Gets the file system path to the root folder of this vault.
            public string vaultSilentMode;// Gets whether the add-in is running in silent mode.  
            public string vaultClientType;// Gets the type of client installation.
            public string vaultItemRootFolder;// Gets the interface to the invisible root folder of all items and item folders.
            public string vaultItemRootFolderID;// Gets the ID of the invisible root folder of all items and item folders.
            public string userFullName; //Gets this user's full name.  
            public string userInitials;//Gets this user's initials.
            public string userUserData;//Gets and sets arbitrary text data associated with this user.
            public string userEmail;// Gets this user's e-mail address
            public string userID;//Gets this user's ID.
            public string userGroupIds;// Gets the ids of the user's groups
            public string userGroupNames;// Gets the names of the user's groups
            public string fileCurrentRevision; // Gets the file's current revision.  
            public string fileCurrentStateName; // Name of the current state
            public string fileCurrentStateID; // ID of the current state
            public string fileCurrentVersion;// Gets the file's current version number.  
            public string fileID; // Gets the database ID of this file.
            public string fileIsLocked; // Gets whether the file is checked out.  
            public string fileLockedByUser;// Gets the user who has the file checked out.  
            public string fileLockedByUserID;// Gets the ID of the user who has the file checked out.  
            public string fileLockedInFolder;// Gets the folder in which this file is checked out.  
            public string fileLockedInFolderID;// Gets the ID of the folder in which this file is checked out.  
            public string fileLockedOnComputer;// Gets the name of the computer to which the file is checked out.  
            public string fileLockPath;// Gets the full path to the checked-out file.
            public string fileName;// Gets the name of the file.
            public string fileObjectType;// Gets the type of object.  
            public string fileCategoryName;// Gets the ID of the category to which this file belongs.  
            public string fileCategoryID;// Gets the ID of the category to which this file belongs.  
            public string fileFileType;// Gets the type of this file.
            public string cardButtonNameOfAddInToCall; //ID of file data card
            public string menuParentFolderID; //ID of file or folder
            public string menuMenuID; //ID of file or folder
            public string cardButtonID; //ID of file data card
            public string cardButtonActiveConfiguration; //Name of active configuration; can be changed to switch to a new configuration
            public string cardButtonEdmCardFlag; //Optionally return a EdmCardFlag return code here
            public string cardButtonControlId; //Optionally return the ID of a card control to set focus to here
            public string cardButtonAllFileConfigurations; //List with the names of all configurations
            public string cardInputModifiedControlID; //ID of the modified card control
            public string cardInputCardID; //ID of the card
            public string cardInputActiveConfiguration; //Name of the active configuration
            public string cardInputUpdatedVariable; //ID of the updated variable
            public string cardInputAllFileConfigurations; //List with the names of all configurations
            public string cardInputVariableName; //Name of the variable
            public string cardListSrcControlID; //ID of the modified card control
            public string cardListSrcCardID; //ID of the card
            public string cardListSrcActiveConfiguration; //Name of the active configuration
            public string cardListSrcControlVariableName; //Name of the Control variable
            public string cardListSrcControlVariableID; //ID of the Control variable
            public string cardListSrcAllFileConfigurations; //List with the names of all configurations
            public string preAddParentFolderID; //ID of parent folder
            public string preAddLocalFileName; //Local file path
            public string preAddSourceFileName; //Source file path, if copied or moved
            public string preAddNetworkSharingLinks; //0 for normal files; 1 for network sharing links
            public string postAddParentFolderID; //ID of parent folder
            public string postAddNetworkSharingLinks; //0 for normal files; 1 for network sharing links
            public string preLockParentFolderID; //ID of folder where put checked-out file
            public string postLockParentFolderID; //ID of folder where put checked-out file
            public string preAddFolderParentFolderID; //ID of parent folder
            public string preAddFolderNewFolderName; //Path to new folder
            public string postAddFolderFolderID; //ID of new folder
            public string postAddFolderParentFolderID; //ID of parent folder
            public string postAddFolderNewFolderName; //Path to new folder
            public string preCopyFolderDestinationID; //ID of destination folder
            public string preCopyFieID; //ID of File
            public string preCopySourceFolderID; //ID of source folder
            public string preCopySourceFileName; //Source file path
            public string preCopyDestinationFileName; //Destination file path
            public string postCopySourceFolderID; //ID of source folder
            public string postCopySourceFileName; //Source file path
            public string preCopyFolderSourceFolderID; //ID of source folder
            public string preCopyFolderDestinationParentFolderID; //ID of destination folder
            public string preCopyFolderNewFolderName; //Path to new folder
            public string postCopyFolderFolderID; //ID of new folder
            public string postCopyFolderSourceFolderID; //ID of source folder
            public string postCopyFolderParentFolderID; //ID of the parent folder
            public string postCopyFolderFolderName; //ID of the parent folder
            public string preDeleteParentFolder; //ID of folder to delete file in
            public string postDeleteDeletedFileID; //ID of file that was deleted
            public string postDeleteParetnFolderID; //ID of parent folder of the deleted file
            public string postDeleteDeletedFileName; // Path to file that was deleted
            public string postDeleteNumberOfFoldersInWhichItWasShared; //Number of folders to which the file is shared
            public string preDeleteFolderFolderID; //ID of folder to Delete
            public string preDeleteFolderFolderName; //Path to folder to delete
            public string postDeleteFolderFolderID; //ID of folder to Delete
            public string postDeleteFolderFolderName; //Path to folder to delete
            public string preGetFolderID; //ID of folder to get file to; 0 to retrieve a file to a temporary folder
            public string postGetFolderID; //ID of folder to get file to; 0 to retrieve a file to a temporary folder
            public string preMoveSourceFolderID; //ID of source folderID of source folder
            public string preMoveDestinationFolderID; //ID of destination folder
            public string preMoveDestinationFileName; //Destination file path
            public string postMoveSourceFolderID; //ID of source folderID of source folder
            public string postMoveDestinationFolderID; //ID of destination folder
            public string postMoveSourceFileName; //Source file path
            public string preMoveFolderFolderID; //ID of folder to move
            public string preMoveFolderSourceParentFolderID; //ID of source parent folder
            public string preMoveFolderDestinationParentFolderID; //ID of destination parent folder
            public string preMoveFolderSourceFolderName; //ID of folder to move
            public string preMoveFolderDestinationFolderName; //Destination folder path
            public string postMoveFolderFolderID; //ID of folder to move
            public string postMoveFolderSourceParentFolderID; //ID of source parent folder
            public string postMoveFolderDestinationParentFolderID; //ID of destination parent folder
            public string postMoveFolderSourceFolderName; //ID of folder to move
            public string postMoveFolderDestinationFolderName; //Destination folder path
            public string preRenameFileToRenameID; //ID of the file to rename
            public string preRenameFilesParentFolderID; //ID of the file's parent folder
            public string preRenameNewFileName; //New file name
            public string preRenameOldFileName; //Old file name
            public string postRenameFileToRenameID; //ID of the file to rename
            public string postRenameFilesParentFolderID; //ID of the file's parent folder
            public string postRenameNewFileName; //New file name
            public string postRenameOldFileName; //New file name
            public string preRenameFolderFolderID; //ID of the folder to rename
            public string preRenameFolderParentFolderID; //ID of the folder's parent folder
            public string preRenameFolderOldFolderName; //Old folder name
            public string preRenameFolderNewFolderName; //New folder name
            public string postRenameFolderFolderID; //ID of the folder to rename
            public string postRenameFolderParentFolderID; //ID of the folder's parent folder
            public string postRenameFolderOldFolderName; //Old folder name
            public string postRenameFolderNewFolderName; //New folder name
            public string preShareParentFolderID; //ID of folder to share file from
            public string preShareDestinationFolderID; //ID of folder to share file to
            public string postShareParentFolderID; //ID of folder to share file from
            public string postShareDestinationFolderID; //ID of folder to share file to
            public string preStateParentFolderID; //ID of the file's parent folder
            public string preStateTransitionID; // ID of the transition (state change) to perform
            public string preStateDestinationStateName; // Name of the destination state
            public string preStateSourceStateID; // ID of the source state
            public string preStateDestinationStateID; // ID of the Destination state
            public string preStateCommentText; // Text of the comment
            public string postStateParentFolderID; //ID of the file's parent folder
            public string postStateTransitionID; // ID of the transition (state change) to perform
            public string postStateDestinationStateName; // Name of the destination state
            public string postStateSourceStateID; // ID of the source state
            public string postStateDestinationStateID; // ID of the Destination state
            public string postStateCommentText; // Text of the comment
            public string preUndoLockParentFolderID; //ID of the file's parent folder
            public string postUndoLockParentFolderID; //ID of the file's parent folder
            public string preUnlockParentFolderID; //ID of the file's parent folder
            public string preUnlockCommentText; // Text of the comment
            public string postUnlockParentFolderID; //ID of the file's parent folder
            public string postUnlockCommentText; // Text of the comment
            public string preLabelFileID; //ID of file to set label on; 0 for folders
            public string preLabelFolderID; //ID of file to set label on; 0 for folders
            public string preLabelLabelID; //0 for EdmCmd_PreLabel; ID of the created label for EdmCmd_PostLabel
            public string preLabelLabelName; //Label
            public string preLabelLabelComment; //Label
            public string preLabelFileName; //Path to file or folder to create label for; note that this member will only contain the file name without path when file labels are created via the API, since that is not done within the context of a folder
            public string preLabelCreatedRecursively; //Non 0 if label is created recursively for this folder, 0 otherwise
            public string postLabelParentFolderID; //ID of the file's parent folder
            public string postLabelFileID; //ID of file to set label on; 0 for folders
            public string postLabelFolderID; //ID of file to set label on; 0 for folders
            public string postLabelLabelID; //0 for EdmCmd_postLabel; ID of the created label for EdmCmd_PostLabel
            public string postLabelLabelName; //Label
            public string postLabelComment; //Comment
            public string postLabelFileName; //Path to file or folder to create label for; note that this member will only contain the file name without path when file labels are created via the API, since that is not done within the context of a folder
            public string postLabelCreatedRecursively; //Non 0 if label is created recursively for this folder, 0 otherwise
            public string preLabelDeleteLabelID; //ID of the label
            public string preLabelDeleteLabel; //label
            public string postLabelDeleteLabelID; //ID of the label
            public string postLabelDeleteLabel; //label
            public string preLabelModifyLabelID; //ID of the label
            public string preLabelModifiyLabel; //label
            public string preLabelModifiyComment; //Comment
            public string postLabelModofiyLabelID; //ID of the label
            public string postLabelModifyLabel; //label
            public string postLabelModifiyComment; //Comment
        }

        public struct EdmCmdHooksInfo
        {
            public string script;
            public int waitUntilExit;
            public int timeout;
            public int hideScriptWindow;
            public string executionCondition;
        }

        public struct EdmCmdHooks
        {
            public bool EdmCmd_CardButton;
            public bool EdmCmd_CardInput;
            public bool EdmCmd_CardListSrc;
            public bool EdmCmd_InstallAddIn;
            public bool EdmCmd_Menu;
            public bool EdmCmd_PostAdd;
            public bool EdmCmd_PostAddFolder;
            public bool EdmCmd_PostCopy;
            public bool EdmCmd_PostCopyFolder;
            public bool EdmCmd_PostDelete;
            public bool EdmCmd_PostDeleteFolder;
            public bool EdmCmd_PostGet;
            public bool EdmCmd_PostLabel;
            public bool EdmCmd_PostLabelAddItem;
            public bool EdmCmd_PostLabelDelete;
            public bool EdmCmd_PostLabelModify;
            public bool EdmCmd_PostLock;
            public bool EdmCmd_PostMove;
            public bool EdmCmd_PostMoveFolder;
            public bool EdmCmd_PostRename;
            public bool EdmCmd_PostRenameFolder;
            public bool EdmCmd_PostShare;
            public bool EdmCmd_PostState;
            public bool EdmCmd_PostUndoLock;
            public bool EdmCmd_PostUnlock;
            public bool EdmCmd_PreAdd;
            public bool EdmCmd_PreAddFolder;
            public bool EdmCmd_PreCopy;
            public bool EdmCmd_PreCopyFolder;
            public bool EdmCmd_PreDelete;
            public bool EdmCmd_PreDeleteFolder;
            public bool EdmCmd_PreGet;
            public bool EdmCmd_PreLabel;
            public bool EdmCmd_PreLabelAddItem;
            public bool EdmCmd_PreLabelDelete;
            public bool EdmCmd_PreLabelModify;
            public bool EdmCmd_PreLock;
            public bool EdmCmd_PreMove;
            public bool EdmCmd_PreMoveFolder;
            public bool EdmCmd_PreRename;
            public bool EdmCmd_PreRenameFolder;
            public bool EdmCmd_PreShare;
            public bool EdmCmd_PreState;
            public bool EdmCmd_PreUndoLock;
            public bool EdmCmd_PreUnlock;
            public bool EdmCmd_SerialNo;
            public bool EdmCmd_TaskDetails;
            public bool EdmCmd_TaskLaunch;
            public bool EdmCmd_TaskLaunchButton;
            public bool EdmCmd_TaskRun;
            public bool EdmCmd_TaskSetup;
            public bool EdmCmd_TaskSetupButton;
            public bool EdmCmd_UninstallAddIn;
            public string EdmCmd_CardButtonScript;
            public string EdmCmd_CardInputScript;
            public string EdmCmd_CardListSrcScript;
            public string EdmCmd_InstallAddInScript;
            public string EdmCmd_MenuScript;
            public string EdmCmd_PostAddScript;
            public string EdmCmd_PostAddFolderScript;
            public string EdmCmd_PostCopyScript;
            public string EdmCmd_PostCopyFolderScript;
            public string EdmCmd_PostDeleteScript;
            public string EdmCmd_PostDeleteFolderScript;
            public string EdmCmd_PostGetScript;
            public string EdmCmd_PostLabelScript;
            public string EdmCmd_PostLabelAddItemScript;
            public string EdmCmd_PostLabelDeleteScript;
            public string EdmCmd_PostLabelModifyScript;
            public string EdmCmd_PostLockScript;
            public string EdmCmd_PostMoveScript;
            public string EdmCmd_PostMoveFolderScript;
            public string EdmCmd_PostRenameScript;
            public string EdmCmd_PostRenameFolderScript;
            public string EdmCmd_PostShareScript;
            public string EdmCmd_PostStateScript;
            public string EdmCmd_PostUndoLockScript;
            public string EdmCmd_PostUnlockScript;
            public string EdmCmd_PreAddScript;
            public string EdmCmd_PreAddFolderScript;
            public string EdmCmd_PreCopyScript;
            public string EdmCmd_PreCopyFolderScript;
            public string EdmCmd_PreDeleteScript;
            public string EdmCmd_PreDeleteFolderScript;
            public string EdmCmd_PreGetScript;
            public string EdmCmd_PreLabelScript;
            public string EdmCmd_PreLabelAddItemScript;
            public string EdmCmd_PreLabelDeleteScript;
            public string EdmCmd_PreLabelModifyScript;
            public string EdmCmd_PreLockScript;
            public string EdmCmd_PreMoveScript;
            public string EdmCmd_PreMoveFolderScript;
            public string EdmCmd_PreRenameScript;
            public string EdmCmd_PreRenameFolderScript;
            public string EdmCmd_PreShareScript;
            public string EdmCmd_PreStateScript;
            public string EdmCmd_PreUndoLockScript;
            public string EdmCmd_PreUnlockScript;
            public string EdmCmd_SerialNoScript;
            public string EdmCmd_TaskDetailsScript;
            public string EdmCmd_TaskLaunchScript;
            public string EdmCmd_TaskLaunchButtonScript;
            public string EdmCmd_TaskRunScript;
            public string EdmCmd_TaskSetupScript;
            public string EdmCmd_TaskSetupButtonScript;
            public string EdmCmd_UninstallAddInScript;
            public int EdmCmd_CardButtonWaitUntilExit;
            public int EdmCmd_CardInputWaitUntilExit;
            public int EdmCmd_CardListSrcWaitUntilExit;
            public int EdmCmd_InstallAddInWaitUntilExit;
            public int EdmCmd_MenuWaitUntilExit;
            public int EdmCmd_PostAddWaitUntilExit;
            public int EdmCmd_PostAddFolderWaitUntilExit;
            public int EdmCmd_PostCopyWaitUntilExit;
            public int EdmCmd_PostCopyFolderWaitUntilExit;
            public int EdmCmd_PostDeleteWaitUntilExit;
            public int EdmCmd_PostDeleteFolderWaitUntilExit;
            public int EdmCmd_PostGetWaitUntilExit;
            public int EdmCmd_PostLabelWaitUntilExit;
            public int EdmCmd_PostLabelAddItemWaitUntilExit;
            public int EdmCmd_PostLabelDeleteWaitUntilExit;
            public int EdmCmd_PostLabelModifyWaitUntilExit;
            public int EdmCmd_PostLockWaitUntilExit;
            public int EdmCmd_PostMoveWaitUntilExit;
            public int EdmCmd_PostMoveFolderWaitUntilExit;
            public int EdmCmd_PostRenameWaitUntilExit;
            public int EdmCmd_PostRenameFolderWaitUntilExit;
            public int EdmCmd_PostShareWaitUntilExit;
            public int EdmCmd_PostStateWaitUntilExit;
            public int EdmCmd_PostUndoLockWaitUntilExit;
            public int EdmCmd_PostUnlockWaitUntilExit;
            public int EdmCmd_PreAddWaitUntilExit;
            public int EdmCmd_PreAddFolderWaitUntilExit;
            public int EdmCmd_PreCopyWaitUntilExit;
            public int EdmCmd_PreCopyFolderWaitUntilExit;
            public int EdmCmd_PreDeleteWaitUntilExit;
            public int EdmCmd_PreDeleteFolderWaitUntilExit;
            public int EdmCmd_PreGetWaitUntilExit;
            public int EdmCmd_PreLabelWaitUntilExit;
            public int EdmCmd_PreLabelAddItemWaitUntilExit;
            public int EdmCmd_PreLabelDeleteWaitUntilExit;
            public int EdmCmd_PreLabelModifyWaitUntilExit;
            public int EdmCmd_PreLockWaitUntilExit;
            public int EdmCmd_PreMoveWaitUntilExit;
            public int EdmCmd_PreMoveFolderWaitUntilExit;
            public int EdmCmd_PreRenameWaitUntilExit;
            public int EdmCmd_PreRenameFolderWaitUntilExit;
            public int EdmCmd_PreShareWaitUntilExit;
            public int EdmCmd_PreStateWaitUntilExit;
            public int EdmCmd_PreUndoLockWaitUntilExit;
            public int EdmCmd_PreUnlockWaitUntilExit;
            public int EdmCmd_SerialNoWaitUntilExit;
            public int EdmCmd_TaskDetailsWaitUntilExit;
            public int EdmCmd_TaskLaunchWaitUntilExit;
            public int EdmCmd_TaskLaunchButtonWaitUntilExit;
            public int EdmCmd_TaskRunWaitUntilExit;
            public int EdmCmd_TaskSetupWaitUntilExit;
            public int EdmCmd_TaskSetupButtonWaitUntilExit;
            public int EdmCmd_UninstallAddInWaitUntilExit;
            public int EdmCmd_CardButtonTimeout;
            public int EdmCmd_CardInputTimeout;
            public int EdmCmd_CardListSrcTimeout;
            public int EdmCmd_InstallAddInTimeout;
            public int EdmCmd_MenuTimeout;
            public int EdmCmd_PostAddTimeout;
            public int EdmCmd_PostAddFolderTimeout;
            public int EdmCmd_PostCopyTimeout;
            public int EdmCmd_PostCopyFolderTimeout;
            public int EdmCmd_PostDeleteTimeout;
            public int EdmCmd_PostDeleteFolderTimeout;
            public int EdmCmd_PostGetTimeout;
            public int EdmCmd_PostLabelTimeout;
            public int EdmCmd_PostLabelAddItemTimeout;
            public int EdmCmd_PostLabelDeleteTimeout;
            public int EdmCmd_PostLabelModifyTimeout;
            public int EdmCmd_PostLockTimeout;
            public int EdmCmd_PostMoveTimeout;
            public int EdmCmd_PostMoveFolderTimeout;
            public int EdmCmd_PostRenameTimeout;
            public int EdmCmd_PostRenameFolderTimeout;
            public int EdmCmd_PostShareTimeout;
            public int EdmCmd_PostStateTimeout;
            public int EdmCmd_PostUndoLockTimeout;
            public int EdmCmd_PostUnlockTimeout;
            public int EdmCmd_PreAddTimeout;
            public int EdmCmd_PreAddFolderTimeout;
            public int EdmCmd_PreCopyTimeout;
            public int EdmCmd_PreCopyFolderTimeout;
            public int EdmCmd_PreDeleteTimeout;
            public int EdmCmd_PreDeleteFolderTimeout;
            public int EdmCmd_PreGetTimeout;
            public int EdmCmd_PreLabelTimeout;
            public int EdmCmd_PreLabelAddItemTimeout;
            public int EdmCmd_PreLabelDeleteTimeout;
            public int EdmCmd_PreLabelModifyTimeout;
            public int EdmCmd_PreLockTimeout;
            public int EdmCmd_PreMoveTimeout;
            public int EdmCmd_PreMoveFolderTimeout;
            public int EdmCmd_PreRenameTimeout;
            public int EdmCmd_PreRenameFolderTimeout;
            public int EdmCmd_PreShareTimeout;
            public int EdmCmd_PreStateTimeout;
            public int EdmCmd_PreUndoLockTimeout;
            public int EdmCmd_PreUnlockTimeout;
            public int EdmCmd_SerialNoTimeout;
            public int EdmCmd_TaskDetailsTimeout;
            public int EdmCmd_TaskLaunchTimeout;
            public int EdmCmd_TaskLaunchButtonTimeout;
            public int EdmCmd_TaskRunTimeout;
            public int EdmCmd_TaskSetupTimeout;
            public int EdmCmd_TaskSetupButtonTimeout;
            public int EdmCmd_UninstallAddInTimeout;
            public int EdmCmd_CardButtonHideScriptWindow;
            public int EdmCmd_CardInputHideScriptWindow;
            public int EdmCmd_CardListSrcHideScriptWindow;
            public int EdmCmd_InstallAddInHideScriptWindow;
            public int EdmCmd_MenuHideScriptWindow;
            public int EdmCmd_PostAddHideScriptWindow;
            public int EdmCmd_PostAddFolderHideScriptWindow;
            public int EdmCmd_PostCopyHideScriptWindow;
            public int EdmCmd_PostCopyFolderHideScriptWindow;
            public int EdmCmd_PostDeleteHideScriptWindow;
            public int EdmCmd_PostDeleteFolderHideScriptWindow;
            public int EdmCmd_PostGetHideScriptWindow;
            public int EdmCmd_PostLabelHideScriptWindow;
            public int EdmCmd_PostLabelAddItemHideScriptWindow;
            public int EdmCmd_PostLabelDeleteHideScriptWindow;
            public int EdmCmd_PostLabelModifyHideScriptWindow;
            public int EdmCmd_PostLockHideScriptWindow;
            public int EdmCmd_PostMoveHideScriptWindow;
            public int EdmCmd_PostMoveFolderHideScriptWindow;
            public int EdmCmd_PostRenameHideScriptWindow;
            public int EdmCmd_PostRenameFolderHideScriptWindow;
            public int EdmCmd_PostShareHideScriptWindow;
            public int EdmCmd_PostStateHideScriptWindow;
            public int EdmCmd_PostUndoLockHideScriptWindow;
            public int EdmCmd_PostUnlockHideScriptWindow;
            public int EdmCmd_PreAddHideScriptWindow;
            public int EdmCmd_PreAddFolderHideScriptWindow;
            public int EdmCmd_PreCopyHideScriptWindow;
            public int EdmCmd_PreCopyFolderHideScriptWindow;
            public int EdmCmd_PreDeleteHideScriptWindow;
            public int EdmCmd_PreDeleteFolderHideScriptWindow;
            public int EdmCmd_PreGetHideScriptWindow;
            public int EdmCmd_PreLabelHideScriptWindow;
            public int EdmCmd_PreLabelAddItemHideScriptWindow;
            public int EdmCmd_PreLabelDeleteHideScriptWindow;
            public int EdmCmd_PreLabelModifyHideScriptWindow;
            public int EdmCmd_PreLockHideScriptWindow;
            public int EdmCmd_PreMoveHideScriptWindow;
            public int EdmCmd_PreMoveFolderHideScriptWindow;
            public int EdmCmd_PreRenameHideScriptWindow;
            public int EdmCmd_PreRenameFolderHideScriptWindow;
            public int EdmCmd_PreShareHideScriptWindow;
            public int EdmCmd_PreStateHideScriptWindow;
            public int EdmCmd_PreUndoLockHideScriptWindow;
            public int EdmCmd_PreUnlockHideScriptWindow;
            public int EdmCmd_SerialNoHideScriptWindow;
            public int EdmCmd_TaskDetailsHideScriptWindow;
            public int EdmCmd_TaskLaunchHideScriptWindow;
            public int EdmCmd_TaskLaunchButtonHideScriptWindow;
            public int EdmCmd_TaskRunHideScriptWindow;
            public int EdmCmd_TaskSetupHideScriptWindow;
            public int EdmCmd_TaskSetupButtonHideScriptWindow;
            public int EdmCmd_UninstallAddInHideScriptWindow;
            public string EdmCmd_CardButtonExecutionCondition;
            public string EdmCmd_CardInputExecutionCondition;
            public string EdmCmd_CardListSrcExecutionCondition;
            public string EdmCmd_InstallAddInExecutionCondition;
            public string EdmCmd_MenuExecutionCondition;
            public string EdmCmd_PostAddExecutionCondition;
            public string EdmCmd_PostAddFolderExecutionCondition;
            public string EdmCmd_PostCopyExecutionCondition;
            public string EdmCmd_PostCopyFolderExecutionCondition;
            public string EdmCmd_PostDeleteExecutionCondition;
            public string EdmCmd_PostDeleteFolderExecutionCondition;
            public string EdmCmd_PostGetExecutionCondition;
            public string EdmCmd_PostLabelExecutionCondition;
            public string EdmCmd_PostLabelAddItemExecutionCondition;
            public string EdmCmd_PostLabelDeleteExecutionCondition;
            public string EdmCmd_PostLabelModifyExecutionCondition;
            public string EdmCmd_PostLockExecutionCondition;
            public string EdmCmd_PostMoveExecutionCondition;
            public string EdmCmd_PostMoveFolderExecutionCondition;
            public string EdmCmd_PostRenameExecutionCondition;
            public string EdmCmd_PostRenameFolderExecutionCondition;
            public string EdmCmd_PostShareExecutionCondition;
            public string EdmCmd_PostStateExecutionCondition;
            public string EdmCmd_PostUndoLockExecutionCondition;
            public string EdmCmd_PostUnlockExecutionCondition;
            public string EdmCmd_PreAddExecutionCondition;
            public string EdmCmd_PreAddFolderExecutionCondition;
            public string EdmCmd_PreCopyExecutionCondition;
            public string EdmCmd_PreCopyFolderExecutionCondition;
            public string EdmCmd_PreDeleteExecutionCondition;
            public string EdmCmd_PreDeleteFolderExecutionCondition;
            public string EdmCmd_PreGetExecutionCondition;
            public string EdmCmd_PreLabelExecutionCondition;
            public string EdmCmd_PreLabelAddItemExecutionCondition;
            public string EdmCmd_PreLabelDeleteExecutionCondition;
            public string EdmCmd_PreLabelModifyExecutionCondition;
            public string EdmCmd_PreLockExecutionCondition;
            public string EdmCmd_PreMoveExecutionCondition;
            public string EdmCmd_PreMoveFolderExecutionCondition;
            public string EdmCmd_PreRenameExecutionCondition;
            public string EdmCmd_PreRenameFolderExecutionCondition;
            public string EdmCmd_PreShareExecutionCondition;
            public string EdmCmd_PreStateExecutionCondition;
            public string EdmCmd_PreUndoLockExecutionCondition;
            public string EdmCmd_PreUnlockExecutionCondition;
            public string EdmCmd_SerialNoExecutionCondition;
            public string EdmCmd_TaskDetailsExecutionCondition;
            public string EdmCmd_TaskLaunchExecutionCondition;
            public string EdmCmd_TaskLaunchButtonExecutionCondition;
            public string EdmCmd_TaskRunExecutionCondition;
            public string EdmCmd_TaskSetupExecutionCondition;
            public string EdmCmd_TaskSetupButtonExecutionCondition;
            public string EdmCmd_UninstallAddInExecutionCondition;
        }
    }
}
