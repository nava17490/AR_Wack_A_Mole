using MoleRehabilitation.Interfaz;
using System;

namespace MoleRehabilitation
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
                using (GestorInterfaz game = GestorInterfaz.Instancia)
                {
                    game.Run();
                }
        }
    }
#endif
}

