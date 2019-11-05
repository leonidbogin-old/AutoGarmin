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
using System.Windows.Shapes;

namespace AutoGarmin.View
{
    /// <summary>
    /// Логика взаимодействия для WindowDeviceRename.xaml
    /// </summary>
    public partial class WindowDeviceRename : Window
    {
        public string Result
        {
            get { return TextBoxNickname.Text.TrimStart().TrimEnd(); }
        }

        public WindowDeviceRename(string nicknameLast)
        {
            InitializeComponent();
            TextBoxNickname.Text = nicknameLast;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
