using System;
using System.Windows;
using System.Windows.Controls;

namespace ParserApp
{
    public partial class OutdatedAndRefreshedInfoWindow : Window, IWritable
    {
        private string refreshedInfo;

        public OutdatedAndRefreshedInfoWindow()
        {
            InitializeComponent();
        }

        public OutdatedAndRefreshedInfoWindow(string refreshedInfo) : this()
        {
            this.refreshedInfo = refreshedInfo;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WriteInfo();
        }

        public void WriteInfo() //вывод обновлённой информации
        {
            int count = 0;
            string s = DataProcessing.GetDiffrentThreatProperties(out count);

            if (DataProcessing.listOfThreatsBefore.Count != 0 || DataProcessing.listOfThreatsAfter.Count != 0)
            {
                refreshedInfo += "Общее количество обновлённых записей: " + count.ToString() + "\n" + s;
            }
            else
                refreshedInfo += "Общее количество обновлённых записей: " + count.ToString() + "\nВ результате обновления изменения не были обнаружены!";

            RefreshedInfo.Text = refreshedInfo;
        }

        private void RefreshedInfo_TextChanged(object sender, TextChangedEventArgs e)
        { }
    }
}
