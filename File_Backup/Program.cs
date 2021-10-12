using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using NLog;
using NLog.Fluent;

namespace File_Backup
{
    class Program
    {
        private static string FolderName;
        private static List<DirectoryModel> SettingsList;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Запуск программы File_Backup");
            //name-2021-09-14-10-02
            //DateTime.Now;
            ReadSettings();
            CreateFolder();
            //Opentxt();
            CopyFiles();
        }

        private static void ReadSettings()
        {
            try
            {
                logger.Info("Начало чтения насроек");
                string path = @"C:\Sber_TZ_Vxod\TZ3\File_Backup\File_Backup\Settings.json";
                StreamReader sr = new StreamReader(path);
                var json = sr.ReadToEnd();
                SettingsList = JsonConvert.DeserializeObject<List<DirectoryModel>>(json);
                logger.Debug("Прочитан файл с настройками по пути {value1}", path);
            }
            catch(Exception msg)
            {
                logger.Error(msg.ToString());
            }


        }

        private static void CreateFolder()
        {
            try
            {
                logger.Info("Начало процесса создания каталогов");
                string st1 = "";
                string st3 = "";
                foreach (var item in SettingsList)
                {
                    if (item.outst != st1)
                    {
                        st1 = item.outst;
                        DirectoryInfo dirinfo = new DirectoryInfo(item.outst);
                        if (!dirinfo.Exists)
                        {
                            dirinfo.Create();
                            logger.Debug("Создан каталог по пути {value1}",item.outst);
                        }
                        string st2 = Convert.ToString(DateTime.Now);
                        char[] a = st2.ToCharArray();
                        int i = 0;
                        while (i < a.Length)
                        {
                            if (a[i] == '.' | a[i] == ':')
                                a[i] = '-';
                            st3 += a[i];
                            i++;
                        }
                        FolderName = st3;
                        dirinfo.CreateSubdirectory(st3);
                        logger.Debug("Создан каталог {value1}", st3);
                    }
                }
            }
            catch(Exception msg)
            {
                logger.Error(msg.ToString());
            }
        }

        private static void CopyFiles()
        {
            try
            {
                foreach (var item in SettingsList)
                {
                    logger.Info("Копирование из каталога {value1}", item.inst);
                    string[] lst = Directory.GetFiles(item.inst);
                    foreach (var item1 in lst)
                    {
                        FileInfo fileinfo = new FileInfo(item1);
                        if (fileinfo.Exists)
                        {
                            logger.Debug("Копирование файла {value1} из {value2} в {value3}", fileinfo.Name, item1, item.outst+FolderName);
                            File.Copy(item1, item.outst + "\\" + FolderName + "\\" + fileinfo.Name, true);
                        }
                    }
                }
            }
            catch(Exception msg)
            {
                logger.Error(msg.ToString());
            }
        }

        private static void Opentxt()
        {
            using (StreamReader sr1 = new StreamReader(@"C:\Sber_TZ_Vxod\TZ3\FilesIn2\1 — копия.txt"))
            {
                String line = sr1.ReadToEnd();
                Console.WriteLine(line);
                //sr.Close();
                
            }
        }

        private static void WriteSettings()
        {
            DirectoryModel[] SettingsFile = new DirectoryModel[]
{
                new DirectoryModel
                {
                    inst = @"C:\Sber_TZ_Vxod\TZ3\FilesIn1",
                    outst = @"C:\Sber_TZ_Vxod\TZ3\FilesOut"
                },
                new DirectoryModel
                {
                    inst = @"C:\Sber_TZ_Vxod\TZ3\FilesIn2",
                    outst = @"C:\Sber_TZ_Vxod\TZ3\FilesOut"
                }
};

            var json = JsonConvert.SerializeObject(SettingsFile);
            string writePath = @"C:\Sber_TZ_Vxod\TZ3\File_Backup\File_Backup\Settings.json";
            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
                sw.WriteLine(json);
            }
        }
    }
}
