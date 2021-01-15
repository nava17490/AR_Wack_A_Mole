using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using GoblinXNA;
using MoleRehabilitation.LogicaJuego;
using ConfiguracionInterfaz;


namespace MoleRehabilitation.Interfaz
{
    /// <summary>
    /// Clase Principal de la interfaz del programa. Cumple con el patron Singleton.
    /// </summary>
    public class GestorInterfaz : Microsoft.Xna.Framework.Game
    {
        #region Atributos

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Pantalla pantallaSeleccionada;
        private Song musica;
        private static GestorInterfaz instancia;
        private PosicionesPantalla posiciones;
        private bool enReproduccion;
        private SoundEffect acierto;
        private SoundEffect fallo;

        #endregion

        #region Propiedades

        /// <summary>
        /// Instancia de la clase para que cumpla con el patron Singleton
        /// </summary>
        public static GestorInterfaz Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new GestorInterfaz();
                    return instancia;
                }
                return instancia;
            }
        }

        #endregion

        #region Constructores

        /// <summary>
        /// Crea un nuevo Gestor para la Interfaz del Juego que manejara las diferentes pantallas de la aplicacion
        /// </summary>
        private GestorInterfaz()
        {
            graphics = new GraphicsDeviceManager(this);

            if (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width == 1366 && System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height == 768)
            {
                graphics.PreferredBackBufferWidth = 1366;
                graphics.PreferredBackBufferHeight = 768;
            }
            else if (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width >= 1024 && System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height >= 768)
            {
                graphics.PreferredBackBufferWidth = 1024;
                graphics.PreferredBackBufferHeight = 768;
            }
            
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            //this.IsFixedTimeStep = true;
            //this.TargetElapsedTime = new TimeSpan(1000000);
            Services.AddService(typeof(ContentManager), Content);
        }

        #endregion

        #region metodos sobreescritos

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            State.InitGoblin(graphics, Content, "");
            State.ThreadOption = 4;

            //State.ShowFPS = true;

            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            spriteBatch = new SpriteBatch(GraphicsDevice);
            musica = Content.Load<Song>("Musica/fondo");
            this.acierto = Content.Load<SoundEffect>("Sonidos/acierto");
            this.fallo = Content.Load<SoundEffect>("Sonidos/fallo"); 
            if (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width == 1366 && System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height == 768)
            {
                posiciones = Content.Load<PosicionesPantalla>("posiciones(" + 1366 + "x" + 768 + ")");
            }
            else if (System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width >= 1024 && System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height >= 768)
            {
                posiciones = Content.Load<PosicionesPantalla>("posiciones(" + 1024 + "x" + 768 + ")");
            }
            Services.AddService(typeof(PosicionesPantalla), posiciones);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
            this.Window.Title = "MoleRehabilitation";
            this.mostrarPantallaInicio();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            pantallaSeleccionada.Update(gameTime.ElapsedGameTime, gameTime.IsRunningSlowly, true);
            if (!enReproduccion)
            {
                MediaPlayer.Play(musica);
                MediaPlayer.IsRepeating = true;
                enReproduccion = true;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            pantallaSeleccionada.Draw(gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);
        }

        #endregion

        #region metodos publicos


        public void playAcierto()
        {
            acierto.Play();
        }

        public void playFallo()
        {
            fallo.Play();
        }

        /// <summary>
        /// Muestra el menu principal en la pantalla de la aplicacion
        /// </summary>
        public void mostrarPantallaMenuPrincipal()
        {
            pantallaSeleccionada = new PantallaMenuPrincipal();
            this.IsMouseVisible = true;
        }

        private void mostrarPantallaInicio()
        {
            pantallaSeleccionada = new PantallaInicio();
            this.IsMouseVisible = false;
        }

        /// <summary>
        /// Muestra la pantalla para seleccionar un paciente en la pantalla de la aplicacion
        /// </summary>
        public void mostrarPantallaSeleccionPacientes()
        {
            pantallaSeleccionada = new PantallaSeleccionPaciente();
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Muestra la pantalla de configuracion en la pantalla de la aplicacion
        /// </summary>
        public void mostrarPantallaConfiguracion()
        {
            pantallaSeleccionada = new PantallaConfiguracion();
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Muestra la pantalla de juego de la aplicacion
        /// </summary>
        public void mostrarPantallaJuego()
        {
            pantallaSeleccionada = new PantallaJuego();
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Muestra la pantalla de Estadisticas de la aplicacion
        /// </summary>
        public void mostrarPantallaEstadisticas()
        {
            pantallaSeleccionada = new PantallaEstadisticas();
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Muestra la pantalla de descanso entre sesiones de la aplicacion
        /// </summary>
        public void mostrarPantallaDescanso()
        {
            pantallaSeleccionada = new PantallaDescanso();
            this.IsMouseVisible = false;
        }

        #endregion

        #region metodos privados
        #endregion
    }
}
