using System;
using System.Collections.Generic;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using GoblinXNA.UI.UI2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MoleRehabilitation.BD;
using MoleRehabilitation.LogicaJuego;
using ConfiguracionInterfaz;

namespace MoleRehabilitation.Interfaz
{
    class PantallaConfiguracion : Pantalla
    {
        #region Atributos
        private ContentManager content;
        private SpriteFont fuente;
        private G2DPanel panel;
        private G2DLabel titulo;
        private G2DLabel panelParametros;
        private G2DLabel numeroSesiones;
        private G2DLabel duracionSesion;
        private G2DLabel segundosMargen;
        private G2DLabel tiempoDescanso;
        private G2DLabel resultadoNumeroSesiones;
        private G2DLabel resultadoDuracionSesion;
        private G2DLabel resultadoSegundosMargen;
        private G2DLabel resultadoTiempoDescanso;
        private G2DButton incrementarSesiones;
        private G2DButton decrementarSesiones;
        private G2DButton incrementarDuracionSesion;
        private G2DButton decrementarDuracionSesion;
        private G2DButton incrementarTiempoMargen;
        private G2DButton decrementarTiempoMargen;
        private G2DButton incrementarTiempoDescanso;
        private G2DButton decrementarTiempoDescanso;
        private G2DButton continuar;
        private G2DButton volver;
        private int codigoConfiguracion;
        private PosicionesPantalla posiciones;
        #endregion

        #region Constructores

        /// <summary>
        /// Crea una nueva pantalla de configuracion.
        /// </summary>
        public PantallaConfiguracion()
            : base()
        {
            this.content = (ContentManager)GestorInterfaz.Instancia.Services.GetService(typeof(ContentManager));
            this.posiciones = (PosicionesPantalla)GestorInterfaz.Instancia.Services.GetService(typeof(PosicionesPantalla));
            this.fuente = content.Load<SpriteFont>("Fuentes/textos");

            CreateLights();

            CreateCamera();

            CreateUI2D();

        }

        #endregion

        #region metodos publicos

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
            ConfiguracionBD c = new ConfiguracionBD();
            List<int> datos = c.getConfiguracion(GestorJuego.Instancia.Paciente);
            if (datos == null) GestorInterfaz.Instancia.mostrarPantallaSeleccionPacientes();
            this.codigoConfiguracion = datos[0];

            panel = new G2DPanel();
            panel.Bounds = posiciones.resolucion;
            panel.Transparency = 1.0f;
            panel.Texture = content.Load<Texture2D>("Imagenes/Menus/fondo");

            titulo = new G2DLabel("     CONFIGURACIÓN");
            titulo.Bounds = posiciones.titulo;
            titulo.TextFont = fuente;
            titulo.Texture = content.Load<Texture2D>("Imagenes/Menus/barra_titulo");
            titulo.TextColor = Color.Black;

            panelParametros = new G2DLabel();
            panelParametros.Bounds = posiciones.panelParametros;
            panelParametros.Texture = content.Load<Texture2D>("Imagenes/Menus/fondo_config_estad");

            numeroSesiones = new G2DLabel("NÚMERO DE SESIONES");
            numeroSesiones.Bounds = posiciones.numeroSesiones;
            numeroSesiones.TextFont = fuente;
            numeroSesiones.TextColor = Color.Black;

            decrementarSesiones = new G2DButton();
            decrementarSesiones.Bounds = posiciones.decrementarNumeroSesiones;
            decrementarSesiones.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(decrementarSesiones_ActionPerformedEvent);
            decrementarSesiones.Texture = content.Load<Texture2D>("Imagenes/Menus/menos");
            decrementarSesiones.DrawBorder = false;
            
            resultadoNumeroSesiones = new G2DLabel(datos[1].ToString());
            resultadoNumeroSesiones.Bounds = posiciones.resultadoNumeroSesiones;
            resultadoNumeroSesiones.TextFont = fuente;
            resultadoNumeroSesiones.TextColor = Color.Black;

            incrementarSesiones = new G2DButton();
            incrementarSesiones.Bounds = posiciones.incrementarNumeroSesiones;
            incrementarSesiones.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(incrementarSesiones_ActionPerformedEvent);
            incrementarSesiones.Texture = content.Load<Texture2D>("Imagenes/Menus/mas");
            incrementarSesiones.DrawBorder = false;

            duracionSesion = new G2DLabel("DURACIÓN DE SESIÓN (MINUTOS)");
            duracionSesion.Bounds = posiciones.duracionSesion;
            duracionSesion.TextFont = fuente;
            duracionSesion.TextColor = Color.Black;

            decrementarDuracionSesion = new G2DButton();
            decrementarDuracionSesion.Bounds = posiciones.decrementarDuracionSesion;
            decrementarDuracionSesion.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(decrementarDuracionSesion_ActionPerformedEvent);
            decrementarDuracionSesion.Texture = content.Load<Texture2D>("Imagenes/Menus/menos");
            decrementarDuracionSesion.DrawBorder = false;

