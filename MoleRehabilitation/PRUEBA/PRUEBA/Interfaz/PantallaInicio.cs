using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoblinXNA.UI.UI2D;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using GoblinXNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ConfiguracionInterfaz;

namespace MoleRehabilitation.Interfaz
{
    class PantallaInicio : Pantalla
    {
        private G2DLabel titulo;
        private G2DPanel panel;
        private G2DLabel instrucciones;
        private ContentManager content;
        private PosicionesPantalla posiciones;
        private SpriteFont fuenteGrande;
        private SpriteFont fuentePequeña;
        private DateTime tiempoAnterior;

        public PantallaInicio()
            : base()
        {
            this.content = (ContentManager)GestorInterfaz.Instancia.Services.GetService(typeof(ContentManager));
            this.posiciones = (PosicionesPantalla)GestorInterfaz.Instancia.Services.GetService(typeof(PosicionesPantalla));

            this.fuenteGrande = content.Load<SpriteFont>("Fuentes/textosGrandes");
            this.fuentePequeña = content.Load<SpriteFont>("Fuentes/textos");

            CreateLights();

            CreateCamera(); //punto de vista de la imagen

            CreateUI2D();
        }

        public override void Update(TimeSpan elapsedTime, bool isRunningSlowly, bool isFocused)
        {            
            if (Keyboard.GetState().IsKeyDown(Keys.Enter)) GestorInterfaz.Instancia.mostrarPantallaSeleccionPacientes();
            if (tiempoAnterior == null) tiempoAnterior = DateTime.Now;
            if (DateTime.Now.Second >= tiempoAnterior.Second+1) {
                if (instrucciones.TextColor.Equals(Color.Black)) instrucciones.TextColor = Color.Red;
                else if (instrucciones.TextColor.Equals(Color.Red)) instrucciones.TextColor = Color.Black;
            } 
            tiempoAnterior = DateTime.Now;
            base.Update(elapsedTime, isRunningSlowly, isFocused);
        }

        public override void Draw(TimeSpan elapsedTime, bool isRunningSlow)
        {
            base.Draw(elapsedTime, isRunningSlow);
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
            panel.Bounds = posiciones.resolucion;
            panel.Transparency = 1.0f;
            panel.Texture = content.Load<Texture2D>("Imagenes/Menus/pantallaInicio");

            titulo = new G2DLabel("BIENVENIDO A MOLE REHABILITATION"); 
            titulo.Bounds = posiciones.titulo;
            titulo.TextFont = fuentePequeña;
            titulo.Texture = content.Load<Texture2D>("Imagenes/Menus/barra_titulo");
            titulo.TextColor = Color.Black;

            instrucciones = new G2DLabel("PULSE ENTER PARA CONTINUAR");
            instrucciones.Bounds = posiciones.introduccion;
            instrucciones.TextFont = fuenteGrande;
            instrucciones.TextColor = Color.Black;

            panel.AddChild(titulo);
            panel.AddChild(instrucciones);

            base.Escena.UIRenderer.Add2DComponent(panel);
            
        }
    }
}
