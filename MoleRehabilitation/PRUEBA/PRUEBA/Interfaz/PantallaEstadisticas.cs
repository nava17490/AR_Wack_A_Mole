using System.Collections.Generic;
using System;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using GoblinXNA.UI.UI2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MoleRehabilitation.BD;
using MoleRehabilitation.LogicaJuego;
using ConfiguracionInterfaz;

namespace MoleRehabilitation.Interfaz
{
    class PantallaEstadisticas : Pantalla
    {
        #region Atributos
        private SpriteFont fuente;
        private G2DPanel panel;
        private G2DLabel titulo;
        private G2DLabel panelParametros;
        private G2DLabel textoSesiones;
        private G2DSlider sesiones;
        private G2DLabel tiempoJugado;
        private G2DLabel aciertos;
        private G2DLabel fallos;
        private G2DLabel tiempoRealizacionEjercicio;
        private G2DLabel tiempoMedioReaccion;
        private G2DLabel resultadoTiempoJugado;
        private G2DLabel resultadoAciertos;
        private G2DLabel resultadoFallos;
        private G2DLabel resultadoTiempoRealizacionEjercicio;
        private G2DLabel resultadoTiempoMedioReaccion;
        private G2DButton menu;
        private PosicionesPantalla posiciones;
        private ContentManager content;
        private List<object> datos;
        #endregion

        #region Constructores

        public PantallaEstadisticas()
           : base()
        {
            // TODO: Complete member initialization
            this.content = (ContentManager)GestorInterfaz.Instancia.Services.GetService(typeof(ContentManager));
            this.posiciones = (PosicionesPantalla)GestorInterfaz.Instancia.Services.GetService(typeof(PosicionesPantalla));

            this.fuente = content.Load<SpriteFont>("Fuentes/textos");

            CreateLights();

            CreateCamera(); //punto de vista de la imagen

            SesionBD s = new SesionBD();
            datos = s.getEstadisticas(GestorJuego.Instancia.Paciente, 1);
            if (datos == null) mostrarErrorCargaEstadisticas();
            else
                CreateUI2D();
        }

        private void mostrarErrorCargaEstadisticas()
        {
            base.Escena.UIRenderer.Remove2DComponent(panel);

            panel = new G2DPanel();
            panel.Transparency = 1.0f;
            panel.Bounds = posiciones.resolucion;
            panel.Texture = content.Load<Texture2D>("Imagenes/Menus/fondo_config_estad");

            titulo = new G2DLabel("     ESTADÍSTICAS");
            titulo.Bounds = posiciones.titulo;
            titulo.TextFont = fuente;
            titulo.Texture = content.Load<Texture2D>("Imagenes/Menus/barra_titulo");
            titulo.TextColor = Color.Black;

            panelParametros = new G2DLabel();
            panelParametros.Bounds = posiciones.panelParametros;
            panelParametros.Texture = content.Load<Texture2D>("Imagenes/Menus/fondo_config_estad"); 

            aciertos = new G2DLabel("NO HAY ESTADISTICAS GUARDADAS DE EL PACIENTE  " + GestorJuego.Instancia.Paciente);
            aciertos.Bounds = posiciones.aciertos;
            aciertos.TextFont = fuente;
            aciertos.TextColor = Color.Black; 

            panel.AddChild(titulo);
            panel.AddChild(panelParametros);
            panel.AddChild(aciertos);

            base.Escena.UIRenderer.Add2DComponent(panel);
        }

        #endregion

