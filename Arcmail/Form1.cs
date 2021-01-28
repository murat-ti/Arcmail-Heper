using System;
using System.Windows.Forms;
using Arcmail.Helpers;
using System.IO;

namespace Arcmail
{
    public partial class Form1 : Form
    {
        private AppConfigHelper appConfigHelper;
        private FileHelper fileHelper;
        private LogHelper logHelper;
        private FtpHelper ftpHelper;
        private ArcmailHelper arcmailHelper;
        private string tempDirIn;
        private string tempDirOut;
        
        public Form1()
        {
            InitializeComponent();

            //work with app config
            appConfigHelper = new AppConfigHelper();

            //work with files
            fileHelper = new FileHelper();
            //work with log
            logHelper = new LogHelper();
            //work with arcmail
            arcmailHelper = new ArcmailHelper();

            populateFtpCredentials();
            tempDirIn = appConfigHelper.readConfigKey("tempDirIn");
            tempDirOut = appConfigHelper.readConfigKey("tempDirOut");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void populateFtpCredentials()
        {
            string[] data = fileHelper.readFtpCredentials();

            if (data.Length > 0 )
            {
                textBoxHost.Text = data[0];
                textBoxUsername.Text = data[1];
                textBoxPassword.Text = data[2];
            }
        }

        private void clientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientList clientList = new ClientList();
            clientList.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            string[][] clientList = fileHelper.readClientsToArray();
            if (clientList.GetLength(0) > 0)
            {
                if (checkIfFtpSettingsEmpty())
                {
                    //ftp helper
                    ftpHelper = new FtpHelper(textBoxHost.Text, textBoxUsername.Text, textBoxPassword.Text);

                    if (ftpHelper.checkFtpConnection())
                    {
                        //IN OPERATIONS
                        //---------------------------------------------------------------------------
                        logHelper.showLog("In operation started:\n", null);
                        for (int i = 1; i < clientList.GetLength(0); i++)
                        {
                            //Console.Write(clientList[i][0] + "\n");
                            //third parameter flag, in this case it is IN
                            //Download all files from banks/in to local temp/in folder
                            ftpHelper.DownloadAllFromFtpDirectory(clientList[i][2], 0);

                            //moved this part to for loop, because I need to save log of each uploaded file
                            if (tempDirIn != "")
                            {
                                logHelper.showLog("Start ARCMAIL unarchive: ");
                                //return false if folder empty
                                if (arcmailHelper.unZipAllFilesInDir(tempDirIn))
                                {
                                    logHelper.showLog("Start RAR unarchive: ");
                                    fileHelper.unZipAllFilesInDir(tempDirIn);
                                    logHelper.showLog("Start upload to FTP: " + clientList[0][2]);
                                    ftpHelper.uploadAllFromDirectoryToFtp(tempDirIn, clientList[0][2], clientList[0][1]);
                                }
                            }
                        }

                        //OUT OPERATIONS
                        //---------------------------------------------------------------------------
                        logHelper.showLog("\nOut operation started:", null);
                        string rarFileNamePath = "";
                        string lckFileNamePath = "";
                        string tempDir = "";
                        //look for bank files in title with bank codes

                        //Download all files from kartadmin/out to local temp/out folder
                        ftpHelper.DownloadAllFromFtpDirectory(clientList[0][3], 1);

                        for (int i = 1; i < clientList.GetLength(0); i++)
                        {
                            //some banks have different code number of processing center and bank code
                            fileHelper.searchAllBankFilesInDirByBankCode(clientList[i][1], clientList[i][4]);
                            tempDir = Path.Combine(tempDirOut, "..") + "\\" + clientList[i][1];

                            //check if temp folder have files
                            if (Directory.Exists(tempDir)) {
                                //fileHelper.zip(@"C:\Users\AT\Desktop\test\zipfiles", @"C:\Users\AT\Desktop\test\files.rar");
                                //all files for zip, I put in separate folder with bank code name
                                string fileTitle = fileHelper.getFreeTitle(clientList[i][1], "out");
                                if (fileTitle != "")
                                {
                                    rarFileNamePath = tempDirOut + "\\" + fileTitle; // clientList[i][1] + "_" + DateTime.Now.ToString("ddMMhhmm") + ".rar";

                                    if (fileHelper.zip(tempDir, rarFileNamePath))
                                    {
                                        fileHelper.deleteAllFilesAndSubdirs(Path.Combine(tempDirOut, "..") + "\\" + clientList[i][1]);
                                        //encript with arcmail
                                        lckFileNamePath = arcmailHelper.zipRarFileInDir(rarFileNamePath, clientList[i][1]);
                                        //upload to ftp
                                        if (lckFileNamePath != "" && Path.GetExtension(lckFileNamePath) == ".lck")
                                        {
                                            logHelper.showLog("Arcmail archived file:" + lckFileNamePath, null);
                                            if (ftpHelper.uploadByBankCodeFromDirectoryToFtp(lckFileNamePath, clientList[i][3]))
                                            {
                                                //move original files to archive 
                                                fileHelper.makeArchive(rarFileNamePath, "out");
                                                fileHelper.makeArchive(lckFileNamePath, "out");
                                            }
                                        }
                                        else
                                            logHelper.showLog("Arcmail archive problem in file:" + rarFileNamePath + "\tLck result: " + lckFileNamePath, null);

                                    }
                                }
                                else
                                    logHelper.showLog("Archive/out folder is full (1000-9999). Please delete some files", null);
                            }
                            //else
                                //logHelper.showLog(tempDir+" directory not exists", null);
                        }
                        //MessageBox.Show("The operation completed");
                    }
                    else
                    {
                        logHelper.showLog("Please check your FTP connection!", null);
                    }
                }
                else
                    logHelper.showLog("Please fill all fields in FTP Settings", null);
            }
            else
            {
                MessageBox.Show("The clients file not found!");
            }

            button1.Enabled = true;
        }

        private bool checkIfFtpSettingsEmpty()
        {
            if (String.IsNullOrEmpty(textBoxHost.Text) || String.IsNullOrEmpty(textBoxUsername.Text) || String.IsNullOrEmpty(textBoxPassword.Text))
                return false;
            else
                return true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }
    }
}
