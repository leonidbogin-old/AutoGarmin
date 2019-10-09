using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutoGarmin
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            if (!InstanceCheck())
            {
                MessageBox.Show(Const.Error.DuplicateApplication, Const.Error.DuplicateApplicationTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }

        // держим в переменной, чтобы сохранить владение им до конца пробега программы
        static System.Threading.Mutex InstanceCheckMutex;
        static bool InstanceCheck()
        {
            bool isNew;
            InstanceCheckMutex = new System.Threading.Mutex(
                true, "AutoGarmin", out isNew);
            return isNew;
        }
    }
}
