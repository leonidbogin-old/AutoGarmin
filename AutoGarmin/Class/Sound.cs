using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin.Class
{
    public static class Sound
    {
        public static void Play(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    new System.Media.SoundPlayer(path).Play();
                }
                catch
                {

                }
            }
        }
    }
}
