using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using GoblinXNA.UI.UI2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MoleRehabilitation.LogicaJuego;
using ConfiguracionInterfaz;
using System;

namespace MoleRehabilitation.Interfaz
{
    class PantallaMenuPrincipal : Pantalla
    {
        #region Atributos
        private SpriteFont fuente;
        private ContentManager content;
        private G2DPanel panel;
        private G2DLabel saludo;
        private G2DButton jugar;
        private G2DButton estadisticas;
        private G2DButton salir;
        private G2DButton volver;
        private PosicionesPantalla posiciones;
        #endregion

        #region Constructores

        public PantallaMenuPrincipal()
            : base()
        {
            this.content = (ContentManager)GestorInterfaz.Instancia.Services.GetService(typeof(ContentManager));
            this.posiciones = (PosicionesPantalla)GestorInterfaz.Instancia.Services.GetService(typeof(PosicionesPantalla));
            this.fuente = content.Load<SpriteFont>("Fuentes/textos");

            CreateLights();

            CreateCamera(); //punto de vista de la imagen

            CreateUI2D();
        }
        
        #endregion

        #region metodos sobrescritos
        public override void Update(TimeSpan elapsedTime, bool isRunningSlowly, bool isFocused)
        {
            base.Escena.Update(elapsedTime, isRunningSlowly, isFocused);
        }
        #endregion

        #region metodos privados

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

            saludo = new G2DLabel("     BIENVENIDO " + GestorJuego.Instancia.Paciente + " ¿QUÉ QUIERES HACER?");
            saludo.Bounds = posiciones.titulo;
            saludo.TextFont = fuente;
            saludo.TextColor = Color.Black;
            saludo.Texture = content.Load<Texture2D>("Imagenes/Menus/barra_titulo");

            jugar = new G2DButton("JUGAR");
            jugar.Bounds = posiciones.botonJugar;
            jugar.TextFont = fuente;
            jugar.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(jugar_ActionPerformedEvent);
            jugar.Texture = content.Load<Texture2D>("Imagenes/Menus/boton_azul_grande");
            jugar.TextColor = Color.Black;

            volver = new G2DButton("OTRO PACIENTE");
            volver.Bounds = posiciones.botonOtroPaciente;
            volver.TextFont = fuente;
            volver.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(volver_ActionPerformedEvent);
            volver.Texture = content.Load<Texture2D>("Imagenes/Menus/boton_azul_grande");
            volver.TextColor = Color.Black;

            estadisticas = new G2DButton("VER ESTADÍSTICAS");
            estadisticas.Bounds = posiciones.botonEstadisticas;
            estadisticas.TextFont = fuente;
            estadisticas.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(estadisticas_ActionPerformedEvent);
            estadisticas.Texture = content.Load<Texture2D>("Imagenes/Menus/boton_azul_grande");
            estadisticas.TextColor = Color.Black;

            salir = new G2DButton("SALIR");
            salir.Bounds = posiciones.botonSalir;
            salir.TextFont = fuente;
            salir.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(salir_ActionPerformedEvent);
            salir.Texture = content.Load<Texture2D>("Imagenes/Menus/boton_naranja_grande");
            salir.TextColor = Color.Black;

            panel.AddChild(saludo);
            panel.AddChild(jugar);
            panel.AddChild(estadisticas);
            panel.AddChild(salir);
            panel.AddChild(volver);

            
            base.Escena.UIRenderer.Add2DComponent(panel);
            
        }

        private void volver_ActionPerformedEvent(object source)
        {
            GestorInterfaz.Instancia.mostrarPantallaSeleccionPacientes();
        }

        /// <summary>
        /// Manejador del evento de pulsar sobre el boton salir. Cierra la aplicacion.
        /// </summary>
        /// <param name="source"></param>
        private void salir_ActionPerformedEvent(object source)
        {
            GestorInterfaz.Instancia.Exit();
        }

        /// <summary>
        /// Manejador del evento de pulsar sobre el boton estadisticas. Carga la pantalla de estadisticas.
        /// </summary>
        /// <param name="source"></param>
        private void estadisticas_ActionPerformedEvent(object source)
        {
            GestorInterfaz.Instancia.mostrarPantallaEstadisticas();
        }

        /// <summary>
        /// Manejador del evento de pulsar sobre el boton jugar. Carga la pantalla de seleccion de paciente.
        /// </summary>
        /// <param name="source"></param>
        private void jugar_ActionPerformedEvent(object source)
        {
            GestorInterfaz.Instancia.mostrarPantallaConfiguracion();
        }
            
        #endregion

        #region Destructores
        #endregion

    }
}
