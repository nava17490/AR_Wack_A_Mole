using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GoblinXNA.UI.UI2D;
using GoblinXNA.SceneGraph;
using GoblinXNA.Shaders;
using GoblinXNA.Graphics;
using ConfiguracionInterfaz;
using MoleRehabilitation.LogicaJuego;

namespace MoleRehabilitation.Interfaz
{
    class PantallaDescanso : Pantalla
    {

        #region Atributos
        private ContentManager content;
        private SpriteFont fuenteTextos;
        private SpriteFont fuenteNumeros;
        private PosicionesPantalla posiciones;
        private G2DPanel panel;
        private G2DLabel titulo;
        private G2DLabel marcador;
        #endregion

        #region Constructores
        public PantallaDescanso()
        {
            this.content = (ContentManager)GestorInterfaz.Instancia.Services.GetService(typeof(ContentManager));
            this.posiciones = (PosicionesPantalla)GestorInterfaz.Instancia.Services.GetService(typeof(PosicionesPantalla));
            this.fuenteNumeros = content.Load<SpriteFont>("Fuentes/numerosGrandes");
            this.fuenteTextos = content.Load<SpriteFont>("Fuentes/textos");

            CreateLights();

            CreateCamera();

            CreateUI2D();

        }
        #endregion

        #region Métodos Públicos
        public override void Update(TimeSpan elapsedTime, bool isRunningSlowly, bool isFocused)
        {
            TimeSpan tiempoRestante = GestorJuego.Instancia.TiempoRestanteDescanso;
            this.marcador.Text = toString(tiempoRestante);
            base.Update(elapsedTime, isRunningSlowly, isFocused);

            if (tiempoRestante.Seconds < 0)
            {
                GestorJuego.Instancia.reanudar(true);
                GestorInterfaz.Instancia.mostrarPantallaJuego();
            }
        }
        #endregion

        #region Métodos Privados

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
            panel.Bounds = posiciones.resolucion;
            panel.Transparency = 1.0f;
            panel.Texture = content.Load<Texture2D>("Imagenes/Menus/fondo");

            titulo = new G2DLabel("     DESCANSO");
            titulo.Bounds = posiciones.titulo;
            titulo.TextFont = fuenteTextos;
            titulo.Texture = content.Load<Texture2D>("Imagenes/Menus/barra_titulo");
            titulo.TextColor = Color.Black;

            TimeSpan tiempoRestante = GestorJuego.Instancia.TiempoRestanteDescanso;
            marcador = new G2DLabel(toString(tiempoRestante));
            marcador.Bounds = posiciones.marcadorDescanso;
            marcador.TextFont = fuenteNumeros;
            marcador.TextColor = Color.Red;

            panel.AddChild(titulo);
            panel.AddChild(marcador);

            base.Escena.UIRenderer.Add2DComponent(panel);
            posiciones = null;

        }

        private string toString(TimeSpan timeSpan)
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
    }
}
