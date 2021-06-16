using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Windows.Forms;

namespace AnnealFileRecovery
{
    public partial class frmFileRecovery : Form
    {
        public frmFileRecovery()
        {
            InitializeComponent();
        }
        private string _root = @"C:\VitalData\";
        private void frmFileRecovery_Load(object sender, EventArgs e)
        {
            GetRoot();
            List<string> dirs = ListAllFolder(_root);
            if (dirs != null && dirs.Count > 0)
            {
                txtPath.Text = dirs[0];
                UpdateInfo(txtPath.Text);
                GetNumberOfAdmits(txtPath.Text);
            }
            else
            {
                //
            }
        }
        //Get root if AnnalRoot.txt exists
        private void GetRoot()
        {
            string fn = GetExePath() + "\\AnnealRoot.txt";
            if (File.Exists(fn))
            {
                string root = "";
                try
                {
                    using (FileStream fs = new FileStream(fn, FileMode.Open))
                    {
                        using (var sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            try
                            {
                                string line;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    root = line;
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.Print("file reading exception:" + ex);
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print("file reading exception:" + ex);
                    return;
                }
                if (!string.IsNullOrEmpty(root))
                    _root = root;
            }
        }
        private static string GetExePath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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
                    string fn = GetNumberOfAdmits(path);
                    //
                    //- explore and calculate anneal count
                    /*string admit = new DirectoryInfo(fn).Name;
                    int ct = CalculateAnnealCountByEvent(fn, admit);*/
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            //[Step 1]
            //case 1: file_1 anneal count > 1
            KeyValuePair<string, string> cmb = GetCmbSelectedKeyValue(cmbAdmit);
            string targetpath = ($"{cmb.Value}\\target.txt");
            List<string> files = GetAnnealedFilesFromTarget(targetpath);
            
            //[Step 2] Parse anneals before and after
            ParseAnneals($"{cmb.Value}", files);
        }
        private void ParseAnneals(string fn, List<string> annealList)
        {
            if (string.IsNullOrEmpty(fn) || annealList == null || annealList.Count <= 0) return;
            //[Step 0] Assume current files reflects final anneals
            string patnID = lblPatnID.Text;
            string admit = new DirectoryInfo(fn).Name;
            System.Diagnostics.Debug.Print("@ParseAnneals:{0} (admit folder path), patnID={1}, admit={2}",
                fn, patnID, admit);
            //
            AddLogMessage($"ParseAnneals(0): {patnID}_{admit}.");

            //[Step 1] backup whole folder first
            string fnBak = fn + "_bak";
            BackupFolder(fn, fnBak);
            //[Step 2] Anneal files: last annealed first
            for (int i = annealList.Count - 1; i >= 0; i--)
            {
                string fTarget = annealList[i];
                int ct = i + 1;
                ParseAnnealFolders(fn, admit, fTarget, ct);
            }
            AddLogMessage($"Completed!!");
        }
        private void ParseAnnealFolders(string fn, string admit, string target, int ct)
        {
            //[Step 1] check current folder and create before/after
            bool isLast = false;
            string fnPrvBefore = $"{fn}_{ct+1}_before";
            string fnCurBefore = $"{fn}_{ct}_before";
            isLast = (!Directory.Exists(fnPrvBefore));
            //
            string fnAfter = $"{fn}_{ct}_after";
            /*//create patn_n_after folder
            if(isLast)
                BackupFolder(fn, fnAfter);
            else
                BackupFolder(fnPrvBefore, fnAfter);*/
            //
            if (isLast)
                BackupFolder($"{fn}_bak", fnCurBefore);
            else
                BackupFolder(fnPrvBefore, fnCurBefore);
            //
            //[Step 2] recover annealed files
            if (Directory.Exists(fnCurBefore))
            {
                int ctAnneal = CalculateAnnealCountByPattern(target, fnCurBefore);
                //Remove origin (*.vts) files
                string patternDelete = $"{target}.vts.*";
                string[] filePaths = Directory.GetFiles(fnCurBefore, patternDelete);
                foreach (string fPath in filePaths)
                {
                    File.Delete(fPath);
                }
                //Rename (~{ctAnneal}vts*) to origin (*.vts*)
                string n = (ctAnneal == 0) ? ("") : (ctAnneal.ToString());
                string patternRename = $"{target}.~{n}vts*";
                string[] filesToRename = Directory.GetFiles(fnCurBefore, patternRename);
                foreach (string fPath in filesToRename)
                {
                    string ext = ".vts";
                    string ori = $".~{n}vts";
                    string fDest = fPath.Replace(ori, ext);
                    File.Copy(fPath, fDest);
                    File.Delete(fPath);
                }
            }
            //
            AddLogMessage($"ParseAnnealFolders: {admit}_{ct} recovered!");
        }
        private void BackupFolder(string oriFolder, string destFolder)
        {
            if (!Directory.Exists(oriFolder))
            {
                AddLogMessage($"BackupFolder(fail 0): {oriFolder} not exist!");
                return;
            }
            string fileName = "", destFile = "";
            Directory.CreateDirectory(destFolder);
            if (Directory.Exists(oriFolder))
            {
                string[] files = Directory.GetFiles(oriFolder);
                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = Path.GetFileName(s);
                    destFile = Path.Combine(destFolder, fileName);
                    File.Copy(s, destFile, true);
                }
            }
            else
            {
                AddLogMessage($"ParseAnneals(fail 1): {fileName} not exist!");
            }
            System.Diagnostics.Debug.Print($"ParseAnneals: {Path.GetDirectoryName(destFolder)} backup!");
            //AddLogMessage($"ParseAnneals: {destFolder} backup!");
        }
        private KeyValuePair<string, string> GetCmbSelectedKeyValue(ComboBox comboBox)
        {
            if (comboBox.SelectedIndex == -1)
                return new KeyValuePair<string, string>();
            KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)comboBox.Items[comboBox.SelectedIndex];
            return kvp;
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
        private string GetNumberOfAdmits(string path = null)
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
            if (lstAdmits.Count > 0)
                fn = lstAdmits[0];
            return fn;
        }
        private int CalculateAnnealCountByPattern(string pattern, string fn)
        {
            //[Check vts files only]
            int ct = 0;
            bool isFound = false;
            if (Directory.Exists(fn))
            {
                while (!isFound)
                {
                    string target = (ct == 0) ? ($"{pattern}.~vts") : ($"{pattern}.~{ct.ToString()}vts");
                    string[] files = Directory.GetFiles(fn, target);
                    if (files.Length > 0) ct++;
                    else isFound = true;
                }
            }
            return (ct-1);
        }
        private void AddAdmitsToComboBox(List<string> lst)
        {
            if (cmbAdmit == null) return;
            cmbAdmit.SelectedIndex = -1;
            cmbAdmit.Items.Clear();
            if (lst == null || lst.Count == 0)
            {
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
        private List<string> GetAnnealedFilesFromTarget(string fn)
        {
            List<string> lst = new List<string>();
            if (!File.Exists(fn))
            {
                AddLogMessage($"GetAnnealedFilesFromTarget: {fn} not exists.");
                return lst;
            }//
            try
            {
                using (FileStream fs = new FileStream(fn, FileMode.Open))
                {
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        try
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                lst.Add(line);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Print("file reading exception:" + ex);
                            AddLogMessage($"GetAnnealedFilesFromTarget (ex 1): {ex}.");
                            return lst;
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("file reading exception:" + ex);
                AddLogMessage($"GetAnnealedFilesFromTarget (ex 2): {ex}.");
                return lst;
            }
            AddLogMessage($"GetAnnealedFilesFromTarget: count={lst.Count}.");
            return lst;
        }
        //[Log]
        private void AddLogMessage(string str)
        {
            if (txtLog == null) return;
            txtLog.AppendText("- " + str + "\n");
        }
        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
