﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace EasySaveModel
{
    public class SaveFiles
    {
        private List<FileInfo> m_files = new List<FileInfo>();
        private List<DirectoryInfo> subDirs = new List<DirectoryInfo>();
        private string m_pathFrom, m_pathTo; //C:/dir/dir/dir
        private long totalSizeFile = 0;

        public SaveFiles(string pathFrom, string pathTo)
        {
            m_pathFrom = pathFrom;
            if (pathTo == null || pathTo == "" || pathTo == "Destination")
            {
                m_pathTo = System.Environment.CurrentDirectory
                    + @"\Backups\"
                    + System.Threading.Thread.CurrentThread.ManagedThreadId;
            }
            else
            {
                m_pathTo = pathTo;
            }
            init();
            calcSizeFiles();
        }

        private void init()
        {
            //Need to make a feature for subdirectory
            string[] names = System.IO.Directory.GetFiles(m_pathFrom);
            if (names.Length == 0)
            {
                throw new DirectoryNotFoundException("DirectoryError" + m_pathFrom);
            }

            foreach (string filename in names)
            {
                m_files.Add(new FileInfo(filename));
            }

            DirectoryInfo dirFrom = new DirectoryInfo(m_pathFrom);
            subDirs = new List<DirectoryInfo>(dirFrom.GetDirectories());
        }
        ~SaveFiles()
        {
            //File.close();
        }
        public void calcSizeFiles()
        {
            foreach (FileInfo file in m_files)
            {
                totalSizeFile += file.Length;
            }
            foreach (DirectoryInfo dir in subDirs)
            {
                FileInfo[] subFiles = dir.GetFiles();
                foreach (FileInfo file in subFiles)
                {
                    totalSizeFile += file.Length;
                }
            }
            Logging();
        }
        public void Logging()
        {
            string m_nameLog = System.IO.Path.GetFileName(m_pathFrom);
            string totalSizeFileLog = totalSizeFile.ToString();
            string transferTime = "0";

            LogsFile JSONmyLogs = LogsFile.GetInstance(true);
            LogsFile XMLmyLogs = LogsFile.GetInstance(true);

            JSONmyLogs.WriteLog(m_nameLog, m_pathFrom, m_pathTo, totalSizeFileLog, transferTime);
            XMLmyLogs.WriteLog(m_nameLog, m_pathFrom, m_pathTo, totalSizeFileLog, transferTime);
        }

        public List<FileInfo> Files { get => m_files; }
        public List<DirectoryInfo> SubDirs { get => subDirs; }
        public string PathFrom { get => m_pathFrom; set => m_pathFrom = value; }
        public string PathTo { get => m_pathTo; set => m_pathTo = value; }
        public long TotalSizeFile { get => totalSizeFile; }
    }
}