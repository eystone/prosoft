﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Resources;

namespace EasySave

{
    class Controller
    {
        private ConsoleCLI m_cli;
        private List<SaveFiles> m_workingFiles = new List<SaveFiles>();
        private List<TransfertStatesItems> m_transferts = new List<TransfertStatesItems>();
        ResourceManager rm = new ResourceManager("EasySave.Resources.Langue", typeof(EasySave).Assembly);
        public Controller()
        {
            while (true)
            {
                createUI(); //Summon UI to get choice
            }
        }
        private void createUI() // clement
        {
            if (m_cli == null)
            {
                m_cli = new ConsoleCLI();

                if (m_cli.ChoixLangue())
                {
                    switch (m_cli.ChoixSave())
                    {
                        case 1:
                            addWorkingFiles();
                            break;
                        case 2:
                            delWorkingFiles();
                            break;
                        case 3:
                            createJob();
                            break;
                        default:
                            System.Environment.Exit(0);
                            break;
                    }
                }
            }
            else
            {
                switch (m_cli.ChoixSave())
                {
                    case 1:
                        addWorkingFiles();
                        break;
                    case 2:
                        delWorkingFiles();
                        break;
                    case 3:
                        createJob();
                        break;
                    default:
                        System.Environment.Exit(0);
                        break;
                }
            }
        }

        private void addWorkingFiles()
        {
            if (m_workingFiles.Count >= 5)
            {
                Console.WriteLine(rm.GetString("Max 5 work"));
            }
            else
            {
                if (m_cli.PathTo1 == null)
                {
                    m_workingFiles.Add(new SaveFiles(m_cli.PathFrom1));
                }
                else
                {
                    m_workingFiles.Add(new SaveFiles(m_cli.PathFrom1, m_cli.PathTo1));
                }
            }
        }

        private void delWorkingFiles()
        {
            foreach (SaveFiles file in m_workingFiles)
            {
                if (file.PathFrom == m_cli.DeletePath1)
                {
                    m_workingFiles.Remove(file);
                }
            }
        }

        private void createJob()
        {
            foreach (SaveFiles file in m_workingFiles)
            {
                Console.WriteLine(file.ToString());
            }

            foreach (SaveFiles file in m_workingFiles)
            {
                if (file.PathFrom == m_cli.ExecutePath1)
                {
                    m_transferts.Add(new TransfertStatesItems(file));
                    Console.WriteLine(rm.GetString("BeginBackup"));
                    m_transferts[m_transferts.Count-1].BackUp();
                    Console.Write(rm.GetString($"BackupTime {0}"), m_transferts[m_transferts.Count-1].ElapsedTransfertTime);
                }
            }
        }
    }
}