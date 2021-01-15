using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using GoblinXNA.UI.UI2D;

namespace RehabilitationMenu
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Scene escena;
        private G2DPanel panel;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

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
            State.ThreadOption = 2;
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.ApplyChanges();
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
            escena = new Scene();

            escena.BackgroundColor = Color.CornflowerBlue;
            CreateLights();

            CreateCamera();

            CreateUI2D();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            // TODO: Add your update logic here
            escena.Update(gameTime.ElapsedGameTime, gameTime.IsRunningSlowly, this.IsActive);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            escena.Draw(gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);
            base.Draw(gameTime);
        }

        #region

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
            escena.RootNode.AddChild(nodoCamara);

            escena.CameraNode = nodoCamara;

        }

        /// <summary>
        /// Crea el punto de luz desde el que se proyectará la misma y el ángulo con el que incidirá
        /// </summary>
        private void CreateLights()
        {
            LightSource fuenteLuz = new LightSource();
            fuenteLuz.Direction = new Vector3(-1, -1, -1);

            fuenteLuz.Diffuse = Color.Black.ToVector4();
            fuenteLuz.Specular = new Vector4(0.6f, 0.6f, 0.6f, 1);

            LightNode luz = new LightNode();
            luz.LightSource = fuenteLuz;

            escena.RootNode.AddChild(luz);
        }

        /// <summary>
        /// Crea los elementos de Interfaz de Usuario necesarios para esta pantalla
        /// </summary>
        private void CreateUI2D()
        {

            panel = new G2DPanel();
            panel.Bounds = new Rectangle(0,0,1024,768);
            panel.Transparency = 1.0f;
            SpriteFont fuente = Content.Load<SpriteFont>("fuente");

            G2DLabel titulo = new G2DLabel("Menu de diferentes aplicaciones");
            titulo.Bounds = new Rectangle(50,50,900,100);
            titulo.TextFont = fuente;
            titulo.TextColor = Color.Black;

            G2DLabel subtitulo = new G2DLabel("Pulse sobre la aplicacion a ejecutar");
            subtitulo.Bounds = new Rectangle(100, 100, 900, 100);
            subtitulo.TextFont = fuente;
            subtitulo.TextColor = Color.Black;

            G2DButton app1 = new G2DButton("App1");
            app1.Bounds = new Rectangle(200,210,200,200);
            app1.TextFont = fuente;
            app1.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(app1_ActionPerformedEvent);
            app1.TextColor = Color.Black;

            G2DButton app2 = new G2DButton("App2");
            app2.Bounds = new Rectangle(650, 210, 200, 200);
            app2.TextFont = fuente;
            app2.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(app2_ActionPerformedEvent);
            app2.TextColor = Color.Black;

            G2DButton app3 = new G2DButton("App3");
            app3.Bounds = new Rectangle(200, 450, 200, 200);
            app3.TextFont = fuente;
            app3.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(app3_ActionPerformedEvent);
            app3.TextColor = Color.Black;

            G2DButton app4 = new G2DButton("App4");
            app4.Bounds = new Rectangle(650, 450, 200, 200);
            app4.TextFont = fuente;
            app4.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(app4_ActionPerformedEvent);
            app4.TextColor = Color.Black;

            panel.AddChild(titulo);
            panel.AddChild(subtitulo);
            panel.AddChild(app1);
            panel.AddChild(app2);
            panel.AddChild(app3);
            panel.AddChild(app4);

            escena.UIRenderer.Add2DComponent(panel);
            
        }

        void app4_ActionPerformedEvent(object source)
        {
            Process.Start("calc");
        }

        void app3_ActionPerformedEvent(object source)
        {
            Process.Start("C:\\Users\\Usuario\\Dropbox\\Proyecto_Fin_Carrera\\AplicacionFinal\\MoleRehabilitation\\PRUEBA\\PRUEBA\\bin\\x86\\Debug\\MoleRehabilitation.exe");
        }

        void app2_ActionPerformedEvent(object source)
        {
            Process.Start("notepad.exe");
        }

        void app1_ActionPerformedEvent(object source)
        {
            Process.Start("cmd.exe");
        }

        #endregion

    }
}
