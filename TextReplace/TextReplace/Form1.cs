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
using System.Diagnostics;

namespace TextReplace
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {   
            //Create new folder dialog
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            //Show the dialog to search for a folder and if the user has read/write access to that folder then,
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                //Set the text in TextBox1 to the path of the selected folder
                textBox1.Text = (Path.GetFullPath(fbd.SelectedPath)).ToString();
            }

            else
            {
                //If there is an error of some sort, just close the window and have them try again
                //I could put a try/catch here that would display the error, but the only reason this ever fails is because of permissions issues
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Set local string variables
            string dirPath = textBox1.Text;
            string strOld = textBox2.Text;
            string strNew = textBox3.Text;

            //Set local integer variables
            int successfulInt = 0;
            int failedInt = 0;

            //Create local Lists
            List<string> successfulList = new List<string>();
            List<string> failedList = new List<string>();

            //Populate information for the directory selected in Step 1
            DirectoryInfo di = new DirectoryInfo(dirPath);

            //Tells the DirectoryInfo instance (above) to get a list of the files from the selected folder then, for each file in the directory,
            foreach (FileInfo fi in di.GetFiles())
            {
                //Set its file path as a local variable
                string filePath = (Path.GetFullPath(fi.FullName)).ToString();

                try
                { 
                    //Read the file and load it into a local variable named "fileRead"
                    string fileRead = File.ReadAllText(filePath);
                    //Create a new local variable that holds the contents of fileRead after replacing the text
                    string fileTmp = fileRead.Replace(strOld, strNew);

                    //Overwrite the file with the new text
                    File.WriteAllText(filePath, fileTmp);

                    //Count how many files were successfully modified
                    successfulInt++;
                    //Add those successful files to a list of strings
                    successfulList.Add(DateTime.Now.ToShortTimeString() + ": Successfully replaced text in file - (" + filePath + ")");
                }

                catch (Exception ex)
                {
                    //Count how many files failed to be modified
                    failedInt++;
                    //Add the files that failed to a list of strings
                    failedList.Add(DateTime.Now.ToShortTimeString() + ": Failed to replace text in file - (" + filePath + ")" + Environment.NewLine + "Error Details: " + ex.Message + Environment.NewLine + Environment.NewLine);
                    //Even though the current file failed, we want to continue the operation until all files have been touched
                    continue;
                }
            }

            //Set local variable for user's desktop path
            string userDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //Set local variable for the path of the log file we're going to create
            string logPath = userDesktop + @"\TextReplaceLog-" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".log";
            //Combine all errors into a string
            string errorsToString = string.Join(Environment.NewLine, failedList.ToArray());
            //Combine all successes into a string
            string successesToString = string.Join(Environment.NewLine, successfulList.ToArray());
            //Set local variable for the text of the full log that we're creating
            string logStr = "======= ERRORS: " + failedInt.ToString() + " =======" + Environment.NewLine + errorsToString + Environment.NewLine + Environment.NewLine + "======= Processed Files: " + successfulInt.ToString() + " =======" + Environment.NewLine + successesToString;
            //Create the log file in the path from above with the full text ^
            File.WriteAllText(logPath, logStr);

            //Display a message to the user that the operation completed successfully, inform them of any errors along the way and show them the path where the log file was saved
            MessageBox.Show("Operation completed successfully with " + failedInt.ToString() + " errors." + Environment.NewLine + Environment.NewLine + "Log saved to: " + logPath, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //This could be changed to ask if they wanted to open the log file, like this:
            /**
             * if (MessageBox.Show("Operation completed successfully! Do you want to open the log file?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
             * {
             *      Process.Start(logPath);
             * }
             * 
             * else
             * {
             *      return;
             * }
             * */

            return;
        }
    }
}
