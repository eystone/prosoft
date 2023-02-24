﻿using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using EasySaveModel;
using System.Diagnostics;
using System.Threading;
using CheckBox = System.Windows.Controls.CheckBox;
using System.Windows.Controls;
using System.ComponentModel;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private static List<SaveFiles> _jobsProps = new List<SaveFiles>();
        private List<TransfertJob> _transferts = new List<TransfertJob>();
        private SaveFiles _savefiles;
        private TransfertJob _transfertjob;
        private bool isPaused = false;
        private bool isStopped = false;
        private static readonly Mutex _pauseMutex = new Mutex();
        private readonly SettingsWindow settingsWindow;
        private string _BusinessAppName;
        public string BusinessAppName { get => _BusinessAppName; set => _BusinessAppName = value; }


        public MainWindow()
        {
            InitializeComponent();
            Thread BusinessAppThread = new Thread(BusinessApp);
            BusinessAppThread.Start();
            JobsGrid.ItemsSource = JobsProps;
            settingsWindow = new SettingsWindow();
            LoadJobsPropsFromCsv();
            Thread ProgressBarThread = new Thread(ProgressBarLoop);
            ProgressBarThread.Start();

            TransfertJob.PauseMutex = _pauseMutex;
        }
        private void CreateWindowButtonClick(object sender, RoutedEventArgs e) //Bouton creer
        {
            CreateWindow1.Show();
            Close();
        }
        private void LaunchMainButtonClick(object sender, RoutedEventArgs e)
        {
            //trying to build an anonymous fonction for progress bar
            /*Func<bool> refreshProgressBar = () =>
            {
                uint tmpProgress = 0;
                while (_transferts[_transferts.Count - 1].ActualStates)
                {
                    if (tmpProgress == 0 || tmpProgress != _transferts[_transferts.Count - 1].CalcProgress())
                    {
                        tmpProgress = _transferts[_transferts.Count - 1].CalcProgress();
                        Progress progress = new Progress();
                        progress.progress = tmpProgress; 
                        JobsGrid.Items.Refresh();
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                return true;
            };*/

            //If both or neither are checked error
            if (DifferentialCheckBox.IsChecked == SequentialCheckBox.IsChecked)
            {
                MessageBox.Show("Erreur, aucun type de sauvegarde n'est choisi");
                return;
            }
            //Get all checked jobs
            foreach (SaveFiles item in JobsGrid.ItemsSource)
            {
                if (((CheckBox)CheckboxColumn.GetCellContent(item)).IsChecked == true)
                {
                    //Check cryptosoft option
                    if (((TextBlock)CryptosoftColumn.GetCellContent(item)).Text == "True")
                    {
                        _savefiles = new SaveFiles(((TextBlock)PathFromColumn.GetCellContent(item)).Text,
                                               ((TextBlock)PathToColumn.GetCellContent(item)).Text);
                        foreach (SaveFiles file in _jobsProps)
                        {
                            if (file.PathFrom == _savefiles.PathFrom)
                            {
                                //Create an thread job
                                _transferts.Add(new TransfertJob(file));
                                _transferts[_transferts.Count - 1].Activecrypto = true;
                                _transferts[_transferts.Count - 1].PrioritizeExts = settingsWindow.ExtensionsPrioList;
                                _transferts[_transferts.Count - 1].MaxSizeFile = settingsWindow.MaxSizeTransfert;
                                _transferts[_transferts.Count - 1].CryptoSoftPath = settingsWindow.CryptoSoftPath;
                                _transferts[_transferts.Count - 1]._extensionList = settingsWindow.ExtensionsList;
                                _transferts[_transferts.Count - 1].ThreadBackUp((bool)DifferentialCheckBox.IsChecked);

                            }
                        }
                    }
                    else
                    {
                        _savefiles = new SaveFiles(((TextBlock)PathFromColumn.GetCellContent(item)).Text,
                                               ((TextBlock)PathToColumn.GetCellContent(item)).Text);
                        foreach (SaveFiles file in _jobsProps)
                        {
                            if (file.PathFrom == _savefiles.PathFrom)
                            {
                                //Create an thread job
                                _transferts.Add(new TransfertJob(file));
                                _transferts[_transferts.Count - 1].PrioritizeExts = settingsWindow.ExtensionsPrioList;
                                _transferts[_transferts.Count - 1].MaxSizeFile = settingsWindow.MaxSizeTransfert;
                                _transferts[_transferts.Count - 1].ThreadBackUp((bool)DifferentialCheckBox.IsChecked);
                            }
                        }
                    }
                }
            }
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow1.Show();
            Close();
        }
        private void PauseButtonClick(object sender, RoutedEventArgs e)
        {
            if (!isStopped)
            {
                if (!isPaused)
                {
                    _pauseMutex.WaitOne();
                }
                else
                {
                    _pauseMutex.ReleaseMutex();
                }
                isPaused = !isPaused;
            }
        }
        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            if (!isStopped)
            {
                isStopped = true;
                foreach (TransfertJob job in _transferts)
                {
                    job.MainThread.Abort();
                    job.MainThread.Join();
                }
            }
        }
        private void EnglishButtonClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.languageCode = "en-US";
            Properties.Settings.Default.Save();
            MessageBox.Show("Redémarrer l'application pour que les changements prennent effet");
        }
        private void FrenchButtonClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.languageCode = "fr-FR";
            Properties.Settings.Default.Save();
            MessageBox.Show("Reload application for changes to take effect");
        }
        private void BusinessApp()
        {
            // Faire un fichier settings pour extensions, logiciel metier, max transfert size --> revoir methodes
            while (true)
            {
                Process[] processes = Process.GetProcessesByName(BusinessAppName);
                if (processes.Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        BusinessSoftwareLabel.Content = "Logiciel métier détecté, travail mis en pause";
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        BusinessSoftwareLabel.Content = "";
                    });
                }
                //Ajouter la methode pause a ça
                Thread.Sleep(800);
            }
        }
        private void ProgressBarLoop()
        {
            while (true)
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        JobsGrid.Items.Refresh();
                    }
                    catch { }
                });
                Thread.Sleep(800);
            }
        }
        private void Delete(object sender, RoutedEventArgs e)
        {
            JobsProps.Clear();
            JobsProps = (List<SaveFiles>)JobsGrid.ItemsSource;
            CreateWindow cw = new CreateWindow();
            cw.SaveJobsPropsToCsv();
            JobsGrid.Items.Refresh();
        }
        private void LoadJobsPropsFromCsv()
        {
            if (!File.Exists(CreateWindow.CsvFilePath1))
            {
                return;
            }
            JobsProps.Clear();
            StreamReader reader = new StreamReader(CreateWindow.CsvFilePath1);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string[] props = reader.ReadLine().Split(',');
                SaveFiles saveFiles = new SaveFiles(props[0], props[1], props[2], bool.Parse(props[3]));
                JobsProps.Add(saveFiles);
            }
            JobsGrid.ItemsSource = JobsProps;
            reader.Close();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            App.CloseApp(this);
        }

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            App.ResizeApp(this);
        }
        private void Window_MouseDownClick(object sender, MouseButtonEventArgs e)
        {
            App.Window_MouseDown(this, e);
        }
        public void SaveBusinessAppButtonClick(object sender, RoutedEventArgs e)
        {
            BusinessAppName = SaveBusinessApp.Text;
        }
        public bool IsPaused { get => isPaused; set => isPaused = value; }
        public bool IsStopped { get => isStopped; set => isStopped = value; }
        public static List<SaveFiles> JobsProps { get => _jobsProps; set => _jobsProps = value; }
        public string WPFCreationButtonText { get; set; }
    }
}
