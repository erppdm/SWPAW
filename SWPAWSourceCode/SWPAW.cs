//using EPDM.Interop.epdm;
//using EdmLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SWPAW
{
  [Guid("ef2c72bb-3677-4390-b6b6-765ba5da2a7f"), ComVisible(true)]
  public class EpdmAddIn : IEdmAddIn5
  {

    #region <Global variables>
    int hWnd = 0;
    string DelimiterSysProps = string.Empty;
    string DelimiterSysScriptName = string.Empty;
    string DelimiterIEdmStrLst = string.Empty;
    string DelimiterSysExecutionCondition1 = string.Empty;
    string DelimiterSysExecutionCondition2 = string.Empty;
    bool DebugMode = false;
    bool hookExecuted = false;
    // Menus
    readonly int menuStartNumber = 10000;
    int menuStartNum;
    int menuEndNum;
    #endregion </Global variables>

    #region <Only for debug mode>
    DateTime savedTime = new DateTime();
    DateTime currentTime = new DateTime();
    System.Text.StringBuilder debugInfo = new System.Text.StringBuilder();
    #endregion </Only for debug mode>

    #region <DLL Import>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UpdateWindow(IntPtr hWnd);
    #endregion </DLL Import>

    #region <Debug>
    private void PauseToAttachProcess(string callbackType)
    {
      try
      {
        if (!Debugger.IsAttached)
        {
          MessageBox.Show("Attach debugger to process \"" + Process.GetCurrentProcess().ProcessName + "\" for callback \"" + callbackType + "\" before clicking OK.");
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }
    #endregion </Debug>

    void IEdmAddIn5.GetAddInInfo(ref EdmAddInInfo poInfo, IEdmVault5 poVault, IEdmCmdMgr5 poCmdMgr)
    {
      // Have a look here for more information: https://forum.solidworks.com/thread/94443
      //PauseToAttachProcess("GetAddInInfo");
      //
      EdmCmd poCmd = new EdmCmd();
      poInfo.mbsAddInName = Consts.AddInVer;
      poInfo.mbsCompany = "Copyright (c) 2014 Ulf-Dirk Stockburger";
      poInfo.mbsDescription = Consts.AddInVer;
      poInfo.mlAddInVersion = 1;
      poInfo.mlRequiredVersionMajor = 14;
      poInfo.mlRequiredVersionMinor = 0;

      #region <Read ini file>
      Structs.EdmCmdHooks hooks = new Structs.EdmCmdHooks();
      
      ReadIniFile(ref poCmd, ref hooks, true);
      if (poCmd.mbCancel != 0)
      {
        string extMessageText = Environment.NewLine + @"Attention, the add-in is not being processed. Please contact the Administrator.";
        MessageBox.Show(poCmd.mbsComment + extMessageText, Consts.AddInVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      #endregion </Read ini file>

      #region <Create context menu>
      if (hooks.EdmCmd_Menu)
      {
        string iniKeyValue = string.Empty;
        string iniPath = GetIniPathFromEnvironment(ref poCmd);
        List<string[]> menusList = new List<string[]>();
        
        #region <Get content from ini-file>
        if (poCmd.mbCancel == 0)
        {
          BiIIniFile ini = new BiIIniFile(iniPath);
          iniKeyValue = ini.IniReadValue(nameof(EdmCmdType.EdmCmd_Menu), @"EdmCmd_MenuContextMenuEntries");
          if (iniKeyValue == string.Empty)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "Key " + Consts.sysInfoPropertiesDelimiter + @" in Section [" + Consts.sectionDelimiter + @"] not found or invalid.";
          }
        }
        #endregion </Get content from ini-file>
        //
        #region <Prepare data for creating the menu>
        if (poCmd.mbCancel == 0)
        {
          try
          {
            string[] menus = iniKeyValue.Split(new[] { DelimiterSysExecutionCondition2 }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string _menu in menus)
            {
              string[] menu = _menu.Split(new[] { DelimiterSysExecutionCondition1 }, StringSplitOptions.RemoveEmptyEntries);
              if (menu.Length != 4)
              {
                poCmd.mbCancel = 1;
                poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + @"The context menu definition is incorrect.";
                break;
              }
              try
              {
                int flags = Convert.ToInt32(menu[1]);
                menusList.Add(menu);
              }
              catch (Exception ex)
              {
                poCmd.mbCancel = 1;
                poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
                break;
              }
            }
          }
          catch (Exception ex)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
          }
        }
        #endregion </Prepare data for creating the menu>
        
        #region <Create menu>
        if (poCmd.mbCancel == 0)
        {
          try
          {
            menuStartNum = menuStartNumber;
            menuEndNum = menuStartNum - 1;
            for (int i = 0; i < menusList.Count; i++)
            {
              //http://help.solidworks.com/2016/english/api/epdmapi/csharpmenuitem.htm?id=f38cafd35e4e47ceb12707761b41633d#Pg0
              poCmdMgr.AddCmd(++menuEndNum, menusList[i][0], (int)EdmMenuFlags.EdmMenu_OnlyInContextMenu | (int)EdmMenuFlags.EdmMenu_MustHaveSelection | Convert.ToInt32(menusList[i][1]), menusList[i][2], menusList[i][3], i, 99);
            }
          }
          catch (Exception ex)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
          }
        }
        #endregion </Create menu>
      }
      #endregion </Create context menu>

      #region <Create task>
      #endregion </Create task>

      #region <Add the hooks specified in the ini file>
      AddHooks(ref poCmd, ref poCmdMgr, hooks);
      if (poCmd.mbCancel != 0)
      {
        MessageBox.Show(poCmd.mbsComment, Consts.AddInVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      #endregion </Add the hooks specified in the ini file>

      #region <Only for debug mode>
      if (DebugMode)
      {
        currentTime = DateTime.Now;
        TimeSpan duration = currentTime - savedTime;
        string msg = @"EPDM - Time needed to install the add-in: " + duration.Milliseconds.ToString() + @" milliseconds" + Environment.NewLine + @"Installed hooks: " + debugInfo.ToString().Trim();
        MessageBox.Show(msg, Consts.AddInVerDebugMode, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      #endregion </Only for debug mode>
    }

    // CLR4
    //public void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
    // CLR2
    //public void OnCmd(ref EdmCmd poCmd, ref Array ppoData)
    {
      IEdmVault5 vault5 = poCmd.mpoVault as IEdmVault5;
      try
      {
        #region <Reset global variables>
        savedTime = DateTime.Now;
        currentTime = new DateTime();
        debugInfo = new System.Text.StringBuilder();
        hookExecuted = false;
        #endregion </Reset global variables>

        #region <Only for debug mode>
        if (DebugMode)
        {
          currentTime = DateTime.Now;
          TimeSpan duration = currentTime - savedTime;
          debugInfo.Append(@"EPDM - Time needed to read the INI file: " + duration.Milliseconds.ToString() + @" milliseconds" + Environment.NewLine);
        }
        #endregion </Only for debug mode>

        #region <Get the parent hWnd>
        hWnd = poCmd.mlParentWnd;
        #endregion </Get the parent hWnd>

        #region <Call the hooks>
        if (poCmd.mbCancel == 0)
        {
          Structs.EdmCmdHooksInfo hInfo = new Structs.EdmCmdHooksInfo();
          hInfo.script = string.Empty;
          hInfo.waitUntilExit = 1;
          hInfo.timeout = 666;
          #region <Only required if the.NET 2 framework is used>
          EdmCmdData[] _ppoData = new EdmCmdData[ppoData.Length];
          int i = 0;
          foreach (var _data in ppoData)
          {
            EdmCmdData data = new EdmCmdData();
            data = (EdmCmdData)_data;
            _ppoData[i] = data;
            ++i;
          }
          #endregion </Only required if the.NET 2 framework is used>
          CallHooks(hWnd, ref vault5, ref poCmd, ref _ppoData, ref hInfo);
        }
        #endregion </Call the hooks>

        #region <Error message>
        if (poCmd.mbCancel != 0)
        {
          vault5.MsgBox(hWnd, poCmd.mbsComment, EdmMBoxType.EdmMbt_OKOnly, Consts.AddInVer);
        }
        #endregion </Error message>
      }
      catch (COMException ex)
      {
        string errorName, errorDesc;
        vault5.GetErrorString(ex.ErrorCode, out errorName, out errorDesc);
        vault5.MsgBox(0, errorDesc, EdmMBoxType.EdmMbt_OKOnly, errorName);
      }
    }

    #region <Ini file>
    private string GetIniPathFromEnvironment(ref EdmCmd poCmd)
    {
      string ret = string.Empty;
      try
      {
        ret = Environment.GetEnvironmentVariable(Consts.environmentIniPath);
        if (ret == null)
        {
          throw (new Exception());
        }
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Cannot read the path for the ini file from environment variable: " + Consts.environmentIniPath;
      }
      return ret;
    }

    private void ReadIniFile(ref EdmCmd poCmd, ref Structs.EdmCmdHooks hooks, bool onlyHooks = false)
    {
      string iniPath = string.Empty;
      
      #region <Get the path from the ini file from environment>
      try
      {
        iniPath = GetIniPathFromEnvironment(ref poCmd);
        if (iniPath == null || iniPath == string.Empty)
        {
          // Error code and message are stored in poCmd from caller
          return;
        }
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
        return;
      }
      #endregion </Get the path from the ini file from environment>
      
      #region <Get content from ini-file>
      BiIIniFile ini = new BiIIniFile(iniPath);
      try
      {
        #region <Read global variables>
        if (poCmd.mbCancel == 0)
        {
          string iniKeyValue = ini.IniReadValue(Consts.sectionDelimiter, Consts.sysInfoPropertiesDelimiter);
          if (iniKeyValue == string.Empty)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "Key " + Consts.sysInfoPropertiesDelimiter + @" in Section [" + Consts.sectionDelimiter + @"] not found or invalid.";
            return;
          }
          if (iniKeyValue == Consts.sysInfoDelimiterNewLine)
          {
            DelimiterSysProps = Environment.NewLine;
          }
          else
          {
            DelimiterSysProps = iniKeyValue;
          }
        }

        if (poCmd.mbCancel == 0)
        {
          string iniKeyValue = ini.IniReadValue(Consts.sectionDelimiter, Consts.sysInfoSysScriptNameDelimiter);
          if (iniKeyValue == string.Empty)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "Key " + Consts.sysInfoSysScriptNameDelimiter + @" in Section [" + Consts.sectionDelimiter + @"] not found or invalid.";
            return;
          }
          DelimiterSysScriptName = iniKeyValue;
        }

        if (poCmd.mbCancel == 0)
        {
          string iniKeyValue = ini.IniReadValue(Consts.sectionDelimiter, Consts.sysInfoIEdmStrLstDelimiter);
          if (iniKeyValue == string.Empty)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "Key " + Consts.sysInfoIEdmStrLstDelimiter + @" in Section [" + Consts.sectionDelimiter + @"] not found or invalid.";
            return;
          }
          DelimiterIEdmStrLst = iniKeyValue;
        }

        if (poCmd.mbCancel == 0)
        {
          string iniKeyValue = ini.IniReadValue(Consts.sectionDelimiter, Consts.sysInfoSysExecutionCondition1Delimiter);
          if (iniKeyValue == string.Empty)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "Key " + Consts.sysInfoSysExecutionCondition2Delimiter + @" in Section [" + Consts.sectionDelimiter + @"] not found or invalid.";
            return;
          }
          DelimiterSysExecutionCondition1 = iniKeyValue;
        }

        if (poCmd.mbCancel == 0)
        {
          string iniKeyValue = ini.IniReadValue(Consts.sectionDelimiter, Consts.sysInfoSysExecutionCondition2Delimiter);
          if (iniKeyValue == string.Empty)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "Key " + Consts.sysInfoSysExecutionCondition1Delimiter + @" in Section [" + Consts.sectionDelimiter + @"] not found or invalid.";
            return;
          }
          DelimiterSysExecutionCondition2 = iniKeyValue;
        }

        if (poCmd.mbCancel == 0)
        {
          string iniKeyValue = ini.IniReadValue(Consts.sysSection, Consts.sysInfoDebugMode);
          if (iniKeyValue == string.Empty)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "Key " + Consts.sysInfoDebugMode + @" in Section [" + Consts.sysSection + @"] not found or invalid.";
            return;
          }
          DebugMode = Convert.ToBoolean(iniKeyValue);
        }
        #endregion </Read global variables>

        #region <Checking all posible Structs.EdmCmdHooks items if needed>
        if (poCmd.mbCancel == 0)
        {
          Type _structItem = hooks.GetType();
          FieldInfo[] _structItems = _structItem.GetFields(BindingFlags.Instance | BindingFlags.Public);
          foreach (FieldInfo _item in _structItems)
          {
            bool useHook = false;

            #region <Get all hooks to install>
            if (_item.FieldType.Name == "Boolean")
            {
              #region <Checking, if INI item exists>
              if (!useHook)
              {
                string iniKeyValue = ini.IniReadValue(_item.Name, _item.Name);
                if (iniKeyValue != string.Empty && Convert.ToInt32(iniKeyValue) != 0)
                {
                  useHook = true;
                }
              }
              #endregion </Checking, if INI item exists>

              #region <Set struct item via invoke>
              if (useHook)
              {
                FieldInfo fi = typeof(Structs.EdmCmdHooks).GetField(_item.Name, BindingFlags.Public | BindingFlags.Instance);
                TypedReference reference = __makeref(hooks);
                fi.SetValueDirect(reference, true);
              }
              #endregion </Set struct item via invoke>
              
              #region <Get all informtion for the hooks>
              if (!onlyHooks)
              {
                #region <Set script path item via invoke>
                if (useHook)
                {
                  string iniKeyValue = ini.IniReadValue(_item.Name, Consts.scriptForHook.Replace(Consts.iniSectionPlaceHolder, _item.Name));
                  if (iniKeyValue != string.Empty)
                  {
                    FieldInfo fi = typeof(Structs.EdmCmdHooks).GetField(Consts.scriptForHook.Replace(Consts.iniSectionPlaceHolder, _item.Name), BindingFlags.Public | BindingFlags.Instance);
                    TypedReference reference = __makeref(hooks);
                    fi.SetValueDirect(reference, iniKeyValue);
                  }
                }
                #endregion </Set script path item via invoke>

                #region <Set wait until exit item via invoke>
                if (useHook)
                {
                  string iniKeyValue = ini.IniReadValue(_item.Name, Consts.waitUntilExitForHook.Replace(Consts.iniSectionPlaceHolder, _item.Name));
                  if (iniKeyValue != string.Empty)
                  {
                    FieldInfo fi = typeof(Structs.EdmCmdHooks).GetField(Consts.waitUntilExitForHook.Replace(Consts.iniSectionPlaceHolder, _item.Name), BindingFlags.Public | BindingFlags.Instance);
                    TypedReference reference = __makeref(hooks);
                    fi.SetValueDirect(reference, Convert.ToInt32(iniKeyValue));
                  }
                }
                #endregion </Set wait until exit item via invoke>

                #region <Set timeout item via invoke>
                if (useHook)
                {
                  string iniKeyValue = ini.IniReadValue(_item.Name, Consts.timeoutForHook.Replace(Consts.iniSectionPlaceHolder, _item.Name));
                  if (iniKeyValue != string.Empty)
                  {
                    FieldInfo fi = typeof(Structs.EdmCmdHooks).GetField(Consts.timeoutForHook.Replace(Consts.iniSectionPlaceHolder, _item.Name), BindingFlags.Public | BindingFlags.Instance);
                    TypedReference reference = __makeref(hooks);
                    fi.SetValueDirect(reference, Convert.ToInt32(iniKeyValue));
                  }
                }
                #endregion </Set timeout item via invoke>

                #region <Set hideScriptWindow item via invoke>
                if (useHook)
                {
                  string iniKeyValue = ini.IniReadValue(_item.Name, Consts.hideScriptWindowForHook.Replace(Consts.iniSectionPlaceHolder, _item.Name));
                  if (iniKeyValue != string.Empty)
                  {
                    FieldInfo fi = typeof(Structs.EdmCmdHooks).GetField(Consts.hideScriptWindowForHook.Replace(Consts.iniSectionPlaceHolder, _item.Name), BindingFlags.Public | BindingFlags.Instance);
                    TypedReference reference = __makeref(hooks);
                    fi.SetValueDirect(reference, Convert.ToInt32(iniKeyValue));
                  }
                }
                #endregion </Set hideScriptWindow item via invoke>

                #region <executionCondition via invoke>
                if (useHook)
                {
                  string iniKeyValue = ini.IniReadValue(_item.Name, Consts.executionConditionForHook.Replace(Consts.iniSectionPlaceHolder, _item.Name));
                  if (iniKeyValue != string.Empty)
                  {
                    FieldInfo fi = typeof(Structs.EdmCmdHooks).GetField(Consts.executionConditionForHook.Replace(Consts.iniSectionPlaceHolder, _item.Name), BindingFlags.Public | BindingFlags.Instance);
                    TypedReference reference = __makeref(hooks);
                    fi.SetValueDirect(reference, Convert.ToString(iniKeyValue));
                  }
                }
                #endregion </executionCondition via invoke>
              }
              #endregion </Get all informtion for the hooks>
            }
            #endregion <Get all hooks to install>
          }
        }
        #endregion </Checking all posible Structs.EdmCmdHooks items if needed>
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
      }
      #endregion </Get content from ini-file>
    }
    #endregion <Ini file>

    #region <Hooks>
    void AddHooks(ref EdmCmd poCmd, ref IEdmCmdMgr5 poCmdMgr, Structs.EdmCmdHooks hooks)
    {
      try
      {
        #region <Add hooks>
        if (hooks.EdmCmd_CardButton)
        {
          // 37 = The user clicked either OK or a button whose command is enclosed in brackets ("<...>") in the file data card
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_CardButton, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_CardButton) + "; ");
        }
        if (hooks.EdmCmd_PreAdd)
        {
          // 11 = One or more files are about to be added to the file vault
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreAdd, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreAdd) + "; ");
        }
        if (hooks.EdmCmd_InstallAddIn)
        {
          // 23 = The add-in is being installed
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_InstallAddIn, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_InstallAddIn) + "; ");
        }
        if (hooks.EdmCmd_UninstallAddIn)
        {
          // 24 = The add-in is about to be uninstalled
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_UninstallAddIn, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_UninstallAddIn) + "; ");
        }
        if (hooks.EdmCmd_CardInput)
        {
          // 38 = The user modified a value in a file or folder data card
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_CardInput, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_CardInput) + "; ");
        }
        if (hooks.EdmCmd_CardListSrc)
        {
          // 39 = The add-in should provide a list that is used in a card
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_CardListSrc, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_CardListSrc) + "; ");
        }
        //
        if (hooks.EdmCmd_Menu)
        {
          // 1 = User clicked a menu command or a toolbar button that was created by the add-in
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_Menu, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_Menu) + "; ");
        }
        if (hooks.EdmCmd_PostAdd)
        {
          // 12 = One or more files were added to the file vault
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostAdd, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostAdd) + "; ");
        }
        if (hooks.EdmCmd_PostAddFolder)
        {
          // 28 = One or more folders were added to the file vault
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostAddFolder, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostAddFolder) + "; ");
        }
        if (hooks.EdmCmd_PostCopy)
        {
          // 20 = One or more files were copied to a new folder
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostCopy, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostCopy) + "; ");
        }
        if (hooks.EdmCmd_PostCopyFolder)
        {
          // 36 = One or more folders were copied to a new parent folder
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostCopyFolder, null);
          debugInfo.Append(nameof(EdmCmd_PostCopyFolder) + "; ");
        }
        if (hooks.EdmCmd_PostDelete)
        {
          // 14 = One or more files have been deleted
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostDelete, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostDelete) + "; ");
        }
        if (hooks.EdmCmd_PostDeleteFolder)
        {
          // 30 = One or more folders were deleted from the file vault
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostDeleteFolder, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostDeleteFolder) + "; ");
        }
        if (hooks.EdmCmd_PostGet)
        {
          // 26 = One or more files were copied from the archive to the local hard disk
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostGet, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostGet) + "; ");
        }
        if (hooks.EdmCmd_PostLabel)
        {
          // 47 = A label has been created
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostLabel, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostLabel) + "; ");
        }
        if (hooks.EdmCmd_PostLabelAddItem)
        {
          // 53 = A label has gotten a file or folder added to it
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostLabelAddItem, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostLabelAddItem) + "; ");
        }
        if (hooks.EdmCmd_PostLabelDelete)
        {
          // 49 = A label has been deleted
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostLabelDelete, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostLabelDelete) + "; ");
        }
        if (hooks.EdmCmd_PostLabelModify)
        {
          // 51 = A label has been renamed or gotten its comment updated
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostLabelModify, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostLabelModify) + "; ");
        }
        if (hooks.EdmCmd_PostLock)
        {
          // 4 = One or more files have been checked out
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostLock, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostLock) + "; ");
        }
        if (hooks.EdmCmd_PostMove)
        {
          // 22 = One or more files were moved to a new folder
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostMove, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostMove) + "; ");
        }
        if (hooks.EdmCmd_PostMoveFolder)
        {
          // 34 = One or more folders were moved to a new parent folder
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostMoveFolder, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostMoveFolder) + "; ");
        }
        if (hooks.EdmCmd_PostRename)
        {
          // 16 = One or more files were renamed
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostRename, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostRename) + "; ");
        }
        if (hooks.EdmCmd_PostRenameFolder)
        {
          // 32 = One or more folders were renamed
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostRenameFolder, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostRenameFolder) + "; ");
        }
        if (hooks.EdmCmd_PostShare)
        {
          // 18 = One or more files were shared to a new folder
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostShare, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostShare) + "; ");
        }
        if (hooks.EdmCmd_PostState)
        {
          // 10 = One or more files had their states changed
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostState, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostState) + "; ");
        }
        if (hooks.EdmCmd_PostUndoLock)
        {
          // 8 = One or more files had their locks removed without any changes sent to the file vault
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostUndoLock, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostUndoLock) + "; ");
        }
        if (hooks.EdmCmd_PostUnlock)
        {
          // 6 = One or more files have been checked in
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PostUnlock, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PostUnlock) + "; ");
        }
        //
        if (hooks.EdmCmd_PreAddFolder)
        {
          // 27 = One or more folders are about to be added to the file vault
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreAddFolder, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreAddFolder) + "; ");
        }
        if (hooks.EdmCmd_PreCopy)
        {
          // 19 = One or more files are about to be copied to a new folder
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreCopy, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreCopy) + "; ");
        }
        if (hooks.EdmCmd_PreCopyFolder)
        {
          // 35 = One or more folders are about to be copied to a new parent folder
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreCopyFolder, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreCopyFolder) + "; ");
        }
        if (hooks.EdmCmd_PreDelete)
        {
          // 13 = One or more files are about to be deleted
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreDelete, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreDelete) + "; ");
        }
        if (hooks.EdmCmd_PreDeleteFolder)
        {
          // 29 = One or more folders are about to be deleted from the file vault
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreDeleteFolder, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreDeleteFolder) + "; ");
        }
        if (hooks.EdmCmd_PreGet)
        {
          // 25 = One or more files are about to be copied from the archive to the local hard disk
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreGet, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreGet) + "; ");
        }
        if (hooks.EdmCmd_PreLabel)
        {
          // 46 = A label is about to be created
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreLabel, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreLabel) + "; ");
        }
        if (hooks.EdmCmd_PreLabelAddItem)
        {
          // 52 = A label is about to get a file or folder added to it
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreLabelAddItem, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreLabelAddItem) + "; ");
        }
        if (hooks.EdmCmd_PreLabelDelete)
        {
          // 48 = A label is about to be deleted
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreLabelDelete, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreLabelDelete) + "; ");
        }
        if (hooks.EdmCmd_PreLabelModify)
        {
          // 50 = A label is about to be renamed or get its comment updated
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreLabelModify, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreLabelModify) + "; ");
        }
        if (hooks.EdmCmd_PreLock)
        {
          // 3 = One or more files are about to be checked out
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreLock, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreLock) + "; ");
        }
        if (hooks.EdmCmd_PreMove)
        {
          // 21 = One or more files are about to be moved to a new folder
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreMove, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreMove) + "; ");
        }
        if (hooks.EdmCmd_PreMoveFolder)
        {
          // 33 = One or more folders are about to be moved to a new parent folder
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreMoveFolder, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreMoveFolder) + "; ");
        }
        if (hooks.EdmCmd_PreRename)
        {
          // 15 = One or more files are about to be renamed
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreRename, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreRename) + "; ");
        }
        if (hooks.EdmCmd_PreRenameFolder)
        {
          // 31 = One or more folders are about to be renamed
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreRenameFolder, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreRenameFolder) + "; ");
        }
        if (hooks.EdmCmd_PreShare)
        {
          // 17 = One or more files are about to be shared to a new folder
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreShare, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreShare) + "; ");
        }
        if (hooks.EdmCmd_PreState)
        {
          // 9 = One or more files are about to have their states changed
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreState, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreState) + "; ");
        }
        if (hooks.EdmCmd_PreUndoLock)
        {
          // 7 = One or more files are about to get their locks removed without any changes sent to the file vault
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreUndoLock, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreUndoLock) + "; ");
        }
        if (hooks.EdmCmd_PreUnlock)
        {
          // 5 = One or more files are about to be checked in
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_PreUnlock, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_PreUnlock) + "; ");
        }
        //
        if (hooks.EdmCmd_SerialNo)
        {
          // 2 = The add-in should generate a new serial number
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_SerialNo, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_SerialNo) + "; ");
        }
        //
        if (hooks.EdmCmd_TaskDetails)
        {
          // 42 = Use this hook to add your own custom page to the task details dialog box in the task list
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskDetails, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_TaskDetails) + "; ");
        }
        if (hooks.EdmCmd_TaskLaunch)
        {
          // 44 = The task is being launched; add your own user interface to permit user input
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskLaunch, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_TaskLaunch) + "; ");
        }
        if (hooks.EdmCmd_TaskLaunchButton)
        {
          // 45 = OK or Cancel was clicked in the task launch dialog box
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskLaunchButton, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_TaskLaunchButton) + "; ");
        }
        if (hooks.EdmCmd_TaskRun)
        {
          // 43 = This hook is called on the task server; you should perform the actual work there
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskRun, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_TaskRun) + "; ");
        }
        if (hooks.EdmCmd_TaskSetup)
        {
          // 40 = Use this hook to add a task setup page to a task properties dialog box wizard
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskSetup, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_TaskSetup) + "; ");
        }
        if (hooks.EdmCmd_TaskSetupButton)
        {
          // 41 = OK or Cancel was clicked in the task properties dialog box wizard
          poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskSetupButton, null);
          debugInfo.Append(nameof(EdmCmdType.EdmCmd_TaskSetupButton) + "; ");
        }
        #endregion </Add hooks>
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
      }
    }

    void CallHooks(int hWnd, ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, ref Structs.EdmCmdHooksInfo hInfo)
    {
      Structs.EdmCmdHooks hooks = new Structs.EdmCmdHooks();
      #region <Read ini file>
      try
      {
        ReadIniFile(ref poCmd, ref hooks);
        if (poCmd.mbCancel != 0)
        {
          string extMessageText = Environment.NewLine + @"Attention, the add-in is not being processed. Please contact the Administrator.";
          MessageBox.Show(poCmd.mbsComment + extMessageText, Consts.AddInVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }

        #region <Only for debug mode>
        if (DebugMode)
        {
          currentTime = DateTime.Now;
          TimeSpan duration = currentTime - savedTime;
          savedTime = DateTime.Now;
          debugInfo.Append(@"EPDM - Time needed to read the INI file: " + duration.Milliseconds.ToString() + @" milliseconds" + Environment.NewLine);
        }
        #endregion </Only for debug mode>
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
        return;
      }
      #endregion </Read ini file>

      #region <Call hooks>
      try
      {
        switch (poCmd.meCmdType)
        {
          case EdmCmdType.EdmCmd_CardButton:
            EdmCmd_CardButton(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_CardInput:
            EdmCmd_CardInput(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_CardListSrc:
            EdmCmd_CardListSrc(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_InstallAddIn:
            EdmCmd_InstallAddIn(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_Menu:
            EdmCmd_Menu(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostAdd:
            EdmCmd_PostAdd(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostAddFolder:
            EdmCmd_PostAddFolder(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostCopy:
            EdmCmd_PostCopy(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostCopyFolder:
            EdmCmd_PostCopyFolder(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostDelete:
            EdmCmd_PostDelete(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostDeleteFolder:
            EdmCmd_PostDeleteFolder(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostGet:
            EdmCmd_PostGet(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostLabel:
            EdmCmd_PostLabel(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostLabelAddItem:
            EdmCmd_PostLabelAddItem(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostLabelDelete:
            EdmCmd_PostLabelDelete(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostLabelModify:
            EdmCmd_PostLabelModify(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostLock:
            EdmCmd_PostLock(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostMove:
            EdmCmd_PostMove(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostMoveFolder:
            EdmCmd_PostMoveFolder(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostRename:
            EdmCmd_PostRename(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostRenameFolder:
            EdmCmd_PostRenameFolder(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostShare:
            EdmCmd_PostShare(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostState:
            EdmCmd_PostState(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostUndoLock:
            EdmCmd_PostUndoLock(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PostUnlock:
            EdmCmd_PostUnlock(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreAdd:
            EdmCmd_PreAdd(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreAddFolder:
            EdmCmd_PreAddFolder(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreCopy:
            EdmCmd_PreCopy(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreCopyFolder:
            EdmCmd_PreCopyFolder(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreDelete:
            EdmCmd_PreDelete(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreDeleteFolder:
            EdmCmd_PreDeleteFolder(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreGet:
            EdmCmd_PreGet(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreLabel:
            EdmCmd_PreLabel(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreLabelAddItem:
            EdmCmd_PreLabelAddItem(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreLabelDelete:
            EdmCmd_PreLabelDelete(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreLabelModify:
            EdmCmd_PreLabelModify(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreLock:
            EdmCmd_PreLock(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreMove:
            EdmCmd_PreMove(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreMoveFolder:
            EdmCmd_PreMoveFolder(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreRename:
            EdmCmd_PreRename(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreRenameFolder:
            EdmCmd_PreRenameFolder(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreShare:
            EdmCmd_PreShare(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreState:
            EdmCmd_PreState(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreUndoLock:
            EdmCmd_PreUndoLock(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_PreUnlock:
            EdmCmd_PreUnlock(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_SerialNo:
            EdmCmd_SerialNo(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_TaskDetails:
            EdmCmd_TaskDetails(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_TaskLaunch:
            EdmCmd_TaskLaunch(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_TaskLaunchButton:
            EdmCmd_TaskLaunchButton(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_TaskRun:
            EdmCmd_TaskRun(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_TaskSetupButton:
            EdmCmd_TaskSetupButton(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
          case EdmCmdType.EdmCmd_UninstallAddIn:
            EdmCmd_UninstallAddIn(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo);
            break;
        }
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
      }
      #endregion </Call hooks>
    }
    #endregion </Hooks>

    #region <Helper>
    private void GetHookSettingsFromIniFile(ref EdmCmd poCmd, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo, string hookName)
    {
      try
      {
        int finished = 0;
        int finishedCount = 5;
        Type _structItem = hooks.GetType();
        FieldInfo[] _structItems = _structItem.GetFields(BindingFlags.Instance | BindingFlags.Public);
        foreach (FieldInfo _item in _structItems)
        {
          if (_item.Name == Consts.scriptForHook.Replace(Consts.iniSectionPlaceHolder, hookName))
          {
            FieldInfo fi = typeof(Structs.EdmCmdHooks).GetField(_item.Name, BindingFlags.Public | BindingFlags.Instance);
            TypedReference reference = __makeref(hooks);
            hInfo.script = Convert.ToString(fi.GetValueDirect(reference));
            ++finished;
            if (finished == finishedCount) { break; }
          }
          else if (_item.Name == Consts.waitUntilExitForHook.Replace(Consts.iniSectionPlaceHolder, hookName))
          {
            FieldInfo fi = typeof(Structs.EdmCmdHooks).GetField(_item.Name, BindingFlags.Public | BindingFlags.Instance);
            TypedReference reference = __makeref(hooks);
            hInfo.waitUntilExit = Convert.ToInt32(fi.GetValueDirect(reference));
            ++finished;
            if (finished == finishedCount) { break; }
          }
          else if (_item.Name == Consts.timeoutForHook.Replace(Consts.iniSectionPlaceHolder, hookName))
          {
            FieldInfo fi = typeof(Structs.EdmCmdHooks).GetField(_item.Name, BindingFlags.Public | BindingFlags.Instance);
            TypedReference reference = __makeref(hooks);
            hInfo.timeout = Convert.ToInt32(fi.GetValueDirect(reference));
            ++finished;
            if (finished == finishedCount) { break; }
          }
          else if (_item.Name == Consts.hideScriptWindowForHook.Replace(Consts.iniSectionPlaceHolder, hookName))
          {
            FieldInfo fi = typeof(Structs.EdmCmdHooks).GetField(_item.Name, BindingFlags.Public | BindingFlags.Instance);
            TypedReference reference = __makeref(hooks);
            hInfo.hideScriptWindow = Convert.ToInt32(fi.GetValueDirect(reference));
            ++finished;
            if (finished == finishedCount) { break; }
          }
          else if (_item.Name == Consts.executionConditionForHook.Replace(Consts.iniSectionPlaceHolder, hookName))
          {
            FieldInfo fi = typeof(Structs.EdmCmdHooks).GetField(_item.Name, BindingFlags.Public | BindingFlags.Instance);
            TypedReference reference = __makeref(hooks);
            hInfo.executionCondition = Convert.ToString(fi.GetValueDirect(reference));
            ++finished;
            if (finished == finishedCount) { break; }
          }
        }
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
      }
    }

    private List<Structs.EdmSystemInformation> GetSysteInformationForScript(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, List<string> fileNames, bool getFileInfo, string hookName = "")
    {
      List<Structs.EdmSystemInformation> sysInfos = new List<Structs.EdmSystemInformation>();

      IEdmVault13 vault13 = (IEdmVault13)vault5;
      IEdmFile8 file8 = default(IEdmFile8);
      IEdmFolder7 folder7 = default(IEdmFolder7);
      IEdmUser10 user10 = default(IEdmUser10);
      #region <Get system information>
      for (int i = 0; i < fileNames.Count; i++)
      {
        string userGroupIds = string.Empty;
        string userGroupNames = string.Empty;
        string fileName = fileNames[i];
        Structs.EdmSystemInformation sysInfo = new Structs.EdmSystemInformation();
        if (poCmd.mbCancel != 0)
        {
          return sysInfos;
        }
        try
        {
          if (poCmd.mbCancel == 0)
          {
            IEdmUserMgr5 uMgr5 = default(IEdmUserMgr5);
            uMgr5 = (IEdmUserMgr5)vault5;
            user10 = (IEdmUser10)uMgr5.GetLoggedInUser();
            // User gouups
            IEdmUserMgr7 uMgr7 = (IEdmUserMgr7)uMgr5;
            IEdmUser8 user8 = (IEdmUser8)uMgr7.GetUser(user10.ID);
            // CLR4
            //object[] user8Groups = null;
            // CLR2
            //System.Array user8Groups = null;
            user8.GetGroupMemberships(out user8Groups);
            foreach (IEdmUserGroup7 userGroup in user8Groups)
            {
              userGroupIds = userGroupIds + userGroup.ID.ToString() + DelimiterIEdmStrLst; ;
              userGroupNames = userGroupNames + userGroup.Name + DelimiterIEdmStrLst;
            }
            userGroupIds = userGroupIds.Substring(0, userGroupIds.Length - DelimiterIEdmStrLst.Length) + Environment.NewLine;
            if (userGroupIds.EndsWith(Environment.NewLine))
            {
              userGroupIds = userGroupIds.Substring(0, userGroupIds.Length - Environment.NewLine.Length);
            }
            userGroupNames = userGroupNames.Substring(0, userGroupNames.Length - DelimiterIEdmStrLst.Length) + Environment.NewLine;
            if (userGroupNames.EndsWith(Environment.NewLine))
            {
              userGroupNames = userGroupNames.Substring(0, userGroupNames.Length - Environment.NewLine.Length);
            }
          }
          if (getFileInfo)
          {
            IEdmFolder5 folder5 = null;

            #region <Only for 'EdmCmd_Menu'>
            if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_Menu))
            {
              if (ppoData[i].mlObjectID1 != 0)
              {
                // ID of folder
                file8 = (IEdmFile8)vault5.GetObject(EdmObjectType.EdmObject_File, ppoData[i].mlObjectID1);
                folder5 = (IEdmFolder5)vault5.GetObject(EdmObjectType.EdmObject_Folder, ppoData[0].mlObjectID3);
                fileName = folder5.LocalPath + @"\" + file8.Name;
              }
              else
              {
                folder5 = (IEdmFolder5)vault5.GetObject(EdmObjectType.EdmObject_Folder, ppoData[0].mlObjectID2);
                fileName = folder5.LocalPath;
                getFileInfo = false;
              }
              fileNames[i] = fileName;
            }
            #endregion </Only for 'EdmCmd_Menu'>

            #region <Only for 'EdmCmd_PostAdd'>
            if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostAdd))
            {
              sysInfo.fileID = ppoData[i].mlObjectID2.ToString();
              folder5 = (IEdmFolder5)vault5.GetObject(EdmObjectType.EdmObject_Folder, ppoData[i].mlObjectID1);
              getFileInfo = false;
            }
            #endregion </Only for 'EdmCmd_PostAdd'>

            if (getFileInfo & fileName != string.Empty)
            {
              file8 = (IEdmFile8)vault5.GetFileFromPath(fileName, out folder5);
            }
            if (getFileInfo & file8 == null)
            {
              poCmd.mbCancel = 1;
              poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + @"File '" + fileName + @"' not found.";
            }
            if (poCmd.mbCancel == 0)
            {
              folder7 = (IEdmFolder7)folder5;
              if (folder7 == null)
              {
                poCmd.mbCancel = 1;
                poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + @"Folder with ID '" + fileName + @"' not found.";
              }
            }
          }
        }
        catch (Exception ex)
        {
          poCmd.mbCancel = 1;
          poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
        }

        #region <Default Properties>>
        if (poCmd.mbCancel == 0)
        {
          try
          {
            sysInfo.vaultCommandID = vault13.CommandID == null ? "" : vault13.CommandID.ToString();
            sysInfo.vaultIsLoggedIn = vault13.IsLoggedIn == null ? "" : vault13.IsLoggedIn.ToString();
            sysInfo.vaultLanguage = vault13.Language == null ? "" : vault13.Language.ToString();
            sysInfo.vaultName = vault13.Name == null ? "" : vault13.Name.ToString();
            sysInfo.vaultRootFolder = vault13.RootFolder == null ? "" : vault13.RootFolder.Name.ToString();
            sysInfo.vaultRootFolderPath = vault13.RootFolderPath == null ? "" : vault13.RootFolderPath.ToString();
            sysInfo.vaultRootFolderID = vault13.RootFolderID == null ? "" : vault13.RootFolderID.ToString();
            sysInfo.vaultSilentMode = vault13.SilentMode == null ? "" : vault13.SilentMode.ToString();
            sysInfo.vaultClientType = vault13.ClientType == null ? "" : vault13.ClientType.ToString();
            sysInfo.vaultItemRootFolder = vault13.ItemRootFolder == null ? "" : vault13.ItemRootFolder.Name.ToString();
            sysInfo.vaultItemRootFolderID = vault13.ItemRootFolderID == null ? "" : vault13.ItemRootFolderID.ToString();
            //
            sysInfo.userFullName = user10.FullName == null ? "" : user10.FullName.ToString();
            sysInfo.userInitials = user10.Initials == null ? "" : user10.Initials.ToString();
            sysInfo.userUserData = user10.UserData == null ? "" : user10.UserData.ToString();
            sysInfo.userEmail = user10.Email == null ? "" : user10.Email.ToString();
            sysInfo.userID = user10.ID.ToString();
            sysInfo.userGroupIds = userGroupIds;
            sysInfo.userGroupNames = userGroupNames;
            if (getFileInfo)
            {
              sysInfo.fileCurrentRevision = file8.CurrentRevision == null ? "" : file8.CurrentRevision.ToString();
              sysInfo.fileCurrentStateName = file8.CurrentState == null ? "" : file8.CurrentState.Name.ToString();
              sysInfo.fileCurrentStateID = file8.CurrentState == null ? "" : file8.CurrentState.ID.ToString();
              sysInfo.fileCurrentVersion = file8.CurrentVersion == null ? "" : file8.CurrentVersion.ToString();
              sysInfo.fileID = file8.ID == null ? "" : file8.ID.ToString();
              sysInfo.fileIsLocked = file8.IsLocked == null ? "" : file8.IsLocked.ToString();
              sysInfo.fileLockedByUser = file8.LockedByUser == null ? "" : file8.LockedByUser.Name.ToString();
              sysInfo.fileLockedByUserID = file8.LockedByUserID == null ? "" : file8.LockedByUserID.ToString();
              sysInfo.fileLockedInFolder = file8.LockedInFolder == null ? "" : file8.LockedInFolder.LocalPath.ToString();
              sysInfo.fileLockedInFolderID = file8.LockedInFolderID == null ? "" : file8.LockedInFolderID.ToString();
              sysInfo.fileLockedOnComputer = file8.LockedOnComputer == null ? "" : file8.LockedOnComputer.ToString();
              sysInfo.fileLockPath = file8.LockPath == null ? "" : file8.LockPath.ToString();
              sysInfo.fileName = fileName.ToString();
              sysInfo.fileObjectType = file8.ObjectType == null ? "" : file8.ObjectType.ToString();
              sysInfo.fileCategoryName = file8.Category == null ? "" : file8.Category.Name.ToString();
              sysInfo.fileCategoryID = file8.Category == null ? "" : file8.Category.ID.ToString();
              sysInfo.fileFileType = file8.FileType == null ? "" : file8.FileType.ToString();
            }
          }
          catch (Exception ex)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
          }
        }
        #endregion </Default Properties>>

        #region <EdmCmd_CardButton>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_CardButton))
        {
          string cfgNames = string.Empty;
          try
          {
            #region <Get a list of all configurations in the current file>
            cfgNames = GetConfigurationsFromIEdmStrLst5(ref poCmd, ppoData[i], ref file8);
            #endregion </Get a list of all configurations in the current file>
          }
          catch (Exception ex)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
          }
          if (poCmd.mbCancel == 0)
          {
            sysInfo.cardButtonNameOfAddInToCall = poCmd.mbsComment == null ? "" : poCmd.mbsComment;
            sysInfo.cardButtonID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
            sysInfo.cardButtonActiveConfiguration = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
            sysInfo.cardButtonEdmCardFlag = ppoData[i].mlLongData1 == null ? "" : ppoData[i].mlLongData1.ToString();
            sysInfo.cardButtonControlId = ppoData[i].mlLongData2 == null ? "" : ppoData[i].mlLongData2.ToString();
            sysInfo.cardButtonAllFileConfigurations = cfgNames == null ? "" : cfgNames.ToString();
          }
        }
        #endregion </EdmCmd_CardButton>

        #region <EdmCmd_CardInput>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_CardInput))
        {
          string cfgNames = string.Empty;
          try
          {
            #region <Get a list of all configurations in the current file>
            cfgNames = GetConfigurationsFromIEdmStrLst5(ref poCmd, ppoData[i], ref file8);
            #endregion </Get a list of all configurations in the current file>
          }
          catch (Exception ex)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
          }
          if (poCmd.mbCancel == 0)
          {
            sysInfo.cardInputModifiedControlID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
            sysInfo.cardInputCardID = ppoData[i].mlObjectID4 == null ? "" : ppoData[i].mlObjectID4.ToString();
            sysInfo.cardInputActiveConfiguration = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
            sysInfo.cardInputUpdatedVariable = ppoData[i].mlLongData1 == null ? "" : ppoData[i].mlLongData1.ToString();
            sysInfo.cardInputAllFileConfigurations = cfgNames == null ? "" : cfgNames.ToString();
            sysInfo.cardInputVariableName = poCmd.mbsComment == null ? "" : poCmd.mbsComment.ToString();
          }
        }
        #endregion </EdmCmd_CardInput>

        #region <EdmCmd_CardListSrc>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_CardListSrc))
        {
          string cfgNames = string.Empty;
          try
          {
            #region <Get a list of all configurations in the current file>
            cfgNames = GetConfigurationsFromIEdmStrLst5(ref poCmd, ppoData[i], ref file8);
            #endregion </Get a list of all configurations in the current file>
          }
          catch (Exception ex)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
          }
          if (poCmd.mbCancel == 0)
          {
            sysInfo.cardListSrcControlID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
            sysInfo.cardListSrcCardID = ppoData[i].mlObjectID4 == null ? "" : ppoData[i].mlObjectID4.ToString();
            sysInfo.cardListSrcActiveConfiguration = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
            sysInfo.cardListSrcControlVariableName = ppoData[i].mbsStrData3 == null ? "" : ppoData[i].mbsStrData3.ToString();
            sysInfo.cardListSrcControlVariableID = ppoData[i].mlLongData1 == null ? "" : ppoData[i].mlLongData1.ToString();
            sysInfo.cardListSrcAllFileConfigurations = cfgNames == null ? "" : cfgNames.ToString();
          }
        }
        #endregion </EdmCmd_CardListSrc>

        #region <EdmCmd_Menu>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_Menu))
        {
          sysInfo.menuParentFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.menuMenuID = poCmd.mlCmdID.ToString() == null ? "" : poCmd.mlCmdID.ToString();
        }
        #endregion </EdmCmd_Menu>

        #region <EdmCmd_PreAdd>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreAdd))
        {
          sysInfo.preAddLocalFileName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.preAddSourceFileName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
          sysInfo.preAddParentFolderID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.preAddNetworkSharingLinks = ppoData[i].mlLongData1 == null ? "" : ppoData[i].mlLongData1.ToString();
        }
        #endregion </EdmCmd_PreAdd>

        #region <EdmCmd_PostAdd>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostAdd))
        {
          sysInfo.postAddNetworkSharingLinks = ppoData[i].mlLongData1 == null ? "" : ppoData[i].mlLongData1.ToString();
          sysInfo.postAddParentFolderID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
        }
        #endregion </EdmCmd_PostAdd>

        #region <PreLock>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreLock))
        {
          sysInfo.preLockParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
        }
        #endregion </PreLock>

        #region <PostLock>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostLock))
        {
          sysInfo.postLockParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
        }
        #endregion </PostLock>

        #region <EdmCmd_PreAddFolder>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreAddFolder))
        {
          sysInfo.preAddFolderParentFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.preAddFolderNewFolderName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
        }
        #endregion </EdmCmd_PreAddFolder>

        #region <EdmCmd_PostAddFolder>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostAddFolder))
        {
          sysInfo.postAddFolderFolderID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.postAddFolderParentFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.postAddFolderNewFolderName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
        }
        #endregion </EdmCmd_PostAddFolder>

        #region <EdmCmd_PreCopy>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreCopy))
        {
          sysInfo.preCopyFolderDestinationID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.preCopyFieID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.preCopySourceFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.preCopySourceFileName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.preCopyDestinationFileName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
        }
        #endregion </EdmCmd_PostAddFolder>

        #region <EdmCmd_PostCopy>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostCopy))
        {
          sysInfo.postCopySourceFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.postCopySourceFileName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
        }
        #endregion </EdmCmd_PostCopy>

        #region <EdmCmd_PreCopyFolder>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreCopyFolder))
        {
          sysInfo.preCopyFolderSourceFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.preCopyFolderDestinationParentFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.preCopyFolderNewFolderName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
        }
        #endregion </EdmCmd_PreCopyFolder>

        #region <EdmCmd_PostCopyFolder>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmd_PostCopyFolder))
        {
          sysInfo.postCopyFolderFolderID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.postCopyFolderSourceFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.postCopyFolderParentFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.postCopyFolderFolderName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
        }
        #endregion </EdmCmd_PostCopyFolder>

        #region <EdmCmd_PreDelete>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreDelete))
        {
          sysInfo.preDeleteParentFolder = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
        }
        #endregion </EdmCmd_PreDelete>

        #region <EdmCmd_PostDelete>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostDelete))
        {
          sysInfo.postDeleteDeletedFileID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.postDeleteParetnFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.postDeleteDeletedFileName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.postDeleteNumberOfFoldersInWhichItWasShared = ppoData[i].mlLongData1 == null ? "" : ppoData[i].mlLongData1.ToString();
        }
        #endregion </EdmCmd_PostDelete>

        #region <EdmCmd_PreDeleteFolder>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreDeleteFolder))
        {
          sysInfo.preDeleteFolderFolderID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.preDeleteFolderFolderName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
        }
        #endregion </EdmCmd_PreDeleteFolder>

        #region <EdmCmd_PostDeleteFolder>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostDeleteFolder))
        {
          sysInfo.postDeleteFolderFolderID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.postDeleteFolderFolderName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
        }
        #endregion </EdmCmd_PostDeleteFolder>

        #region <EdmCmd_PreGet>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreGet))
        {
          sysInfo.preGetFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
        }
        #endregion </EdmCmd_PreGet>

        #region <EdmCmd_PostGet>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostGet))
        {
          sysInfo.postGetFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
        }
        #endregion </EdmCmd_PostGet>

        #region <EdmCmd_PreMove>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreMove))
        {
          sysInfo.preMoveSourceFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.preMoveDestinationFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.preMoveDestinationFileName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
        }
        #endregion </EdmCmd_PreMove>

        #region <EdmCmd_PostMove>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostMove))
        {
          sysInfo.postMoveSourceFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.postMoveDestinationFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.postMoveSourceFileName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
        }
        #endregion </EdmCmd_PostMove>

        #region <EdmCmd_PreMoveFolder>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreMoveFolder))
        {
          sysInfo.preMoveFolderFolderID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.preMoveFolderSourceParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.preMoveFolderDestinationParentFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.preMoveFolderSourceFolderName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.preMoveFolderDestinationFolderName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
        }
        #endregion </EdmCmd_PreMoveFolder>

        #region <EdmCmd_PostMoveFolder>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostMoveFolder))
        {
          sysInfo.postMoveFolderFolderID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.postMoveFolderSourceParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.postMoveFolderDestinationParentFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.postMoveFolderSourceFolderName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.postMoveFolderDestinationFolderName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
        }
        #endregion </EdmCmd_PostMoveFolder>

        #region <EdmCmd_PreRename>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreRename))
        {
          sysInfo.preRenameFileToRenameID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.preRenameFilesParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.preRenameNewFileName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
          sysInfo.preRenameOldFileName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          fileNames[i] = sysInfo.preRenameOldFileName;
        }
        #endregion </EdmCmd_PreRename>

        #region <EdmCmd_PostRename>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostRename))
        {
          sysInfo.postRenameFileToRenameID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.postRenameFilesParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.postRenameNewFileName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
          sysInfo.postRenameOldFileName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
        }
        #endregion </EdmCmd_PostRename>

        #region <EdmCmd_PreRenameFolder>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreRenameFolder))
        {
          sysInfo.preRenameFolderFolderID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.preRenameFolderParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.preRenameFolderOldFolderName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.preRenameFolderNewFolderName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
        }
        #endregion </EdmCmd_PreRenameFolder>

        #region <EdmCmd_PostRenameFolder>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostRenameFolder))
        {
          sysInfo.postRenameFolderFolderID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.postRenameFolderParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.postRenameFolderOldFolderName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.postRenameFolderNewFolderName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
        }
        #endregion </EdmCmd_PostRenameFolder>

        #region <EdmCmd_PreShare>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreShare))
        {
          sysInfo.preShareParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.preShareDestinationFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
        }
        #endregion </EdmCmd_PreShare>

        #region <EdmCmd_PostShare>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostShare))
        {
          sysInfo.postShareParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.postShareDestinationFolderID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
        }
        #endregion </EdmCmd_PostShare>

        #region <EdmCmd_PreState>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreState))
        {
          sysInfo.preStateParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.preStateTransitionID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.preStateDestinationStateName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
          sysInfo.preStateSourceStateID = ppoData[i].mlLongData1 == null ? "" : ppoData[i].mlLongData1.ToString();
          sysInfo.preStateDestinationStateID = ppoData[i].mlLongData2 == null ? "" : ppoData[i].mlLongData2.ToString();
          sysInfo.preStateCommentText = poCmd.mbsComment == null ? "" : poCmd.mbsComment.ToString();
        }
        #endregion </EdmCmd_PreState>

        #region <EdmCmd_PostState>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostState))
        {
          sysInfo.postStateParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.postStateTransitionID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.postStateDestinationStateName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
          sysInfo.postStateSourceStateID = ppoData[i].mlLongData1 == null ? "" : ppoData[i].mlLongData1.ToString();
          sysInfo.postStateDestinationStateID = ppoData[i].mlLongData2 == null ? "" : ppoData[i].mlLongData2.ToString();
          sysInfo.postStateCommentText = poCmd.mbsComment == null ? "" : poCmd.mbsComment.ToString();
        }
        #endregion </EdmCmd_PostState>

        #region <EdmCmd_PreUndoLock>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreUndoLock))
        {
          sysInfo.preUndoLockParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
        }
        #endregion </EdmCmd_PreUndoLock>

        #region <EdmCmd_PostUndoLock>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostUndoLock))
        {
          sysInfo.postUndoLockParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
        }
        #endregion </EdmCmd_PostUndoLock>

        #region <EdmCmd_PreUnlock>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreUnlock))
        {
          sysInfo.preUnlockParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.preUnlockCommentText = poCmd.mbsComment == null ? "" : poCmd.mbsComment.ToString();

        }
        #endregion </EdmCmd_PreUnlock>

        #region <EdmCmd_PostUnlock>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostUnlock))
        {
          sysInfo.postUnlockParentFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.postUnlockCommentText = poCmd.mbsComment == null ? "" : poCmd.mbsComment.ToString();
        }
        #endregion </EdmCmd_PostUnlock>

        #region <EdmCmd_PreLabel>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreLabel))
        {
          sysInfo.preLabelFileID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.preLabelFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.preLabelLabelID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.preLabelLabelName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.preLabelLabelComment = poCmd.mbsComment == null ? "" : poCmd.mbsComment.ToString();
          sysInfo.preLabelFileName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
          sysInfo.preLabelCreatedRecursively = ppoData[i].mlLongData1 == null ? "" : ppoData[i].mlLongData1.ToString();
        }
        #endregion </EdmCmd_PreLabel>

        #region <EdmCmd_PostLabel>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostLabel))
        {
          sysInfo.postLabelFileID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.postLabelFolderID = ppoData[i].mlObjectID2 == null ? "" : ppoData[i].mlObjectID2.ToString();
          sysInfo.postLabelLabelID = ppoData[i].mlObjectID3 == null ? "" : ppoData[i].mlObjectID3.ToString();
          sysInfo.postLabelLabelName = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.postLabelComment = poCmd.mbsComment == null ? "" : poCmd.mbsComment.ToString();
          sysInfo.postLabelFileName = ppoData[i].mbsStrData2 == null ? "" : ppoData[i].mbsStrData2.ToString();
          sysInfo.postLabelCreatedRecursively = ppoData[i].mlLongData1 == null ? "" : ppoData[i].mlLongData1.ToString();
        }
        #endregion </EdmCmd_PostLabel>

        #region <EdmCmd_PreLabelDelete>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreLabelDelete))
        {
          sysInfo.preLabelDeleteLabelID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.preLabelDeleteLabel = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
        }
        #endregion </EdmCmd_PreLabelDelete>

        #region <EdmCmd_PostLabelDelete>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostLabelDelete))
        {
          sysInfo.postLabelDeleteLabelID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.postLabelDeleteLabel = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
        }
        #endregion </EdmCmd_PostLabelDelete>

        #region <EdmCmd_PreLabelModify>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreLabelModify))
        {
          sysInfo.preLabelModifyLabelID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.preLabelModifiyLabel = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.preLabelModifiyComment = poCmd.mbsComment == null ? "" : poCmd.mbsComment.ToString();
        }
        #endregion </EdmCmd_PreLabelModify>

        #region <EdmCmd_PostLabelModify>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostLabelModify))
        {
          sysInfo.postLabelModofiyLabelID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.postLabelModifyLabel = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.postLabelModifiyComment = poCmd.mbsComment == null ? "" : poCmd.mbsComment.ToString();
        }
        #endregion </EdmCmd_PostLabelModify>

        #region <EdmCmd_PreLabelAddItem>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PreLabelAddItem))
        {
          sysInfo.preLabelModifyLabelID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.preLabelModifiyLabel = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.preLabelModifiyComment = poCmd.mbsComment == null ? "" : poCmd.mbsComment.ToString();
        }
        #endregion </EdmCmd_PreLabelAddItem>

        #region <EdmCmd_PostLabelAddItem>
        if (poCmd.mbCancel == 0 && hookName == nameof(EdmCmdType.EdmCmd_PostLabelAddItem))
        {
          sysInfo.postLabelModofiyLabelID = ppoData[i].mlObjectID1 == null ? "" : ppoData[i].mlObjectID1.ToString();
          sysInfo.postLabelModifyLabel = ppoData[i].mbsStrData1 == null ? "" : ppoData[i].mbsStrData1.ToString();
          sysInfo.postLabelModifiyComment = poCmd.mbsComment == null ? "" : poCmd.mbsComment.ToString();
        }
        #endregion </EdmCmd_PostLabelAddItem>

        if (poCmd.mbCancel == 0)
        {
          sysInfos.Add(sysInfo);
        }
        else
        {
          sysInfos = new List<Structs.EdmSystemInformation>();
          return sysInfos;
        }
      }
      #endregion </Get system information>

      return sysInfos;
    }

    private string GetConfigurationsFromIEdmStrLst5(ref EdmCmd poCmd, EdmCmdData ppoData, ref IEdmFile8 file8)
    {
      string cfgNames = string.Empty;
      try
      {
        if (ppoData.mpoExtra != null)
        {
          EdmStrLst5 cfgList = default(EdmStrLst5);
          IEdmPos5 pos = default(IEdmPos5);
          cfgList = file8.GetConfigurations();
          pos = cfgList.GetHeadPosition();
          string cfgName = string.Empty;
          while (!pos.IsNull)
          {
            cfgName = cfgList.GetNext(pos);
            cfgNames = cfgNames + cfgName + DelimiterIEdmStrLst;
          }
          cfgNames = cfgNames.Substring(0, cfgNames.Length - DelimiterIEdmStrLst.Length) + Environment.NewLine;
          if (cfgNames.EndsWith(Environment.NewLine))
          {
            cfgNames = cfgNames.Substring(0, cfgNames.Length - Environment.NewLine.Length);
          }
        }
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
      }
      return cfgNames;
    }

    private void GetFileListFromPpoData(ref EdmCmd poCmd, ref EdmCmdData[] ppoData, ref List<string> fileNames, int ppoDataString)
    {
      try
      {
        if (ppoData == null || ppoData.Length == 0)
        {
          poCmd.mbCancel = 1;
          poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "No files to process";
          return;
        }
        for (int i = 0; i < ppoData.Length; i++)
        {
          if (ppoDataString == 1)
          {
            fileNames.Add(ppoData[i].mbsStrData1);
          }
          else if (ppoDataString == 2)
          {
            fileNames.Add(ppoData[i].mbsStrData2);
          }
        }
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
      }
    }

    private void SaveSystemProperties(ref EdmCmd poCmd, ref List<Structs.EdmSystemInformation> sysInfos, List<string> fileNames, ref string hookInfoFile, string hookName = "")
    {
      try
      {
        #region <Create temp file>
        if (!System.IO.File.Exists(hookInfoFile))
        {
          poCmd.mbCancel = 1;
          poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "Cannot create temp file: " + hookInfoFile;
          hookInfoFile = string.Empty;
          return;
        }
        #endregion </Create temp file>

        #region <Create the string with properties and save>
        System.Text.StringBuilder sysProps = new System.Text.StringBuilder();
        for (int i = 0; i < sysInfos.Count; i++)
        {
          System.Text.StringBuilder sB = new System.Text.StringBuilder();
          // Create [section]
          sB.Append(@"[" + fileNames[i] + @"]" + DelimiterSysProps);
          // Create keys
          
          // Insert main window handle as int
          sB.Append(@"mainWindowHandleInt=" + hWnd.ToString() + Environment.NewLine);
          //Insert debug mode as int
          int dM = 0;
          if (DebugMode)
          {
            dM = 1;
          }
          sB.Append(@"debugMode=" + dM.ToString() + Environment.NewLine);
          // Insert all keys without empty values
          Structs.EdmSystemInformation _sysInfos = sysInfos[i];
          Type sysInfo = _sysInfos.GetType();
          FieldInfo[] _sysInfo = sysInfo.GetFields(BindingFlags.Instance | BindingFlags.Public);
          foreach (FieldInfo _item in _sysInfo)
          {
            FieldInfo fi = typeof(Structs.EdmSystemInformation).GetField(_item.Name, BindingFlags.Public | BindingFlags.Instance);
            TypedReference reference = __makeref(_sysInfos);
            string refString = Convert.ToString(fi.GetValueDirect(reference));
            if (refString != string.Empty)
            {
              sB.Append(_item.Name + @"=" + refString + DelimiterSysProps);
            }
          }
          sysProps.Append(sB.ToString().Substring(0, sB.ToString().Length - DelimiterSysProps.Length) + Environment.NewLine);
        }
        if (sysProps.ToString() == string.Empty)
        {
          poCmd.mbCancel = 1;
          poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "Properties cant# be found";
          return;
        }
        System.IO.File.WriteAllText(hookInfoFile, sysProps.ToString());
        #endregion </Create the string with properties and save>

      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
      }
    }

    private void CollectDataAndRunScript(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo, int mbsStrData, bool getFileInfo, string hookName = "")
    {
      try
      {
        bool executeHook = true;

        #region <Only for debug mode>
        if (DebugMode)
        {
          savedTime = DateTime.Now;
          debugInfo.Append(@"EPDM - Called hook: " + hookName + Environment.NewLine);
        }
        #endregion </Only for debug mode>

        #region <Get hook settings>
        GetHookSettingsFromIniFile(ref poCmd, hooks, ref hInfo, hookName);
        if (poCmd.mbCancel != 0)
        {
          // Error code and message are stored in poCmd from caller
          return;
        }
        #endregion </Get hook settings>

        #region <Check execute conditions for hooks>
        if (hookName == nameof(EdmCmdType.EdmCmd_Menu))
        {
          string[] parentFolders = new string[ppoData.Length];
          string[] fileNames = new string[ppoData.Length];

          for (int i = 0; i < ppoData.Length; i++)
          {
            //= ppoData.mlObjectID1;
            parentFolders[i] = System.IO.Path.GetFileName(ppoData[i].mbsStrData1);
            fileNames[i] = System.IO.Path.GetFileName(ppoData[i].mbsStrData1);
          }
          CheckExecutionConditionForHook(ref poCmd, ref hInfo, hookName, ref executeHook, ref parentFolders, ref fileNames);
          if (poCmd.mbCancel != 0)
          {
            // Error code and message are stored in poCmd from caller
            return;
          }

        }
        else if (hookName == nameof(EdmCmdType.EdmCmd_CardButton))
        {
          string cardId = ppoData[0].mlObjectID3.ToString();
          string addInDestination = poCmd.mbsComment;
          CheckExecutionConditionForHook(ref poCmd, ref hInfo, hookName, ref executeHook, cardId, addInDestination);
          if (poCmd.mbCancel != 0)
          {
            // Error code and message are stored in poCmd from caller
            return;
          }
        }
        else if (hookName == nameof(EdmCmdType.EdmCmd_CardInput))
        {
          string cardID = ppoData[0].mlObjectID4.ToString();
          string variableName = poCmd.mbsComment;
          CheckExecutionConditionForHook(ref poCmd, ref hInfo, hookName, ref executeHook, cardID, variableName);
          if (poCmd.mbCancel != 0)
          {
            // Error code and message are stored in poCmd from caller
            return;
          }
        }
        else if (hookName == nameof(EdmCmdType.EdmCmd_CardListSrc))
        {
          //string folderName = ppoData[0].mbsStrData3;
          string folderName = System.IO.Path.GetFullPath(ppoData[0].mbsStrData1);
          string fileName = System.IO.Path.GetFileName(ppoData[0].mbsStrData1);
          CheckExecutionConditionForHook(ref poCmd, ref hInfo, hookName, ref executeHook, folderName, fileName);
          if (poCmd.mbCancel != 0)
          {
            // Error code and message are stored in poCmd from caller
            return;
          }
        }
        else if (hookName == nameof(EdmCmdType.EdmCmd_PreLock)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostLock)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreAdd)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostAdd)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreCopy)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostCopy)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreAddFolder)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostAddFolder)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreCopyFolder)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostCopyFolder)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreDelete)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostDelete)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreDeleteFolder)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostDeleteFolder)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreGet)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostGet)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreShare)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostShare)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreUndoLock)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostUndoLock)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreUnlock)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostUnlock)
                )
        {
          string[] parentFolders = new string[ppoData.Length];
          string[] fileNames = new string[ppoData.Length];
          for (int i = 0; i < ppoData.Length; i++)
          {
            parentFolders[i] = System.IO.Path.GetDirectoryName(ppoData[i].mbsStrData1).Substring(ppoData[i].mbsStrData1.IndexOf(vault5.Name, StringComparison.OrdinalIgnoreCase)).TrimStart('\\').TrimEnd('\\');
            fileNames[i] = System.IO.Path.GetFileName(ppoData[i].mbsStrData1);
          }
          CheckExecutionConditionForHook(ref poCmd, ref hInfo, hookName, ref executeHook, ref parentFolders, ref fileNames);
          if (poCmd.mbCancel != 0)
          {
            // Error code and message are stored in poCmd from caller
            return;
          }
        }
        else if (hookName == nameof(EdmCmdType.EdmCmd_PreMove)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostMove)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreMoveFolder)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostMoveFolder)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreRename)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostRename)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreRenameFolder)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostRenameFolder)
                )
        {

          string[] parentFolders = new string[ppoData.Length];
          string[] fileNames = new string[ppoData.Length];

          for (int i = 0; i < ppoData.Length; i++)
          {
            parentFolders[i] = System.IO.Path.GetDirectoryName(ppoData[i].mbsStrData2).Substring(ppoData[i].mbsStrData2.IndexOf(vault5.Name, StringComparison.OrdinalIgnoreCase)).TrimStart('\\').TrimEnd('\\');
            fileNames[i] = System.IO.Path.GetFileName(ppoData[i].mbsStrData2);
          }
          CheckExecutionConditionForHook(ref poCmd, ref hInfo, hookName, ref executeHook, ref parentFolders, ref fileNames);
          if (poCmd.mbCancel != 0)
          {
            // Error code and message are stored in poCmd from caller
            return;
          }
        }
        else if (hookName == nameof(EdmCmdType.EdmCmd_PostAdd)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostState)
                )
        {

          string[] sourceStates = new string[ppoData.Length];
          string[] fileNames = new string[ppoData.Length];

          for (int i = 0; i < ppoData.Length; i++)
          {
            sourceStates[i] = ppoData[i].mlLongData1.ToString();
            fileNames[i] = ppoData[i].mbsStrData1;
          }
          CheckExecutionConditionForHook(ref poCmd, ref hInfo, hookName, ref executeHook, ref sourceStates, ref fileNames);
          if (poCmd.mbCancel != 0)
          {
            // Error code and message are stored in poCmd from caller
            return;
          }
        }
        else if (hookName == nameof(EdmCmdType.EdmCmd_PreLabel)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostLabel)
                )
        {

          string[] labels = new string[ppoData.Length];
          string[] fileNames = new string[ppoData.Length];

          for (int i = 0; i < ppoData.Length; i++)
          {
            labels[i] = ppoData[i].mbsStrData1;
            fileNames[i] = ppoData[i].mbsStrData2;
          }
          CheckExecutionConditionForHook(ref poCmd, ref hInfo, hookName, ref executeHook, ref labels, ref fileNames);
          if (poCmd.mbCancel != 0)
          {
            // Error code and message are stored in poCmd from caller
            return;
          }
        }
        else if (hookName == nameof(EdmCmdType.EdmCmd_PreLabelDelete)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostLabelDelete)
                 || hookName == nameof(EdmCmdType.EdmCmd_PreLabelModify)
                 || hookName == nameof(EdmCmdType.EdmCmd_PostLabelModify)
                )
        {
          string[] labels = new string[ppoData.Length];
          string[] comments = new string[ppoData.Length];

          for (int i = 0; i < ppoData.Length; i++)
          {
            labels[i] = ppoData[i].mbsStrData1;
            comments[i] = poCmd.mbsComment;
          }
          CheckExecutionConditionForHook(ref poCmd, ref hInfo, hookName, ref executeHook, ref labels, ref comments);
          if (poCmd.mbCancel != 0)
          {
            // Error code and message are stored in poCmd from caller
            return;
          }

          #region <Only for debug mode>
          if (DebugMode)
          {
            currentTime = DateTime.Now;
            TimeSpan duration = currentTime - savedTime;
            savedTime = DateTime.Now;
            debugInfo.Append(@"EPDM - Time needed to check the execute conditions for hooks: " + duration.Milliseconds.ToString() + @" milliseconds" + Environment.NewLine);
          }
          #endregion </Only for debug mode>

        }
        #endregion </Check execute conditions for hooks>

        #region <Execute hook when conditions are met>
        if (executeHook)
        {
          string hookInfoFile = System.IO.Path.GetTempFileName();

          #region <Getting the file list from EPDM>
          List<string> fileNames = new List<string>();
          GetFileListFromPpoData(ref poCmd, ref ppoData, ref fileNames, mbsStrData);
          if (poCmd.mbCancel != 0)
          {
            return;
          }

          #region <Only for debug mode>
          if (DebugMode)
          {
            currentTime = DateTime.Now;
            TimeSpan duration = currentTime - savedTime;
            savedTime = DateTime.Now;
            debugInfo.Append(@"EPDM - Time needed to get the file list: " + duration.Milliseconds.ToString() + @" milliseconds" + Environment.NewLine);
          }
          #endregion </Only for debug mode>

          #endregion <Getting the file list from EPDM>

          #region <Getting and saving system information for all files>

          List<Structs.EdmSystemInformation> sysInfos = GetSysteInformationForScript(ref vault5, ref poCmd, ref ppoData, fileNames, getFileInfo, hookName);
          if (poCmd.mbCancel != 0)
          {
            // Error code and message are stored in poCmd from caller
            return;
          }
          #region <Only for debug mode>
          if (DebugMode)
          {
            currentTime = DateTime.Now;
            TimeSpan duration = currentTime - savedTime;
            savedTime = DateTime.Now;
            debugInfo.Append(@"EPDM - Time needed to get system information: " + duration.Milliseconds.ToString() + @" milliseconds" + Environment.NewLine);
          }
          #endregion </Only for debug mode>

          SaveSystemProperties(ref poCmd, ref sysInfos, fileNames, ref hookInfoFile, hookName);
          if (poCmd.mbCancel != 0)
          {
            // Error code and message are stored in poCmd from caller
            return;
          }
          #region <Only for debug mode>
          if (DebugMode)
          {
            currentTime = DateTime.Now;
            TimeSpan duration = currentTime - savedTime;
            savedTime = DateTime.Now;
            debugInfo.Append(@"EPDM - Time needed to save system information: " + duration.Milliseconds.ToString() + @" milliseconds" + Environment.NewLine);
          }
          #endregion </Only for debug mode>

          #endregion </Getting and saving system information for all files>

          #region <Start the script>
          RunScript(ref poCmd, hInfo.script, ref hookInfoFile, hInfo.waitUntilExit, hInfo.timeout, hInfo.hideScriptWindow);
          if (executeHook)
          {
            hookExecuted = true;
          }
          #endregion </Start the script>

        }
        #endregion </Execute hook when conditions are met>
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
      }
    }

    private void CheckExecutionConditionForHook(ref EdmCmd poCmd, ref Structs.EdmCmdHooksInfo hInfo, string hookName, ref bool executeHook, ref string[] items01, ref string[] items02)
    {
      executeHook = false;
      try
      {
        if (items01.Length != items02.Length)
        {
          poCmd.mbCancel = 1;
          poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Section: " + hookName + Environment.NewLine + "Message: " + @"The number of directories and files to be compared may not be different.";
          return;
        }
        for (int i = 0; i < items01.Length; i++)
        {
          CheckExecutionConditionForHook(ref poCmd, ref hInfo, hookName, ref executeHook, items01[i], items02[i]);
          if (executeHook)
          {
            return;
          }

        }
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Section: " + hookName + Environment.NewLine + "Message: " + ex.Message;
      }
    }

    private void CheckExecutionConditionForHook(ref EdmCmd poCmd, ref Structs.EdmCmdHooksInfo hInfo, string hookName, ref bool executeHook, string item01, string item02)
    {
      executeHook = false;
      try
      {
        string[] conditionGroups = hInfo.executionCondition.Split(new[] { DelimiterSysExecutionCondition2 }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string condition in conditionGroups)
        {
          string regEx01 = condition.Substring(0, condition.IndexOf(DelimiterSysExecutionCondition1));
          string regEx02 = condition.Substring(condition.IndexOf(DelimiterSysExecutionCondition1) + DelimiterSysExecutionCondition1.Length);
          Match match = Regex.Match(item01, regEx01);
          if (match.Success)
          {
            match = Regex.Match(item02, regEx02);
            if (match.Success)
            {
              executeHook = true;
              break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Section: " + hookName + Environment.NewLine + "Message: " + ex.Message;
      }
    }

    private void RunScript(ref EdmCmd poCmd, string script, ref string hookInfoFile, int waitUntilExit, int timeout, int hideScriptWindow)
    {
      try
      {
        IntPtr hWndProcess = new IntPtr();
        Process pScript = new Process();
        ProcessStartInfo processStartInfo = new ProcessStartInfo();
        Process process = new Process();
        string pScriptCommand = string.Empty;
        string hookInfoFileDebugMode = hookInfoFile + Consts.debugInfoExtension;
        if (hideScriptWindow == 0)
        {
          process.StartInfo.CreateNoWindow = false;
          process.StartInfo.UseShellExecute = true;
        }
        else
        {
          process.StartInfo.CreateNoWindow = true;
          process.StartInfo.UseShellExecute = false;
        }
        string[] fileNameArguments = script.Replace(Consts.sysInfoFilePlaceHolder, hookInfoFile).Split(new[] { DelimiterSysScriptName }, StringSplitOptions.RemoveEmptyEntries);
        process.StartInfo.FileName = fileNameArguments[0].Trim();
        process.StartInfo.Arguments = fileNameArguments[1];
        try
        {
          #region <Only for debug mode>
          if (DebugMode)
          {
            #region <Stopwatch>
            currentTime = DateTime.Now;
            TimeSpan duration = currentTime - savedTime;
            savedTime = DateTime.Now;
            debugInfo.Append(@"EPDM - Time needed to start the script: " + duration.Milliseconds.ToString() + @" milliseconds" + Environment.NewLine);
            #endregion </Stopwatch>

            #region <Creating the file with debug information>
            try
            {
              if (hookInfoFile != string.Empty)
              {
                if (!System.IO.File.Exists(hookInfoFileDebugMode))
                {
                  debugInfo.Append(@"EPDM - Time when the shell was started.: " + DateTime.Now.ToString() + Environment.NewLine + @"EPDM - TickCount: " + System.Environment.TickCount.ToString());
                  System.IO.File.WriteAllText(hookInfoFileDebugMode, debugInfo.ToString());
                }
              }
              else
              {
                string msg = @"The file with the debug information cannot be created. The script will continue to be executed.";
                MessageBox.Show(msg, Consts.AddInVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
              }
            }
            catch (Exception ex)
            {
              string msg = @"The file with the debug information cannot be created. The script will continue to be executed." + Environment.NewLine + @"Error: " + ex.Message;
              MessageBox.Show(msg, Consts.AddInVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion </Creating the file with debug information>
          }
          #endregion </Only for debug mode>

          process.Start();
          hWndProcess = process.Handle;
          if (timeout == 0)
          {
            process.WaitForExit();
          }
          else
          {
            process.WaitForExit(Math.Abs(timeout) * 1000);
          }
          if (!process.HasExited)
          {
            poCmd.mbCancel = 1;
            poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "The script was not executed within the specified time of " + timeout + " sec. In case of a PreHooks, the operation is cancelled. In all other cases, you have to check the success of the operation manually." + Environment.NewLine + "Script: " + script.Replace(Consts.sysInfoFilePlaceHolder, hookInfoFile);
            try
            {
              process.CloseMainWindow();
            }
            catch (Exception ex)
            {
              poCmd.mbCancel = 1;
              poCmd.mbsComment = poCmd.mbsComment + Environment.NewLine + ex.Message;
            }
          }
          else
          {
            if (process.ExitCode != 0)
            {
              string msg = string.Empty;
              try
              {
                var _msg = System.IO.File.ReadAllLines(hookInfoFile, System.Text.Encoding.UTF8);
                msg = _msg[_msg.Length - 1].ToString();
                if (_msg[_msg.Length - 1].ToString().IndexOf(Consts.scriptErrorMsg) == 0)
                {
                  msg = msg.Substring(Consts.scriptErrorMsg.Length).Replace(Consts.sysInfoDelimiterNewLine, Environment.NewLine);
                }
                else
                {
                  msg = string.Empty;
                }
                if (!DebugMode)
                {
                  try
                  {
                    System.IO.File.Delete(hookInfoFile);
                  }
                  catch
                  {
                    // Nothing to do
                  }
                }
              }
              catch
              {
                msg = string.Empty;
              }
              if (msg == string.Empty)
              {
                poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + "The script returned the error number " + process.ExitCode + ". In case of a PreHooks, the operation is cancelled. In all other cases, you have to check the success of the operation manually." + Environment.NewLine + "Script: " + script.Replace(Consts.sysInfoFilePlaceHolder, hookInfoFile);
              }
              else
              {
                poCmd.mbsComment = msg;
              }
              poCmd.mbCancel = 1;
            }
          }
        }
        catch (Exception ex)
        {
          poCmd.mbCancel = 1;
          poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
        }

        #region <Only for debug mode>
        if (DebugMode)
        {
          if (System.IO.File.Exists(hookInfoFileDebugMode))
          {
            try
            {
              Process.Start(hookInfoFileDebugMode);
            }
            catch (Exception ex)
            {
              string msg = @"File '" + hookInfoFileDebugMode + @"' cannot be opened." + Environment.NewLine + ex.Message;
              MessageBox.Show(msg, Consts.AddInVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          }
          else
          {
            string msg = @"File '" + hookInfoFileDebugMode + @"' does not exist.";
            MessageBox.Show(msg, Consts.AddInVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
          currentTime = DateTime.Now;
          TimeSpan duration = currentTime - savedTime;
          debugInfo.Append(@"EPDM - Time needed to read the INI file: " + duration.Milliseconds.ToString() + @" milliseconds" + Environment.NewLine);
        }
        #endregion </Only for debug mode>

      }
      catch (Exception ex)
      {
        poCmd.mbCancel = 1;
        poCmd.mbsComment = @"Error in: " + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Message: " + ex.Message;
      }
    }

    #endregion </Helper>

    #region <Call the hooks>
    void EdmCmd_CardButton(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_CardInput(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_CardListSrc(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_Menu(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostAdd(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostAddFolder(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostCopy(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostCopyFolder(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostDelete(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostDeleteFolder(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostGet(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostLabel(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostLabelAddItem(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostLabelDelete(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostLabelModify(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostLock(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostMove(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostMoveFolder(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostRename(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostRenameFolder(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostShare(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostState(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostUndoLock(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PostUnlock(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreAdd(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreAddFolder(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreCopy(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreCopyFolder(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreDelete(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreDeleteFolder(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreGet(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreLabel(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreLabelAddItem(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreLabelDelete(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreLabelModify(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreLock(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreMove(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreMoveFolder(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreRename(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 2, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreRenameFolder(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, false, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreShare(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreState(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreUndoLock(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_PreUnlock(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      CollectDataAndRunScript(ref vault5, ref poCmd, ref ppoData, hooks, ref hInfo, 1, true, MethodBase.GetCurrentMethod().Name);
    }

    void EdmCmd_SerialNo(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      // Not supported
    }

    void EdmCmd_TaskDetails(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      // Not supported
    }

    void EdmCmd_TaskLaunch(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      // Not supported
    }

    void EdmCmd_TaskLaunchButton(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      // Not supported
    }

    void EdmCmd_TaskRun(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      // Not supported
    }

    void EdmCmd_TaskSetup(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      // Not supported
    }

    void EdmCmd_TaskSetupButton(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      // Not supported
    }

    void EdmCmd_InstallAddIn(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      // Not supported
    }

    void EdmCmd_UninstallAddIn(ref IEdmVault5 vault5, ref EdmCmd poCmd, ref EdmCmdData[] ppoData, Structs.EdmCmdHooks hooks, ref Structs.EdmCmdHooksInfo hInfo)
    {
      // Not supported
    }
    #endregion </Call the hooks>
  }
}
