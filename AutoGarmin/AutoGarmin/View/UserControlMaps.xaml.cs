using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<MapFile> files = new ObservableCollection<MapFile>();

        public class MapFile
        {
            public string DisplayName { get; set; }
            public string DisplaySize { get; set; }
            public string DisplayTooltip { get; set; }
            public Visibility DisplayClose { get; set; }
        }

        public UserControlMaps()
        {
            InitializeComponent();
            DataGridFiles.ItemsSource = files;

            MapFile file = new MapFile()
            {
                DisplayName = "zal.kmz",
                DisplaySize = "3.5 Мб",
                DisplayTooltip = "zal.kmz (3.5 Мб) 15.10.2019 16:23:31",
                DisplayClose = Visibility.Hidden
            };
            MapFile file2 = new MapFile()
            {
                DisplayName = "grid.kmz",
                DisplaySize = "5 Мб",
                DisplayTooltip = "zal.kmz (3.5 Мб) 15.10.2019 16:23:31",
                DisplayClose = Visibility.Hidden
            };

            files.Add(file);
            files.Add(file2);
            DataGridFiles.ScrollIntoView(files.Last());
        }

        private void DataGridDelete(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("delete");
        }

        private void DataGridFiles_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
        }

        private void DataGridFiles_SelectedCellsChanged_1(object sender, SelectedCellsChangedEventArgs e)
        {
            int i = DataGridFiles.SelectedIndex;
            files[0].DisplayClose = Visibility.Visible;
        }
    }
}
