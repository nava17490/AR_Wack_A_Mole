using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics;
using GoblinXNA.UI.UI2D;
using GoblinXNA.Device.Capture;
using MoleRehabilitation.LogicaJuego;
using MoleRehabilitation.BD;
using MoleRehabilitation.Kinect;
using Microsoft.Kinect;
using ConfiguracionInterfaz;

namespace MoleRehabilitation.Interfaz
{
    class PantallaJuego : Pantalla
    {
        #region Atributos
        private ContentManager content;
        private SpriteFont fuente;
        private SpriteBatch spriteBatch;
        private Texture2D iconoAciertos;
        private Texture2D iconoFallos;
        private Texture2D marcadorTiempo;
        private G2DPanel panel;
        private G2DLabel aciertos;
        private G2DLabel fallos;
        private PosicionesPantalla posiciones;
        private Texture2D pantallaFueraRango;
        private Texture2D barraTitulo;
        private Texture2D botonAzul;
        private Texture2D botonNaranja;
        private ConexionKinect kinect;
        private Texture2D pantallaErrorKinect;
        #endregion

        #region Constructores

        public PantallaJuego()
            : base()
        {
            this.content = (ContentManager)GestorInterfaz.Instancia.Services.GetService(typeof(ContentManager));
            this.posiciones = (PosicionesPantalla)GestorInterfaz.Instancia.Services.GetService(typeof(PosicionesPantalla));
            this.spriteBatch = (SpriteBatch)GestorInterfaz.Instancia.Services.GetService(typeof(SpriteBatch));
            this.fuente = content.Load<SpriteFont>("Fuentes/numeros");
            this.iconoAciertos = content.Load<Texture2D>("Imagenes/Juego/acierto");
            this.iconoFallos = content.Load<Texture2D>("Imagenes/Juego/fallo");
            this.marcadorTiempo = content.Load<Texture2D>("Imagenes/Juego/marcador");
            this.pantallaFueraRango = content.Load<Texture2D>("PantallasError/fueraRango");
            this.barraTitulo = content.Load<Texture2D>("Imagenes/Menus/barra_titulo");
            this.botonAzul = content.Load<Texture2D>("Imagenes/Menus/boton_azul_grande");
            this.botonNaranja = content.Load<Texture2D>("Imagenes/Menus/boton_naranja_grande");
            this.pantallaErrorKinect = content.Load<Texture2D>("PantallasError/kinect");

            try
            {
                kinect = new ConexionKinect(true, false, true, content.Load<Texture2D>("PantallasError/kinect"));
                kinect.initializeColorImage(new Vector2(System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width, System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height));
                kinect.initializeSkeleton();
                base.Escena.AddVideoCaptureDevice(kinect);
                base.Escena.ShowCameraImage = true;
            }
            catch
            {
                dibujarPantallaErrorKinect();
            }

            GestorJuego j = GestorJuego.Instancia;
            List<Texture2D> imagenes = new List<Texture2D>(8);
            imagenes.Add(content.Load<Texture2D>("Imagenes/Juego/agujero")); 
            imagenes.Add(content.Load<Texture2D>("Imagenes/Juego/topoFuera"));
            imagenes.Add(content.Load<Texture2D>("Imagenes/Juego/topoAplastado"));
            j.cargarImagenes(imagenes, posiciones);
            ConfiguracionBD c = new ConfiguracionBD();
            List<int> l = c.getConfiguracion(j.Paciente);
            j.cargarConfiguracion(l[1], l[2], l[3], l[4]);

            j.iniciarJuego();

            CreateCamera();

            CreateLights();

            CreateUI2D();
        }

        #endregion

        #region metodos sobreescritos

        /// <summary>
        /// Actualiza el contenido en pantalla. Sobrescribe el metodo de la clase padre.
        /// </summary>
        /// <param name="elapsedTime">Instante de tiempo actual</param>
        /// <param name="isRunningSlowly">Indica si la aplicacion se esta ejecutando lentamente</param>
        /// <param name="isFocused">Indica si la ventana esta seleccionada</param>
        public override void Update(TimeSpan elapsedTime, bool isRunningSlowly, bool isFocused)
        {
            GestorJuego g = GestorJuego.Instancia;
            fallos.Text = g.FallosSesionActiva.ToString();
            aciertos.Text = g.AciertosSesionActiva.ToString();
            g.Update();
            base.Escena.Update(elapsedTime, isRunningSlowly, isFocused);
        }

