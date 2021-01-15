using System;

namespace Tutorial11___Shader
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Tutorial11 game = new Tutorial11())
            {
                game.Run();
            }
        }
    }
#endif
}