            resultadoDuracionSesion = new G2DLabel(datos[4].ToString());
            resultadoDuracionSesion.Bounds = posiciones.resultadoDuracionSesion;
            resultadoDuracionSesion.TextFont = fuente;
            resultadoDuracionSesion.TextColor = Color.Black;

            incrementarDuracionSesion = new G2DButton();
            incrementarDuracionSesion.Bounds = posiciones.incrementarDuracionSesion;
            incrementarDuracionSesion.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(incrementarDuracionSesion_ActionPerformedEvent);
            incrementarDuracionSesion.Texture = content.Load<Texture2D>("Imagenes/Menus/mas");
            incrementarDuracionSesion.DrawBorder = false;

            segundosMargen = new G2DLabel("TIEMPO HASTA FALLO (SEGUNDOS)");
            segundosMargen.Bounds = posiciones.tiempoMargen;
            segundosMargen.TextFont = fuente;
            segundosMargen.TextColor = Color.Black;

            decrementarTiempoMargen = new G2DButton();
            decrementarTiempoMargen.Bounds = posiciones.decrementarTiempoMargen;
            decrementarTiempoMargen.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(decrementarTiempoMargen_ActionPerformedEvent);
            decrementarTiempoMargen.Texture = content.Load<Texture2D>("Imagenes/Menus/menos");
            decrementarTiempoMargen.DrawBorder = false;

            resultadoSegundosMargen = new G2DLabel(datos[2].ToString());
            resultadoSegundosMargen.Bounds = posiciones.resultadoTiempoMargen;
            resultadoSegundosMargen.TextFont = fuente;
            resultadoSegundosMargen.TextColor = Color.Black;

            incrementarTiempoMargen = new G2DButton();
            incrementarTiempoMargen.Bounds = posiciones.incrementarTiempoMargen;
            incrementarTiempoMargen.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(incrementarTiempoMargen_ActionPerformedEvent);
            incrementarTiempoMargen.Texture = content.Load<Texture2D>("Imagenes/Menus/mas");
            incrementarTiempoMargen.DrawBorder = false;

            tiempoDescanso = new G2DLabel("TIEMPO DE DESCANSO (MINUTOS)");
            tiempoDescanso.Bounds = posiciones.tiempoDescanso;
            tiempoDescanso.TextFont = fuente;
            tiempoDescanso.TextColor = Color.Black;

            decrementarTiempoDescanso = new G2DButton();
            decrementarTiempoDescanso.Bounds = posiciones.decrementarTiempoDescanso;
            decrementarTiempoDescanso.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(decrementarTiempoDescanso_ActionPerformedEvent);
            decrementarTiempoDescanso.Texture = content.Load<Texture2D>("Imagenes/Menus/menos");
            decrementarTiempoDescanso.DrawBorder = false;

            resultadoTiempoDescanso = new G2DLabel(datos[3].ToString());
            resultadoTiempoDescanso.Bounds = posiciones.resultadoTiempoDescanso;
            resultadoTiempoDescanso.TextFont = fuente;
            resultadoTiempoDescanso.TextColor = Color.Black;

            incrementarTiempoDescanso = new G2DButton();
            incrementarTiempoDescanso.Bounds = posiciones.incrementarTiempoDescanso;
            incrementarTiempoDescanso.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(incrementarTiempoDescanso_ActionPerformedEvent);
            incrementarTiempoDescanso.Texture = content.Load<Texture2D>("Imagenes/Menus/mas");
            incrementarTiempoDescanso.DrawBorder = false;

            continuar = new G2DButton("JUGAR");
            continuar.Bounds = posiciones.botonContinuar;
            continuar.TextFont = fuente;
            continuar.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(continuar_ActionPerformedEvent);
            continuar.Texture = content.Load<Texture2D>("Imagenes/Menus/boton_azul_pequeño");
            continuar.TextColor = Color.Black;

            volver = new G2DButton("VOLVER");
            volver.Bounds = posiciones.botonMedio;
            volver.TextFont = fuente;
            volver.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(volver_ActionPerformedEvent);
            volver.Texture = content.Load<Texture2D>("Imagenes/Menus/boton_naranja_pequeño");
            volver.TextColor = Color.Black;

            panel.AddChild(titulo);
            panel.AddChild(panelParametros);
            panel.AddChild(numeroSesiones);
            panel.AddChild(decrementarSesiones);
            panel.AddChild(resultadoNumeroSesiones);
            panel.AddChild(incrementarSesiones);
            panel.AddChild(duracionSesion);
            panel.AddChild(decrementarDuracionSesion);
            panel.AddChild(resultadoDuracionSesion);
            panel.AddChild(incrementarDuracionSesion);
            panel.AddChild(segundosMargen);
            panel.AddChild(decrementarTiempoMargen);
            panel.AddChild(resultadoSegundosMargen);
            panel.AddChild(incrementarTiempoMargen);
            panel.AddChild(tiempoDescanso);
            panel.AddChild(decrementarTiempoDescanso);
            panel.AddChild(resultadoTiempoDescanso);
            panel.AddChild(incrementarTiempoDescanso);
            panel.AddChild(continuar);
            panel.AddChild(volver);

