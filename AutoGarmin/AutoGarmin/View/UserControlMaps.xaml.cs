using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoGarmin.View
{
    /// <summary>
    /// Логика взаимодействия для UserControlMaps.xaml
    /// </summary>
    public partial class UserControlMaps : UserControl
    {
        public ObservableCollection<MapFile> files { get; set; }

        public class MapFile
        {
            public string DisplayName { get; set; }
            public string DisplaySize { get; set; }
            public string DisplayTooltip { get; set; }
            public bool DisplayClose { get; set; }
        }

        public UserControlMaps()
        {
            InitializeComponent();
            files = new ObservableCollection<MapFile>();
            DataGridFiles.ItemsSource = files;

            MapFile file = new MapFile()
            {
                DisplayName = "zal.kmz",
                DisplaySize = "3.5 Мб",
                DisplayTooltip = "zal.kmz (3.5 Мб) 15.10.2019 16:23:31",
                DisplayClose = false
            };
            MapFile file2 = new MapFile()
            {
                DisplayName = "grid.kmz",
                DisplaySize = "5 Мб",
                DisplayTooltip = "zal.kmz (3.5 Мб) 15.10.2019 16:23:31",
                DisplayClose = true
            };

            files.Add(file);
            files.Add(file2);
            DataGridFiles.ScrollIntoView(files.Last());
        }

        private void DataGridDelete(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("delete");
            files.Add(new MapFile());
        }

        private void DataGridFiles_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //(sender as DataGrid).SelectedCells.Clear();
        }

        //private void DataGridFiles_SelectedCellsChanged_1(object sender, SelectedCellsChangedEventArgs e)
        //{
        //    for (int i = 0; i < files.Count; i++)
        //        if (i != DataGridFiles.SelectedIndex)
        //        {
        //            if (files[i].DisplayClose) files[i].DisplayClose = false;
        //        }
        //        else
        //        {
        //            files[i].DisplayClose = true;
        //        }

        //    DataGridFiles.Items.Refresh();
        //}

        private void DataGridFiles_MouseMove(object sender, MouseEventArgs e)
        {
            
        }
    }
}
