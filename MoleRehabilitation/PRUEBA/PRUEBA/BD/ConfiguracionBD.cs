using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using MoleRehabilitation.DAO;

namespace MoleRehabilitation.BD
{
    /// <summary>
    /// Clase que implementa los metodos de ConfiguracionDAO con una base de datos
    /// </summary>
    class ConfiguracionBD : ConfiguracionDAO
    {

        #region Constructores

        /// <summary>
        /// Crea una instancia para poder hacer cambios en la configuracion almacenada en la base de datos
        /// </summary>
        public ConfiguracionBD()
        {

        }

        #endregion

        #region Métodos

        /// <summary>
        /// Devuelve la ultima configuracion de un paciente
        /// </summary>
        /// <param name="paciente"></param>
        /// <returns></returns>
        public List<int> getConfiguracion(string paciente)
        {
            GestorBD gestorBD = GestorBD.Instancia;
            gestorBD.conectar();
            MySqlDataReader lector = gestorBD.query("SELECT codigo, numSesiones, margenTiempo, tiempoDescanso, duracionSesion FROM configuracion WHERE paciente='" + paciente + "';");
            List<int> datos = new List<int>();
            int i = 0;
            if (lector.Read())
            {
                for (i = 0; i < 5; i++)
                    datos.Add(lector.GetInt32(i));
            }
            gestorBD.desconectar();
            return datos;
        }

        /// <summary>
        /// Inserta una nueva configuracion en la base de datos
        /// </summary>
        /// <param name="obj">Datos de la configuracion a insertar segun el formato (String nombrePaciente, int numeroSesiones, int margenTiempo, int tiempoDescanso, int duracionSesion)</param>
        /// <returns></returns>
        public int insert(object obj)
        {
            Object[] l = (Object[])obj;
            GestorBD gestorBD = GestorBD.Instancia;
            gestorBD.conectar();
            gestorBD.nonQuery("INSERT INTO configuracion VALUES (null, " + (int)l[1] + ", " + (int)l[2] + ", " + (int)l[3] + ", " + (int)l[4] + ", '" + l[0].ToString() + "');");
            gestorBD.desconectar();
            return getConfiguracion(l[0].ToString())[0]; 
        }

        /// <summary>
        /// Modifica una configuracion ya almacenada
        /// </summary>
        /// <param name="obj">Valores a modificar</param>
        /// <returns></returns>
        public int update(object obj)
        {
            List<int> l = (List<int>)obj;
            GestorBD gestorBD = GestorBD.Instancia;
            gestorBD.conectar();
            gestorBD.nonQuery("UPDATE configuracion SET numSesiones = '" + l[1] + "', margenTiempo ='" + l[2] + "', tiempoDescanso ='" + l[3] + "', duracionSesion ='" + l[4] + "' WHERE codigo = '" + l[0] + "'");
            gestorBD.desconectar();
            return 0;
        }

        #endregion
    }
}
