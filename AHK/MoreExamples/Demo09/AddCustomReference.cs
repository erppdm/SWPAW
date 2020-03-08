/*
More information about compiling and using can be found here:
https://github.com/erppdm/SWPAW/tree/master/SWPAWHelper
*/
using System;
using System.Runtime.InteropServices;
using EdmLib;
using System.Windows.Forms;
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
// Create a new GUID and replace this with it.  
//[Guid("00000000-0000-0000-0000-000000000000")]
[ComVisible(true)]
public interface ICustomRef {
	int Add(string fileName, string fFilter, string vName);
}
[ClassInterface(ClassInterfaceType.None)]
// Create a new GUID and replace this with it  
//[Guid("00000000-0000-0000-0000-000000000000")]
[ProgId("AddCustomReference.CustomRef")]
[ComVisible(true)]
public class CustomRef : ICustomRef {
	public int Add(string fileName, string fFilter, string vName) {
		int exitCode = 0;
		IEdmVault5 vault = (IEdmVault5)Activator.CreateInstance(System.Type.GetTypeFromProgID("Conisiolib.EdmVault"));
		LogIn(ref exitCode, ref vault, vName);
		if (exitCode != 0) {
			return exitCode;
		}
		string refFileName = string.Empty;
		GetFileName(ref refFileName, vault.RootFolderPath, fFilter, 0);
		if (refFileName == string.Empty) {
			exitCode = 11;
			return exitCode;
		}
		if (refFileName.ToLower() == fileName.ToLower()) {
			exitCode = 12;
			return exitCode;
		}
		IEdmEnumeratorCustomReference6 addCustRefs = default(IEdmEnumeratorCustomReference6);
		IEdmFolder5 rFolder = default(IEdmFolder5);
		IEdmFile5 rFile = default(IEdmFile5);
		IEdmFolder5 zFolder = default(IEdmFolder5);
		IEdmFile5 file = default(IEdmFile5);
		try {
			rFile = vault.GetFileFromPath(refFileName, out rFolder);
			file = vault.GetFileFromPath(fileName, out zFolder);
			if (rFile == null | file == null) {
				exitCode = 13;
				return exitCode;
			}
			if (!file.IsLocked) {
				exitCode = 14;
				return exitCode;
			}
			addCustRefs = (IEdmEnumeratorCustomReference6)file;
			try {
				addCustRefs.AddReference2(rFile.ID, rFolder.ID, 1);
			} catch (COMException e) {
				exitCode = 15;
				return exitCode;
			}
		} catch (COMException e) {
			exitCode = 16;
			return exitCode;
		} finally {
			if (rFolder != null) {
				Marshal.FinalReleaseComObject(rFolder);
				rFolder = null;
			}
			if (rFile != null) {
				Marshal.FinalReleaseComObject(rFile);
				rFile = null;
			}
			if (zFolder != null) {
				Marshal.FinalReleaseComObject(zFolder);
				zFolder = null;
			}
			if (file != null) {
				Marshal.FinalReleaseComObject(file);
				file = null;
			}
			if (addCustRefs != null) {
				Marshal.FinalReleaseComObject(addCustRefs);
				addCustRefs = null;
			}
			if (vault != null) {
				Marshal.FinalReleaseComObject(vault);
				vault = null;
			}
		}
		return exitCode;
	}
	private static void LogIn(ref int exitCode, ref IEdmVault5 vault, string vName) {
		if (vault == null) {
			exitCode = 10;
			return;
		}
		vault.LoginAuto(vName, 0);
		if (!vault.IsLoggedIn) {
			exitCode = 10;
			return;
		}
	}
	private static void GetFileName(ref string refFileName, string path, string filter, int version) {
		using (OpenFileDialog oFileDialog = new OpenFileDialog()) {
			oFileDialog.InitialDirectory = path;
			oFileDialog.Filter = filter;
			oFileDialog.FilterIndex = 2;
			oFileDialog.RestoreDirectory = true;
			if (oFileDialog.ShowDialog() == DialogResult.OK) {
				refFileName = oFileDialog.FileName;
			}
		}
	}
}
