using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using MoleRehabilitation.DAO;
using MoleRehabilitation.LogicaJuego;

namespace MoleRehabilitation.BD
{
    /// <summary>
    /// CLASE SIN TERMINAR DE IMPLEMENTAR
    /// </summary>
    class SesionBD : SesionDAO
    {
        #region Atributos

        #endregion

        #region Constructores

        public SesionBD()
        {

        }

        #endregion

        #region Métodos

        public int insert(object obj)
        {
            List<Object> l = (List<Object>)obj;
            GestorBD gestorBD = GestorBD.Instancia;
            int codigo = this.getCodigoMax();
            codigo++;
            gestorBD.conectar();
            gestorBD.nonQuery("INSERT INTO sesion VALUES (" + codigo + "," + 0 + ", " + 0 + ", '" + l[1].ToString() + "', " + (long)l[0] + ", " + (long)l[0] + ");");
            gestorBD.desconectar();
            return codigo; 
        }

        private int  getCodigoMax()
        {
            GestorBD gestorBD = GestorBD.Instancia;
            gestorBD.conectar();
            MySqlDataReader lector = gestorBD.query("SELECT MAX(codigo) FROM sesion");
            if (lector.Read())
            {
                int codigo;
                try
                {
                    codigo = (int)lector.GetInt32(0);
                }
                catch
                {
                    codigo = 1;
                }
                finally
                {
                    gestorBD.desconectar();
                }
                return codigo;
            }
            else
            {
                gestorBD.desconectar();
                return 1;
            }
        }

        public int update(object obj)
        {
            List<Object> l = (List<Object>)obj;
            GestorBD gestorBD = GestorBD.Instancia;
            gestorBD.conectar();
            gestorBD.nonQuery("UPDATE sesion SET fallos = " + (int)l[1] + ", aciertos = " + (int)l[2] + ", tiempoFinal = " + (long)l[3] + " WHERE codigo = " + (int)l[0] + ";");
            gestorBD.desconectar();
            return 0; 
        }

        public List<object> getEstadisticas(string paciente, int numSesiones)
        {
            List<List<object>> sesiones = getUltimasSesiones(paciente, numSesiones);
            if (sesiones == null) return null;
            int aciertos = 0;
            int fallos = 0;
            long tiempoJugado = 0;
            float tiempoEjercicio = 0;
            float tiempoReaccion = 0;
            numSesiones = 0;
            foreach (List<object> sesion in sesiones)
            {
                tiempoJugado += (Int64)sesion[3];
                tiempoEjercicio += (float)sesion[4];
                tiempoReaccion += (float)sesion[5];
                aciertos += (int)sesion[1];
                fallos += (int)sesion[2];
                numSesiones++;
            }
            tiempoReaccion = tiempoReaccion / numSesiones;
            tiempoReaccion = (tiempoReaccion / 10000000);
            tiempoEjercicio = tiempoEjercicio / numSesiones;
            tiempoEjercicio = (tiempoEjercicio/10000000);
            if (tiempoEjercicio > GestorJuego.Instancia.TiempoMargen || tiempoEjercicio<1.0f) tiempoEjercicio = GestorJuego.Instancia.TiempoMargen-(0.2568f*GestorJuego.Instancia.TiempoMargen);
            //if (tiempoReaccion > GestorJuego.Instancia.TiempoMargen) tiempoReaccion = GestorJuego.Instancia.TiempoMargen - (0.9025f * GestorJuego.Instancia.TiempoMargen);
            if (tiempoReaccion < tiempoEjercicio) tiempoReaccion =tiempoEjercicio-0.735f;
            tiempoJugado = ((tiempoJugado / 10000000) / 60)+1;
            List<object> datosEnvio = new List<object> { tiempoJugado, aciertos, fallos, tiempoEjercicio, tiempoReaccion };
            return datosEnvio;
        }

        private List<List<object>> getUltimasSesiones(string paciente, int numSesiones)
        {
            GestorBD g = GestorBD.Instancia;
            List<List<object>> sesiones = new List<List<object>>();
            g.conectar();
            MySqlDataReader lector = g.query("SELECT S.codigo, S.fallos, S.aciertos, S.tiempoFinal-S.tiempoInicio, AVG(E.tiempoReaccion), AVG(E.tiempoFinal - E.tiempoInicio) FROM ejercicio E, sesion S WHERE E.sesion = S.codigo AND S.paciente ='" + paciente + "' GROUP BY S.codigo ORDER BY S.tiempoFinal DESC LIMIT " + numSesiones + ";");
            if (lector.FieldCount == 0 || lector.RecordsAffected == 0) return null;
            while (lector.Read())
            {
                List<object> sesion = new List<object>();
                sesion.Add(lector.GetInt32(0));//idSesion
                sesion.Add(lector.GetInt32(1));//aciertos
                sesion.Add(lector.GetInt32(2));//fallos
                sesion.Add(lector.GetInt64(3));//duracion
                sesion.Add(lector.GetFloat(4));//tiempoReaccion
                sesion.Add(lector.GetFloat(5));//tiempoEjercicio
                sesiones.Add(sesion);
            }
            g.desconectar();
            return sesiones;
        }

        #endregion

        #region metodos privados

        #endregion

        #region destructores

        ~SesionBD()
        {

        }

        #endregion        
    }
}
