using System;

namespace Tutorial1___Getting_Started
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Tutorial1 game = new Tutorial1())
            {
                game.Run();
            }
        }
    }
#endif
}

