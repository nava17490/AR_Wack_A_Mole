using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ConfiguracionInterfaz;
using MoleRehabilitation.BD;
using MoleRehabilitation.Interfaz;
using Microsoft.Kinect;

namespace MoleRehabilitation.LogicaJuego
{
    class GestorJuego
    {

        #region Atributos

        private static GestorJuego instancia;
        private Topo[] topos;
        private DateTime tiempoInicio;
        private DateTime tiempoInicioDescanso;
        private DateTime tiempoReaccion;
        private int tiempoMargen;
        private int duracionSesion;
        private int tiempoDescanso;
        private int numeroSesiones;
        private Sesion sesionActiva;
        private int idSesionActiva;
        private String paciente;
        private bool colision;
        private bool fallo;
        private bool posicionIncorrecta;
        private bool enEjecucion;
        private Random numAleatorio;
        private Joint pieIzqAnterior;
        private Joint pieDchoAnterior;
        private Hashtable joints;
        private bool tiempoReaccionGuardado;
        private bool estaIniciado;
        private DateTime tiempoIncrementoFallos;
        private bool toposMostrados;
        private string ultimaVez;
        private int numApariciones;
        #endregion

        #region Properties
        public static GestorJuego Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new GestorJuego();
                    return instancia;
                }
                return instancia;
            }
        }
        public Topo TopoIzquierda
        {
            get
            {
                return topos[0];
            }
        }
        public Topo TopoDerecha
        {
            get
            {
                return this.topos[1];
            }
        }
        public int TiempoMargen
        {
            get
            {
                return this.tiempoMargen;
            }
        }
        public TimeSpan TiempoRestante
        {
            get
            {
                DateTime tiempo = tiempoInicio;
                tiempo = tiempo.AddMinutes(duracionSesion);
                return tiempo.Subtract(DateTime.Now);
            }
        }
        public TimeSpan TiempoRestanteDescanso
        {
            get
            {
                DateTime tiempo = tiempoInicioDescanso;
                tiempo = tiempo.AddMinutes(tiempoDescanso);
                return tiempo.Subtract(DateTime.Now);
            }
        }
        public int AciertosSesionActiva
        {
            get
            {
                return sesionActiva.Aciertos;
            }
        }
        public int FallosSesionActiva
        {
            get
            {
                return sesionActiva.Fallos;
            }
        }
        public int TiempoJuego
        {
            get
            {
                return duracionSesion * numeroSesiones;
            }
        }
        public DateTime TiempoReaccion
        {
            get
            {
                return this.tiempoReaccion;
            }
        }
        public String Paciente
        {
            get
            {
                return this.paciente;
            }
            set
            {
                this.paciente = value;
            }
        }
        public bool ExisteColision
        {
            get
            {
                return colision;
            }
        }
        public bool ExisteFallo
        {
            get
            {
                return this.fallo;
            }
            set
            {
                this.fallo = value;
            }
        }
        public bool ExistePosicionIncorrecta
        {
            get
            {
                return this.posicionIncorrecta;
            }
            set
            {
                this.posicionIncorrecta = value;
                this.enEjecucion = value;
            }
        }
        public bool EstaEnEjecucion
        {
            get
            {
                return this.enEjecucion;
            }
        }
        public Hashtable Joints
        {
            set
            {
                this.joints = value;
            }
        }
        public Sesion SesionActiva
        {
            get
            {
                return this.sesionActiva;
            }
        }
        #endregion

        #region Constructores

        /// <summary>
        /// Crea un gestor para todos los eventos que puedan suceder en el juego
        /// </summary>
        private GestorJuego()
        {
            enEjecucion = true;
            topos = new Topo[2];
            numAleatorio = new Random(DateTime.Now.Millisecond);
            tiempoReaccionGuardado = false;
            joints = new Hashtable();
            foreach (JointType jt in Enum.GetValues(typeof(JointType))) {
                Joint j = new Joint();
                j.Position = new SkeletonPoint();
                j.TrackingState = JointTrackingState.Tracked;
                joints.Add(jt,j);
            }
            ultimaVez = "izq";
            sesionActiva = new Sesion();
            sesionActiva.almacenarDatosPosiciones(joints);
        }
        #endregion

        #region metodos publicos

        /// <summary>
        /// Pausa el juego en el estado actual
        /// </summary>
        public void pausar()
        {
            tiempoInicioDescanso = DateTime.Now;
            enEjecucion = false;
        }

        /// <summary>
        /// Reanuda el juego desde el estado en el que se dejo
        /// </summary>
        /// <param name="iniciarSesion"></param>
        public void reanudar(bool iniciarSesion)
        {
            enEjecucion = true;
            if (iniciarSesion)
                sesionActiva.Iniciar(DateTime.Now);
            tiempoIncrementoFallos = DateTime.Now;
        }
        
        /// <summary>
        /// Carga en el juego las imagenes con las que se mostraran los topos en sus diferentes estados
        /// </summary>
        /// <param name="imagenes"></param>
        /// <param name="posiciones"></param>
        public void cargarImagenes(List<Texture2D> imagenes, PosicionesPantalla posiciones)
        {
            topos[0] = new Topo(posiciones.topoIzq);
            topos[1] = new Topo(posiciones.topoDcha);
            topos[0].cargarImagenes(imagenes);
            topos[1].cargarImagenes(imagenes);
        }
        
        /// <summary>
        /// Carga la configuracion en memoria para poder gestionar el juego en base a ella
        /// </summary>
        /// <param name="numSesiones">marca el numero de sesiones que se van a realizar.</param>
        /// <param name="margenTiempo">marca el margen de tiempo que se dará al jugador desde que el topo ha salido completamente hasta que se contabilice como un fallo si no se lo pisa</param>
        /// <param name="tiempoDescanso">marca el tiempo de descanso entre sesiones</param>
        /// <param name="duracionSesion">marca la duracion de cada una de las sesiones</param>
        public void cargarConfiguracion(int numSesiones, int margenTiempo, int tiempoDescanso, int duracionSesion)
        {
            this.tiempoMargen = margenTiempo;
            this.duracionSesion = duracionSesion;
            this.tiempoDescanso = tiempoDescanso;
            this.numeroSesiones = numSesiones;
        }

        /// <summary>
        /// Inicia el juego guardando el instante de tiempo en que este ha empezado
        /// </summary>
        public void iniciarJuego()
        {
            this.tiempoInicio = DateTime.Now;
            if (!estaIniciado)
            {
                sesionActiva = new Sesion();
                idSesionActiva = 1;
            }
            sesionActiva.Iniciar(tiempoInicio);
            mostrarToposAleatoriamente();
            estaIniciado = true;
        }        

        /// <summary>
        /// Actualiza el juego comprobando si se ha pisado al topo, si se ha acabado el tiempo de la sesion y actuando en consecuencia
        /// </summary>
        public void Update()
        {
            if (!enEjecucion) return;
            if (TiempoRestante.Seconds<=0 && TiempoRestante.Minutes<=0)
            {
                sesionActiva.finalizarSesion();                
                if (idSesionActiva >= numeroSesiones)
                    GestorInterfaz.Instancia.mostrarPantallaEstadisticas();
                else
                {
                    idSesionActiva++;
                    sesionActiva = new Sesion(AciertosSesionActiva, FallosSesionActiva);
                    GestorInterfaz.Instancia.mostrarPantallaDescanso();
                }
                pausar();
            }
            else
            {
                if (pieDchoAnterior == null || pieIzqAnterior == null)
                {
                    this.pieDchoAnterior = (Joint)joints[JointType.FootRight];
                    this.pieIzqAnterior = (Joint)joints[JointType.FootLeft];
                }
                else if (!tiempoReaccionGuardado)
                {                    
                    if (equals(pieIzqAnterior.Position.X, ((Joint)joints[JointType.FootLeft]).Position.X) || equals(pieDchoAnterior.Position.X, ((Joint)joints[JointType.FootLeft]).Position.X))
                    {
                        this.tiempoReaccion = DateTime.Now;
                        sesionActiva.almacenarDatosPosiciones(joints);
                        tiempoReaccionGuardado = true;
                    }
                }
                Joint pieIzq = (Joint)joints[JointType.FootLeft];
                Vector3 posicionPieIzq = new Vector3(pieIzq.Position.X, pieIzq.Position.Y, pieIzq.Position.Z);
                Joint pieDcho = (Joint)joints[JointType.FootRight];
                Vector3 posicionPieDcho = new Vector3(pieDcho.Position.X, pieDcho.Position.Y, pieDcho.Position.Z);

                if (topos[0].colisiona(posicionPieIzq))
                {
                    aplastadoTopo(0);
                } 
                else if (topos[1].colisiona(posicionPieDcho))
                {
                    aplastadoTopo(1);            
                } 
                if (ExisteFallo)
                {
                    mostrarToposAleatoriamente();
                    ExisteFallo = false;
                } 
                else if (ExisteColision && (TopoIzquierda.AgotadoTiempoAplastado || TopoDerecha.AgotadoTiempoAplastado))
                {
                    mostrarToposAleatoriamente();
                    ExisteFallo = false;
                }
                if (!TopoDerecha.EstaAplastado && !TopoDerecha.EstaFuera && !TopoIzquierda.EstaAplastado && !TopoIzquierda.EstaFuera) mostrarToposAleatoriamente();
            }
        }

        /// <summary>
        /// Finaliza el juego
        /// </summary>
        public void finalizar()
        {
            this.estaIniciado = false;
            String paciente = this.paciente;
            instancia = new GestorJuego();
            this.paciente = paciente;
        }

        /// <summary>
        /// Incrementa los fallos de la sesion que se esta jugando
        /// </summary>
        public void incrementarFallosSesionActiva()
        {
            /*DateTime tiempoActual = DateTime.Now;
            DateTime tiempoAcumulado = tiempoIncrementoFallos.AddSeconds(1);
            Console.WriteLine("tiempoAnterior: " + tiempoIncrementoFallos.ToUniversalTime() + "tiempoActual: " + tiempoActual.ToUniversalTime());
            if ((tiempoAcumulado.Subtract(tiempoActual)).Milliseconds <= 0) {
            */    sesionActiva.aumentarFallos();
            //}
        }

        /// <summary>
        /// Incrementa los aciertos de la sesion que se esta jugando
        /// </summary>
        public void incrementarAciertosSesionActiva()
        {
            /*DateTime tiempoActual = DateTime.Now;
            DateTime tiempoAcumulado = tiempoIncrementoFallos.AddSeconds(1);
            if ((tiempoAcumulado.Subtract(tiempoActual)).Milliseconds <= 0)
            {
            */    sesionActiva.aumentarAciertos();
            //}
        }

        /// <summary>
        /// muestra aleatoriamente un topo u otro
        /// </summary>
        public void mostrarToposAleatoriamente()
        {
            //numAleatorio = new Random((int)DateTime.Now.Ticks);
            if (toposMostrados) toposMostrados=false;
            else
            {
                int num = numAleatorio.Next();
                num = num % 2;
                if (ultimaVez == "izq" && num == 0 && numApariciones >= 3) num = 1;
                if (ultimaVez == "dcha" && num == 0 && numApariciones >= 3) num = 0;
                switch (num)
                {
                    case 0:
                        topos[0].mostrar();
                        topos[1].esconder();
                        if (ultimaVez == "izq") numApariciones++;
                        else
                            numApariciones = 0;
                        Console.WriteLine(numApariciones + "izq");
                        ultimaVez = "izq";
                        break;
                    case 1:
                        topos[1].mostrar();
                        topos[0].esconder();
                        if (ultimaVez == "dcha") numApariciones++;
                        else
                            numApariciones = 0;
                        Console.WriteLine(numApariciones + " derecha");
                        ultimaVez = "dcha";
                        break;
                }
                sesionActiva.almacenarDatosPosiciones(joints);
                toposMostrados=true;
            }
        } 

        #endregion

        #region metodos privados

        /// <summary>
        /// compara dos puntos y determina si son identicos teniendo en cuenta un rango de error
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private bool equals(float p1, float p2)
        {
            if (p1 - p2 > 1 || p1 - p2 < 1)
                return true;
            else return false;
        } 

        /// <summary>
        /// Es llamado cuando un topo cuyo indice es el parametro es aplastado
        /// </summary>
        private void aplastadoTopo(int i)
        {
            if (topos[i].EstaFuera)
            {
                this.ExisteFallo = false;
                this.incrementarAciertosSesionActiva();
                this.colision = true;
                topos[i].aplastar();
                sesionActiva.almacenarDatosPosiciones(joints);                
            }
        }      

        #endregion
    }
}
