using System;

namespace CameraCalibration
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            int ID = 0;

            switch (ID)
            {
                case 0:
                    using (Main game = new Main())
                    {
                        game.Run();
                    }
                    break;
                case 1:
                    using (ProjectionMatrixGenerator gen = new ProjectionMatrixGenerator())
                    {
                        gen.GenerateXML();
                    }
                    break;
            }
        }
    }
}

