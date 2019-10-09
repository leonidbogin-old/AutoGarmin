using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
                System.Threading.Thread myThread = new System.Threading.Thread(
                                new System.Threading.ParameterizedThreadStart(Start));
                myThread.Priority = System.Threading.ThreadPriority.Lowest;
                myThread.Start(path);
            }
        }

        private static void Start(object path)
        {
            try
            {
                SoundPlayer soundPlayer = new SoundPlayer();
                soundPlayer.SoundLocation = (string)path;
                soundPlayer.Play();
                //new System.Media.SoundPlayer((string)path).Play();
            }
            catch
            {

            }
        }
    }
}
