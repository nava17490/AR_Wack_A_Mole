using System;
using System.Collections.Generic;
using GoblinXNA.SceneGraph;
using Microsoft.Xna.Framework;

namespace MoleRehabilitation.Interfaz
{
    class Pantalla
    {
        #region Atributos
        private Scene escena;
        #endregion

        #region Propiedades
        public Scene Escena
        {
            get
            {
                return this.escena;
            }
        }
        #endregion

        #region Constructores

        /// <summary>
        /// Crea una nueva pantalla genérica de la aplicacion
        /// </summary>
        public Pantalla()
        {
            escena = new Scene();

            escena.BackgroundColor = Color.CornflowerBlue;
            GC.Collect();
        }
        #endregion

        #region metodos publicos
        
        /// <summary>
        /// Actualiza lo mostrado en pantalla. Es un método virtual por lo que puede ser sobreescrito en clases que hereden de esta.
        /// </summary>
        /// <param name="elapsedTime">Instante de tiempo actual</param>
        /// <param name="isRunningSlowly">Indica si la aplicacion se esta ejecutando de forma lenta</param>
        /// <param name="isFocused">Indica si la pantalla esta seleccionada</param>
        public virtual void Update(TimeSpan elapsedTime, bool isRunningSlowly, bool isFocused)
        {
            escena.Update(elapsedTime, isRunningSlowly, isFocused);
        }

        /// <summary>
        /// Dibuja la pantalla de nuevo. Es un método virtual por lo que puede ser sobreescrito en clases que hereden de esta.
        /// </summary>
        /// <param name="elapsedTime">Instante de tiempo actual</param>
        /// <param name="isRunningSlow">Indica si la aplicacion se esta ejecutando de forma lenta</param>
        public virtual void Draw(TimeSpan elapsedTime, bool isRunningSlow)
        {
            escena.Draw(elapsedTime, isRunningSlow);
        }

        #endregion

        #region Destructores
        ~Pantalla() {
            this.escena = null;
        }
        #endregion

    }
}
