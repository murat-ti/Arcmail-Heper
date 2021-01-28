using Arcmail.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Arcmail
{
    class ArcmailHelper
    {
        private string arcmailPath;
        private AppConfigHelper appConfigHelper;
        private LogHelper logHelper;
        private FileHelper fileHelper;
        private static readonly Regex rxNonDigits = new Regex(@"[^\d_]+");

        public ArcmailHelper()
        {
            logHelper = new LogHelper();
            fileHelper = new FileHelper();
            appConfigHelper = new AppConfigHelper();
            arcmailPath = appConfigHelper.readConfigKey("arcmailPath");
        }
        
        public bool unZipAllFilesInDir(string strPath)
        {
            string[] files = Directory.GetFiles(strPath, "*.lck");
            string filename;

            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    //return unarchived filename with full path
                    filename = ExecuteArc(0, Path.GetFileName(file), strPath, strPath, 1);
                    if (filename != "")
                    {
                        //logHelper.showLog("makeArchive file: " + file, null);
                        fileHelper.makeArchive(file);
                    }
                    else
                        logHelper.showLog("Can not unarchive arcmail file: " + file, null);
                }

                string[] directories = Directory.GetDirectories(strPath);
                foreach (string directory in directories)
                {
                    // iterate subdirectories
                    unZipAllFilesInDir(directory);
                }
                return true;
            }
            else
                return false;
        }

        public string zipRarFileInDir(string fileNamePath, string code)
        {
            return ExecuteArc(1, Path.GetFileName(fileNamePath), Path.GetDirectoryName(fileNamePath), Path.GetDirectoryName(fileNamePath), 1, code);
        }

        //commandType = unArchive/ archive
        //filename = *.lck
        //inPath = input file location
        //output = output file location
        //fileCounter = Arcmail can not see full filename length, so it changes titles like (1706_1xxx_xxxx => 1706_1~1, 1706_1xxx_xxxx => 1706_1~2)
        //code = arcmail search encrypt key by code in archiving
        public string ExecuteArc(int commandType, string filename, string inPath, string outPath, int fileCounter = 1, string code = "")
        {
            string resultFile = "";

            if (arcmailPath != "")
            {
                try
                {
                    string command = "";

                    //unarchive = 0 && archive = 1
                    if (commandType == 0)
                        command = string.Concat(" u -l", inPath, filename, " -b -e -y", outPath);//when file without extension legnth more than 8 chars, so it takes only 6 and add ~ and fileCounter
                    else {
                        string keyPath = arcmailPath + "doc\\"+ code + "\\spis"; 
                        command = string.Concat(" a -v0 -j", keyPath, " -m", filenameFormatter(filename,8), " -l", inPath,"\\", filename, " -a", outPath, "\\");
                    }

                    //logHelper.showLog(command, null);

                    // create the ProcessStartInfo using "cmd" as the program to be run,
                    // and "/c " as the parameters.
                    // Incidentally, /c tells cmd that we want it to execute the command that follows,
                    // and then exit.
                    System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", string.Concat("/c ", arcmailPath + "arcmail.exe", command));

                    // The following commands are needed to redirect the standard output.
                    // This means that it will be redirected to the Process.StandardOutput StreamReader.
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.UseShellExecute = false;
                    // Do not create the black window.
                    procStartInfo.CreateNoWindow = true;
                    procStartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);

                    // Now we create a process, assign its ProcessStartInfo and start it
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();

                    string result = "";
                    string output = "";
                    int steps = 1;

                    //Unarchive result output
                    //1. Filename (output filepath+filename)
                    //2. Result code
                    //3. Sign count
                    //4. Client id (open key title)

                    while (!proc.StandardOutput.EndOfStream)
                    {
                        output = proc.StandardOutput.ReadLine();
                        if (output != "")
                        {
                            result = resultCodes(output);

                            if (result != "")
                            {
                                logHelper.showLog(result + ": " + inPath +"\\"+ filename, null);
                                //break after getting first result
                                break;
                            }
                            else
                            {
                                resultFile = output;
                            }

                            steps++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (commandType == 0)
                        logHelper.showLog("Arcmail can not unarchive file: "+ inPath + filename, null);
                    else
                        logHelper.showLog("Arcmail can not archive file: " + inPath + filename, null);
                }
            }
            else
                logHelper.showLog("Arcmail not found", null, true);

            return resultFile;
        }

        public string filenameFormatter(string title, int length = 6)
        {
            if (string.IsNullOrEmpty(title))
                return title;

            //remove all non-numeric symbols
            string result = rxNonDigits.Replace(title, "");

            //arcmail see only first 6 symbols of filename
            return result.Substring(0, length);
        }

        public string resultCodes(string code)
        {
            code = rxNonDigits.Replace(code, "");

            if (code.Length == 1 || code.Length == 2)
            {
                int result = Int32.Parse(code);
                
                if (result >= 0 && result <= 26)
                {
                    string[] codes = new string[27];

                    codes[0] = "Успешное завершение без ошибок";
                    codes[1] = "Пользователь прервал операцию";
                    codes[2] = "Подписей у файла нет";
                    codes[3] = "Открытый ключ не сертифицирован";
                    codes[4] = "Введен пароль для секретного ключа, не закрытого паролем";
                    codes[5] = "­Нет пароля для закрытого паролем секретного ключа";
                    codes[6] = "Пароль не верен";
                    codes[7] = "Не верна дата ключа";
                    codes[8] = "Не фатальная ошибка ввода / вывода";
                    codes[9] = "Не найден открытый ключ для проверки или создания архива";
                    codes[10] = "Не хватает памяти";
                    codes[11] = "­Не верна дата подписи";
                    codes[12] = "­Не верна подпись открытого ключа";
                    codes[13] = "­Не верна подпись файла";
                    codes[14] = "Ошибка записи выходного файла";
                    codes[15] = "Синтаксическая ошибка в подписях";
                    codes[16] = "Фатальная ошибка(бет необходимых программе файлов и т.д.)";
                    codes[17] = "Отсутствует или поврежден конфигурационный файл";
                    codes[18] = "Конфигурационный файл не может быть декодирован";
                    codes[19] = "Внутренняя ошибка тестирования математики постановки подписи";
                    codes[20] = "Секретный ключ не загружен(по какой - либо причине)";
                    codes[21] = "­ Нарушена целостность файла";
                    codes[22] = "Ключ - сертификат не найден";
                    codes[23] = "Шифровальное устройство не инициалиизировано";
                    codes[24] = "Главный ключ поврежден или не соответствует загруженному УЗ";
                    codes[25] = "При проверке подписи не обнаружена подпись заданного обязательного абонента (только для командного режима)";
                    codes[26] = "Невозможно при выходе запустить Оверлейную программу";

                    return codes[result];
                }
                else
                {
                    MessageBox.Show("Code result not in range: " + result);
                }
            }

            return "";
        }
    }
}
