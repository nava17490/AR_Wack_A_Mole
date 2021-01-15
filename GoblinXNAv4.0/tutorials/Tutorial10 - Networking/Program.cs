using System;

namespace Tutorial10___Networking
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Tutorial10 game = new Tutorial10(true))
            {
                game.Run();
            }
        }
    }
#endif
}

