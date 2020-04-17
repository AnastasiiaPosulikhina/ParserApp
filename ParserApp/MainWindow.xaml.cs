using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Text;

namespace ParserApp
{
    public partial class MainWindow : Window
    {
        public static int koefOfSkipedPages = 0;

        public MainWindow()
        {
            InitializeComponent();
        }
                
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WriteListOfThreatsToDataGrid(1);
        }


        public void WriteListOfThreatsToDataGrid(int currentNumPage) //метод записи угроз в DataGrid
        {
            CollectionViewSource threatViewSource = (CollectionViewSource)(this.FindResource("threatViewSource"));
            int pageCount = DataProcessing.PageCount();
            int numberfOfSkipedPages = 15 * koefOfSkipedPages;

            TotalNumberOfPages.Text = pageCount.ToString();
            CurrentNumberOfPages.Text = currentNumPage.ToString();
            var list = DataProcessing.listOfThreats.Skip(numberfOfSkipedPages).Take(15).ToList();

            threatListView.ItemsSource = list;
        }

        private void Button_InfoAboutOneThreat(object sender, RoutedEventArgs e) //вывод информации о конкретной угрозе
        {     
            string Id = (sender as Button).Content.ToString();
            foreach(Threat thr in DataProcessing.listOfThreats)
            {
                if (thr.Id == Id)
                {
                    OneThreatWindow infoAboutOneThreat = new OneThreatWindow(thr);
                    infoAboutOneThreat.Show();
                }
            }
        }

        private void Button_PreviousPage(object sender, RoutedEventArgs e) //переход на предыдущую страницу
        {
            int currentNumPage = int.Parse(CurrentNumberOfPages.Text);
            if (currentNumPage > 1)
            {
                koefOfSkipedPages--;
                WriteListOfThreatsToDataGrid(currentNumPage - 1);
            }
        }

        private void Button_NextPage(object sender, RoutedEventArgs e) //переход на следущую страницу
        {
            int total = int.Parse(TotalNumberOfPages.Text);
            int currentNumPage = int.Parse(CurrentNumberOfPages.Text);
            if (currentNumPage < total)
            {
                koefOfSkipedPages++;
                WriteListOfThreatsToDataGrid(currentNumPage + 1);
            }
        }

        private void Button_Refresher(object sender, RoutedEventArgs e) // обновление данных
        {
            string refreshedInfo;
            List<Threat> refreshedListOfThreats = new DataProcessing().RefreshTableWithThreats(out refreshedInfo);
            WriteListOfThreatsToDataGrid(int.Parse(CurrentNumberOfPages.Text));
            OutdatedAndRefreshedInfoWindow updatedInfo = new OutdatedAndRefreshedInfoWindow(refreshedInfo);
            updatedInfo.Show();
        }

        private void Button_Saver(object sender, RoutedEventArgs e) // сохранение данных в файл формата .txt
        {
            try
            {
                int count = 0;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt";

                if (saveFileDialog.ShowDialog() == true)
                {
                    StreamWriter file = File.AppendText(saveFileDialog.FileName);
                    foreach (Threat thr in DataProcessing.listOfThreats)
                    {
                        file.Write("***\n" + thr.ToString() + "***\n\n");
                        count++;
                    }
                    file.Close();
                }
                if(count == 0)
                    MessageBox.Show("Вы не сохранили файл!", "Уведомление");
                else
                    MessageBox.Show("Файл успешно сохранён.", "Уведомление");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.ToString(), "Ошибка");
            }
        }

        private void threatDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { }

        private void threatListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { }
    }
}
