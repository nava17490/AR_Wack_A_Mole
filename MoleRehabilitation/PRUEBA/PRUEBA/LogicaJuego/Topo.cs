using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoleRehabilitation.LogicaJuego
{
    class Topo
    {
        #region Atributos
        private List<Texture2D> imagenes;
        private int imagenSeleccionada;
        private Vector2 posicion;
        private DateTime tiempoAparicionUltimaImagen;
        private DateTime tiempoAparicionImagenAgujero;
        private int estadoSeleccionado;
        private String[] estados;
        private bool estaActivo;
        private bool tiempoAgotadoAplastado;
        #endregion

        #region Propiedades
        public Texture2D Imagen
        {
            get
            {
                if (!this.estaActivo) return imagenes[0];
                if (GestorJuego.Instancia.EstaEnEjecucion && (this.EstaFuera || this.EstaAplastado))
                {
                    comprobarTiempo(); 
                }
                return imagenes[imagenSeleccionada];
            }
        }
        public Vector2 Posicion
        {
            get
            {
                return this.posicion;
            }
        }
        public Rectangle Rectangulo
        {
            get
            {
                Rectangle rectangulo = new Rectangle((int)posicion.X, (int)posicion.Y, imagenes[imagenSeleccionada].Bounds.Width, imagenes[imagenSeleccionada].Bounds.Height);
                return rectangulo;
            }
        }
        public bool EstaFuera
        {
            get
            {
                return this.estados[estadoSeleccionado].Equals("fuera"); 
            }
        }
        public bool EstaAplastado
        {
            get
            {
                return estados[estadoSeleccionado].Equals("aplastado"); 
            }
        }
        public bool AgotadoTiempoAplastado
        {
            get
            {
                return tiempoAgotadoAplastado;
            }
        }
        #endregion

        #region Constructores

        /// <summary>
        /// Inicializa un topo y lo crea dada su posicion en pantalla
        /// </summary>
        /// <param name="posicion">Es la posicion en pantalla en 2D</param>
        public Topo(Vector2 posicion)
        {
            this.posicion = posicion;
            this.estados = new String[3]{ "agujero", "fuera", "aplastado" };
            this.estadoSeleccionado = 0;
            this.imagenSeleccionada = 0;
            tiempoAgotadoAplastado = false;
        }
        #endregion

        #region metodos publicos

        /// <summary>
        /// Carga las imagenes en memoria para poder ser tratadas internamente segun el instante de juego que sea
        /// </summary>
        /// <param name="imagenes"></param>
        public void cargarImagenes(List<Texture2D> imagenes)
        {
            this.imagenes = imagenes;
            seleccionarImagen(0);
            estaActivo = true;
        }

        /// <summary>
        /// Devuelve si una determinada posición esta contenida dentro de la imagen del Topo
        /// </summary>
        /// <param name="posicionPie">posicion a verificar</param>
        /// <returns></returns>
        public bool colisiona(Vector3 posicionPie)
        {
            if (!EstaFuera) return false;
            Rectangle rp = new Rectangle((int)posicionPie.X, (int)posicionPie.Y, 1, 1);
            Rectangle rs = new Rectangle((int)posicion.X, (int)posicion.Y, imagenes[imagenSeleccionada].Bounds.Width, imagenes[imagenSeleccionada].Bounds.Height);
            return Colisionador.existeColisionPixeles(rs, rp, Colisionador.getPixels(imagenes[imagenSeleccionada]),null);
        }
        
        /// <summary>
        /// Hace que el topo se esconda en su agujero
        /// </summary>
        public void esconder()
        {
            this.seleccionarImagen(0);
            this.estaActivo = false;
            tiempoAgotadoAplastado = false;
        }
        
        /// <summary>
        /// hace que el topo salga de su agujero
        /// </summary>
        public void mostrar()
        {
            this.estaActivo = true;
            this.seleccionarImagen(1);
            tiempoAgotadoAplastado = false;
        }

        /// <summary>
        /// Aplasta el topo
        /// </summary>
        public void aplastar()
        {
            if (estaActivo)
            {
                seleccionarImagen(2);
                tiempoAgotadoAplastado = false;
            }
        }
        
        #endregion

        #region metodos privados

        /// <summary>
        /// Comprueba si el tiempo que ha estado visible una imagen coincide con el maximo
        /// </summary>
        private void comprobarTiempo()
        {
            DateTime tiempoActual = DateTime.Now;
            DateTime tiempoAcumulado = tiempoAparicionImagenAgujero.AddSeconds(GestorJuego.Instancia.TiempoMargen);
            if ((tiempoAcumulado.Subtract(tiempoActual)).Seconds <= 0)
            {
                switch (estadoSeleccionado)
                {
                    case 0:
                        tiempoAgotadoAplastado = false;
                        seleccionarImagen(0);
                        GestorJuego.Instancia.SesionActiva.ImagenTopoAnterior = "agujero";
                        break;
                    case 1:
                        GestorJuego.Instancia.ExisteFallo = true;
                        GestorJuego.Instancia.incrementarFallosSesionActiva();
                        tiempoAgotadoAplastado = false;
                        GestorJuego.Instancia.SesionActiva.ImagenTopoAnterior = "fuera";
                        seleccionarImagen(0);
                        break;
                    case 2:
                        GestorJuego.Instancia.ExisteFallo = false;
                        tiempoAgotadoAplastado = true;
                        GestorJuego.Instancia.SesionActiva.ImagenTopoAnterior = "aplastado";
                        seleccionarImagen(0);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Selecciona una imagen segun la cadena pasada como parametro
        /// </summary>
        /// <param name="estado"></param>
        private void seleccionarImagen(int estado)
        {
            tiempoAparicionUltimaImagen = DateTime.Now;
            switch (estado)
            {
                case 2:
                    this.imagenSeleccionada = imagenes.Count - 1;
                    this.estadoSeleccionado = 2;
                    break;
                case 0:
                    this.imagenSeleccionada = 0;
                    this.estadoSeleccionado = 0;
                    this.tiempoAparicionImagenAgujero = DateTime.Now;
                    break;
                case 1:
                    this.imagenSeleccionada++;
                    this.imagenSeleccionada = imagenSeleccionada % imagenes.Count;
                    this.estadoSeleccionado = 1;
                    break;

            }
        }

        #endregion
    }
}