            base.Escena.UIRenderer.Add2DComponent(panel);

        }

        void volver_ActionPerformedEvent(object source)
        {
            guardarDatos();
            GestorInterfaz.Instancia.mostrarPantallaMenuPrincipal();
        }

        /// <summary>
        /// Manejador del Evento de pulsar sobre el boton para incrementar el tiempo de descanso
        /// </summary>
        /// <param name="source"></param>
        private void incrementarTiempoDescanso_ActionPerformedEvent(object source)
        {
            int valor = Convert.ToInt32(resultadoTiempoDescanso.Text);
            valor++;
            if (valor > 99) valor = 99;
            resultadoTiempoDescanso.Text = valor.ToString();
        }

        /// <summary>
        /// Manejador del Evento de pulsar sobre el boton para decrementar el tiempo de descanso
        /// </summary>
        /// <param name="source"></param>
        private void decrementarTiempoDescanso_ActionPerformedEvent(object source)
        {
            int valor = Convert.ToInt32(resultadoTiempoDescanso.Text);
            valor--;
            if (valor <= 0) valor = 1;
            resultadoTiempoDescanso.Text = valor.ToString();
        }

        /// <summary>
        /// Manejador del Evento de pulsar sobre el boton para decrementar el tiempo de margen
        /// </summary>
        /// <param name="source"></param>
        private void decrementarTiempoMargen_ActionPerformedEvent(object source)
        {
            int valor = Convert.ToInt32(resultadoSegundosMargen.Text);
            valor--;
            if (valor <= 0) valor = 1;
            resultadoSegundosMargen.Text = valor.ToString();
        }

        /// <summary>
        /// Manejador del Evento de pulsar sobre el boton para incrementar el tiempo de margen
        /// </summary>
        /// <param name="source"></param>
        private void incrementarTiempoMargen_ActionPerformedEvent(object source)
        {
            int valor = Convert.ToInt32(resultadoSegundosMargen.Text);
            valor++;
            if (valor > 99) valor = 99;
            resultadoSegundosMargen.Text = valor.ToString();
        }

        /// <summary>
        /// Manejador del Evento de pulsar sobre el boton para incrementar la duracion de cada sesion
        /// </summary>
        /// <param name="source"></param>
        private void incrementarDuracionSesion_ActionPerformedEvent(object source)
        {
            int valor = Convert.ToInt32(resultadoDuracionSesion.Text);
            valor++;
            if (valor > 99) valor = 99;
            resultadoDuracionSesion.Text = valor.ToString();
        }

        /// <summary>
        /// Manejador del Evento de pulsar sobre el boton para decrementar la duracion de cada sesion
        /// </summary>
        /// <param name="source"></param>
        private void decrementarDuracionSesion_ActionPerformedEvent(object source)
        {
            int valor = Convert.ToInt32(resultadoDuracionSesion.Text);
            valor--;
            if (valor <= 0) valor = 1;
            resultadoDuracionSesion.Text = valor.ToString();
        }

        /// <summary>
        /// Manejador del Evento de pulsar sobre el boton para incrementar el numero de sesiones
        /// </summary>
        /// <param name="source"></param>
        private void incrementarSesiones_ActionPerformedEvent(object source)
        {
            int valor = Convert.ToInt32(resultadoNumeroSesiones.Text);
            valor++;
            if (valor > 99) valor = 99;
            resultadoNumeroSesiones.Text = valor.ToString();
        }

        /// <summary>
        /// Manejador del Evento de pulsar sobre el boton para decrementar el numero de sesiones
        /// </summary>
        /// <param name="source"></param>
        private void decrementarSesiones_ActionPerformedEvent(object source)
        {
            int valor = Convert.ToInt32(resultadoNumeroSesiones.Text);
            valor--;
            if (valor <= 0) valor = 1;
            resultadoNumeroSesiones.Text = valor.ToString();
        }

        /// <summary>
        /// Manejador del evento de pulsar sobre el boton continuar. Carga la pantalla de juego guardando la configuracion.
        /// </summary>
        /// <param name="source"></param>
        private void continuar_ActionPerformedEvent(object source)
        {
            guardarDatos();
            GestorInterfaz.Instancia.mostrarPantallaJuego();
        }

        /// <summary>
        /// Guarda los datos de la configuracion en la base de datos
        /// </summary>
        private void guardarDatos()
        {
            List<int> datos = new List<int>();
            datos.Add(codigoConfiguracion);
            datos.Add(Convert.ToInt32(resultadoNumeroSesiones.Text));
            datos.Add(Convert.ToInt32(resultadoSegundosMargen.Text));
            datos.Add(Convert.ToInt32(resultadoTiempoDescanso.Text));
            datos.Add(Convert.ToInt32(resultadoDuracionSesion.Text));
            ConfiguracionBD c = new ConfiguracionBD();
            c.update(datos);
        }

        

        #endregion

        #region Destructores
        ~PantallaConfiguracion() {
            base.Escena.UIRenderer.Remove2DComponent(panel);
            content.Unload();
        }
        #endregion

    }
}
