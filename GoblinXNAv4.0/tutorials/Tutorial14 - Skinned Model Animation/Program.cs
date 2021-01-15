using System;

namespace Tutorial14___Skinned_Model_Animation
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Tutorial14 game = new Tutorial14())
            {
                game.Run();
            }
        }
    }
#endif
}

