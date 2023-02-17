﻿using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using EasySaveModel;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private VisualModel model;
        internal VisualModel Model { get => model; set => model = value; }
        private Jobs selectedRow;

        public MainWindow()
        {
            InitializeComponent();
            JobsGrid.ItemsSource = CreateWindow.JobsProps;
            LoadJobsPropsFromCsv();
        }
        private void CreateWindowButtonClick(object sender, RoutedEventArgs e) //Bouton creer
        {
            CreateWindow1.Show();
            Close();
        }
        private void LaunchMainButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (Jobs item in JobsGrid.ItemsSource)
            {
                if (((System.Windows.Controls.CheckBox)CheckboxColumn.GetCellContent(item)).IsChecked == true)
                {
                    GridFromTo.ColumnPathTo1 = ((System.Windows.Controls.TextBlock)PathToColumn.GetCellContent(item)).Text;
                    GridFromTo.ColumnPathFrom1 = ((System.Windows.Controls.TextBlock)PathFromColumn.GetCellContent(item)).Text;
                    VisualModel model = new VisualModel();
                    model.addWorkingFiles();
                    model.createJob();
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

        }
        private void StopButtonClick(object sender, RoutedEventArgs e)
        {

        }
        private void DeleteMainButtonClick(object sender, RoutedEventArgs e)
        {

        }
        private void EnglishButtonClick(object sender, RoutedEventArgs e)
        {

        }
        private void FrenchButtonClick(object sender, RoutedEventArgs e)
        {

        }
        private void LogicielMetier()
        {
            // Faire un fichier settings pour extensions, logiciel metier, max transfert size --> revoir methodes
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            CreateWindow.JobsProps.Clear();
            CreateWindow.JobsProps = (System.Collections.Generic.List<Jobs>)JobsGrid.ItemsSource;
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
            CreateWindow.JobsProps.Clear();
            StreamReader reader = new StreamReader(CreateWindow.CsvFilePath1);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string[] props = reader.ReadLine().Split(',');
                Jobs job = new Jobs();
                job.Checkbox = bool.Parse(props[0]);
                job.Nom = props[1];
                job.Source = props[2];
                job.Destination = props[3];
                job.Cryptosoft = bool.Parse(props[4]);
                job.Type = props[5];
                job.Progression = double.Parse(props[6]);
                CreateWindow.JobsProps.Add(job);
            }
            JobsGrid.ItemsSource = CreateWindow.JobsProps;
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
    }
    public class Jobs
    {
        public bool Checkbox { get; set; }
        public string Nom { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public bool Cryptosoft { get; set; }
        public double Progression { get; set; }
        public string Type { get; set; }
    }
    public static class GridFromTo
    {
        private static string ColumnPathFrom;
        private static string ColumnPathTo;

        public static string ColumnPathFrom1 { get => ColumnPathFrom; set => ColumnPathFrom = value; }
        public static string ColumnPathTo1 { get => ColumnPathTo; set => ColumnPathTo = value; }
    }
}
