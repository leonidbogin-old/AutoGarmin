using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin.Class
{
    static class FilesWork
    {
        public static class Folder
        {
            public static void Copy(DirectoryInfo source, DirectoryInfo target) //Копирование папки
            {
                // Если директория для копирования файлов не существует, то создаем ее
                if (Directory.Exists(target.FullName) == false)
                {
                    Directory.CreateDirectory(target.FullName);
                }

                // Копируем все файлы в новую директорию
                foreach (FileInfo fi in source.GetFiles())
                {
                    fi.CopyTo(System.IO.Path.Combine(target.ToString(), fi.Name), true);
                }

                // Копируем рекурсивно все поддиректории
                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    // Создаем новую поддиректорию в директории
                    DirectoryInfo nextTargetSubDir =
                      target.CreateSubdirectory(diSourceSubDir.Name);
                    // Опять вызываем функцию копирования
                    // Рекурсия
                    Copy(diSourceSubDir, nextTargetSubDir);
                }
            }

            public static void Clean(DirectoryInfo source) //Очистка папки
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
