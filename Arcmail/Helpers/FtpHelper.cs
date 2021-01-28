using Arcmail.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Arcmail
{
    class FtpHelper
    {
        private string host;
        private string user;
        private string pass;
        private string ftpSubDir = "";
        private string tempDirIn = "";
        private string tempDirOut = "";
        private AppConfigHelper appConfigHelper;
        private LogHelper logHelper;
        private string[] allowedExtensions = { ".rar", ".RAR" };
        public FtpHelper(string host, string user, string pass)
        {
            this.host = host.Trim();
            this.user = user.Trim();
            this.pass = pass.Trim();

            appConfigHelper = new AppConfigHelper();
            logHelper = new LogHelper();

            this.tempDirIn = appConfigHelper.readConfigKey("tempDirIn");
            this.tempDirOut = appConfigHelper.readConfigKey("tempDirOut");
            this.ftpSubDir = appConfigHelper.readConfigKey("ftpSubDir");
        }
        public bool checkFtpConnection()
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest) FtpWebRequest.Create(host);
                request.Credentials = new NetworkCredential(user, pass);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.UsePassive = true;
                request.UseBinary = false;
                request.KeepAlive = false; //close the connection when done
                //request.Timeout = 1000;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //Console.WriteLine("#############################Move status: {0}", response.StatusDescription);
                response.Close();
            }
            catch (WebException ex)
            {
                //logHelper.showLog("Test ftp connection is failed", ex);
                return false;
            }

            //logHelper.showLog("Test ftp connection is successfully", null);
            return true;
        }

        public void DownloadAllFromFtpDirectory(string source, int type)
        {
            string resultHost = host + ftpSubDir;
            logHelper.showLog("\nDownload all from: " + resultHost + source, null);
            string tempDir;

            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(resultHost + source);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            listRequest.Credentials = new NetworkCredential(user, pass); ;

            List<string> lines = new List<string>();
            string tempFile;

            try
            {
                using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
                using (Stream listStream = listResponse.GetResponseStream())
                using (StreamReader listReader = new StreamReader(listStream))
                {
                    while (!listReader.EndOfStream)
                    {
                        tempFile = listReader.ReadLine();
                        if (tempFile != "." && tempFile != "..")//&& allowedExtensions.Any(Path.GetExtension(tempFile).Contains)
                        {
                            //Console.Write(Path.GetExtension(tempFile) + "\n");
                            tempFile = Path.GetFileName(tempFile);
                            //logHelper.showLog("File: " + tempFile, null);
                            lines.Add(tempFile);
                        }
                    }
                }

                if (type == 0)
                    tempDir = tempDirIn;
                else
                    tempDir = tempDirOut;

                //create temp dir, if not exists
                if (!Directory.Exists(tempDirIn))
                    Directory.CreateDirectory(tempDirIn);
                if (!Directory.Exists(tempDirOut))
                    Directory.CreateDirectory(tempDirOut);

                foreach (string line in lines)
                {
                    if (DownloadFile(host, ftpSubDir + source + "/" + line, user, pass, tempDir))
                    {                        
                        logHelper.showLog("Successfully downloaded: " + line, null);
                        //delete file in ftp
                        if (deleteFile(resultHost + source + "/" + line))
                            logHelper.showLog("Ftp file successfully deleted: " + line, null);
                    }
                }
            }
            catch (WebException ex)
            {
                Console.Write(ex.Message + "\n");
            }
        }

        public void uploadAllFromDirectoryToFtp(string strPath, string ftpDest, string code)
        {
            string[] files = Directory.GetFiles(strPath, "*");
            foreach (string file in files)
            {
                //Console.Write(Path.GetDirectoryName(file) + ": " + file + "\n");
                if (UploadFile(host, file, user, pass, ftpSubDir + ftpDest + "/"))
                {
                    logHelper.fileLog(Path.GetFileName(file), code);
                    File.Delete(file);
                }
            }

            string[] directories = Directory.GetDirectories(strPath);
            foreach (string directory in directories)
            {
                // iterate subdirectories
                uploadAllFromDirectoryToFtp(directory, ftpDest, code);
            }
        }

        public bool uploadByBankCodeFromDirectoryToFtp(string strPath, string ftpDest)
        {
            logHelper.showLog("Upload by bank code: " + strPath);
            if (File.Exists(strPath) && UploadFile(host, strPath, user, pass, ftpSubDir + ftpDest + "/"))
                return true;
            else
            {
                logHelper.showLog("Can not upload to ftp file: " + strPath);
                return false;
            }
        }

        // Upload File to Specified FTP Url with username and password and Upload Directory 
        //if need to upload in sub folders /// 
        //Base FtpUrl of FTP Server
        //Local Filename to Upload
        //Username of FTP Server
        //Password of FTP Server
        //[Optional]Specify sub Folder if any
        //Status String from Server
        public bool UploadFile(string FtpUrl, string fileName, string userName, string password, string
        UploadDirectory = "")
        {
            bool status = false;
            string PureFileName = Path.GetFileName(fileName);
            String uploadUrl = String.Format("{0}{1}{2}", FtpUrl, UploadDirectory, PureFileName);
            logHelper.showLog("Upload ftp path: " + uploadUrl);
            FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
            //req.Proxy = null;

            try
            {
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Credentials = new NetworkCredential(userName, password);
                req.UseBinary = true;
                req.UsePassive = true;
                byte[] data = File.ReadAllBytes(fileName);
                req.ContentLength = data.Length;
                Stream stream = req.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                FtpWebResponse res = (FtpWebResponse)req.GetResponse();
                res.Close();
                status = true;
            }
            catch (Exception e)
            {
                logHelper.showLog("Can not upload file: " + fileName + " from FTP\n", e);
            }
            return status;
        }



        //Download File From FTP Server 
        //Base url of FTP Server
        //if file is in root then write FileName Only if is in use like "subdir1/subdir2/filename.ext"
        //Username of FTP Server
        //Password of FTP Server
        //Folderpath where you want to Download the File
        // Status String from Server
        public bool DownloadFile(string FtpUrl, string FileNameToDownload,
                            string userName, string password, string tempDirPath)
        {
            //logHelper.showLog("DownloadFile geldi: " + FileNameToDownload + "\n", null);
            bool status = false;
            //string ResponseDescription = "";
            string PureFileName = Path.GetFileName(FileNameToDownload);
            string DownloadedFilePath = tempDirPath + PureFileName;
            //logHelper.showLog("Download path: " + DownloadedFilePath + "\n", null);
            string downloadUrl = String.Format("{0}/{1}", FtpUrl, FileNameToDownload);
            FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(downloadUrl);
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.Credentials = new NetworkCredential(userName, password);
            req.UseBinary = true;
            //logHelper.showLog("DownloadFile: " + downloadUrl + "\n", null);
            //req.Proxy = null;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[2048];
                FileStream fs = new FileStream(DownloadedFilePath, FileMode.Create);
                int ReadCount = stream.Read(buffer, 0, buffer.Length);
                while (ReadCount > 0)
                {
                    fs.Write(buffer, 0, ReadCount);
                    ReadCount = stream.Read(buffer, 0, buffer.Length);
                }
                //ResponseDescription = response.StatusDescription;
                fs.Close();
                stream.Close();
                status = true;
            }
            catch (Exception e)
            {
                //Console.Write("FtpUrl: " + downloadUrl + "\nFileNameToDownload: " + FileNameToDownload + "\ntempDirPath: " + tempDirPath + "\n");
                //Console.WriteLine(e.Message);
                logHelper.showLog("Can not download file: "+ FileNameToDownload + " from FTP\n", null);
            }
            return status;
        }

        private bool deleteFile(string source)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(source);
                request.Credentials = new NetworkCredential(user, pass);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                //request.Timeout = 1000;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (WebException ex)
            {
                logHelper.showLog("Can not delete ftp file: "+ source, ex);
                return false;
            }

            //logHelper.showLog("Test ftp connection is successfully", null);
            return true;
        }

        /*public bool MoveByOne(string source, string destination)
        {
            if (source == destination)
            {
                logHelper.showLog("Source and destion files are in the same ftp directory", null);
                return false;
            }

            //Stream ftpStream = null;

            try
            {
                //Console.Write(source + " ++++++++++ " + destination + "\n");
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(source);
                request.Credentials = new NetworkCredential(user, pass);
                request.Method = WebRequestMethods.Ftp.Rename;
                request.RenameTo = destination;
                request.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //Console.WriteLine("#############################Move status: {0}", response.StatusCode);//FileActionOK
                response.Close();
            }
            catch (WebException ex)
            {
                logHelper.showLog("Can not move file:"+source+" to "+destination, ex);
                return false;
            }

            return true;
        }*/

        /*public bool downloadOneByOne(string source)
        {
            try
            {
                if (tempDir != "")
                {
                    //Console.Write(source + " ++++++++++ " + destination + "\n");
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(source);
                    request.Credentials = new NetworkCredential(user, pass);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true; // use true for .zip file or false for a text file
                    request.UsePassive = false;
                    //request.KeepAlive = false;
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    //Console.WriteLine("#############################Move status: {0}", response.StatusCode);//FileActionOK
                    Console.WriteLine("#############################Download status: {0}", response.StatusDescription);


                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);

                    using (FileStream writer = new FileStream(tempDir, FileMode.Create))
                    {

                        long length = response.ContentLength;
                        int bufferSize = 2048;
                        int readCount;
                        byte[] buffer = new byte[2048];

                        readCount = responseStream.Read(buffer, 0, bufferSize);
                        while (readCount > 0)
                        {
                            writer.Write(buffer, 0, readCount);
                            readCount = responseStream.Read(buffer, 0, bufferSize);
                        }
                    }

                    reader.Close();
                    response.Close();
                }
                else
                {
                    logHelper.showLog("Temp directory is not initialized in config file", null);
                }

            }
            catch (WebException ex)
            {
                logHelper.showLog("Can not download ftp file:" + source, ex);
                return false;
            }

            return true;
        }*/

        /*static void DeleteFtpDirectory(string url, NetworkCredential credentials)
        {
            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(url);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = credentials;

            List<string> lines = new List<string>();

            using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
            using (Stream listStream = listResponse.GetResponseStream())
            using (StreamReader listReader = new StreamReader(listStream))
            {
                while (!listReader.EndOfStream)
                {
                    lines.Add(listReader.ReadLine());
                }
            }

            foreach (string line in lines)
            {
                string[] tokens =
                    line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                string name = tokens[8];
                string permissions = tokens[0];

                string fileUrl = url + name;

                if (permissions[0] == 'd')
                {
                    DeleteFtpDirectory(fileUrl + "/", credentials);
                }
                else
                {
                    FtpWebRequest deleteRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
                    deleteRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                    deleteRequest.Credentials = credentials;

                    deleteRequest.GetResponse();
                }
            }

            FtpWebRequest removeRequest = (FtpWebRequest)WebRequest.Create(url);
            removeRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
            removeRequest.Credentials = credentials;

            removeRequest.GetResponse();
        }*/
    }
}