        /// <summary>
        /// Redibuja el contenido de la pantalla. Sobrescribe el metodo de la clase padre para poder dibujar imagenes en 2D con SpriteBatch.
        /// </summary>
        /// <param name="elapsedTime">Instante de tiempo actual</param>
        /// <param name="isRunningSlow">Indica si la aplicacion se esta ejecutando lentamente</param>
        public override void Draw(TimeSpan elapsedTime, bool isRunningSlow)
        {

            if (!kinect.isEnabled())
            {
                dibujarPantallaErrorKinect();
                GestorJuego.Instancia.pausar();
                return;
            }
            else GestorJuego.Instancia.reanudar(false);
            if (GestorJuego.Instancia.EstaEnEjecucion)
            {
                base.Escena.UIRenderer.Add2DComponent(panel);
                base.Escena.Draw(elapsedTime, isRunningSlow);
                spriteBatch.Begin();
                GestorJuego j = GestorJuego.Instancia;                
                spriteBatch.Draw(marcadorTiempo, posiciones.marcador, Color.White);
                spriteBatch.DrawString(fuente, toString(j.TiempoRestante), posiciones.tiempo, Color.White);
                Texture2D imagenTopoIzq = j.TopoIzquierda.Imagen;
                Texture2D imagenTopoDcha = j.TopoDerecha.Imagen;
                spriteBatch.Draw(imagenTopoIzq, j.TopoIzquierda.Rectangulo, Color.White);
                spriteBatch.Draw(imagenTopoDcha, j.TopoDerecha.Rectangulo, Color.White);
                spriteBatch.Draw(iconoAciertos, posiciones.iconoAciertos, Color.White);
                spriteBatch.Draw(iconoFallos, posiciones.iconoFallos, Color.White);
                spriteBatch.End();
                return;
            }
            else
            {
                base.Escena.Draw(elapsedTime, isRunningSlow);
                if (GestorJuego.Instancia.ExistePosicionIncorrecta)
                {
                    dibujarPantallaFueraRango();
                }
            }
        }

        #endregion

        #region metodos publicos
        #endregion

        #region metodos privados



        private void dibujarPantallaErrorKinect()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(pantallaErrorKinect, posiciones.panelParametros, Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// Crea un punto de vista a modo de cámara desde el que se visualizará la imagen
        /// </summary>
        private void CreateCamera()
        {

            GoblinXNA.SceneGraph.Camera camara = new GoblinXNA.SceneGraph.Camera();

            camara.Translation = new Vector3(0, 0, 0);
            camara.FieldOfViewY = MathHelper.ToRadians(60);
            camara.ZFarPlane = 1000;

            CameraNode nodoCamara = new CameraNode(camara);
            base.Escena.RootNode.AddChild(nodoCamara);

            base.Escena.CameraNode = nodoCamara;

        }

        /// <summary>
        /// Crea el punto de luz desde el que se proyectará la misma y el ángulo con el que incidirá
        /// </summary>
        private void CreateLights()
        {
            LightSource fuenteLuz = new LightSource();
            fuenteLuz.Direction = new Vector3(-1, -1, -1);

            fuenteLuz.Diffuse = Color.White.ToVector4();
            fuenteLuz.Specular = new Vector4(0.6f, 0.6f, 0.6f, 1);

            LightNode luz = new LightNode();
            luz.LightSource = fuenteLuz;

            base.Escena.RootNode.AddChild(luz);
        }

        /// <summary>
        /// Crea los elementos de Interfaz de Usuario necesarios para esta pantalla
        /// </summary>
        private void CreateUI2D()
        {

            panel = new G2DPanel();
            panel.Bounds = Rectangle.Empty;
            panel.Transparency = 1.0f;

            aciertos = new G2DLabel(GestorJuego.Instancia.AciertosSesionActiva.ToString());
            aciertos.Bounds = posiciones.aciertosJuego;
            aciertos.TextFont = fuente;
            aciertos.TextColor = Color.Red;

            fallos = new G2DLabel(GestorJuego.Instancia.FallosSesionActiva.ToString());
            fallos.Bounds = posiciones.fallosJuego;
            fallos.TextFont = fuente;
            fallos.TextColor = Color.Red;

            panel.AddChild(aciertos);
            panel.AddChild(fallos);

            base.Escena.UIRenderer.Add2DComponent(panel);
        }

        private void dibujarPantallaFueraRango()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(pantallaFueraRango, posiciones.panelParametros, Color.White);
            spriteBatch.End();
        }

        private String toString(TimeSpan timeSpan)
        {
            String cadena = "";
            cadena = timeSpan.Minutes + ":";
            if (timeSpan.Seconds < 10)
            {
                cadena = cadena + "0";
            }
            cadena = cadena + timeSpan.Seconds;
            return cadena;
        }

        #endregion

        #region Destructores

        ~PantallaJuego() {
            base.Escena.UIRenderer.Remove2DComponent(panel);
            spriteBatch = null;
            //content.Unload();
        }

        #endregion

    }
}
