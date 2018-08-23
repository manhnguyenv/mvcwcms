using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace MVCwCMS.Models
{
    //Reference:
    //http://browscap.org/
    //https://github.com/browscap/browscap/wiki/Using-Browscap
    //http://www.gocher.me/C-Sharp-Browscap

    public class BrowsCapProvider
    {
        [DllImport("kernel32")]
        static extern int GetPrivateProfileSectionNames([MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileSection(string Section, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);
        // inserted for prevent matching of group entries https://github.com/browscap/browscap/issues/3
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder Result, int Size, string FileName);

        private string _iniPath;
        private string[] _sectionNames;

        private static int CompareByLength(string x, string y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (y == null)
                {
                    return -1;
                }
                else
                {
                    int retval = x.Length.CompareTo(y.Length);
                    if (retval != 0)
                    {
                        return -retval;
                    }
                    else
                    {
                        return -x.CompareTo(y);
                    }
                }
            }
        }

        private string[] GetSectionNames()
        {
            for (int maxsize = 500; true; maxsize *= 2)
            {
                byte[] bytes = new byte[maxsize];
                int size = GetPrivateProfileSectionNames(bytes, maxsize, _iniPath);
                if (size < maxsize - 2)
                {
                    string Selected = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                    return Selected.Split(new char[] { '\0' });
                }
            }
        }

        private string FindMatch(string userAgent)
        {
            if (_sectionNames != null)
            {
                foreach (string SecHead in _sectionNames)
                {
                    try
                    {
                        // following line changed for https://github.com/browscap/browscap/issues/438
                        if ((SecHead.IndexOf("*", 0) == -1) && (SecHead.IndexOf("?", 0) == -1) && (SecHead == userAgent))
                        {   
                            // inserted for prevent matching of group entries https://github.com/browscap/browscap/issues/3
                            if (IniReadValue(SecHead, "parent") != "DefaultProperties")
                            {
                                return SecHead;
                            }
                        }

                        // following line changed for https://github.com/browscap/browscap/issues/438
                        if ((SecHead.IndexOf("*", 0) > -1) || (SecHead.IndexOf("?", 0) > -1))
                        {
                            if (Regex.IsMatch(userAgent, "^" + Regex.Escape(SecHead).Replace(@"\*", ".*").Replace(@"\?", ".") + "$"))
                            {
                                return SecHead;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
                return "*";
            }
            return "";
        }

        public BrowsCapProvider(string iniPath)
        {
            _iniPath = iniPath;
            _sectionNames = GetSectionNames();
            Array.Sort(_sectionNames, CompareByLength);
        }

        // inserted for prevent matching of group entries https://github.com/browscap/browscap/issues/3
        public string IniReadValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", temp, 255, _iniPath);
            return temp.ToString();
        }

        public NameValueCollection GetValues(string userAgent)
        {
            string match = FindMatch(userAgent);

            NameValueCollection col = new NameValueCollection();
            do
            {
                string[] entries = new string[0];
                bool goon = true;
                for (int maxsize = 500; goon; maxsize *= 2)
                {
                    byte[] bytes = new byte[maxsize];
                    int size = GetPrivateProfileSection(match, bytes, maxsize, _iniPath);
                    if (size < maxsize - 2)
                    {
                        string section = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                        entries = section.Split(new char[] { '\0' });
                        goon = false;
                    }
                }
                match = "";
                if (entries.Length > 0)
                {
                    foreach (string entry in entries)
                    {
                        string[] ent = entry.Split(new char[] { '=' });
                        if (ent[0] == "Parent")
                        {
                            match = ent[1];
                        }
                        else if (col[ent[0]] == null)
                        {
                            col.Add(ent[0], ent[1]);
                        }
                    }
                }
            } while (match != "");

            return col;
        }
    }
}