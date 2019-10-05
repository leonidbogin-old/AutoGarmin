using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoGarmin
{
    public partial class WindowDeviceRename : Window
    {
        public string nickname;

        public WindowDeviceRename(string nickname)
        {
            InitializeComponent();
            TextBoxNickname.Text = nickname;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            nickname = TextBoxNickname.Text;
            this.DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TextBoxNickname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                nickname = TextBoxNickname.Text;
                this.DialogResult = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxNickname.Focus();
        }
    }
}
