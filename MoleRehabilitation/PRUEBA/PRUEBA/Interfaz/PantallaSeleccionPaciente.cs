using System;
using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using GoblinXNA.UI.UI2D;
using GoblinXNA.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoleRehabilitation.BD;
using MoleRehabilitation.LogicaJuego;
using ConfiguracionInterfaz;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace MoleRehabilitation.Interfaz
{
    class PantallaSeleccionPaciente : Pantalla
    {
        #region Atributos
        private ContentManager content;
        private SpriteFont fuente;
        private G2DList lista;
        private G2DTextField texto;
        private G2DPanel panel;
        private G2DButton continuar;
        private G2DButton guardar;
        private G2DButton salir;
        private G2DLabel textoTitulo;
        private G2DLabel error;
        private G2DScrollBar scroll;
        private PosicionesPantalla posiciones;
        private KeyboardState estadoAnteriorTeclado;
        #endregion

        #region Constructores

        /// <summary>
        /// Crea una nueva pantalla de seleccion de paciente
        /// </summary>
        public PantallaSeleccionPaciente()
            : base()
        {
            this.content = (ContentManager)GestorInterfaz.Instancia.Services.GetService(typeof(ContentManager));
            this.posiciones = (PosicionesPantalla)GestorInterfaz.Instancia.Services.GetService(typeof(PosicionesPantalla));
            this.fuente = content.Load<SpriteFont>("Fuentes/textos");

            CreateLights();

            CreateCamera(); //punto de vista de la imagen

            CreateUI2D();

            estadoAnteriorTeclado = Keyboard.GetState();
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

            panel = new G2DPanel();
            panel.Bounds = posiciones.resolucion;
            panel.Transparency = 1.0f;
            panel.Texture = content.Load<Texture2D>("Imagenes/Menus/fondo");

            PacientesBD p = new PacientesBD();
            String[] elementos = p.getPacientes().ToArray();
            lista = new G2DList(elementos);
            lista.TextFont = fuente;
            lista.Bounds = posiciones.listaPacientes;
            lista.SelectionModel.SetSelectionInterval(0, 0);
            lista.Texture = content.Load<Texture2D>("Imagenes/Menus/lista_pacientes");
            lista.SelectionBackgroundColor = Color.DarkOrange;
            


            //EL SCROLL NO FUNCIONA PERO LO HARA EN CUANTO SE ACTUALICE LA LIBRERIA QUE USO PARA IU
            scroll = new G2DScrollBar(GoblinEnums.Orientation.Vertical);
            scroll.Bounds=posiciones.scroll;
            scroll.Parent = lista;
            scroll.Visible = true;

            continuar = new G2DButton("AceptarPaciente");
            continuar.Bounds = posiciones.botonContinuar;
            continuar.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(continuar_ActionPerformedEvent);
            continuar.Text = "CONTINUAR";
            continuar.TextFont = fuente;
            continuar.TextColor = Color.Black;
            continuar.Texture = content.Load<Texture2D>("Imagenes/Menus/boton_azul_pequeño"); ;

            texto = new G2DTextField();
            texto.Bounds = posiciones.campoTexto;
            texto.TextFont = fuente;
            texto.VerticalAlignment = GoblinEnums.VerticalAlignment.Center;
            texto.HorizontalAlignment = GoblinEnums.HorizontalAlignment.Left;
            texto.Texture = content.Load<Texture2D>("Imagenes/Menus/campo_texto");

            guardar = new G2DButton();
            guardar.Bounds = posiciones.botonGuardar;
            guardar.Texture = content.Load<Texture2D>("Imagenes/Menus/disquete");
            guardar.TextFont = fuente;
            guardar.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(guardar_ActionPerformedEvent);

            salir = new G2DButton("SALIR");
            salir.Bounds = posiciones.botonVolver;
            salir.Text = "SALIR";
            salir.TextFont = fuente;
            salir.ActionPerformedEvent += new GoblinXNA.UI.ActionPerformed(salir_ActionPerformedEvent);
            salir.Texture = content.Load<Texture2D>("Imagenes/Menus/boton_naranja_pequeño");
            salir.TextColor = Color.Black;

            textoTitulo = new G2DLabel("     SELECCIONE UN PACIENTE");         
            textoTitulo.Bounds = posiciones.titulo;
            textoTitulo.TextFont = fuente;
            textoTitulo.Texture = content.Load<Texture2D>("Imagenes/Menus/barra_titulo");
            textoTitulo.TextColor = Color.Black;

            error = new G2DLabel();
            error.Bounds = posiciones.etiquetaError;
            error.TextFont = fuente;
            error.TextColor = Color.Black;

            panel.AddChild(lista);
            panel.AddChild(continuar);
            panel.AddChild(texto);
            panel.AddChild(guardar);
            panel.AddChild(salir);
            panel.AddChild(textoTitulo);
            panel.AddChild(error);
            //panel.AddChild(scroll);

            base.Escena.UIRenderer.Add2DComponent(panel);
        }

        /// <summary>
        /// Maneja el evento de pulsar sobre el boton salir de la pantalla. Sale de la aplicacion
        /// </summary>
        /// <param name="source"></param>
        private void salir_ActionPerformedEvent(object source)
        {
            GestorInterfaz.Instancia.Exit();
        }

        /// <summary>
        /// Maneja el evento de pulsar sobre el boton guardar de la pantalla. Guarda el paciente en la base de datos
        /// </summary>
        /// <param name="source"></param>
        private void guardar_ActionPerformedEvent(object source)
        {
            if (!this.texto.Text.Equals(""))
            {
                String paciente = texto.Text;
                PacientesBD p = new PacientesBD();
                p.insert(paciente);
                Object[] datos = { paciente, 1, 1, 1, 1 };
                ConfiguracionBD c = new ConfiguracionBD();
                c.insert(datos);
                CreateUI2D();
                error.Text = "PACIENTE GUARDADO CON ÉXITO";
            }
            else
                error.Text = "TECLEE UN NUMERO PARA GUARDAR";
        }

        /// <summary>
        /// Maneja el evento de pulsar sobre el boton continuar de la pantalla. Carga la pantalla de configuracion
        /// </summary>
        /// <param name="source"></param>
        private void continuar_ActionPerformedEvent(object source)
        {
            String paciente = "";
            if (lista.SelectionModel.SelectedIndices.Count == 0)
                error.Text = "Seleccione un elemento";
            else
            {
                foreach (int indice in lista.SelectionModel.SelectedIndices)
                {
                    paciente = (String)lista.Model.Elements[indice]; //OBTENER ELEMENTO SELECCIONADO
                }
                GestorJuego.Instancia.Paciente = paciente;
                GestorInterfaz.Instancia.mostrarPantallaMenuPrincipal();
            }
        }

        #endregion

        #region Metodos Sobrescritos
        public override void Update(TimeSpan elapsedTime, bool isRunningSlowly, bool isFocused)
        {
            KeyboardState estadoActual = Keyboard.GetState();
            
            if (estadoAnteriorTeclado.IsKeyUp(Keys.Delete) && estadoActual.IsKeyDown(Keys.Delete) && lista.SelectionModel.SelectedIndices.Count != 0)
            {
                PacientesBD p = new PacientesBD();
                p.delete((String)lista.Model.Elements[lista.SelectionModel.SelectedIndices[0]]);
                CreateUI2D();
                error.Text = "PACIENTE DADO DE BAJA CON ÉXITO";
            }
            if (lista.SelectionModel.SelectedIndices.Count == 0)
            {
                error.Text = "SELECCIONE UN PACIENTE";
            }
            
            estadoAnteriorTeclado = estadoActual;
            
            base.Update(elapsedTime, isRunningSlowly, isFocused);
        }
        #endregion

        #region Destructores
        ~PantallaSeleccionPaciente() {
            base.Escena.UIRenderer.Remove2DComponent(panel);
        }
        #endregion

    }
}
