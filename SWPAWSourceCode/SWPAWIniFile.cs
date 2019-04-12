using System.Runtime.InteropServices;
using System.Text;

namespace SWPAW
{
	public class BiIIniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public BiIIniFile(string INIPath)
        {
            path = INIPath;
        }
        // Write data to file
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }
        // Read data from file
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 1024, this.path);
            return temp.ToString();
        }
    }
}



