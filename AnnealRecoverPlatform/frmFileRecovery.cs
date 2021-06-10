using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AnnealFileRecovery
{
    public partial class frmFileRecovery : Form
    {
        public frmFileRecovery()
        {
            InitializeComponent();
        }
        private string _root = @"C:\VitalData\[VT-Trace]\";
        private void frmFileRecovery_Load(object sender, EventArgs e)
        {
            List<string> dirs = ListAllFolder(_root);
            if (dirs != null && dirs.Count > 0)
            {
                txtPath.Text = dirs[0];
                UpdateInfo(txtPath.Text);
                GetNumberOfAnneals(txtPath.Text);
            }
            else
            {
                //
            }

        }
        private List<string> ListAllFolder(string root)
        {
            List<string> files = new List<string>();
            foreach (var subDir in Directory.EnumerateDirectories(root))
            {
                try
                {
                    files.Add(subDir);
                }
                catch (UnauthorizedAccessException ex)
                {
                    // ...
                }
            }
            return files;
        }
        private void btnFolderPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog loadFolderDlg = new FolderBrowserDialog();
            loadFolderDlg.SelectedPath = _root;
            if (loadFolderDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string path = loadFolderDlg.SelectedPath;
                    UpdateInfo(path);
                    string fn = GetNumberOfAnneals(path);
                    //
                    //- explore and calculate anneal count
                    string admit = new DirectoryInfo(fn).Name;
                    CalculateAnnealCount(fn, admit);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void btnRun_Click(object sender, EventArgs e)
        {




        }
        private void UpdateInfo(string path)
        {
            txtPath.Text = path;
            string dir = Path.GetDirectoryName(path) + "\\";
            string patnID = Path.GetFileName(path);
            lblPatnID.Text = patnID;
            System.Diagnostics.Debug.Print("txtPath.Text={0} => dir={1}, patnID={2}", txtPath.Text, dir, patnID);
        }
        //
        private string GetNumberOfAnneals(string path = null)
        {
            string fn = string.Empty;
            if (string.IsNullOrEmpty(path)) return fn;
            //[List admits]
            List<string> lst = new List<string>();
            if (Directory.Exists(path))
                lst = ListAllFolder(path);
            //
            string rx =
                @"^(19[5-9][0-9]|20[0-4][0-9]|2050)(0?[1-9]|1[0-2])(0?[1-9]|[12][0-9]|3[01])(([0-1]?[0-9])|(2[0-3]))([0-5][0-9])([0-5][0-9])$";
            Regex rgx = new Regex(rx);
            List<string> lstAdmits = new List<string>();
            for (int i = 0; i < lst.Count; i++)
            {
                string admitTag = new DirectoryInfo(lst[i]).Name;
                if (rgx.IsMatch(admitTag))
                {
                    System.Diagnostics.Debug.Print($"[{i}]: {admitTag}");
                    lstAdmits.Add(lst[i]);
                }
            }
            if (lstAdmits == null) return fn;

            //[Select one admit]
            AddAdmitsToComboBox(lstAdmits);
            //TODO: to select: cmbAdmit select item changed
            //if lstAdmits.Count > 1 => choose one to explore

            fn = lstAdmits[0];
            return fn;
        }
        private void CalculateAnnealCount(string path, string admit)
        {
            //[Check event files and find anneal count]
            string patnID = Path.GetFileName(path);
            string fn = path;
            string evtOri = $"{fn}+\\{patnID}_{admit}.event";
            //
            int n = 1;
            bool exists = (File.Exists(evtOri));
            /*while (!exists)
            {
                string evt = $"{fn}+\\{patnID}_{admit}.~{n++}event";
                if (!File.Exists(evt))
                {
                    exists = false;
                }
            }*/
            System.Diagnostics.Debug.Print("CalculateAnnealCount={0}",n);

        }
        private void AddAdmitsToComboBox(List<string> lst)
        {
            if (cmbAdmit == null) return;
            if (lst == null || lst.Count == 0)
            {
                cmbAdmit.SelectedIndex = -1;
                cmbAdmit.Items.Clear();
                return;
            }
            //
            foreach (var v in lst)
            {
                string admitTag = new DirectoryInfo(v).Name;
                string ts = ToString(admitTag, DT_PATTERN_DISPLAY);
                cmbAdmit.Items.Add(new KeyValuePair<string, string>(ts, v));
            }
            cmbAdmit.DisplayMember = "key";
            cmbAdmit.ValueMember = "value";
            cmbAdmit.SelectedIndex = 0;
        }
        //[DateTime region]
        private const string DT_PATTERN_NOBLANK = "yyyyMMddHHmmss";
        private const string DT_PATTERN_DISPLAY = "yyyy/MM/dd HH:mm:ss";
        private const string RGX_DT_DISPLAY = @"^[1-2]{1}\d{3}\/\d{1,2}\/[0-3]{1}\d{1}\s+\d{2}:\d{2}:\d{2}$";
        private const string RGX_DT_NOBLANK = @"^[1-2]{1}\d{13}$";
        public static string ToString(string src, string pattern = DT_PATTERN_DISPLAY)
        {
            if (Regex.IsMatch(src, RGX_DT_DISPLAY, RegexOptions.None))
            {
                string dest;
                try
                {
                    dest = DateTime.Parse(src).ToString(pattern);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print("TimeUtil Exception={0}", ex);
                    throw;
                }
                return dest;
            }
            if (Regex.IsMatch(src, RGX_DT_NOBLANK, RegexOptions.None))
            {
                return ToDateTime(src, DT_PATTERN_NOBLANK).ToString(pattern);
            }
            return "";
        }
        private static DateTime ToDateTime(string str, string pattern = DT_PATTERN_NOBLANK)
        {
            DateTime dt;
            DateTime.TryParseExact(str, pattern, null, System.Globalization.DateTimeStyles.None, out dt);
            return dt;
        }
        //
    }
}
