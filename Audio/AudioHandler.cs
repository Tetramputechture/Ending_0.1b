using SFML.Audio;

namespace Ending.Audio
{
    public class AudioHandler
    {
        private AudioHandler()
        {

        }

        public static void PlaySound(string filename)
        {
            var buffer = new SoundBuffer(filename);

            var sound = new Sound(buffer);
            sound.Play();
        }

        public static void PlayMusic(string filename)
        {
            var music = new Music(filename);
            music.Play();
        }
    }
}
