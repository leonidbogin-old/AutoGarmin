using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin.Class
{
    static class FilesWork //work with files
    {
        public static class Folder //work with folders
        { 
            public static void Clean(DirectoryInfo source) //Clear folder
            {
                foreach (FileInfo file in source.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo dir in source.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }
    }
}
