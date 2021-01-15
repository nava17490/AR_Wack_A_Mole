using System;

namespace Tutorial12___Advanced_Physics
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Tutorial12 game = new Tutorial12())
            {
                game.Run();
            }
        }
    }
#endif
}

