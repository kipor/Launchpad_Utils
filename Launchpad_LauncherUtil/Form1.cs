using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Security.Cryptography;

namespace Launchpad.LauncherUtil
{
    public partial class Form1 : Form
    {
        bool bIsGeneratingManifest = false;

        public Form1()
        {
            InitializeComponent();
        }

        //function that takes a file stream, then computes the MD5 hash for the stream
        private string GetFileHash(Stream fileStream)
        {

            try
            {
                using (var md5 = MD5.Create())
                {
                    //we got a valid file, calculate the MD5 and return it
                    var resultString = BitConverter.ToString(md5.ComputeHash(fileStream)).Replace("-", "");

                    //release the file
                    fileStream.Close();
                    return resultString;
                }
            }
            catch (IOException)
            {
                //release the file (if we had one)
                fileStream.Close();
                return "ERROR - IOException";
            }

        }

        private void generateManifest_button_Click(object sender, EventArgs e)
        {
            if ( bIsGeneratingManifest == false)
            {
                DialogResult passresult = folderBrowserDialog1.ShowDialog();
                backgroundWorker_manifestGenerator.RunWorkerAsync(passresult);
            }            
        }

        private void generateSingle_button_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();

            if (File.Exists(openFileDialog1.FileName))
            {
                Stream fileStream = File.OpenRead(openFileDialog1.FileName);

                md5Result_Textbox.Text = GetFileHash(fileStream);
                fileStream.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void backgroundWorker_manifestGenerator_DoWork(object sender, DoWorkEventArgs e)
        {
            bIsGeneratingManifest = true;
            DialogResult result = (DialogResult)e.Argument;

            if (result == DialogResult.OK)
            {
				string manifestPath = String.Format(@"{0}{1}LauncherManifest.txt", Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar);

                if (File.Exists(manifestPath))
                {
                    //create a new empty file and close it (effectively deleting the old manifest)
                    File.Create(manifestPath).Close();
                }

                TextWriter tw = new StreamWriter(manifestPath);

                string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*", SearchOption.AllDirectories);                
                int completedFiles = 0;
                
                IEnumerable<string> enumeratedFiles = Directory
                                                    .EnumerateFiles(folderBrowserDialog1.SelectedPath, "*", SearchOption.AllDirectories);

                foreach (string value in enumeratedFiles)
                {
                    if (value != null)
                    {
                        FileStream fileStream = File.OpenRead(value);
                        var skipDirectory = folderBrowserDialog1.SelectedPath;

                        int fileAmount = files.Length; 
                        string currentFile = value.Substring(skipDirectory.Length);
                        string fileSize = fileStream.Length.ToString();

                        //GetFileHash is Stream Safe and closes the file on its own
                        string manifestLine = String.Format(@"{0}:{1}:{2}", value.Substring(skipDirectory.Length), GetFileHash(fileStream), fileSize);

                        completedFiles++;
                        

                        tw.WriteLine(manifestLine);
                        backgroundWorker_manifestGenerator.ReportProgress(completedFiles, new Tuple<int, int, string>(fileAmount, completedFiles, currentFile));
                    }
                }
                tw.Close();
            }
        }

        private void backgroundWorker_manifestGenerator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Tuple<int, int, string> state = (Tuple<int, int, string>)e.UserState;

            utilTools_progressBar.Maximum = state.Item1;

            fileProgress_label.Text = String.Format(@"{0}/{1}", state.Item2, state.Item1);
            fileProgress_label.Refresh();

            currentFile_label.Text = state.Item3;
            currentFile_label.Refresh();

            utilTools_progressBar.Increment(1);
        }

        private void backgroundWorker_manifestGenerator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bIsGeneratingManifest = false;
        }
    }
}
