using System;
using System.Windows;
using System.Windows.Controls;

namespace ParserApp
{
    public partial class OneThreatWindow : Window, IWritable
    {
        private Threat thr;

        public OneThreatWindow()
        {
            InitializeComponent();
        }

        public OneThreatWindow(Threat thr) : this()
        {
            this.thr = thr;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WriteInfo();
        }

        public void WriteInfo() //вывод информации о конкретной угрозе
        {
            Info.Text = thr.ToString();
        }

        private void threatListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { }

        private void Info_TextChanged(object sender, TextChangedEventArgs e)
        { }
    }
}
