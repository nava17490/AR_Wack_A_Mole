using System;

namespace Tutorial13___iWear_VR920
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Tutorial13 game = new Tutorial13())
            {
                game.Run();
            }
        }
    }
}

