using System;

namespace Tutorial15___OpenCV
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Tutorial15 game = new Tutorial15())
            {
                game.Run();
            }
        }
    }
#endif
}

