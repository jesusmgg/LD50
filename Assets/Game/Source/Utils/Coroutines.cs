using System.Collections;

namespace Game.Utils
{
    public static class Coroutines
    {
        public static IEnumerator WaitForFrames(int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
        }
    }
}