using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoleRehabilitation.DAO;
using MySql.Data.MySqlClient;

namespace MoleRehabilitation.BD
{
    /// <summary>
    /// Clase que implementa los métodos de EjercicioDAO con una base de datos
    /// </summary>
    class EjercicioBD : EjercicioDAO
    {

        #region Constructores

        /// <summary>
        /// Crea una instancia para poder almacenar ejercicios en la base de datos
        /// </summary>
        public EjercicioBD()
        {

        }

        #endregion

        #region Métodos

        /// <summary>
        /// Inserta los datos de un ejercicio en la base de datos
        /// </summary>
        /// <param name="obj">Lista de parametros a insertar segun el formato ()</param>
        /// <returns></returns>
        public int insert(object obj)
        {
            List<Object> l = (List<Object>)obj;
            GestorBD gestorBD = GestorBD.Instancia;
            int codigo = this.getCodigoMax();
            codigo++;
            gestorBD.conectar();
            gestorBD.nonQuery("INSERT INTO ejercicio VALUES (" + codigo + ", " + (long)l[0] + ", " + (int)l[1] + ", " + (long)l[0] + ", " + (long)l[0] + ", " + false + ");");
            gestorBD.desconectar();
            return codigo; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int getCodigoMax()
        {
            GestorBD gestorBD = GestorBD.Instancia;
            gestorBD.conectar();
            MySqlDataReader lector = gestorBD.query("SELECT MAX(codigo) FROM ejercicio");

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int update(object obj)
        {
            List<Object> l = (List<Object>)obj;
            GestorBD gestorBD = GestorBD.Instancia;
            gestorBD.conectar();
            gestorBD.nonQuery("UPDATE ejercicio SET tiempoReaccion = " + (long)l[1] + ", tiempoFinal = " + (long)l[2] + ", esAcierto = " + l[3] + " WHERE codigo = " + (int)l[0] +";");
            gestorBD.desconectar();
            return 0; 
        }

        #endregion
    }
}
