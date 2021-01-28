using Arcmail.Helpers;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Arcmail
{
    class FileHelper
    {
        private string filePath;
        private string archivePath;
        private string tempDirOut;
        string filePatterns = "OBR_IBI_####_*|OBI_*_####|OBI_*_####_*|####_OSV_*|OAR_####_*";

        private AppConfigHelper appConfigHelper;
        private LogHelper logHelper;
        public FileHelper()
        {
            appConfigHelper = new AppConfigHelper();
            logHelper = new LogHelper();
            archivePath = appConfigHelper.readConfigKey("archivePath");
            filePatterns = appConfigHelper.readConfigKey("filePatterns");

            makeArchiveFolders();
        }

        public void makeArchiveFolders()
        {
            if (archivePath != "")
            {
                if (!Directory.Exists(archivePath))
                    Directory.CreateDirectory(archivePath);

                if (!Directory.Exists(archivePath + "\\in"))
                    Directory.CreateDirectory(archivePath + "\\in");

                if (!Directory.Exists(archivePath + "\\out"))
                    Directory.CreateDirectory(archivePath + "\\out");
            }
            else
                logHelper.showLog("Archive path not found");
        }

        public string[][] readClientsToArray()
        {
            string[][] data;

            try
            {
                filePath = appConfigHelper.readConfigKey("clientListPath");

                StreamReader sr = new StreamReader(filePath);
                var lines = new List<string[]>();
                int Row = 0;
                while (!sr.EndOfStream)
                {
                    string[] Line = sr.ReadLine().Split(';');
                    lines.Add(Line);
                    Row++;
                }

                data = lines.ToArray();
            }
            catch (Exception ex)
            {
                logHelper.showLog("The clients file path not found", null);
                data = new string[][] { };
            }

            return data;
        }

        public string[] readFtpCredentials()
        {
            string[] data;

            try
            {
                data = new string[] { appConfigHelper.readConfigKey("ftphost"), appConfigHelper.readConfigKey("ftpuser"), appConfigHelper.readConfigKey("ftppass") } ;
            }
            catch (Exception ex)
            {
                logHelper.showLog("Host, username and password are not found in config file");
                data = new string[] {};
            }

            return data;
        } 

        public bool zip(string sourcePath, string resultZipPath)
        {
            try
            {
                if (Directory.Exists(sourcePath))
                {
                    logHelper.showLog("Directory for zipping: " + sourcePath, null);
                    using (var archive = ZipArchive.Create())
                    {
                        archive.AddAllFromDirectory(sourcePath);
                        archive.SaveTo(resultZipPath, CompressionType.Deflate);
                    }
                }
            }
            catch (Exception ex)
            {
                logHelper.showLog("Can not create rar file from: "+sourcePath, null);
                return false;
            }

            return true;
        }

        public bool unZip(string sourceArchivePath, string resultUnZipPath)
        {
            try
            {
                if (File.Exists(sourceArchivePath))
                {
                    //if result folder not found, so create it
                    if (!Directory.Exists(resultUnZipPath))
                    {
                        Directory.CreateDirectory(resultUnZipPath);
                    }

                    using (var archive = ArchiveFactory.Open(sourceArchivePath))
                    {
                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                        {
                            entry.WriteToDirectory(resultUnZipPath, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }
                else
                {
                    logHelper.showLog("Archive file: " + sourceArchivePath + " is not found", null);
                }
            }
            catch (Exception ex)
            {
                logHelper.showLog("Can not unrar file from: " + sourceArchivePath, ex);
            }

            return true;
        }

        public bool unZipAllFilesInDir(string strPath)
        {
            //logHelper.showLog("Uzip path: " + strPath, null);
            string[] files = Directory.GetFiles(strPath, "*.rar");
            logHelper.showLog("Rar files found: " + files.Length, null);
            foreach (string file in files)
            {
                //logHelper.showLog(Path.GetDirectoryName(file) +": "+ file);
                if (unZip(file, Path.GetDirectoryName(file)))
                    makeArchive(file);
                else
                    logHelper.showLog("Can not unarchive rar file: " + file, null);
            }

            string[] directories = Directory.GetDirectories(strPath);
            foreach (string directory in directories)
            {
                // iterate subdirectories
                unZipAllFilesInDir(directory);
            }
            return true;
        }

        public void searchAllBankFilesInDirByBankCode(string code, string proc_code)
        {
            tempDirOut = appConfigHelper.readConfigKey("tempDirOut");
            string tempDir = Path.Combine(tempDirOut,"..") + '\\' + code;

            if (Directory.Exists(tempDirOut))
            {
                string pattern = filePatterns.Replace("####", code);
                string subPattern = "";
                if (!code.Equals(proc_code))
                {
                    subPattern = filePatterns.Replace("####", proc_code);
                    pattern += "|" + subPattern;
                }

                //logHelper.showLog("Patern:\t"+pattern);

                //my own getfiles function
                string[] files = MyGetFiles(tempDirOut, pattern, SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    if (!Directory.Exists(tempDir))
                        Directory.CreateDirectory(tempDir);

                    //Console.WriteLine("Move from " + file + " =================> " + tempDir + "\\" + Path.GetFileName(file));
                    string filename = Path.GetFileName(file);
                    logHelper.fileLog(filename, code);
                    if(File.Exists(file))
                        File.Move(file, tempDir + "\\" + filename);
                }

                string[] directories = Directory.GetDirectories(tempDirOut);
                foreach (string directory in directories)
                {
                    // iterate subdirectories
                    searchAllBankFilesInDirByBankCode(code, proc_code);
                }
            }
        }

        /*private string checkWinrarPath()
        {
            <add key="winrarPath" value="C:\Program Files\WinRAR\"/>
            try
            {
                winrarPath = appConfigHelper.readConfigKey("winrarPath");
            }
            catch (Exception ex)
            {
                logHelper.showLog("Winrar path is not found in config file\n", null);
                winrarPath = "";
            }

            return winrarPath;
        }*/

        public void deleteAllFilesAndSubdirs(string path)
        {
            //Console.WriteLine("########################### "+path);
            if (Directory.Exists(path))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                di.Delete();
            }
        }

        public static string[] MyGetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            string[] searchPatterns = searchPattern.Split('|');
            List<string> files = new List<string>();
            foreach (string sp in searchPatterns)
                files.AddRange(System.IO.Directory.GetFiles(path, sp, searchOption));
            files.Sort();
            return files.ToArray();
        }

        public void makeArchive(string sourceFilePath, string subdir = "in")
        {
            string destPath = archivePath + "\\" + subdir + "\\" + Path.GetFileName(sourceFilePath);

            if (!File.Exists(destPath))
            {
                File.Move(sourceFilePath, destPath);
                logHelper.showLog(Path.GetFileName(sourceFilePath) + " moved to archive " + destPath);
            }
            else
                logHelper.showLog("Archive file already exists in: " + destPath);
        }

        public string getFreeTitle(string code, string subdir = "in", string ext = ".rar")
        {
            //I have only 8 chars in title, where 4 chars bank code + 4 random chars
            string result = "";
            String filePath;
            String fileName;

            for (int i=1000; i<=9999; i++)
            {
                fileName = String.Format("{0}{1}{2}", code, i, ext);
                filePath = String.Format("{0}{1}/{2}", archivePath, subdir, fileName);

                if (!File.Exists(filePath))
                {
                    result = fileName;
                    break;
                }
            }

            return result;
        }
    }
}
