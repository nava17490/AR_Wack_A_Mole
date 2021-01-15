using System;
using System.Data;
using MySql.Data.MySqlClient;
using ConfiguracionInterfaz;
using MoleRehabilitation.Interfaz;

namespace MoleRehabilitation.BD
{
    /// <summary>
    /// Clase que realiza las operaciones con una base de datos MySQL
    /// </summary>
    class GestorBD
    {
        #region Atributos
        private static GestorBD instancia;
        private String cadenaConexion;
        private MySqlConnection conexion;
        private DatosBD datos;

        #endregion

        #region Constructores
        
        /// <summary>
        /// Crea una instancia del Gestor con los datos por defecto
        /// </summary>
        private GestorBD()
        {
            datos = GestorInterfaz.Instancia.Content.Load<DatosBD>("configuracionBD");
            cadenaConexion = "Server=" + datos.servidor + ";User id=" + datos.usuario + ";Database=" + datos.baseDatos + ";Password=" + datos.password + ";";
        }

        public static GestorBD Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new GestorBD();
                    return instancia;
                }
                return instancia;
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Crea una conexion con la base de datos segun los parametros del constructor
        /// </summary>
        public void conectar()
        {
            conexion = new MySqlConnection(cadenaConexion);
            conexion.Open();
        }

        /// <summary>
        /// Cierra una conexion con la base de datos.
        /// </summary>
        public void desconectar()
        {
            conexion.Close();
        }

        /// <summary>
        /// Realiza una consulta a la base de datos
        /// </summary>
        /// <param name="select">Sintaxis de la consulta</param>
        /// <returns>MySqlDataReader --> Select realizado con éxito, null--> Inserción Fallida </returns>
        public MySqlDataReader query(String select)
        {
            MySqlCommand adapter = new MySqlCommand(select, conexion);
            try
            {
                MySqlDataReader lector = adapter.ExecuteReader();
                return lector;
            }
            catch
            {
                Console.WriteLine("Error al ejecutar la consulta " + select);
                return null;
            }
        }

        /// <summary>
        /// Realiza una operacion distinta a una consulta a la base de datos
        /// </summary>
        /// <param name="sentencia">Sintaxis de la operacion</param>
        /// <returns>0 --> Petición realizada con exito, !=0 Petición Fallida </returns>
        public int nonQuery(String sentencia)
        {
            MySqlCommand adapter = new MySqlCommand(sentencia, conexion);
            try
            {
                adapter.ExecuteNonQuery();
                return 0;
            }
            catch 
            {
                Console.WriteLine("Error al ejecutar el codigo SQL " + sentencia);
                return -1;
            }
            
        }

        #endregion
        
    }
}
