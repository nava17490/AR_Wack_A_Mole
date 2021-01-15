using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using MoleRehabilitation.BD;
using Microsoft.Kinect;
using MoleRehabilitation.Interfaz;

namespace MoleRehabilitation.LogicaJuego
{
    class Sesion
    {
        #region Atributos
        private int codigoSesion;
        private int aciertos;
        private int fallos;
        private int aciertosAnteriores;
        private int fallosAnteriores;
        private int codigoEjercicio;
        private DateTime tiempoInicio;
        private bool fallosAumentados;
        private String imagenTopoAnterior;
        #endregion

        #region Propiedades
        public int Aciertos
        {
            get
            {
                return this.aciertos +this.aciertosAnteriores;
            }
        }
        public int Fallos
        {
            get
            {
                return this.fallos + this.fallosAnteriores;
            }
        }
        public int Ejercicio
        {
            get
            {
                return this.codigoEjercicio;
            }
        }
        public String ImagenTopoAnterior
        {
            set
            {
                this.imagenTopoAnterior = value;
            }
        }
        #endregion

        #region Constructor

        public Sesion()
        {
            aciertosAnteriores = 0;
            fallosAnteriores = 0;
            aciertos = 0;
            fallos = 0;
        }

        public Sesion(int aciertos, int fallos)
        {
            this.aciertosAnteriores = aciertos;
            this.fallosAnteriores = fallos;
            aciertos = 0;
            fallos = 0;
        }
        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Inicia la sesion 
        /// </summary>
        /// <param name="tiempoInicio"></param>
        public void Iniciar(DateTime tiempoInicio)
        {
            this.tiempoInicio = tiempoInicio;
            guardarDatosInicio();
            fallosAumentados = false;
            imagenTopoAnterior = "agujero";
        }
        
        /// <summary>
        /// Aumenta el numero de fallos
        /// </summary>
        public void aumentarFallos()
        {
            if (fallosAumentados) fallosAumentados = false;
            else if (imagenTopoAnterior == "fuera")
            {
                fallos++;
                guardarDatosFinEjercicio();
                fallosAumentados = true;
                GestorInterfaz.Instancia.playFallo();
            }
        }

        /// <summary>
        /// Aumenta el número de aciertos de la sesion
        /// </summary>
        public void aumentarAciertos()
        {
            aciertos++;
            guardarDatosFinEjercicio();
            GestorInterfaz.Instancia.playAcierto();
        }   

        /// <summary>
        /// Finaliza la sesion en curso
        /// </summary>
        public void finalizarSesion()
        {
            updateSesion();
            GestorJuego.Instancia.ExisteFallo = true;
            updateEjercicio();
        }

        /// <summary>
        /// Guarda los datos de las posiciones registradas por kinect
        /// </summary>
        /// <param name="joints"></param>
        public void almacenarDatosPosiciones(Hashtable joints)
        {
            if (joints != null)
            {
                foreach (JointType j in joints.Keys)
                {
                    if (j.Equals(JointType.FootLeft) || j.Equals(JointType.FootRight))
                    {
                        Console.WriteLine("hola");
                        Joint jt = (Joint)joints[j];
                        PosicionBD p = new PosicionBD();
                        List<Object> l = new List<object>();
                        l.Add(DateTime.Now.Ticks);
                        l.Add(j.ToString());
                        l.Add(jt.Position.X);
                        l.Add(jt.Position.Y);
                        l.Add(jt.Position.Z);
                        l.Add(codigoEjercicio);
                        p.insert(l);
                    }
                }
            }
        }        

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Actualiza los datos de un ejercicio dandolo por finalizado
        /// </summary>
        private void guardarDatosFinEjercicio()
        {
            updateEjercicio();
            fallosAumentados = false;
            crearEjercicio();
        }

        /// <summary>
        /// Crea una nueva sesion en la base de datos
        /// </summary>
        private void crearSesion()
        {
            SesionBD s = new SesionBD();
            List<Object> l = new List<object>();
            l.Add(DateTime.Now.Ticks);
            l.Add(GestorJuego.Instancia.Paciente);
            codigoSesion = s.insert(l);
        }

        /// <summary>
        /// Crea un nuevo ejercicio en la base de datos
        /// </summary>
        private void crearEjercicio()
        {
            EjercicioBD e = new EjercicioBD();
            List<Object> l = new List<object>();
            l.Add(DateTime.Now.Ticks);
            l.Add(codigoSesion);
            codigoEjercicio = e.insert(l);
        }

        /// <summary>
        /// Actualiza un ejercicio en la base de datos
        /// </summary>
        private void updateEjercicio()
        {
            EjercicioBD e = new EjercicioBD();
            List<Object> l = new List<object>();
            l.Add(codigoEjercicio);
            l.Add(GestorJuego.Instancia.TiempoReaccion.Ticks);
            l.Add(DateTime.Now.Ticks);
            l.Add(GestorJuego.Instancia.ExisteFallo);
            e.update(l);
        }

        /// <summary>
        /// Actualiza una sesion en la base de datos
        /// </summary>
        private void updateSesion()
        {
            SesionBD s = new SesionBD();
            List<Object> l = new List<object>();
            l.Add(codigoSesion);
            l.Add(fallos);
            l.Add(aciertos);
            l.Add(DateTime.Now.Ticks);
            s.update(l);
        }        
        
        /// <summary>
        /// Guarda los datos de inicio de una sesion
        /// </summary>
        private void guardarDatosInicio()
        {
            crearSesion();
            crearEjercicio();
        }
        #endregion
    }
}
