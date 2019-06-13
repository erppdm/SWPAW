using System;
using System.Runtime.InteropServices;
using EPDM.Interop.epdm;
using System.Diagnostics;
using System.Windows.Forms;

[InterfaceType(ComInterfaceType.InterfaceIsDual)]
[Guid("31325F1C-84D9-4243-9C5D-476027DA24A3")]
[ComVisible(true)]
public interface ISWPAWHelper
{
	bool debug { get; set; }
	object[] UserGetLoggedInUserInfo(object vault);
	object VaultLogin(string vaultName);
}

[ClassInterface(ClassInterfaceType.None)]
[Guid("011EAC30-8512-43F6-B3D4-5F7986AB2733")]
[ProgId("SWPAWHelper.Helper")]
[ComVisible(true)]
public class Helper : ISWPAWHelper
{
	private bool _debug;
	public bool debug
	{
		get { return _debug; }
		set { _debug = value; }
	}

	public Helper()
	{
	}

	~Helper()
	{
	}

	public object[] UserGetLoggedInUserInfo(object _vault)
	{
		#region <Debug>
		if (debug)
		{
			DebugPauseToAttachProcess("SWPAWHelper.Helper: " + System.Reflection.MethodInfo.GetCurrentMethod().Name);
		}
		#endregion </Debug>

		IEdmVault13 vault = (IEdmVault13)_vault;
		object[] usrInfo = new object[7];
		try
		{
			if (vault == null)
			{
				usrInfo = null;
				return usrInfo;
			}
			object[] groups = null;
			object[] tmpInfo = new object[2];
			IEdmUserMgr9 usrMgr = vault.CreateUtility(EdmUtility.EdmUtil_UserMgr);
			IEdmUser10 user = (IEdmUser10)usrMgr.GetLoggedInUser();
			user.GetGroupMemberships(out groups);
			//
			tmpInfo = new object[2];
			tmpInfo[0] = @"Id"; ;
			tmpInfo[1] = user.ID;
			usrInfo[0] = tmpInfo;
			//
			tmpInfo = new object[2];
			tmpInfo[0] = @"Initials";
			tmpInfo[1] = user.Initials;
			usrInfo[1] = tmpInfo;
			//
			tmpInfo = new object[2];
			tmpInfo[0] = @"Name";
			tmpInfo[1] = user.Name;
			usrInfo[2] = tmpInfo;
			//
			tmpInfo = new object[2];
			tmpInfo[0] = @"Full name";
			tmpInfo[1] = user.FullName;
			usrInfo[3] = tmpInfo;
			//
			tmpInfo = new object[2];
			tmpInfo[0] = @"Email";
			tmpInfo[1] = user.Email;
			usrInfo[4] = tmpInfo;
			//
			tmpInfo = new object[2];
			tmpInfo[0] = @"Logged in";
			tmpInfo[1] = (user.IsLoggedIn == true ? 1.ToString() : 0.ToString()); ;
			usrInfo[5] = tmpInfo;
			//
			tmpInfo = new object[2];
			tmpInfo[0] = @"Users groups";
			object[] _groups = new object[0];
			for (int i = 0; i < groups.Length; i++)
			{
				IEdmUserGroup8 _group = (IEdmUserGroup8)groups[i];
				Array.Resize(ref _groups, _groups.Length + 1);
				_groups[_groups.Length - 1] = _group.Name;
			}
			tmpInfo[1] = _groups;
			usrInfo[6] = tmpInfo;
		}
		catch (COMException exp)
		{
			usrInfo = null;
			return usrInfo;
		}
		finally
		{
			if (vault != null)
			{
				Marshal.ReleaseComObject(vault);
				vault = null;
			}
		}
		return usrInfo;
	}

	public object VaultLogin(string vaultName)
	{
		#region <Debug>
		if (debug)
		{
			DebugPauseToAttachProcess("SWPAWHelper.Helper: " + System.Reflection.MethodInfo.GetCurrentMethod().Name);
		}
		#endregion </Debug>

		object _vault = System.Activator.CreateInstance(System.Type.GetTypeFromProgID("ConisioLib.EdmVault"));
		IEdmVault13 vault = (IEdmVault13)_vault;
		vault.LoginAuto(vaultName, 0);
		if (!vault.IsLoggedIn)
		{
			vault = null;
		}
		return vault;
	}

	private void DebugPauseToAttachProcess(string mName)
	{
		try
		{
			if (!Debugger.IsAttached)
			{
				MessageBox.Show("Attach debugger to process '" + Process.GetCurrentProcess().ProcessName + "' to debug '" + mName + "' before clicking OK.");
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}
}
