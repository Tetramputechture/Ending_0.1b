using SFML.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.Audio
{
    public class AudioHandler
    {
        private AudioHandler()
        {

        }

        public static void PlaySound(string filename)
        {
            SoundBuffer buffer = new SoundBuffer(filename);

            Sound sound = new Sound(buffer);
            sound.Play();
        }

        public static void PlayMusic(string filename)
        {
            Music music = new Music(filename);
            music.Play();
        }
    }
}
