using System;
using System.Reflection;

namespace SWPAW
{
	class Consts
    {
        static Version _assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly string AddInVer = @"SolidWorks PDM Add-In Wrapper V" + _assemblyVersion.Major.ToString() + "." + _assemblyVersion.Minor.ToString() + "." + _assemblyVersion.Build.ToString() + "." + _assemblyVersion.Revision.ToString();
        public static readonly string AddInVerDebugMode = @"Debug Mode - " + AddInVer;
        public static readonly string debugInfoExtension = @".SWPAW.Debug.Info";
        public static readonly string scriptErrorMsg = "errorMsg=";
        public static readonly string environmentIniPath = @"SWPAW.ini";
        public static readonly string ArrayDelimiter = @";";
        public static readonly string sysInfoFilePlaceHolder = "@placeHolder@";
        public static readonly string sysSection = @"System";
        public static readonly string sysInfoDebugMode = @"DebugMode";
        public static readonly string sysInfoDelimiterNewLine = @"{newLine}";
        public static readonly string sectionDelimiter = @"Delimiter";
        public static readonly string sectionStartProcess = @"Start_Process";
        public static readonly string sysInfoPropertiesDelimiter = @"DelimiterSysProps";
        public static readonly string sysInfoIEdmStrLstDelimiter = @"DelimiterIEdmStrLst";
        public static readonly string sysInfoSysScriptNameDelimiter = @"DelimiterSysScriptName";
        public static readonly string sysInfoSysExecutionCondition1Delimiter = @"DelimiterSysExecutionCondition1";
        public static readonly string sysInfoSysExecutionCondition2Delimiter = @"DelimiterSysExecutionCondition2";
        public static readonly string sysInfoStartProcess = @"Start_Process";
        public static readonly string iniSectionPlaceHolder = @"@ph";
        public static readonly string scriptForHook = iniSectionPlaceHolder + @"Script";
        public static readonly string waitUntilExitForHook = iniSectionPlaceHolder + @"WaitUntilExit";
        public static readonly string timeoutForHook = iniSectionPlaceHolder + @"Timeout";
        public static readonly string hideScriptWindowForHook = iniSectionPlaceHolder + @"HideScriptWindow";
        public static readonly string executionConditionForHook = iniSectionPlaceHolder + @"ExecutionCondition";
        public static readonly string contextMenuForHook = iniSectionPlaceHolder + @"ContextMenu";
	}
}
