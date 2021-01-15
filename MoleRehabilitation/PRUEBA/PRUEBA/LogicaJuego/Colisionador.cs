using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoleRehabilitation.LogicaJuego
{
    class Colisionador
    {
        /// <summary>
        /// Devuelve los pixeles que forman una imagen
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Color[,] getPixels(Texture2D texture) 
        {
            Color[] colors1D = new Color[texture.Bounds.Width * texture.Bounds.Height];
            texture.GetData<Color>(colors1D);
            
            Color[,] colors2D = new Color[texture.Bounds.Width, texture.Bounds.Height];
            for (int x = 0; x < texture.Bounds.Width; x++)
            {
                for (int y = 0; y < texture.Bounds.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * texture.Bounds.Width];
                }
            }
            return colors2D;
        }

        /// <summary>
        /// Devuelve si hay una colision entre dos imagenes basandose en el metodo de colision por pixeles
        /// </summary>
        /// <param name="rectangulo1"></param>
        /// <param name="rectangulo2"></param>
        /// <param name="imagen1"></param>
        /// <param name="imagen2">si es nulo se crea un color para compararlo que siempre satisfacera la condicion de alfa>0</param>
        /// <returns></returns>
        public static bool existeColisionPixeles(Rectangle rectangulo1, Rectangle rectangulo2, Color[,] imagen1, Color[,] imagen2)
        {
            Rectangle result = existeColisionRectangulos(rectangulo1, rectangulo2);if (result != Rectangle.Empty)
            {
                for (int x = result.X; x < result.X + result.Width; x++)
                {
                    for (int y = result.Y; y < result.Y + result.Height; y++)
                    {
                        Color color1 = imagen1[x - rectangulo1.X, y - rectangulo1.Y];
                        Color color2;
                        if (imagen2 == null)
                            color2 = new Color(255, 255, 255, 2);
                        else
                            color2 = imagen2[x - rectangulo2.X, y - rectangulo2.Y];

                        if (color1.A > 0 && color2.A > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Devuelve si hay una colision entre dos imagenes basandose en el metodo de colision por BoundingBox
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="rect2"></param>
        /// <returns></returns>
        public static Rectangle existeColisionRectangulos(Rectangle rect1, Rectangle rect2)
        {
            Rectangle result = Rectangle.Intersect(rect1, rect2);
            return result;
        }
    }
}
