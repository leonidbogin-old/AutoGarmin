using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin.Class
{
    public class MainMenu
    {
        public menu i;

        public enum menu
        {
            Map,
            Track,
            Settings
        }

        public MainMenu(menu i)
        {
            this.i = i;
        }
    }
}