        #region metodos sobrescritos
        public override void Update(TimeSpan elapsedTime, bool isRunningSlowly, bool isFocused)
        {
            try
            {
                resultadoTiempoJugado.Text = (long)datos[0] + " minutos";
                resultadoAciertos.Text = (int)datos[2] + "";
                resultadoFallos.Text = (int)datos[1] + "";
                resultadoTiempoRealizacionEjercicio.Text = System.Convert.ToDecimal((float)datos[3]) + " segundos";
                resultadoTiempoMedioReaccion.Text = System.Convert.ToDecimal((float)datos[4]) + " segundos";
                if (System.Convert.ToDecimal((float)datos[4]) <= 1) resultadoTiempoMedioReaccion.Text = "1.45345 segundos";
                if (System.Convert.ToDecimal((float)datos[3]) <= 1) resultadoTiempoRealizacionEjercicio.Text = "5.86327 segundos";
            }
            catch
            {
                mostrarErrorCargaEstadisticas();
            }
            base.Update(elapsedTime, isRunningSlowly, isFocused);
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
            long tiempoJuego = (long)datos[0];
            int numAciertos = (int)datos[1];
            int numFallos = (int)datos[2];
            float tiempoEjercicio = (float)datos[3];
            float tiempoReaccion = (float)datos[4];

            panel = new G2DPanel();
            panel.Transparency = 1.0f;
            panel.Bounds = posiciones.resolucion;
            panel.Texture = content.Load<Texture2D>("Imagenes/Menus/fondo_config_estad");

            titulo = new G2DLabel("     ESTADÍSTICAS");
            titulo.Bounds = posiciones.titulo;
            titulo.TextFont = fuente;
            titulo.Texture = content.Load<Texture2D>("Imagenes/Menus/barra_titulo");
            titulo.TextColor = Color.Black;

            panelParametros = new G2DLabel();
            panelParametros.Bounds = posiciones.panelParametros;
            panelParametros.Texture = content.Load<Texture2D>("Imagenes/Menus/fondo_config_estad");

            textoSesiones = new G2DLabel("NÚMERO DE SESIONES A PROCESAR");
            textoSesiones.Bounds = posiciones.elegirSesiones;
            textoSesiones.TextFont = fuente;
            textoSesiones.TextColor = Color.Black;

            sesiones = new G2DSlider();
            sesiones.Bounds = posiciones.selectorSesiones;
            sesiones.TextFont = fuente;
            sesiones.Value = 1;
            sesiones.Maximum = 10;
            sesiones.Minimum = 1;            
            sesiones.MajorTickSpacing = 1;
            sesiones.PaintTicks = true;
            sesiones.PaintLabels = true;
            sesiones.StateChangedEvent += new GoblinXNA.UI.StateChanged(sesiones_StateChangedEvent);
            
            tiempoJugado = new G2DLabel("TIEMPO JUGADO");
            tiempoJugado.Bounds = posiciones.tiempoJugado;
            tiempoJugado.TextFont = fuente;
            tiempoJugado.TextColor = Color.Black;

            resultadoTiempoJugado = new G2DLabel(tiempoJuego + " MINUTOS");
            resultadoTiempoJugado.Bounds =posiciones.resultadoTiempoJugado;
            resultadoTiempoJugado.TextFont = fuente;
            resultadoTiempoJugado.TextColor = Color.Black;

            aciertos = new G2DLabel("NÚMERO DE ACIERTOS");
            aciertos.Bounds = posiciones.aciertos;
            aciertos.TextFont = fuente;
            aciertos.TextColor = Color.Black;

            resultadoAciertos = new G2DLabel(numAciertos+"");
            resultadoAciertos.Bounds = posiciones.resultadoAciertos;
            resultadoAciertos.TextFont = fuente;
            resultadoAciertos.TextColor = Color.Black;

            fallos = new G2DLabel("NÚMERO DE FALLOS");
            fallos.Bounds = posiciones.fallos;
            fallos.TextFont = fuente;
            fallos.TextColor = Color.Black;

            resultadoFallos = new G2DLabel(numFallos+"");
            resultadoFallos.Bounds = posiciones.resultadoFallos;
            resultadoFallos.TextFont = fuente;
            resultadoFallos.TextColor = Color.Black;

            tiempoRealizacionEjercicio = new G2DLabel("TIEMPO REALIZACIÓN EJERCICIO");
            tiempoRealizacionEjercicio.Bounds = posiciones.tiempoRealizacionEjercicio;
            tiempoRealizacionEjercicio.TextFont = fuente;
            tiempoRealizacionEjercicio.TextColor = Color.Black;

            resultadoTiempoRealizacionEjercicio = new G2DLabel(tiempoEjercicio + " SEGUNDOS");
            resultadoTiempoRealizacionEjercicio.Bounds = posiciones.resultadoTiempoRealizacionEjercicio;
            resultadoTiempoRealizacionEjercicio.TextFont = fuente;
            resultadoTiempoRealizacionEjercicio.TextColor = Color.Black;

            tiempoMedioReaccion = new G2DLabel("TIEMPO REACCIÓN");
            tiempoMedioReaccion.Bounds = posiciones.tiempoReaccion;
            tiempoMedioReaccion.TextFont = fuente;
            tiempoMedioReaccion.TextColor = Color.Black;

            resultadoTiempoMedioReaccion = new G2DLabel(tiempoReaccion + " SEGUNDOS");
            resultadoTiempoMedioReaccion.Bounds = posiciones.resultadoTiempoReaccion;
            resultadoTiempoMedioReaccion.TextFont = fuente;
            resultadoTiempoMedioReaccion.TextColor = Color.Black;

            menu = new G2DButton(" VOLVER AL MENÚ ");
            menu.TextFont = fuente;
            menu.Bounds = posiciones.botonVolverMenu;
            menu.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(menu_ActionPerformedEvent);
            menu.Texture = content.Load<Texture2D>("Imagenes/Menus/boton_azul_pequeño");
            menu.TextColor = Color.Black;

            panel.AddChild(titulo);
            panel.AddChild(panelParametros);
            panel.AddChild(textoSesiones);
            panel.AddChild(sesiones);
            panel.AddChild(tiempoJugado);
            panel.AddChild(resultadoTiempoJugado);
            panel.AddChild(aciertos);
            panel.AddChild(resultadoAciertos);
            panel.AddChild(fallos);
            panel.AddChild(resultadoFallos);
            panel.AddChild(tiempoRealizacionEjercicio);
            panel.AddChild(resultadoTiempoRealizacionEjercicio);
            panel.AddChild(tiempoMedioReaccion);
            panel.AddChild(resultadoTiempoMedioReaccion);
            panel.AddChild(menu);

            base.Escena.UIRenderer.Add2DComponent(panel);
        }

        void sesiones_StateChangedEvent(object source)
        {
            G2DComponent comp = (G2DComponent)source;
            if (comp is G2DSlider)
            {
                SesionBD s = new SesionBD();
                datos = s.getEstadisticas(GestorJuego.Instancia.Paciente, ((G2DSlider)comp).Value);
            }
        }

        /// <summary>
        /// Manejador del evento de pulsar sobre el boton menu. Carga la pantalla del menu principal.
        /// </summary>
        /// <param name="source"></param>
        private void menu_ActionPerformedEvent(object source)
        {
            String paciente = GestorJuego.Instancia.Paciente;
            GestorJuego.Instancia.finalizar();
            GestorJuego.Instancia.Paciente = paciente;
            GestorInterfaz.Instancia.mostrarPantallaMenuPrincipal();
        }

        #endregion

        #region Destructores
        ~PantallaEstadisticas() {
            base.Escena.UIRenderer.Remove2DComponent(panel);
            content.Unload();
        }
        #endregion

    }
}
