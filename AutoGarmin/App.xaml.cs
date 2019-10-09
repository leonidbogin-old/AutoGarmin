using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutoGarmin
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            //prevent the program from running again
            if (!InstanceCheck()) 
            {
                MessageBox.Show(Const.Error.DuplicateApplication, Const.Error.DuplicateApplicationTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }

        //hold in a variable to keep possession of it until the end of the program run
        static System.Threading.Mutex InstanceCheckMutex;
        static bool InstanceCheck()
        {
            bool isNew;
            InstanceCheckMutex = new System.Threading.Mutex(true, "AutoGarmin", out isNew);
            return isNew;
        }
    }
}
