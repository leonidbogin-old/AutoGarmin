using System.IO;
using System.Media;

namespace AutoGarmin.Class
{
    public static class Sound //work with sound
    {
        public static void Play(string path) //play sound
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
