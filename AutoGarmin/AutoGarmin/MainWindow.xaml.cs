using System;
using System.Collections.Generic;
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

namespace AutoGarmin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GridMargin_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (GridMargin.ActualWidth <= GridMargin.ColumnDefinitions[1].MaxWidth)
            {
                GridMargin.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
                GridMargin.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                GridMargin.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);
            }
            else
            {
                GridMargin.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                GridMargin.ColumnDefinitions[1].Width = new GridLength(
                    GridMargin.ColumnDefinitions[1].MaxWidth, GridUnitType.Star);
                GridMargin.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            }
        }
    }
}
