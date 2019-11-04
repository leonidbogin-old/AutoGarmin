using AutoGarmin.Class;
using AutoGarmin.View;
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
    public partial class MainWindow : Window
    {
        private MainMenu mainMenu;
        private UserControlUSB userControlUSB;
        private UserControlSettings userControlSettings;

        public MainWindow()
        {
            InitializeComponent();
            MainMenuLoad(MainMenu.menu.USB);
        }

        private void MainMenuLoad(MainMenu.menu i)
        {
            if (mainMenu != null) mainMenu.i = i;
            else mainMenu = new MainMenu(i);

            GridContent.Children.Clear();
            switch (mainMenu.i)
            {
                case MainMenu.menu.USB:
                    ButtonMenuUSB.Style = (Style)FindResource("ButtonMenuActive");
                    if (userControlUSB == null) userControlUSB = new UserControlUSB();
                    GridContent.Children.Add(userControlUSB);
                        break;
                case MainMenu.menu.Settings:
                    ButtonMenuSettings.Style = (Style)FindResource("ButtonMenuActive");
                    if (userControlSettings == null) userControlSettings = new UserControlSettings();
                    GridContent.Children.Add(userControlSettings);
                        break;
            }
        }

        private void MainMenuClick(MainMenu.menu i)
        {
            if (i != mainMenu.i)
            {
                Style ButtonMenu = (Style)FindResource("ButtonMenu");
                if (ButtonMenuUSB.Style != ButtonMenu) ButtonMenuUSB.Style = ButtonMenu;
                if (ButtonMenuSettings.Style != ButtonMenu) ButtonMenuSettings.Style = ButtonMenu;
                MainMenuLoad(i);
            }
        }

        private void ButtonMenuUSB_Click(object sender, RoutedEventArgs e)
        {
            MainMenuClick(MainMenu.menu.USB);
        }

        private void ButtonMenuSettings_Click(object sender, RoutedEventArgs e)
        {
            MainMenuClick(MainMenu.menu.Settings);
        }
    }
}
