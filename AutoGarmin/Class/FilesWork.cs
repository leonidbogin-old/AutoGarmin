using System.Collections.Generic;
using System.IO;

namespace AutoGarmin.Class
{
    static class FilesWork //work with files
    {
        public static void GetAllFiles(string rootDirectory, string fileExtension, ref List<string> files) //Search files by extension on device
        {
            string[] directories = Directory.GetDirectories(rootDirectory);
            files.AddRange(Directory.GetFiles(rootDirectory, fileExtension));

            foreach (string path in directories)
                GetAllFiles(path, fileExtension, files);
        }

        public static class Folder //work with folders
        {
            public static void Copy(DirectoryInfo source, DirectoryInfo target) //Copy folder
            {
                if (Directory.Exists(target.FullName) == false) Directory.CreateDirectory(target.FullName);
                
                foreach (FileInfo fi in source.GetFiles()) //Copy all files to a new directory
                {
                    fi.CopyTo(System.IO.Path.Combine(target.ToString(), fi.Name), true);
                }

                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories()) //Copy recursively all subdirectories
                {
                    //Create a new subdirectory in the directory
                    DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                    //Again, call the copy function. Recursion
                    Copy(diSourceSubDir, nextTargetSubDir);
                }
            }

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
